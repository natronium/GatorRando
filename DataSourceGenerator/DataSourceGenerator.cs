using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
// using Newtonsoft.Json;

#nullable enable

namespace DataSourceGenerator;
[Generator]
public class DataSourceGenerator : ISourceGenerator
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

    private static GeneratorExecutionContext ctx;

    enum DataType
    {
        Item,
        Location,
    }
    public void Execute(GeneratorExecutionContext context)
    {
        ctx = context;
        foreach (var file in context.AdditionalFiles)
        {

            string baseFileName = Path.GetFileNameWithoutExtension(file.Path);


            string? fileText = file.GetText()?.ToString();  
            if (fileText == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(failedToReadDiag, null, [file.Path]));
                continue;
            }

            switch (baseFileName)
            {
                case "Items":
                    string itemDataSource = ItemGenerator.GenerateItemSource(fileText, baseFileName);
                    context.AddSource($"{baseFileName}.g.cs", itemDataSource);
                    break;

                case "Locations":
                    List<LocationGenerator.GeneratorLocation> locationInformation = LocationGenerator.ExtractLocationInformation(fileText);

                    string locationDataSource = LocationGenerator.GenerateLocationSource(locationInformation, baseFileName);
                    context.AddSource($"{baseFileName}.g.cs", locationDataSource);

                    string accessDataSource = LocationAccessSourceGenerator.GenerateLocationAccessSource(locationInformation, baseFileName);
                    context.AddSource($"{baseFileName}Access.g.cs", accessDataSource);
                    break;

                default:
                    continue; //Should never happen? but technically logically correct

            }
        }
    }

    private static void JankLog(GeneratorExecutionContext context, string[] args)
    {
        context.ReportDiagnostic(Diagnostic.Create(msgDiag, null, args));
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }
}
