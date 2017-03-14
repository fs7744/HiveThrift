using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Thrift;
using Thrift.Collections;
using Thrift.Protocol;
using Thrift.Transport;

namespace Thrift.Server
{
    public class TThreadedServer : TServer
    {
        private const int DEFAULT_MAX_THREADS = 100;

        private volatile bool stop;

        private readonly int maxThreads;

        private Queue<TTransport> clientQueue;

        private THashSet<Thread> clientThreads;

        private object clientLock;

        private Thread workerThread;

        public TThreadedServer(TProcessor processor, TServerTransport serverTransport) : this(processor, serverTransport, new TTransportFactory(), new TTransportFactory(), new TBinaryProtocol.Factory(), new TBinaryProtocol.Factory(), 100, new TServer.LogDelegate(TServer.DefaultLogDelegate))
        {
        }

        public TThreadedServer(TProcessor processor, TServerTransport serverTransport, TServer.LogDelegate logDelegate) : this(processor, serverTransport, new TTransportFactory(), new TTransportFactory(), new TBinaryProtocol.Factory(), new TBinaryProtocol.Factory(), 100, logDelegate)
        {
        }

        public TThreadedServer(TProcessor processor, TServerTransport serverTransport, TTransportFactory transportFactory, TProtocolFactory protocolFactory) : this(processor, serverTransport, transportFactory, transportFactory, protocolFactory, protocolFactory, 100, new TServer.LogDelegate(TServer.DefaultLogDelegate))
        {
        }

        public TThreadedServer(TProcessor processor, TServerTransport serverTransport, TTransportFactory inputTransportFactory, TTransportFactory outputTransportFactory, TProtocolFactory inputProtocolFactory, TProtocolFactory outputProtocolFactory, int maxThreads, TServer.LogDelegate logDel) : base(processor, serverTransport, inputTransportFactory, outputTransportFactory, inputProtocolFactory, outputProtocolFactory, logDel)
        {
            this.maxThreads = maxThreads;
            this.clientQueue = new Queue<TTransport>();
            this.clientLock = new object();
            this.clientThreads = new THashSet<Thread>();
        }

        private void ClientWorker(object context)
        {
            TTransport tTransport = (TTransport)context;
            TTransport tTransport1 = null;
            TTransport tTransport2 = null;
            TProtocol protocol = null;
            TProtocol tProtocol = null;
            try
            {
                TTransport transport = this.inputTransportFactory.GetTransport(tTransport);
                tTransport1 = transport;
                using (transport)
                {
                    TTransport transport1 = this.outputTransportFactory.GetTransport(tTransport);
                    tTransport2 = transport1;
                    using (transport1)
                    {
                        protocol = this.inputProtocolFactory.GetProtocol(tTransport1);
                        tProtocol = this.outputProtocolFactory.GetProtocol(tTransport2);
                        while (this.processor.Process(protocol, tProtocol))
                        {
                        }
                    }
                }
            }
            catch (TTransportException tTransportException)
            {
            }
            catch (Exception exception)
            {
                this.logDelegate(string.Concat("Error: ", exception));
            }
            lock (this.clientLock)
            {
                this.clientThreads.Remove(Thread.CurrentThread);
                Monitor.Pulse(this.clientLock);
            }
        }

        private void Execute()
        {
            TTransport tTransport;
            Thread thread;
            while (!this.stop)
            {
                lock (this.clientLock)
                {
                    while (this.clientThreads.Count >= this.maxThreads)
                    {
                        Monitor.Wait(this.clientLock);
                    }
                    while (this.clientQueue.Count == 0)
                    {
                        Monitor.Wait(this.clientLock);
                    }
                    tTransport = this.clientQueue.Dequeue();
                    thread = new Thread(new ParameterizedThreadStart(this.ClientWorker));
                    this.clientThreads.Add(thread);
                }
                thread.Start(tTransport);
            }
        }

        public override void Serve()
        {
            try
            {
                this.workerThread = new Thread(new ThreadStart(this.Execute));
                this.workerThread.Start();
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
                    lock (this.clientLock)
                    {
                        this.clientQueue.Enqueue(tTransport);
                        Monitor.Pulse(this.clientLock);
                    }
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
                        this.logDelegate(string.Concat("TThreadPoolServer was shutting down, caught ", tTransportException1));
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
                    this.logDelegate(string.Concat("TServeTransport failed on close: ", tTransportException3.Message));
                }
                this.stop = false;
            }
        }

        public override void Stop()
        {
            this.stop = true;
            this.serverTransport.Close();
            this.workerThread.Join();
            foreach (Thread clientThread in this.clientThreads)
            {
                clientThread.Join();
            }
        }
    }
}