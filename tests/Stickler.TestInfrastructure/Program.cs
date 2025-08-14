using System.Diagnostics.CodeAnalysis;

namespace Stickler.TestInfrastructure;

/// <summary>
///     Console application entry point for generating test assemblies during build.
/// </summary>
internal static class Program
{
    /// <summary>
    ///     Entry point that generates test assemblies to the specified output directory.
    /// </summary>
    /// <param name="args">Command line arguments. First argument should be the output directory path.</param>
    /// <returns>Exit code: 0 for success, 1 for failure.</returns>
    [RequiresAssemblyFiles(
        "Calls Stickler.TestInfrastructure.TestAssemblyGenerator.GenerateAllTestAssemblies(String)")]
    public static int Main(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: Stickler.TestInfrastructure <output-directory>");
                return 1;
            }

            string outputDirectory = args[0];
            Console.WriteLine($"Generating test assemblies in: {outputDirectory}");

            TestAssemblyGenerator.GenerateAllTestAssemblies(outputDirectory);

            Console.WriteLine("Test assembly generation completed successfully.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error generating test assemblies: {ex.Message}");
            return 1;
        }
    }
}
