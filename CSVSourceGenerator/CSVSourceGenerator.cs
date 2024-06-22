using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Sylvan.Data.Csv;

#nullable enable

namespace CsvGenerator;
[Generator]
public class CSVGenerator : ISourceGenerator
{
#pragma warning disable RS2008 // Enable analyzer release tracking
    private static readonly DiagnosticDescriptor failedToReadDiag = new DiagnosticDescriptor("FailedToRead", "Errors Reading AdditionalFile",
                    "AdditionalText reported an error when reading the file at {0}",
                    "File",
                    DiagnosticSeverity.Error, true);
    private static readonly DiagnosticDescriptor msgDiag = new DiagnosticDescriptor("Na001", "Message!",
                    "{0}",
                    "Message",
                    DiagnosticSeverity.Warning, true);
#pragma warning restore RS2008 // Enable analyzer release tracking


    public void Execute(GeneratorExecutionContext context)
    {
        foreach (var file in context.AdditionalFiles)
        {
            var sourceText = file.GetText();
            if (sourceText == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(failedToReadDiag, null, [file.Path]));
                continue;
            }
            var csvName = Path.GetFileNameWithoutExtension(file.Path);
            var csvStr = sourceText.ToString();
            // var csvStr = "foo,bar,baz\n1,qux,\"spam,eggs\"";

            List<(string, string)> headerInfo = GetHeaderInfo(csvStr);

            List<string> structArgStrings = [];
            List<string> structFieldStrings = [];
            foreach (var (name, type) in headerInfo)
            {
                structArgStrings.Add($"{type}? {name}");
                structFieldStrings.Add($"public readonly {type}? {name} = {name};");
            }


            var csv = CsvDataReader.Create(new StringReader(csvStr));
            List<string> entriesStrings = [];
            while (csv.Read())
            {
                List<string> entryStrings = new();
                for (int i = 0; i < csv.FieldCount; i++)
                {
                    var field = csv.GetString(i);
                    var type = headerInfo[i].Item2;

                    if (field == "")
                    {
                        entryStrings.Add("null");
                    }
                    else if (type == "string")
                    {
                        entryStrings.Add($"\"{field}\"");
                    }
                    else
                    {
                        entryStrings.Add(field);
                    }
                }

                entriesStrings.Add($"new({String.Join(",", entryStrings)})");
            }
            var structArgs = String.Join(", ", structArgStrings);
            var structProps = String.Join("\n", structFieldStrings);
            var entries = String.Join(",\n", entriesStrings);
            // var entries = entriesBuilder.ToString();

            var source = @$"
namespace Data;
#nullable enable
public static class {csvName}{{
public readonly struct Entry({structArgs}){{
{structProps}
}}
public static Entry[] Entries = [
{entries}
];
}}
";

            context.AddSource($"{csvName}.g.cs", source);

        }



        // context.ReportDiagnostic(Diagnostic.Create(msgDiag, null, ["It ran!!"]));
    }

    private List<(string, string)> GetHeaderInfo(string csvString)
    {
        var csv = CsvDataReader.Create(new StringReader(csvString));
        csv.Read(); // advance to first row
        List<(string, string)> headerInfo = [];
        for (int i = 0; i < csv.FieldCount; i++)
        {
            var name = csv.GetName(i);
            headerInfo.Add((name, GetCsvFieldType(csv.GetString(i))));
        }
        return headerInfo;
    }

    // From the (MIT Licensed) CSVGenerator Source Generator sample in gh:dotnet/roslyn-sdk
    public static string GetCsvFieldType(string exemplar) => exemplar switch
    {
        _ when bool.TryParse(exemplar, out _) => "bool",
        _ when int.TryParse(exemplar, out _) => "int",
        _ when double.TryParse(exemplar, out _) => "double",
        _ => "string"
    };

    private static void JankLog(GeneratorExecutionContext context, string[] args)
    {
        context.ReportDiagnostic(Diagnostic.Create(msgDiag, null, args));
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }
}
