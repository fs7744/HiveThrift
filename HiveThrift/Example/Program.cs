using Hive2;
using System;
using System.Collections.Generic;

namespace Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                using (var conn = new Connection("xx.xx.xx.xx", 10000, "xx", "xx", 
                    TProtocolVersion.HIVE_CLI_SERVICE_PROTOCOL_V6))
                {
                    var cursor = conn.GetCursor();
                    cursor.Execute("select * from table");
                    var list = cursor.FetchMany(100)[0] as IDictionary<string, object>;
                    foreach (var key in list.Keys)
                    {
                        Console.WriteLine(key + list[key].ToString());
                    }
                  
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }

        //public static void Execute(HiveClient client, string query)
        //{
        //    try
        //    {
        //        var operation = client.ExecuteStatement(query);
        //        var resultsSet = client.FetchResult(operation);
        //        foreach (var item in resultsSet.Columns)
        //        {
        //            if (item.StringVal.Values != null && item.StringVal.Values.Count > 0)
        //                foreach (var info in item.StringVal.Values)
        //                {
        //                    Console.WriteLine(info);
        //                }
        //        }
        //        client.CloseOperation(operation);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
    }
}
