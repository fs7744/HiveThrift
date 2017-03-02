using System;
using System.Net;
using System.Net.Sockets;

namespace Thrift.Transport
{
    public class TServerSocket : TServerTransport
    {
        private TcpListener server;

        private int port;

        private int clientTimeout;

        private bool useBufferedSockets;

        public TServerSocket(TcpListener listener) : this(listener, 0)
        {
        }

        public TServerSocket(TcpListener listener, int clientTimeout)
        {
            this.server = listener;
            this.clientTimeout = clientTimeout;
        }

        public TServerSocket(int port) : this(port, 0)
        {
        }

        public TServerSocket(int port, int clientTimeout) : this(port, clientTimeout, false)
        {
        }

        public TServerSocket(int port, int clientTimeout, bool useBufferedSockets)
        {
            this.port = port;
            this.clientTimeout = clientTimeout;
            this.useBufferedSockets = useBufferedSockets;
            try
            {
                this.server = new TcpListener(IPAddress.Any, this.port);
                this.server.Server.NoDelay = true;
            }
            catch (Exception exception)
            {
                this.server = null;
                throw new TTransportException(string.Concat("Could not create ServerSocket on port ", port, "."));
            }
        }

        protected override TTransport AcceptImpl()
        {
            TTransport tBufferedTransport;
            if (this.server == null)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "No underlying server socket.");
            }
            try
            {
                TSocket tSocket = null;
                var t = this.server.AcceptTcpClientAsync();
                t.Wait();
                TcpClient tcpClient = t.Result;
                try
                {
                    tSocket = new TSocket(tcpClient)
                    {
                        Timeout = this.clientTimeout
                    };
                    if (!this.useBufferedSockets)
                    {
                        tBufferedTransport = tSocket;
                    }
                    else
                    {
                        tBufferedTransport = new TBufferedTransport(tSocket);
                    }
                }
                catch (Exception exception)
                {
                    if (tSocket == null)
                    {
                        ((IDisposable)tcpClient).Dispose();
                    }
                    else
                    {
                        tSocket.Dispose();
                    }
                    throw;
                }
            }
            catch (Exception exception1)
            {
                throw new TTransportException(exception1.ToString());
            }
            return tBufferedTransport;
        }

        public override void Close()
        {
            if (this.server != null)
            {
                try
                {
                    this.server.Stop();
                }
                catch (Exception exception)
                {
                    throw new TTransportException(string.Concat("WARNING: Could not close server socket: ", exception));
                }
                this.server = null;
            }
        }

        public override void Listen()
        {
            if (this.server != null)
            {
                try
                {
                    this.server.Start();
                }
                catch (SocketException socketException)
                {
                    throw new TTransportException(string.Concat("Could not accept on listening socket: ", socketException.Message));
                }
            }
        }
    }
}