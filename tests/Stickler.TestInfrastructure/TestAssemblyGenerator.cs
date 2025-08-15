using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Stickler.TestInfrastructure;

/// <summary>
///     Generates test assemblies with varying complexity for testing streaming analysis performance.
/// </summary>
public static class TestAssemblyGenerator
{
    private const int InterfacePercentage = 10;
    private const int AbstractClassFrequency = 15;
    private const int SealedClassFrequency = 20;
    private const int InterfaceImplementationFrequency = 5;
    private static readonly object _compilationLock = new object();

    /// <summary>
    ///     Small test assembly with 50 types for basic testing scenarios.
    /// </summary>
    public static readonly AssemblySpec Small = new("TestAssembly.Small", 50,
        "TestAssembly.Small.dll");

    /// <summary>
    ///     Medium test assembly with 500 types for moderate complexity testing.
    /// </summary>
    public static readonly AssemblySpec Medium = new("TestAssembly.Medium", 500,
        "TestAssembly.Medium.dll");

    /// <summary>
    ///     Large test assembly with 2000 types for performance and scalability testing.
    /// </summary>
    public static readonly AssemblySpec Large = new("TestAssembly.Large", 2000,
        "TestAssembly.Large.dll");

    /// <summary>
    ///     Generates all three test assemblies (Small, Medium, Large) in the specified directory.
    /// </summary>
    /// <param name="outputDirectory">Directory where assemblies will be created.</param>
    [RequiresAssemblyFiles]
    public static void GenerateAllTestAssemblies(string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);

        GenerateTestAssembly(Small, outputDirectory);
        GenerateTestAssembly(Medium, outputDirectory);
        GenerateTestAssembly(Large, outputDirectory);
    }

    [RequiresAssemblyFiles(
        "Calls Stickler.TestInfrastructure.TestAssemblyGenerator.CompileToAssembly(String, String)")]
    private static void GenerateTestAssembly(AssemblySpec spec, string outputDirectory)
    {
        lock (_compilationLock) // Protect compilation and file I/O
        {
            var sourceCode = GenerateSourceCode(spec.Name, spec.TypeCount);
            var assemblyPath = Path.Combine(outputDirectory, spec.OutputPath);
            CompileToAssembly(sourceCode, assemblyPath);
        }
    }

    private static string GenerateSourceCode(string assemblyName, int typeCount)
    {
        int estimatedSize = typeCount * 200;
        var sb = new StringBuilder(estimatedSize);

        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();

        sb.AppendLine($"namespace {assemblyName};");
        sb.AppendLine();

        int interfaceCount = Math.Max(1, typeCount / InterfacePercentage);
        for (int i = 0; i < interfaceCount; i++)
        {
            GenerateInterface(sb, i);
        }

        int classCount = typeCount - interfaceCount;
        for (int i = 0; i < classCount; i++)
        {
            GenerateClass(sb, i, interfaceCount);
        }

        return sb.ToString();
    }

    private static void GenerateInterface(StringBuilder sb, int index)
    {
        string visibility = index % 3 == 0 ? "public" : "internal";
        sb.AppendLine($"{visibility} interface I{GetTypeName(index)}");
        sb.AppendLine("{");
        sb.AppendLine("    void Execute();");
        sb.AppendLine("    string GetName();");
        sb.AppendLine("}");
        sb.AppendLine();
    }

    private static void GenerateClass(StringBuilder sb, int index, int interfaceCount)
    {
        string visibility = index % 4 == 0 ? "public" : "internal";
        string isAbstract = index % AbstractClassFrequency == 0 ? "abstract " : "";
        string isSealed = index % SealedClassFrequency == 0 && string.IsNullOrEmpty(isAbstract)
            ? "sealed "
            : "";

        string className = GetTypeName(index + 1000);

        bool implementsInterface =
            index % InterfaceImplementationFrequency == 0 && interfaceCount > 0;
        string interfaces = implementsInterface ? $" : I{GetTypeName(index % interfaceCount)}" : "";

        sb.AppendLine($"{visibility} {isAbstract}{isSealed}class {className}{interfaces}");
        sb.AppendLine("{");

        GenerateFields(sb, index);
        GenerateMethods(sb, index);

        if (implementsInterface)
        {
            sb.AppendLine("    public void Execute() { }");
            sb.AppendLine($"    public string GetName() => \"{className}\";");
            sb.AppendLine();
        }

        sb.AppendLine("}");
        sb.AppendLine();
    }

    private static void GenerateFields(StringBuilder sb, int index)
    {
        int fieldCount = 2 + (index % 4);

        for (int i = 0; i < fieldCount; i++)
        {
            string visibility = (i % 3) switch
            {
                0 => "private",
                1 => "protected",
                _ => "public"
            };

            string isStatic = i % 8 == 0 ? "static " : "";
            string isReadonly = i % 6 == 0 ? "readonly " : "";

            int maxClassIndex = Math.Max(10, index);
            string fieldType = (i % 5) switch
            {
                0 => "string",
                1 => "int",
                2 => "List<string>",
                3 => GetTypeName(((index + i) % maxClassIndex) + 1000),
                _ => "object"
            };

            sb.AppendLine(
                $"    {visibility} {isStatic}{isReadonly}{fieldType} _{GetFieldName(i)};");
        }

        sb.AppendLine();
    }

    private static void GenerateMethods(StringBuilder sb, int index)
    {
        int methodCount = 3 + (index % 4);

        for (int i = 0; i < methodCount; i++)
        {
            string visibility = (i % 4) switch
            {
                0 => "public",
                1 => "protected",
                2 => "internal",
                _ => "private"
            };

            string isStatic = i % 10 == 0 ? "static " : "";
            string isVirtual = i % 7 == 0 && string.IsNullOrEmpty(isStatic) ? "virtual " : "";
            const string isOverride = "";

            string returnType = (i % 4) switch
            {
                0 => "void",
                1 => "string",
                2 => "int",
                _ => "object"
            };

            string methodName = GetMethodName(i);
            string parameters = i % 3 == 0 ? $"string param{i}" :
                i % 3 == 1 ? $"int value{i}, object data{i}" : "";

            sb.AppendLine(
                $"    {visibility} {isStatic}{isVirtual}{isOverride}{returnType} {methodName}({parameters})");
            sb.AppendLine("    {");

            if (returnType != "void")
            {
                string defaultReturn = returnType switch
                {
                    "string" => "\"default\"",
                    "int" => "0",
                    _ => "null"
                };
                sb.AppendLine($"        return {defaultReturn};");
            }

            sb.AppendLine("    }");
            sb.AppendLine();
        }
    }

    private static string GetTypeName(int index)
    {
        return $"Type{index:D4}";
    }

    private static string GetFieldName(int index)
    {
        return $"field{index}";
    }

    private static string GetMethodName(int index)
    {
        return $"Method{index}";
    }

    [RequiresAssemblyFiles(
        "Calls Stickler.TestInfrastructure.TestAssemblyGenerator.GetCompilationReferences()")]
    private static void CompileToAssembly(string sourceCode, string outputPath)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        MetadataReference[] references = GetCompilationReferences();

        string assemblyName = Path.GetFileNameWithoutExtension(outputPath);

        var compilation = CSharpCompilation.Create(
            assemblyName,
            [syntaxTree],
            references,
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                platform: Platform.AnyCpu));

        using var stream = new FileStream(outputPath, FileMode.Create);
        EmitResult emitResult = compilation.Emit(stream);

        if (!emitResult.Success)
        {
            string errors = string.Join("\n", emitResult.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => d.ToString()));

            throw new InvalidOperationException(
                $"Compilation failed for {assemblyName}:\n{errors}");
        }
    }

    [RequiresAssemblyFiles(
        "Reads runtime assembly locations to provide references for compilation.")]
    private static MetadataReference[] GetCompilationReferences()
    {
        if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string trustedAssemblies)
        {
            return trustedAssemblies.Split(Path.PathSeparator)
                .Select(path => MetadataReference.CreateFromFile(path))
                .ToArray<MetadataReference>();
        }

        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToArray<MetadataReference>();
    }

    /// <summary>
    ///     Specifications for generating test assemblies with different complexity levels.
    /// </summary>
    /// <param name="Name">The assembly name and namespace.</param>
    /// <param name="TypeCount">Total number of types to generate.</param>
    /// <param name="OutputPath">Filename for the compiled assembly.</param>
    public record AssemblySpec(string Name, int TypeCount, string OutputPath);
}
