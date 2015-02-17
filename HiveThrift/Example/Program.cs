using Hive2;
using System;
using Thrift.Protocol;
using Thrift.Transport;

namespace Example
{
    public class HiveClient
    {
        private TSaslClientTransport transport;
        public TCLIService.Client client;
        private TBinaryProtocol protocol;

        public HiveClient(string host, int port, string userName = "None", string password = "None")
        {
            var socket = new TSocket(host, port);
            transport = new TSaslClientTransport(socket, userName, password);
            protocol = new TBinaryProtocol(transport);
            client = new TCLIService.Client(protocol);
        }

        public void Open()
        {
            if (transport != null && !transport.IsOpen)
                transport.Open();
        }

        public object Execute(string query)
        {
            object result = null;
            try
            {
                TOpenSessionReq openReq = new TOpenSessionReq();
                TOpenSessionResp openResp = client.OpenSession(openReq);
                CheckStatus(openResp.Status);

                TSessionHandle sessHandle = openResp.SessionHandle;

                openReq = new TOpenSessionReq();
                openResp = client.OpenSession(openReq);
                CheckStatus(openResp.Status);

                sessHandle = openResp.SessionHandle;

                TExecuteStatementReq execReq = new TExecuteStatementReq();
                execReq.SessionHandle = sessHandle;
                execReq.Statement = query;
                TExecuteStatementResp execResp = client.ExecuteStatement(execReq);
                CheckStatus(execResp.Status);

                TOperationHandle stmtHandle = execResp.OperationHandle;
                TFetchResultsReq fetchReq = new TFetchResultsReq();
                fetchReq.MaxRows = int.MaxValue;
                fetchReq.OperationHandle = stmtHandle;
                fetchReq.Orientation = TFetchOrientation.FETCH_NEXT;
                TFetchResultsResp resultsResp = client.FetchResults(fetchReq);
                CheckStatus(resultsResp.Status);

                TRowSet resultsSet = resultsResp.Results;
                result = resultsSet;
                foreach (var item in resultsSet.Columns)
                {
                    if (item.StringVal.Values != null && item.StringVal.Values.Count > 0)
                        foreach (var info in item.StringVal.Values)
                        {
                            Console.WriteLine(info);
                        }
                }

                //TGetResultSetMetadataReq schemasReq = new TGetResultSetMetadataReq();
                //schemasReq.OperationHandle = stmtHandle;
                //TGetResultSetMetadataResp schemasResp = client.GetResultSetMetadata(schemasReq);
                //CheckStatus(schemasResp.Status);
                //result.Schemas = schemasResp.Schema.Columns;

                TCloseOperationReq closeReq = new TCloseOperationReq();
                closeReq.OperationHandle = stmtHandle;
                TCloseOperationResp closeOperationResp = client.CloseOperation(closeReq);
                CheckStatus(closeOperationResp.Status);

                TCloseSessionReq closeSessionReq = new TCloseSessionReq();
                closeSessionReq.SessionHandle = sessHandle;
                TCloseSessionResp closeSessionResp = client.CloseSession(closeSessionReq);
                CheckStatus(closeSessionResp.Status);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public static void CheckStatus(TStatus status)
        {
            if ((TStatusCode)status.ErrorCode == TStatusCode.ERROR_STATUS || (TStatusCode)status.ErrorCode == TStatusCode.INVALID_HANDLE_STATUS)
            {
                throw new Exception(status.ErrorMessage);
            }
        }

        public void Close()
        {
            if (transport != null && transport.IsOpen)
                transport.Close();
        }

        public static string Way { get { return "Hive2"; } }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var client = new HiveClient("10.1.54.42", 10000);
            client.Open();
            //client.Execute("show databases");
            client.Execute("use lz33");
            client.Execute("show tables");
            client.Close();
            Console.ReadLine();
        }
    }
}