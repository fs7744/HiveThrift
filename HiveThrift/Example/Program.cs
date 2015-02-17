using Hive2;
using System;

namespace Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var client = new HiveClient("10.1.54.42", 10000);
            client.Open();
            //Execute(client,"show databases");
            Execute(client, "use lz33");
            Execute(client, "show tables");
            client.Close();
            Console.ReadLine();
        }

        public static void Execute(HiveClient client, string query)
        {
            try
            {
                var operation = client.ExecuteStatement(query);
                var resultsSet = client.FetchResult(operation);
                foreach (var item in resultsSet.Columns)
                {
                    if (item.StringVal.Values != null && item.StringVal.Values.Count > 0)
                        foreach (var info in item.StringVal.Values)
                        {
                            Console.WriteLine(info);
                        }
                }
                client.CloseOperation(operation);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}