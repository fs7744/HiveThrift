using System;
using Thrift.Protocol;
using Thrift.Transport;

namespace Hive2
{
    public class HiveClient
    {
        private TSaslClientTransport transport;
        public TCLIService.Client client;
        private TBinaryProtocol protocol;
        private TSessionHandle m_Session;

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
            if (m_Session == null)
                m_Session = GetSession();
        }

        public void Close()
        {
            if (m_Session != null)
                CloseSession();
            if (transport != null && transport.IsOpen)
                transport.Close();
        }

        private void CloseSession()
        {
            TCloseSessionReq closeSessionReq = new TCloseSessionReq();
            closeSessionReq.SessionHandle = m_Session;
            TCloseSessionResp closeSessionResp = client.CloseSession(closeSessionReq);
            CheckStatus(closeSessionResp.Status);
        }

        public static void CheckStatus(TStatus status)
        {
            if ((TStatusCode)status.ErrorCode == TStatusCode.ERROR_STATUS || (TStatusCode)status.ErrorCode == TStatusCode.INVALID_HANDLE_STATUS)
            {
                throw new Exception(status.ErrorMessage);
            }
        }

        private TSessionHandle GetSession()
        {
            TOpenSessionReq openReq = new TOpenSessionReq();
            TOpenSessionResp openResp = client.OpenSession(openReq);
            CheckStatus(openResp.Status);
            return openResp.SessionHandle;
        }

        public TOperationHandle ExecuteStatement(string statement)
        {
            TExecuteStatementReq execReq = new TExecuteStatementReq();
            execReq.SessionHandle = m_Session;
            execReq.Statement = statement;
            TExecuteStatementResp execResp = client.ExecuteStatement(execReq);
            CheckStatus(execResp.Status);
            return execResp.OperationHandle;
        }

        public void ExecuteStatementNoResult(string statement)
        {
            var operation = ExecuteStatement(statement);
            CloseOperation(operation);
        }

        public TRowSet FetchResult(TOperationHandle operationHandle, TFetchResultsReq req = null)
        {
            if (req == null)
            {
                req = new TFetchResultsReq();
                req.MaxRows = int.MaxValue;
                req.Orientation = TFetchOrientation.FETCH_NEXT;
            }
            req.OperationHandle = operationHandle;
            TFetchResultsResp resultsResp = client.FetchResults(req);
            CheckStatus(resultsResp.Status);
            return resultsResp.Results;
        }

        public void CloseOperation(TOperationHandle operationHandle)
        {
            TCloseOperationReq closeReq = new TCloseOperationReq();
            closeReq.OperationHandle = operationHandle;
            TCloseOperationResp closeOperationResp = client.CloseOperation(closeReq);
            CheckStatus(closeOperationResp.Status);
        }
    }
}