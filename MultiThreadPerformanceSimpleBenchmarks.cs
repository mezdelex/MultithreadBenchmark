using BenchmarkDotNet.Attributes;

namespace MultiThreadBenchmark;

public class MultiThreadPerformanceSimpleBenchmarks
{
    private const int Iterations = 4;

    [Benchmark]
    public List<int> NormalBenchmark()
    {
        var tasks = Enumerable.Range(0, Iterations).Select(_ => new Func<int>(() => PerformOperation()));

        var listOfResults = new List<int>();
        tasks.ToList().ForEach(t => listOfResults.Add(t()));

        return listOfResults;
    }

    [Benchmark]
    public List<int> ParallelBenchmark()
    {
        var tasks = Enumerable.Range(0, Iterations).Select(_ => new Func<int>(() => PerformOperation()));

        var listOfResults = new List<int>();
        Parallel.ForEach(tasks, task => listOfResults.Add(task()));

        return listOfResults;
    }

    [Benchmark]
    public List<int> PLINQBenchmark()
    {
        var tasks = Enumerable.Range(0, Iterations).Select(_ => new Func<int>(() => PerformOperation()));

        var listOfResults = new List<int>();
        tasks.AsParallel().ForAll(t => listOfResults.Add(t()));

        return listOfResults;
    }

    private int PerformOperation()
    {
        Thread.Sleep(10);
        var operation = 2 + 4;

        return operation;
    }
}
