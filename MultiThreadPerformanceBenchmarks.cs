using BenchmarkDotNet.Attributes;

namespace MultiThreadBenchmark;

public class MultiThreadPerformanceBenchmarks
{
    private const int Iterations = 10;
    private const int DegreeOfParallelism = 5;

    [Benchmark]
    public List<int> SynchronousBenchmark()
    {
        var tasks = Enumerable.Range(0, Iterations).Select(_ => new Func<int>(() => PerformOperation()));

        var listOfResults = new List<int>();
        tasks.ToList().ForEach(expression => listOfResults.Add(expression()));

        return listOfResults;
    }

    [Benchmark]
    public List<int> ConcurrentBenchmark()
    {
        var tasks = Enumerable.Range(0, Iterations).Select(_ => Task.Run(() => PerformOperation()));

        var listOfResults = new List<int>();
        tasks.ToList().ForEach(task => listOfResults.Add(task.GetAwaiter().GetResult()));

        return listOfResults;
    }

    [Benchmark]
    public async Task<int[]> ConcurrentBenchmarkTaskWhenAll()
    {
        var tasks = Enumerable.Range(0, Iterations).Select(_ => Task.Run(() => PerformOperation()));

        var listOfResults = await Task.WhenAll(tasks);

        return listOfResults;
    }

    [Benchmark]
    public async Task<List<int>> ParallelBenchmark()
    {
        var tasks = Enumerable.Range(0, Iterations).Select(_ => Task.Run(() => PerformOperation()));

        var listOfResults = new List<int>();
        await Parallel.ForEachAsync(tasks, new ParallelOptions
        {
            MaxDegreeOfParallelism = DegreeOfParallelism
        }, async (task, _) => listOfResults.Add(await task));

        return listOfResults;
    }

    [Benchmark]
    public List<int> PLINQBenchmark()
    {
        var tasks = Enumerable.Range(0, Iterations).Select(_ => Task.Run(() => PerformOperation()));

        var listOfResults = new List<int>();
        tasks.AsParallel().WithDegreeOfParallelism(DegreeOfParallelism).ForAll(task => listOfResults.Add(task.GetAwaiter().GetResult()));

        return listOfResults;
    }

    private int PerformOperation()
    {
        Thread.Sleep(10);
        var operation = 2 + 4;

        return operation;
    }
}
