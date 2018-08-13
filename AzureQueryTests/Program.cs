using AzureStorage.V2.Helpers.Context;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AzureQueryTests
{
    internal class Program
    {
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        private static async Task Main(string[] args)
        {
            //   DoWuery().Wait();
            //CreateWordNumTHing(5000);
            await BulkCreateWordNumThing(5000);
        }

        private static async Task DoWuery()
        {
            var ctx = new CloudStorageContext("UseDevelopmentStorage=true");
            var numWords = ctx.CreateTableHelper("xpclNumWords");
            var startDate = DateTime.Today.AddDays(10);
            var endDate = startDate.AddDays(100);
            var qry = SpreadQry(startDate, endDate);
            var tStampQry = SpreadQryTimestamp(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));

            var results = await numWords.EntityQuery<NumbersAndWords>("", 1500, 43);
            Console.WriteLine(results.ToList().Count());
        }

        private static string SpreadQry(DateTime startDate, DateTime endDate)
        {
            return TableQuery.CombineFilters(
                                                     TableQuery.GenerateFilterConditionForDate("Published", QueryComparisons.GreaterThanOrEqual, startDate),
                                                     TableOperators.And,
                                                     TableQuery.GenerateFilterConditionForDate("Published", QueryComparisons.LessThanOrEqual, endDate));
        }

        private static string SpreadQryTimestamp(DateTime startDate, DateTime endDate)
        {
            return TableQuery.CombineFilters(
                                                     TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, startDate),
                                                     TableOperators.And,
                                                     TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThanOrEqual, endDate));
        }

        private static async Task BulkCreateWordNumThing(int limit)
        {
            var currentDate = DateTime.Today;
            var WordNumThings = new List<NumbersAndWords>();
            for (var x = 0; x < limit; ++x)
            {
                var cryptoData = new byte[sizeof(int)];
                rngCsp.GetBytes(cryptoData);

                var number = BitConverter.ToInt32(cryptoData, 0) & (int.MaxValue - 1);
                var numberWord = NumToWor.NumbersToWords(number);
                WordNumThings.Add(new NumbersAndWords(number, numberWord) { Published = currentDate });
                currentDate = currentDate.AddDays(1);
            }

            var ctx = new CloudStorageContext("UseDevelopmentStorage=true");
            var numWords = ctx.CreateTableHelper("xpclNumWords");
            await numWords.InsertBulkToTable(WordNumThings);
        }

        private static void CreateWordNumTHing(int limit)
        {
            var currentDate = DateTime.Today;
            var wordNumbTHing = new List<NumbersAndWords>();
            for (var x = 0; x < limit; ++x)
            {
                var cryptoData = new byte[sizeof(int)];
                rngCsp.GetBytes(cryptoData);

                var number = BitConverter.ToInt32(cryptoData, 0) & (int.MaxValue - 1);
                var numberWord = NumToWor.NumbersToWords(number);
                wordNumbTHing.Add(new NumbersAndWords(number, numberWord) { Published = currentDate });
                currentDate = currentDate.AddDays(1);
            }

            var ctx = new CloudStorageContext("UseDevelopmentStorage=true");
            var numWords = ctx.CreateTableHelper("xpclNumWords");
            var tasks = new List<Task>();

            wordNumbTHing.ForEach(a => tasks.Add(numWords.InsertToTable(a)));
            var task1 = tasks.Skip(0).Take(1000).ToList();
            var task2 = tasks.Skip(1000).Take(1000);
            var task3 = tasks.Skip(2000).Take(1000);
            var task4 = tasks.Skip(3000).Take(1000);
            var task5 = tasks.Skip(4000).Take(1000); 


            Task.WaitAll(task1.ToArray());
            Task.WaitAll(task2.ToArray());
            Task.WaitAll(task3.ToArray());
            Task.WaitAll(task4.ToArray());
            Task.WaitAll(task5.ToArray());

            Console.WriteLine($"Did it");
        }
    }
}
