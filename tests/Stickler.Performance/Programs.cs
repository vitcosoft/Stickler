using BenchmarkDotNet.Running;

namespace Stickler.Performance;

internal static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<AssemblyAnalysisBenchmarks>();
    }
}
