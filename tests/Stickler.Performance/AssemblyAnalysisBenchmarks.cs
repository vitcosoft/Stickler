using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Stickler.TestInfrastructure;

// TODO: enable below ReSharper and disable pragma, then fix
// ReSharper disable UnusedVariable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Stickler.Performance;

[Config(typeof(BenchmarkConfig))]
[MemoryDiagnoser]
public class AssemblyAnalysisBenchmarks
{
    private string _testAssemblyDir = null!;

    [GlobalSetup]
    [RequiresAssemblyFiles]
    public void Setup()
    {
        _testAssemblyDir = Path.Combine(Path.GetTempPath(), "SticklerBenchmarks");
        TestAssemblyGenerator.GenerateAllTestAssemblies(_testAssemblyDir);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_testAssemblyDir))
        {
            Directory.Delete(_testAssemblyDir, true);
        }
    }

    [Benchmark]
    public void SmallAssembly50Types()
    {
        string assemblyPath = Path.Combine(_testAssemblyDir, "TestAssembly.Small.dll");
        // TODO: Add Stickler analysis once core library is implemented
    }

    [Benchmark]
    public void MediumAssembly500Types()
    {
        string assemblyPath = Path.Combine(_testAssemblyDir, "TestAssembly.Medium.dll");
        // TODO: Add Stickler analysis
    }

    [Benchmark]
    public void LargeAssembly2000Types()
    {
        string assemblyPath = Path.Combine(_testAssemblyDir, "TestAssembly.Large.dll");
        // TODO: Add Stickler analysis
    }
}

public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddJob(Job.Default.WithWarmupCount(3).WithIterationCount(5));
    }
}
