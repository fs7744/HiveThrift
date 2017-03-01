using System;
using System.Net.Sockets;
using System.Threading;

namespace Thrift.Transport
{
    public class TSocket : TStreamTransport
    {
        private System.Net.Sockets.TcpClient client;

        private string host;

        private int port;

        private int timeout;

        private bool _IsDisposed;

        public string Host
        {
            get
            {
                return this.host;
            }
        }

        public override bool IsOpen
        {
            get
            {
                if (this.client == null)
                {
                    return false;
                }
                return this.client.Connected;
            }
        }

        public int Port
        {
            get
            {
                return this.port;
            }
        }

        public System.Net.Sockets.TcpClient TcpClient
        {
            get
            {
                return this.client;
            }
        }

        public int Timeout
        {
            set
            {
                System.Net.Sockets.TcpClient tcpClient = this.client;
                int num = value;
                int num1 = num;
                this.timeout = num;
                int num2 = num1;
                int num3 = num2;
                this.client.SendTimeout = num2;
                tcpClient.ReceiveTimeout = num3;
            }
        }

        public TSocket(System.Net.Sockets.TcpClient client)
        {
            this.client = client;
            if (this.IsOpen)
            {
                this.inputStream = client.GetStream();
                this.outputStream = client.GetStream();
            }
        }

        public TSocket(string host, int port) : this(host, port, 0)
        {
        }

        public TSocket(string host, int port, int timeout)
        {
            this.host = host;
            this.port = port;
            this.timeout = timeout;
            this.InitSocket();
        }

        public override void Close()
        {
            base.Close();
            if (this.client != null)
            {
                this.client.Dispose();
                this.client = null;
            }
        }

        private static void ConnectCallback(IAsyncResult asyncres)
        {
            TSocket.ConnectHelper asyncState = asyncres.AsyncState as TSocket.ConnectHelper;
            lock (asyncState.Mutex)
            {
                asyncState.CallbackDone = true;
                try
                {
                    if (asyncState.Client.Client != null)
                    {
                        asyncState.Client.Dispose();
                    }
                }
                catch (SocketException socketException)
                {
                }
                if (asyncState.DoCleanup)
                {
                    asyncres.AsyncWaitHandle.Dispose();
                    if (asyncState.Client != null)
                    {
                        ((IDisposable)asyncState.Client).Dispose();
                    }
                    asyncState.Client = null;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this._IsDisposed && disposing)
            {
                if (this.client != null)
                {
                    ((IDisposable)this.client).Dispose();
                }
                base.Dispose(disposing);
            }
            this._IsDisposed = true;
        }

        private void InitSocket()
        {
            this.client = new System.Net.Sockets.TcpClient();
            System.Net.Sockets.TcpClient tcpClient = this.client;
            System.Net.Sockets.TcpClient tcpClient1 = this.client;
            int num = this.timeout;
            int num1 = num;
            tcpClient1.SendTimeout = num;
            tcpClient.ReceiveTimeout = num1;
            this.client.Client.NoDelay = true;
        }

        public override void Open()
        {
            if (this.IsOpen)
            {
                throw new TTransportException(TTransportException.ExceptionType.AlreadyOpen, "Socket already connected");
            }
            if (string.IsNullOrEmpty(this.host))
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "Cannot open null host");
            }
            if (this.port <= 0)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "Cannot open without port");
            }
            if (this.client == null)
            {
                this.InitSocket();
            }
            if (this.timeout != 0)
            {
                TSocket.ConnectHelper connectHelper = new TSocket.ConnectHelper(this.client);
                var t = this.client.ConnectAsync(this.host, this.port);
                t.Wait();
                //, new AsyncCallback(TSocket.ConnectCallback), connectHelper);
                //if ((!asyncResult.AsyncWaitHandle.WaitOne(this.timeout) ? true : !this.client.Connected))
                //{
                //	lock (connectHelper.Mutex)
                //	{
                //		if (!connectHelper.CallbackDone)
                //		{
                //			connectHelper.DoCleanup = true;
                //			this.client = null;
                //		}
                //		else
                //		{
                //			asyncResult.AsyncWaitHandle.Dispose();
                //			this.client.Dispose();
                //		}
                //	}
                //	throw new TTransportException(TTransportException.ExceptionType.TimedOut, "Connect timed out");
                //}
                //else
                //{
                //                 this.client.ConnectAsync(this.host, this.port);
                //}
                this.inputStream = this.client.GetStream();
                this.outputStream = this.client.GetStream();
            }
        }

        private class ConnectHelper
        {
            public object Mutex;

            public bool DoCleanup;

            public bool CallbackDone;

            public System.Net.Sockets.TcpClient Client;

            public ConnectHelper(System.Net.Sockets.TcpClient client)
            {
                this.Client = client;
            }
        }
    }
}