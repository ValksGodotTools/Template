using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MySourceGenerator
{
    [Generator]
    public class PrefabSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this generator
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Get all additional text files
            IEnumerable<AdditionalText> tscnFiles = context.AdditionalFiles
                .Where(file => Path.GetExtension(file.Path)
                .Equals(".tscn", StringComparison.OrdinalIgnoreCase));

            // Generate the Prefabs class
            string sourceCode = GeneratePrefabsClass(context, tscnFiles);

            // Add the generated source code to the compilation
            context.AddSource("Prefabs.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }

        private static string GeneratePrefabsClass(GeneratorExecutionContext context, IEnumerable<AdditionalText> tscnFiles)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"namespace {context.Compilation.AssemblyName};");
            sb.AppendLine();

            sb.AppendLine("public static class Prefabs");
            sb.AppendLine("{");

            foreach (AdditionalText file in tscnFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.Path);
                string resourcePath = $"res://Scenes/Prefabs/{fileName}.tscn";
                sb.AppendLine($"    public static string {fileName.SnakeCaseToPascalCase()} = \"{resourcePath}\";");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
