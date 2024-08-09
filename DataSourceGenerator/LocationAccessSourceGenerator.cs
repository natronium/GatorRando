using System.Collections.Generic;
using System.Text;
using static DataSourceGenerator.LocationGenerator;

#nullable enable

namespace DataSourceGenerator;

public static class LocationAccessSourceGenerator
{
    public static string GenerateLocationAccessSource(List<GeneratorLocation> locationInformation, string locationsFileName)
    {
        var sb = new StringBuilder();
        sb.Append(@$"
using System.Collections.Generic;
using System.Linq;
namespace Data;

class {locationsFileName}Access
{{
    public static List<int> GetAccessibleLocations(
        Dictionary<string, int> obtainedItems,
        Dictionary<string, bool> options,
        RequiredFunctions functions
    ) => LocationRules.Where(pair => pair.Value(obtainedItems, options, functions)).Select(pair => pair.Key).ToList();

    public static bool has(Dictionary<string, int> obtainedItems, string name) => (obtainedItems.TryGetValue(name, out int val) ? val : 0) > 0;
    public static bool hasCount(Dictionary<string, int> obtainedItems, string name, int count) => (obtainedItems.TryGetValue(name, out int val) ? val : 0) >= count;
    public delegate bool LogicFunction(Dictionary<string, int> obtainedItems, Dictionary<string, bool> options, RequiredFunctions functions);

    public static Dictionary<int, LogicFunction> LocationRules = new()
    {{
        ");
        foreach (GeneratorLocation location in locationInformation)
        {
            sb.Append($"[{location.locationId}] = (obtainedItems, options, functions) => ");
            sb.Append(GenerateAccessRuleLambdaBody(location.accessRules));
            sb.Append(",\n");
        }
        sb.Append(@$"
   }};
    #pragma warning disable CS0649
    public struct RequiredFunctions()
    {{
        ");
        sb.Append(string.Join(null, GenerateRequiredFunctions(locationInformation)));
        sb.Append(@$"
    }}
}}
#pragma warning restore CS0649
        ");
        return sb.ToString();
    }

    private static string GenerateAccessRuleLambdaBody(string[][] access_rules)
    {
        List<string> accessRuleGroupBodies = [];
        if (access_rules.Length == 0)
        {
            return "true";
        }
        else
        {
            foreach (string[] accessRuleGroup in access_rules)
            {
                List<string> groupBody = [];
                var groupSb = new StringBuilder();
                groupSb.Append('(');
                foreach (string accessOption in accessRuleGroup)
                {
                    List<string> orBody = [];
                    var orSb = new StringBuilder();
                    orSb.Append('(');
                    foreach (string subrule in accessOption.Split(','))
                    {
                        orBody.Add(ParseAccessRule(subrule));
                    }
                    orSb.Append(string.Join("&&", orBody));
                    orSb.Append(')');
                    groupBody.Add(orSb.ToString());
                }
                groupSb.Append(string.Join("||", groupBody));
                groupSb.Append(')');
                accessRuleGroupBodies.Add(groupSb.ToString());
            }
            return string.Join("&&", accessRuleGroupBodies);
        }
    }
    private static string ParseAccessRule(string accessRule)
    {
        if (accessRule[0] == '$')
        {
            var pieces = accessRule.Split('|');
            return pieces[0] switch
            {
                "$has_at_least_n_pencil" => $"hasCount(obtainedItems,\"thrown_pencil\", {pieces[1]})",
                "$has_at_least_n_bracelet" => $"hasCount(obtainedItems,\"bracelet\", {pieces[1]})",
                _ => $"functions.{pieces[0].Substring(1)}(obtainedItems, options, functions)"
            };
        }
        else
        { //assume is just a "has item" rule
            return $"has(obtainedItems,\"{accessRule}\")";
        }
    }
    private static HashSet<string> GenerateRequiredFunctions(List<GeneratorLocation> locationInformation)
    {
        HashSet<string> requiredFunctions = [];
        foreach (GeneratorLocation location in locationInformation)
        {
            string[][] access_rules = location.accessRules;
            if (access_rules.Length == 0)
            {
                continue;
            }
            else
            {
                foreach (string[] accessRuleGroup in access_rules)
                {
                    foreach (string accessOption in accessRuleGroup)
                    {
                        foreach (string subrule in accessOption.Split(','))
                        {
                            string? function = ReturnSpecialFunction(subrule);
                            if (function != null)
                            {
                                requiredFunctions.Add(function);
                            }
                        }
                    }
                }
            }
        }
        return requiredFunctions;
    }

    private static string? ReturnSpecialFunction(string accessRule)
    {
        if (accessRule[0] == '$')
        {
            var pieces = accessRule.Split('|');
            if (!pieces[0].Contains("has_at_least_n"))
            {
                return $"public LogicFunction {pieces[0].Substring(1)};\n";
            };
        }
        return null;
    }
}