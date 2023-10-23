using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

namespace MultiThreadBenchmark;

public class MultiThreadBenchmark
{
    public static void Main()
    {
        var config = new ManualConfig()
            .AddColumnProvider(DefaultColumnProviders.Instance)
            .AddLogger(ConsoleLogger.Default)
            .AddValidator(JitOptimizationsValidator.DontFailOnError)
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);

        BenchmarkRunner.Run<MultiThreadPerformanceSimpleBenchmarks>(config);
    }
}
