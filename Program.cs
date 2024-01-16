// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using System.Net;
using System.Text;
using Enyim.Caching.Memcached;
using StackExchange.Redis;

Console.WriteLine("Hello, World!");


var sb = new StringBuilder();

for(var i = 0; i < 10000; i++)
{
    sb.Append("test");
}

var str = sb.ToString();

var cluster = new MemcachedCluster(new [] { new IPEndPoint(IPAddress.Loopback, 11211) });
cluster.Start();
var muxer = ConnectionMultiplexer.Connect("localhost");
var db = muxer.GetDatabase();
var client = cluster.GetClient();

var memcachedWriteTime = await Test("Memcached write", () => client.SetAsync("test", str, Expiration.Never));
var redisWriteTime = await Test("Redis write", () => db.StringSetAsync("test", str));
var memcachedReadTime = await Test("Memcached read", () => client.GetAsync("test"));
var redisReadTime = await Test("Redis read", () => db.StringGetAsync("test"));

Console.WriteLine($"Memcached writes took: {memcachedWriteTime}ms, Redis writes took: {redisWriteTime}ms");
Console.WriteLine($"Memcached reads took: {memcachedReadTime}ms, Redis Read took: {redisReadTime}ms");

static async Task<long> Test(string testName, Func<Task> invocation)
{
    var watch = Stopwatch.StartNew();
    var tasks = new Queue<Task>();
    Console.WriteLine($"commencing {testName}");
    for (var i = 0; i < 10_000; i++)
    {
        tasks.Enqueue(invocation());

        if (i > 0 && i % 1000 == 0)
        {
            Console.WriteLine($"{testName}: {i}");
        }

        if (tasks.Count > 100)
        {
            await tasks.Dequeue();
        }
    }

    await Task.WhenAll(tasks);
    watch.Stop();
    return watch.ElapsedMilliseconds;
}

cluster.Dispose();