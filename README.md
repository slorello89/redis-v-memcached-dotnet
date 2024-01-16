# Redis v. Memcached .NET benchmarks

This project contains an example of running Redis against memcached with some basic usage (e.g. sets /gets) with relatively large payloads (50kb) and running them through 10k times both read and write on each platform.

## How to run

You can bring your own memcached/Redis deployment and configure the clients within the code itself, or you can just run both of them locally:

```sh
docker run -d -p 6379:6379 redis/redis-stack-server
docker run -d -p 11211:11211 memcached
```

When both Redis and Memcached are up and running, you can run this test with:

```sh
dotnet run
```