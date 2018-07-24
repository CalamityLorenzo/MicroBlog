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

        private static void Main(string[] args)
        {
            DoWuery().Wait();


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

        private static void CreateWordNumTHing()
        {
            var currentDate = DateTime.Today;
            var wordNumbTHing = new List<NumbersAndWords>();
            for (var x = 0; x < 2500; ++x)
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

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Did it");
        }
    }
}
