using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

#nullable enable

namespace DataSourceGenerator;

public static class LocationGenerator
{
    public static string GenerateLocationSource(List<GeneratorLocation> locationInformation, string locationsFileName)
    {
        var sb = new StringBuilder();

        sb.Append(@$"
namespace Data;

#nullable enable

public static class {locationsFileName}{{
    public readonly struct Entry(
        string name,
        int apLocationId,
        int? clientId,
        string? clientNameId
    ){{
        public readonly string name = name;
        public readonly int apLocationId = apLocationId;
        public readonly int? clientId = clientId;
        public readonly string? clientNameId = clientNameId;
    }}

    public static Entry[] Entries = [
");

        static string GenerateLocationEntryDeclaration(GeneratorLocation location)
        {
            string clientIDString;
            string clientNameIDString;
            // We expect clientId and clientNameId to be mutually exclusive, one of them should always be null
            if (location.clientId == null)
            {
                clientIDString = "null";
                clientNameIDString = $"\"{location.clientNameId}\"";
            }
            else
            {
                clientIDString = location.clientId.ToString()!;
                clientNameIDString = "null";
            }
            return $"new(\"{location.name}\", {location.locationId}, {clientIDString}, {clientNameIDString})";
        }

        foreach (GeneratorLocation location in locationInformation)
        {
            sb.Append($"{GenerateLocationEntryDeclaration(location)},\n");
        }
        sb.Append(@$"
];
}}
        ");

        return sb.ToString();
    }

    public static List<GeneratorLocation> ExtractLocationInformation(string locationsText)
    {

        var parsedJsonData = JsonConvert.DeserializeObject<JsonLocation[]>(locationsText)!;
        return traverse(parsedJsonData[0], [], []);

        static List<GeneratorLocation> traverse(JsonLocation location, List<GeneratorLocation> resultLocations, string[][] accessRuleContext)
        {
            if (location.sections != null && location.children != null)
            {
                throw new System.Exception("Somehow is both location and region");
            }
            //it's a location location
            if (location.sections != null)
            {
                foreach (Section section in location.sections)
                {
                    string[][] newAccessRules = [.. accessRuleContext, section.access_rules.Where(s => !string.IsNullOrEmpty(s)).ToArray()];
                    resultLocations.Add(new GeneratorLocation(
                        section.name,
                        section.location_id,
                        section.client_id,
                        section.client_name_id,
                        newAccessRules.Where(arr => arr.Length != 0).ToArray()
                    ));
                }
            }
            //it's a region
            else
            {
                if (location.children != null)
                {
                    foreach (JsonLocation child in location.children)
                    {
                        string[][] newAccessRules = [.. accessRuleContext, child.access_rules.Where(s => !string.IsNullOrEmpty(s)).ToArray()];
                        traverse(child, resultLocations, newAccessRules);
                    }
                }
            }

            return resultLocations;
        }
    }

    public struct GeneratorLocation(string name, long locationId, int? clientId, string? clientNameId, string[][] accessRules)
    {
        public string name = name;
        public long locationId = locationId;
        public int? clientId = clientId;
        public string? clientNameId = clientNameId;
        public string[][] accessRules = accessRules;
    }


#pragma warning disable CS0649
    struct JsonLocation
    {
        public string name;
        public string region;
        public string[] access_rules;
        public Map[] map_locations;
        public JsonLocation[] children;
        public Section[] sections;
    }

    struct Map
    {
        public string map;
        public int x;
        public int y;
    }

    struct Section
    {
        public string name;
        public int location_id;
        public string region;
        public int? client_id;
        public string client_name_id;
        public string location_group;
        public string[] access_rules;
    }
    #pragma warning restore CS0649
}