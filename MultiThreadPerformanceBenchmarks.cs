using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;

namespace MultiThreadBenchmark;

public class MultiThreadPerformanceBenchmarks
{
    private readonly static HttpClient _httpClient = new();
    private const int Iterations = 4;

    [Benchmark]
    public List<User> NormalBenchmark()
    {
        var tasks = Enumerable.Range(0, Iterations).Select(_ => new Func<Task<User>>(() => GetUser(_httpClient)));

        var listOfUsers = new List<User>();
        tasks.ToList().ForEach(t => listOfUsers.Add(t().GetAwaiter().GetResult()));

        return listOfUsers;
    }

    [Benchmark]
    public List<User> ParallelBenchmark()
    {
        var tasks = Enumerable.Range(0, Iterations).Select(_ => new Func<User>(() => GetUser(_httpClient).GetAwaiter().GetResult()));

        var listOfUsers = new List<User>();
        Parallel.ForEach(tasks, new ParallelOptions
        {
            MaxDegreeOfParallelism = 4
        }, task => LockAndAdd(listOfUsers, task));

        return listOfUsers;
    }

    [Benchmark]
    public async Task<User[]> ConcurrentBenchmark()
    {
        var tasks = Enumerable.Range(0, Iterations).Select(_ => GetUser(_httpClient));

        var listOfUsers = await Task.WhenAll(tasks);

        return listOfUsers;
    }

    [Benchmark]
    public List<User> ConcurrentParallelBenchmark()
    {
        var tasks = Enumerable.Range(0, Iterations).Select(_ => new Func<User>(() => GetUser(_httpClient).GetAwaiter().GetResult()));

        var listOfUsers = new List<User>();
        tasks.AsParallel().ForAll(t => LockAndAdd(listOfUsers, t));

        return listOfUsers;
    }

    private async Task<User> GetUser(HttpClient httpClient)
    {
        var request = await _httpClient.GetStringAsync("http://localhost:3000/api/users/7");
        var response = JsonConvert.DeserializeObject<Response>(request);

        return response!.Data;
    }

    private void LockAndAdd(List<User> listOfUsers, Func<User> task)
    {
        lock (listOfUsers)
        {
            listOfUsers.Add(task());
        }
    }
}
