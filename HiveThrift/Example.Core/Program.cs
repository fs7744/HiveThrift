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
                using (var conn = new Connection("X.X.X.X", 10000, "hadoop", "hadoop"))
                {
                    var cursor = conn.GetCursor();
                    cursor.Execute("use default");
                    var list = cursor.FetchMany(100);
                    if (!list.IsEmpty())
                    {
                        var dict = list[0] as IDictionary<string, object>;
                        foreach (var key in dict.Keys)
                        {
                            Console.WriteLine(key + dict[key].ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("no result");
                    }
                    cursor.Execute("select * from XXX");
                    var list2 = cursor.FetchMany(100);
                    if (!list2.IsEmpty())
                    {
                        var dict = list2[0] as IDictionary<string, object>;
                        foreach (var key in dict.Keys)
                        {
                            Console.WriteLine(key + dict[key].ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("no result");
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