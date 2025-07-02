using System.Data;
using System.Linq;
using System;
using GatorRando.Archipelago;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GatorRando.Data;

public class AccessRules
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
                "True_" => typeof(TrueRule),
                "And" => typeof(And),
                "Or" => typeof(Or),
                "Has" => typeof(Has),
                "HasAny" => typeof(HasAny),
                "HasAll" => typeof(HasAll),
                "HasGroup" => typeof(HasGroup),
                _ => throw new Exception($"Don't know how to parse rule type: {ruleString}"),
            };
            return token.ToObject(ruleType, serializer);

            
            // parse that "as normal" into the appropriate Rule
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Rules are read-only");
        }

        public override bool CanConvert(Type objectType) => typeof(Rule).IsAssignableFrom(objectType);

        public override bool CanWrite => false;
    }



    public class EntranceRule
    {
        public string StartingRegion;
        public string EndingRegion;

        [JsonProperty(PropertyName = "rule_json")]
        public Rule rule;
    }

    public abstract class Rule
    {
        public OptionFilter[] options;
    }

    public class TrueRule : Rule { }
    public class And : Rule { }
    public class Or : Rule { }
    public class Has : Rule { }
    public class HasAny : Rule { }
    public class HasAll : Rule { }
    public class HasGroup : Rule { }



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

        public Options.Option option;
        public int value;
        public Operator oper;
    }

    public static void LoadAccessRules()
    {
        // using var reader = new StreamReader(assembly.GetManifestResourceStream("Data/Rules.json"));
        // doStuff(reader.ReadToEnd());
    }

    // public static bool Has(Items.Item item)
    // {
    //     return ItemHandling.IsItemUnlocked(item.name);
    // }

    // public static bool HasAny(Items.Item[] items)
    // {
    //     foreach (Items.Item item in items)
    //     {
    //         if (ItemHandling.IsItemUnlocked(item.name))
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }

    // public static bool HasAll(Items.Item[] items)
    // {
    //     foreach (Items.Item item in items)
    //     {
    //         if (!ItemHandling.IsItemUnlocked(item.name))
    //         {
    //             return false;
    //         }
    //     }
    //     return true;
    // }

    // static List<string> ItemsInItemGroup(Items.ItemGroup itemGroup) =>
    //         [.. Items.itemData
    //             .Where(item => item.itemGroups.Contains(itemGroup))
    //             .Select(item => item.name)];

    // static bool HasGroup(Dictionary<string, int> obtainedItems, Items.ItemGroup itemGroup) =>
    //     obtainedItems
    //         .Where(kv => ItemsInItemGroup(itemGroup).Contains(kv.Key))
    //         .Where(kv => kv.Value > 0)
    //         .ToArray().Length > 0;
}