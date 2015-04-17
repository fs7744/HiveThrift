using System;
using Thrift.Protocol;
using Thrift.Transport;

namespace Hive2
{
    public static class Hive2Helper
    {
        public static void CheckStatus(this TStatus status)
        {
            if ((TStatusCode)status.ErrorCode == TStatusCode.ERROR_STATUS || (TStatusCode)status.ErrorCode == TStatusCode.INVALID_HANDLE_STATUS)
            {
                throw new Exception(status.ErrorMessage);
            }
        }
    }

    public class Connection : IDisposable
    {
        private TSaslClientTransport m_Transport;
        private TCLIService.Client m_Client;
        private TSessionHandle m_Session;
        private TProtocolVersion m_Version;

        public Connection(string host, int port, string userName = "None", string password = "None",
            TProtocolVersion version = TProtocolVersion.HIVE_CLI_SERVICE_PROTOCOL_V7)
        {
            var socket = new TSocket(host, port);
            m_Transport = new TSaslClientTransport(socket, userName, password);
            var protocol = new TBinaryProtocol(m_Transport);
            m_Client = new TCLIService.Client(protocol);
            m_Version = version;
        }

        ~Connection()
        {
            Dispose(false);
        }

        private TSessionHandle GetSession()
        {
            TOpenSessionReq openReq = new TOpenSessionReq(m_Version);
            TOpenSessionResp openResp = m_Client.OpenSession(openReq);
            openResp.Status.CheckStatus();
            return openResp.SessionHandle;
        }

        public void Open()
        {
            if (m_Transport != null && !m_Transport.IsOpen)
                m_Transport.Open();
            if (m_Session == null)
                m_Session = GetSession();
        }

        public void Close()
        {
            if (m_Session != null)
                CloseSession();
            if (m_Transport != null && m_Transport.IsOpen)
                m_Transport.Close();
        }

        private void CloseSession()
        {
            TCloseSessionReq closeSessionReq = new TCloseSessionReq();
            closeSessionReq.SessionHandle = m_Session;
            TCloseSessionResp closeSessionResp = m_Client.CloseSession(closeSessionReq);
            closeSessionResp.Status.CheckStatus();
        }

        public Cursor GetCursor()
        {
            Open();
            return new Cursor(m_Session, m_Client, m_Version);
        }

        protected virtual void Dispose(bool disposing)
        {
            Close();
            m_Client = null;
            m_Transport = null;
            m_Session = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}