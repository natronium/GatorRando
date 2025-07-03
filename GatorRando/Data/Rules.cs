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

namespace GatorRando.Data;

public class Rules
{
    // Read in json
    // process json into rules
    // split rules into entrance rules and location rules
    // compose entrance and location rules

    // Rule types
    // And
    // Or
    // Has
    // HasAny
    // HasAll
    // HasGroup

    // OptionFilter

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
        public Rule rule;
    }

    public class LocationRule
    {
        public string LocationName;
        public int LocationId;

        [JsonProperty(PropertyName = "rule_json")]
        public Rule rule;
    }

#pragma warning disable 0649 //is never assigned to and will always have default value
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), MemberSerialization = MemberSerialization.Fields)]
    public abstract class Rule
    {
        readonly List<OptionFilter> options;

        public abstract bool Evaluate();

        public bool EvaluateOptions() => options.All(option => option.Evaluate());
    }

    public class True_ : Rule
    {
        public override bool Evaluate() => EvaluateOptions() /* && true*/;
    }
    public class And : Rule
    {
        readonly List<Rule> children;
        public override bool Evaluate() => children.All(rule => rule.Evaluate()) && EvaluateOptions();
    }
    public class Or : Rule
    {
        readonly List<Rule> children;
        public override bool Evaluate() => children.Any(rule => rule.Evaluate()) && EvaluateOptions();
    }
    public class Has : Rule
    {
        [JsonObject(MemberSerialization = MemberSerialization.Fields)]
        readonly struct Args
        {
            public readonly string itemName;
            public readonly int count;
        }

        readonly Args args;

        public override bool Evaluate() => ItemHandling.GetItemUnlockCount(args.itemName) >= args.count;
    }
    public class HasAny : Rule
    {
        [JsonObject(MemberSerialization = MemberSerialization.Fields)]
        readonly struct Args
        {
            public readonly List<string> itemNames;
        }
        readonly Args args;

        public override bool Evaluate() => args.itemNames.Any(ItemHandling.IsItemUnlocked);
    }
    public class HasAll : Rule
    {
        [JsonObject(MemberSerialization = MemberSerialization.Fields)]
        readonly struct Args
        {
            public readonly List<string> itemNames;
        }
        readonly Args args;

        public override bool Evaluate() => args.itemNames.All(ItemHandling.IsItemUnlocked);
    }
    public class HasGroup : Rule
    {
        [JsonObject(MemberSerialization = MemberSerialization.Fields)]
        readonly struct Args
        {
            public readonly Items.ItemGroup itemNameGroup;
            public readonly int count;
        }
        readonly Args args;

        static List<string> ItemsInItemGroup(Items.ItemGroup itemGroup) =>
            [.. Items.itemData
                .Where(item => item.itemGroups.Contains(itemGroup))
                .Select(item => item.name)];

        public override bool Evaluate() => ItemsInItemGroup(args.itemNameGroup).Where(ItemHandling.IsItemUnlocked).Count() >= args.count;
    }

    public class HasEnoughFriends : Rule
    {
        public override bool Evaluate()
        {
            throw new NotImplementedException();
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
        public readonly Options.Option option;

        [JsonProperty(propertyName: "value")]
        public readonly int value;

        [JsonProperty(propertyName: "operator")]
        public readonly Operator oper;

        public bool Evaluate()
        {
            return oper switch
            {
                Operator.EQ => Options.GetOptionBool(option) == Convert.ToBoolean(value),
                _ => throw new NotImplementedException(),
            };
        }
    }


    public readonly struct GatorRules
    {
        public readonly List<EntranceRule> entranceRules;
        public readonly List<LocationRule> locationRules;
        public GatorRules()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var settings = new JsonSerializerSettings()
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

            using (var reader = new StreamReader(assembly.GetManifestResourceStream("GatorRando.Data.EntranceRules.json")))
            {
                entranceRules = JsonConvert.DeserializeObject<List<EntranceRule>>(reader.ReadToEnd(), settings);
            }

            using (var reader = new StreamReader(assembly.GetManifestResourceStream("GatorRando.Data.LocationRules.json")))
            {
                locationRules = JsonConvert.DeserializeObject<List<LocationRule>>(reader.ReadToEnd(), settings);
            }
        }
    }

}