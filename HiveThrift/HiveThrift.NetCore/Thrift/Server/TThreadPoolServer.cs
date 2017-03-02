using System;
using System.Reflection;
using System.Threading;
using Thrift;
using Thrift.Protocol;
using Thrift.Transport;

namespace Thrift.Server
{
    public class TThreadPoolServer : TServer
    {
        private const int DEFAULT_MIN_THREADS = 10;

        private const int DEFAULT_MAX_THREADS = 100;

        private volatile bool stop;

        public TThreadPoolServer(TProcessor processor, TServerTransport serverTransport) : this(processor, serverTransport, new TTransportFactory(), new TTransportFactory(), new TBinaryProtocol.Factory(), new TBinaryProtocol.Factory(), 10, 100, new TServer.LogDelegate(TServer.DefaultLogDelegate))
        {
        }

        public TThreadPoolServer(TProcessor processor, TServerTransport serverTransport, TServer.LogDelegate logDelegate) : this(processor, serverTransport, new TTransportFactory(), new TTransportFactory(), new TBinaryProtocol.Factory(), new TBinaryProtocol.Factory(), 10, 100, logDelegate)
        {
        }

        public TThreadPoolServer(TProcessor processor, TServerTransport serverTransport, TTransportFactory transportFactory, TProtocolFactory protocolFactory) : this(processor, serverTransport, transportFactory, transportFactory, protocolFactory, protocolFactory, 10, 100, new TServer.LogDelegate(TServer.DefaultLogDelegate))
        {
        }

        public TThreadPoolServer(TProcessor processor, TServerTransport serverTransport, TTransportFactory inputTransportFactory, TTransportFactory outputTransportFactory, TProtocolFactory inputProtocolFactory, TProtocolFactory outputProtocolFactory, int minThreadPoolThreads, int maxThreadPoolThreads, TServer.LogDelegate logDel) : base(processor, serverTransport, inputTransportFactory, outputTransportFactory, inputProtocolFactory, outputProtocolFactory, logDel)
        {
            //lock (typeof(TThreadPoolServer))
            //{
            //	if (!ThreadPool.SetMinThreads(minThreadPoolThreads, minThreadPoolThreads))
            //	{
            //		throw new Exception("Error: could not SetMinThreads in ThreadPool");
            //	}
            //	if (!ThreadPool.SetMaxThreads(maxThreadPoolThreads, maxThreadPoolThreads))
            //	{
            //		throw new Exception("Error: could not SetMaxThreads in ThreadPool");
            //	}
            //}
        }

        private void Execute(object threadContext)
        {
            TTransport tTransport = (TTransport)threadContext;
            TTransport transport = null;
            TTransport transport1 = null;
            TProtocol protocol = null;
            TProtocol tProtocol = null;
            try
            {
                transport = this.inputTransportFactory.GetTransport(tTransport);
                transport1 = this.outputTransportFactory.GetTransport(tTransport);
                protocol = this.inputProtocolFactory.GetProtocol(transport);
                tProtocol = this.outputProtocolFactory.GetProtocol(transport1);
                while (this.processor.Process(protocol, tProtocol))
                {
                }
            }
            catch (TTransportException tTransportException)
            {
            }
            catch (Exception exception)
            {
                this.logDelegate(string.Concat("Error: ", exception));
            }
            if (transport != null)
            {
                transport.Close();
            }
            if (transport1 != null)
            {
                transport1.Close();
            }
        }

        public override void Serve()
        {
            try
            {
                this.serverTransport.Listen();
            }
            catch (TTransportException tTransportException)
            {
                this.logDelegate(string.Concat("Error, could not listen on ServerTransport: ", tTransportException));
                return;
            }
            while (!this.stop)
            {
                int num = 0;
                try
                {
                    TTransport tTransport = this.serverTransport.Accept();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.Execute), tTransport);
                }
                catch (TTransportException tTransportException2)
                {
                    TTransportException tTransportException1 = tTransportException2;
                    if (!this.stop)
                    {
                        num++;
                        this.logDelegate(tTransportException1.ToString());
                    }
                    else
                    {
                        this.logDelegate(string.Concat("TThreadPoolServer was shutting down, caught ", tTransportException1.GetType().Name));
                    }
                }
            }
            if (this.stop)
            {
                try
                {
                    this.serverTransport.Close();
                }
                catch (TTransportException tTransportException3)
                {
                    this.logDelegate(string.Concat("TServerTransport failed on close: ", tTransportException3.Message));
                }
                this.stop = false;
            }
        }

        public override void Stop()
        {
            this.stop = true;
            this.serverTransport.Close();
        }
    }
}