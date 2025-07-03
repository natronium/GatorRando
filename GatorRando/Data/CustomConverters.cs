

using System;
using System.Text.RegularExpressions;
using GatorRando.Archipelago;
using GatorRando.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class GatorOptionConverter : JsonConverter<Options.Option>
{
    public override Options.Option ReadJson(JsonReader reader, Type objectType, Options.Option existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        string prefix = "worlds.lil_gator_game.options.";
        var enumString = JToken.Load(reader).Value<string>();
        Console.WriteLine($"converting gatoroption {enumString}");
        if (!enumString.StartsWith(prefix))
        {
            throw new Exception($"GatorOptions must start with {prefix}");
        }

        return (Options.Option)Enum.Parse(typeof(Options.Option), enumString.Substring(prefix.Length));

    }

    public override void WriteJson(JsonWriter writer, Options.Option value, JsonSerializer serializer) => throw new NotImplementedException();

}

public class ItemGroupConverter : JsonConverter<Items.ItemGroup>
{
    public override Items.ItemGroup ReadJson(JsonReader reader, Type objectType, Items.ItemGroup existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var enumString = JToken.Load(reader).Value<string>();
        Console.WriteLine($"converting itemgroup {enumString}");
        enumString = Regex.Replace(enumString, @"\s+", "");
        return (Items.ItemGroup)Enum.Parse(typeof(Items.ItemGroup), enumString);
    }

    public override void WriteJson(JsonWriter writer, Items.ItemGroup value, JsonSerializer serializer) => throw new NotImplementedException();

}