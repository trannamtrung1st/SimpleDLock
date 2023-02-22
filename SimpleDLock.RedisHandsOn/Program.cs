using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SimpleDLock.RedisHandsOn
{
    internal class Program
    {
        const string MyKey = "SE130097";
        const string CachingKey = MyKey + "-cache";
        const string ConnectionString = "Server=localhost,1434;Database=DLock_HandsOn;Trusted_Connection=False;User Id=readonly;Password=123456";
        const string RedisEndPoint = "localhost";

        static void Main(string[] args)
        {
            var multiplexer = GetConnectionMultiplexer();
            var database = multiplexer.GetDatabase();

            WorkingWithStrings(database);

            WorkingWithList(database);

            WorkingWithSets(database);

            CacheData(database);

            QueryDataNoCache();
        }

        static ConnectionMultiplexer GetConnectionMultiplexer()
        {
            var cfg = new ConfigurationOptions()
            {
                AllowAdmin = true,
            };

            cfg.EndPoints.Add(RedisEndPoint);

            var connection = ConnectionMultiplexer.Connect(cfg);

            return connection;
        }

        static void WorkingWithStrings(IDatabase database)
        {
            Console.WriteLine("------------------");
            Console.WriteLine("WorkingWithStrings");
            Console.WriteLine("------------------");

            database.KeyDelete(MyKey);

            database.StringSet(MyKey, "Hello");

            var myString = database.StringGet(MyKey);

            Console.WriteLine(myString);
            Console.WriteLine();
        }

        static void WorkingWithList(IDatabase database)
        {
            Console.WriteLine("------------------");
            Console.WriteLine("WorkingWithList");
            Console.WriteLine("------------------");

            database.KeyDelete(MyKey);

            database.ListRightPush(MyKey, 1);
            database.ListRightPush(MyKey, 2);
            database.ListRightPush(MyKey, 3);
            database.ListRightPush(MyKey, 4);
            database.ListRightPush(MyKey, 5);

            var myList = database.ListRange(MyKey);

            foreach (var item in myList)
            {
                Console.Write(item + " ");
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        static void WorkingWithSets(IDatabase database)
        {
            Console.WriteLine("------------------");
            Console.WriteLine("WorkingWithSets");
            Console.WriteLine("------------------");

            database.KeyDelete(MyKey);

            database.SetAdd(MyKey, "A");
            database.SetAdd(MyKey, "B");
            database.SetAdd(MyKey, "A");
            database.SetAdd(MyKey, "C");
            database.SetAdd(MyKey, "C");
            database.SetAdd(MyKey, "D");
            database.SetAdd(MyKey, "F");
            database.SetAdd(MyKey, "E");

            var mySetMembers = database.SetMembers(MyKey);
            var mySetLength = database.SetLength(MyKey);

            Console.WriteLine($"Total members: {mySetLength}");

            foreach (var item in mySetMembers)
            {
                Console.Write(item + " ");
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        static void CacheData(IDatabase database)
        {
            Console.WriteLine("------------------");
            Console.WriteLine("CacheData");
            Console.WriteLine("------------------");

            HashEntry[] resources;
            var watch = Stopwatch.StartNew();

            if (!database.KeyExists(CachingKey))
            {
                resources = GetResources();

                database.HashSet(CachingKey, resources);
            }
            else
            {
                resources = database.HashGetAll(CachingKey);
            }

            watch.Stop();

            Console.WriteLine($"Run in {watch.ElapsedMilliseconds}ms");

            foreach (var entry in resources)
            {
                Console.WriteLine(entry);
            }

            Console.WriteLine();
        }

        static void QueryDataNoCache()
        {
            Console.WriteLine("------------------");
            Console.WriteLine("QueryDataNoCache");
            Console.WriteLine("------------------");

            var watch = Stopwatch.StartNew();

            HashEntry[] resources = GetResources();

            watch.Stop();

            Console.WriteLine($"Run in {watch.ElapsedMilliseconds}ms");

            foreach (var entry in resources)
            {
                Console.WriteLine(entry);
            }

            Console.WriteLine();
        }

        static HashEntry[] GetResources()
        {
            var resources = new List<HashEntry>();

            string queryString = "SELECT Name, Value FROM Resources;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                Console.WriteLine("Querying database ...");

                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resources.Add(new HashEntry((string)reader[0], (string)reader[1]));
                    }
                }
            }

            return resources.ToArray();
        }
    }
}
