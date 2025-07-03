using System.IO;
using System.Linq;
using System;
using GatorRando.Archipelago;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;

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

            Plugin.LogDebug($"RulesConverter: Our rule is a {ruleString} aka {ruleType}");

            return token.ToObject(ruleType, serializer);


            // parse that "as normal" into the appropriate Rule
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

        [JsonProperty(PropertyName = "rule_json", ItemConverterType = typeof(RulesJsonConverter))]
        public Rule rule;
    }

    public class LocationRule
    {
        public string LocationName;
        public int LocationId;

        [JsonProperty(PropertyName = "rule_json", ItemConverterType = typeof(RulesJsonConverter))]
        public Rule rule;
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public abstract class Rule
    {
        OptionFilter[] options;

        public abstract bool Evaluate();

        public bool EvaluateOptions() => options.All(option => option.Evaluate());
    }

    public class True_ : Rule
    {
        public override bool Evaluate() => EvaluateOptions() /* && true*/;
    }
    public class And : Rule
    {
        readonly Rule[] children;
        public override bool Evaluate() => children.All(rule => rule.Evaluate()) && EvaluateOptions();
    }
    public class Or : Rule
    {
        readonly Rule[] children;
        public override bool Evaluate() => children.Any(rule => rule.Evaluate()) && EvaluateOptions();
    }
    public class Has : Rule
    {
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
        readonly struct Args
        {
            public readonly string[] itemNames;
        }
        readonly Args args;

        public override bool Evaluate() => args.itemNames.Any(ItemHandling.IsItemUnlocked);
    }
    public class HasAll : Rule
    {
        readonly struct Args
        {
            public readonly string[] itemNames;
        }
        readonly Args args;

        public override bool Evaluate() => args.itemNames.All(ItemHandling.IsItemUnlocked);
    }
    public class HasGroup : Rule
    {
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

        public readonly Options.Option option;
        public readonly int value;
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

            Plugin.LogDebug("Parsing our gator rules!");
            using (var reader = new StreamReader(assembly.GetManifestResourceStream("GatorRando.Data.EntranceRules.json")))
            {
                entranceRules = JsonConvert.DeserializeObject<List<EntranceRule>>(reader.ReadToEnd(), new RulesJsonConverter());
            }
            Plugin.LogDebug("Successfully parsed entrance rules!");

            using (var reader = new StreamReader(assembly.GetManifestResourceStream("GatorRando.Data.LocationRules.json")))
            {
                locationRules = JsonConvert.DeserializeObject<List<LocationRule>>(reader.ReadToEnd(), new RulesJsonConverter());
            }
            Plugin.LogDebug("Successfully parsed location rules!");
        }
    }

}

//var GatorRules = new GatorRando.Data.Rules.GatorRules();