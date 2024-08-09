using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DataSourceGenerator;

public static class ItemGenerator
{

    enum ClientItemType
    {
        Friend,
        Craft_Stuff,
        Bracelet,
        Item,
        Craft,
    }

    enum ItemGroup
    {
        Friends,
        Crafting_Materials,
        Traversal,
        Hat,
        Quest_Item,
        Sword,
        Shield,
        Ranged,
        Craft,
        Item,
        Cardboard_Destroyer,
    }
    #pragma warning disable CS0649
    struct JsonItem
    {
        public string long_name;
        public string short_name;
        public long item_id;
        public string client_name_id;
        public int? client_resource_amount;
        public string classification;
        public ClientItemType client_item_type;
        public int base_quantity_items_in_pool;
        public string item_groups;
    }
    #pragma warning restore CS0649

    public static string GenerateItemSource(string itemsText, string itemsFileName)
    {

        var parsedJsonData = JsonConvert.DeserializeObject<Dictionary<string, JsonItem>[]>(itemsText)!;

        var sb = new StringBuilder();

        sb.Append(@$"
namespace Data;
public static class {itemsFileName}{{
    public enum ClientItemType
    {{
        Friend,
        Craft_Stuff,
        Bracelet,
        Item,
        Craft,
    }}

    public enum ItemGroup
    {{
        Friends,
        Crafting_Materials,
        Traversal,
        Hat,
        Quest_Item,
        Sword,
        Shield,
        Ranged,
        Craft,
        Item,
        Cardboard_Destroyer,
    }}
    
    public readonly struct Entry(
        string shortName,
        string longName,
        int apItemId,
        string clientNameId,
        int? clientResourceAmount,
        ClientItemType clientItemType,
        ItemGroup[] itemGroups
    ){{
        public readonly string shortName = shortName;
        public readonly string longName = longName;
        public readonly int apItemId = apItemId;
        public readonly string clientNameId = clientNameId;
        public readonly int? clientResourceAmount = clientResourceAmount;
        public readonly ClientItemType clientItemType = clientItemType;
        public readonly ItemGroup[] itemGroups = itemGroups;
    }}

    public static Entry[] Entries = [
");

        static string GenerateItemEntryDeclaration(JsonItem item)
        {
            string resourceAmountString;
            if (item.client_resource_amount == null)
            {
                resourceAmountString = "null";
            }
            else
            {
                resourceAmountString = item.client_resource_amount.ToString()!;
            }

            string itemType = $"ClientItemType.{item.client_item_type}";
            string itemGroups = String.Join(",", item.item_groups.Split(',').Select(e => $"ItemGroup.{e}"));

            return $"new(\"{item.short_name}\", \"{item.long_name}\", {item.item_id}, \"{item.client_name_id}\", {resourceAmountString}, {itemType}, [{itemGroups}])";
        }

        foreach (var kv in parsedJsonData[0])
        {
            sb.Append($"{GenerateItemEntryDeclaration(kv.Value)},\n");
        }
        sb.Append(@$"
];
}}
        ");

        return sb.ToString();
    }
}