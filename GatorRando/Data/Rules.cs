using System.IO;
using System.Linq;
using System;
using GatorRando.Archipelago;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Converters;
using HarmonyLib;

namespace GatorRando.Data;

public class Rules
{

    public class RulesJsonConverter : JsonConverter
    {

        // inspired by https://stackoverflow.com/a/30176798
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type != JTokenType.Object)
            {
                throw new Exception("Token wasn't an object. don't know how to parse this rule");
            }
            string ruleString = token["rule"].ToString();
            Type ruleType = ruleString switch
            {
                "True_" => typeof(True_),
                "And" => typeof(And),
                "Or" => typeof(Or),
                "Has" => typeof(Has),
                "HasAny" => typeof(HasAny),
                "HasAll" => typeof(HasAll),
                "HasGroup" => typeof(HasGroup),
                "HasEnoughFriends" => typeof(HasEnoughFriends),
                _ => throw new Exception($"Don't know how to parse rule type: {ruleString}"),
            };


            // parse that "as normal" into the appropriate Rule type
            return token.ToObject(ruleType, serializer);


        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Rules are read-only");
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(Rule);

        public override bool CanWrite => false;
    }


    public class EntranceRule
    {
        public string StartingRegion;
        public string EndingRegion;

        [JsonProperty(PropertyName = "rule_json")]
        public Rule Rule;
    }

    public class LocationRule
    {
        public string LocationName;
        public int LocationId;
        public string Region;

        [JsonProperty(PropertyName = "rule_json")]
        public Rule Rule;
    }

#pragma warning disable 0649 //is never assigned to and will always have default value
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), MemberSerialization = MemberSerialization.Fields)]
    public abstract class Rule
    {
        readonly List<OptionFilter> Options;

        public abstract bool Evaluate();

        public bool EvaluateOptions() => Options?.All(option => option.Evaluate()) ?? true;
    }

    public class True_ : Rule
    {
        public override bool Evaluate() => EvaluateOptions() /* && true*/;
    }
    public class And(List<Rule> children) : Rule
    {
		private readonly List<Rule> Children = children;

        public override bool Evaluate() => Children.All(rule => rule.Evaluate()) && EvaluateOptions();
    }
    public class Or(List<Rule> children) : Rule
    {
		private readonly List<Rule> Children = children;
        public override bool Evaluate() => Children.Any(rule => rule.Evaluate()) && EvaluateOptions();
    }
    public class Has : Rule
    {
        [JsonObject(MemberSerialization = MemberSerialization.Fields)]
        readonly struct Args
        {
            public readonly string ItemName;
            public readonly int Count;
        }

        readonly Args args;

        public override bool Evaluate() => ItemHandling.GetItemUnlockCount(args.ItemName, true) >= args.Count;
    }
    public class HasAny : Rule
    {
        [JsonObject(MemberSerialization = MemberSerialization.Fields)]
        readonly struct Args
        {
            public readonly List<string> ItemNames;
        }
        readonly Args args;

        public override bool Evaluate() => args.ItemNames.Any(item => ItemHandling.IsItemUnlocked(item, true));
    }
    public class HasAll : Rule
    {
        [JsonObject(MemberSerialization = MemberSerialization.Fields)]
        readonly struct Args
        {
            public readonly List<string> ItemNames;
        }
        readonly Args args;

        public override bool Evaluate() => args.ItemNames.All(item => ItemHandling.IsItemUnlocked(item, true));
    }
    public class HasGroup : Rule
    {
        [JsonObject(MemberSerialization = MemberSerialization.Fields)]
        readonly struct Args
        {
            public readonly Items.ItemGroup ItemNameGroup;
            public readonly int Count;
        }
		private readonly Args args;

		private static List<string> ItemsInItemGroup(Items.ItemGroup itemGroup) =>
            [.. Items.itemData
                .Where(item => item.itemGroups.Contains(itemGroup))
                .Select(item => item.name)];

        public override bool Evaluate()
        {
            if (args.Count == 1)
            {
                return ItemsInItemGroup(args.ItemNameGroup).Any(item => ItemHandling.IsItemUnlocked(item, true));
            }
            else
            {
                return ItemsInItemGroup(args.ItemNameGroup).Where(item => ItemHandling.IsItemUnlocked(item, true)).Count() >= args.Count;
            }
        } 
    }

    public class HasEnoughFriends : Rule
    {
        public override bool Evaluate()
        {
            return ItemHandling.GetItemUnlockCount("Friend", true) + ItemHandling.GetItemUnlockCount("Friend x2", true) * 2 + ItemHandling.GetItemUnlockCount("Friend x3", true) * 3 + ItemHandling.GetItemUnlockCount("Friend x4", true) * 4 >= 35;
        }
    }
#pragma warning restore 0649 // never assigned to, always default value, etc.


    //TODO: figure out if there's a better way to get OptionFilters to parse than just annotating the properties
    public class OptionFilter
    {
        public enum Operator
        {
            EQ,
            NE,
            GT,
            LT,
            GE,
            LE,
            Contains,
        }

        [JsonProperty(propertyName: "option")]
        public readonly Options.Option Option;

        [JsonProperty(propertyName: "value")]
        public readonly int Value;

        [JsonProperty(propertyName: "operator")]
        public readonly Operator Oper;

        public bool Evaluate()
        {
            return Oper switch
            {
                Operator.EQ => Options.GetOptionBool(Option) == Convert.ToBoolean(Value),
                _ => throw new NotImplementedException(),
            };
        }
    }


    public static class GatorRules
    {
        public static readonly Dictionary<long, Rule> Rules;

        static GatorRules()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<EntranceRule> entranceRules;
            List<LocationRule> locationRules;
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Converters = [
                    new RulesJsonConverter(),
                    new ItemGroupConverter(),
                    new GatorOptionConverter(),
                    new StringEnumConverter(),
                ],
            };

            using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream("GatorRando.Data.EntranceRules.json")))
            {
                entranceRules = JsonConvert.DeserializeObject<List<EntranceRule>>(reader.ReadToEnd(), settings);
            }

            using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream("GatorRando.Data.LocationRules.json")))
            {
                locationRules = JsonConvert.DeserializeObject<List<LocationRule>>(reader.ReadToEnd(), settings);
            }

            Rules = PrecomputeRules(entranceRules, locationRules);
        }

        private static Dictionary<long, Rule> PrecomputeRules(List<EntranceRule> entranceRules, List<LocationRule> locationRules)
        {
            Dictionary<long, Rule> precomputedRules = [];
            Dictionary<string, Rule> regionRules = ComputeRegionRules(entranceRules);
            foreach (LocationRule location in locationRules)
            {
                precomputedRules[location.LocationId] = new And([regionRules[location.Region], location.Rule]);
            }
            return precomputedRules;
        }

        private static Dictionary<string, Rule> ComputeRegionRules(List<EntranceRule> entranceRules)
        {
            Dictionary<string, Rule> regionRules = [];
            foreach (string region in entranceRules.Select(rule => rule.EndingRegion).Distinct().AddItem("Tutorial Island"))
            {
                regionRules[region] = ComputeRegionRule(entranceRules, region);
            }

            return regionRules;
        }

        private static Rule ComputeRegionRule(List<EntranceRule> entranceRules, string regionName)
        {
            // base case
            if (regionName == "Tutorial Island")
            {
                return new True_();
            }
            List<Rule> orRules = [];
            foreach (EntranceRule entrance in entranceRules.Where(e => e.EndingRegion == regionName))
            {
                orRules.Add(new And([entrance.Rule, ComputeRegionRule(entranceRules, entrance.StartingRegion)]));
            }
            if (orRules.Count() == 1)
            {
                return orRules[0];
            }
            else
            {
                return new Or(orRules);
            }
        }
    }

}