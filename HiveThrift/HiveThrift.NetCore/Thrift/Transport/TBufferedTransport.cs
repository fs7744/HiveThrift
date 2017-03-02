using System;
using System.IO;

namespace Thrift.Transport
{
    public class TBufferedTransport : TTransport, IDisposable
    {
        private BufferedStream inputBuffer;

        private BufferedStream outputBuffer;

        private int bufSize;

        private TStreamTransport transport;

        private bool _IsDisposed;

        public override bool IsOpen
        {
            get
            {
                return this.transport.IsOpen;
            }
        }

        public TTransport UnderlyingTransport
        {
            get
            {
                return this.transport;
            }
        }

        public TBufferedTransport(TStreamTransport transport) : this(transport, 1024)
        {
        }

        public TBufferedTransport(TStreamTransport transport, int bufSize)
        {
            this.bufSize = bufSize;
            this.transport = transport;
            this.InitBuffers();
        }

        public override void Close()
        {
            this.CloseBuffers();
            this.transport.Close();
        }

        private void CloseBuffers()
        {
            if (this.inputBuffer != null && this.inputBuffer.CanRead)
            {
                this.inputBuffer.Dispose();
            }
            if (this.outputBuffer != null && this.outputBuffer.CanWrite)
            {
                this.outputBuffer.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this._IsDisposed && disposing)
            {
                if (this.inputBuffer != null)
                {
                    this.inputBuffer.Dispose();
                }
                if (this.outputBuffer != null)
                {
                    this.outputBuffer.Dispose();
                }
            }
            this._IsDisposed = true;
        }

        public override void Flush()
        {
            this.outputBuffer.Flush();
        }

        private void InitBuffers()
        {
            if (this.transport.InputStream != null)
            {
                this.inputBuffer = new BufferedStream(this.transport.InputStream, this.bufSize);
            }
            if (this.transport.OutputStream != null)
            {
                this.outputBuffer = new BufferedStream(this.transport.OutputStream, this.bufSize);
            }
        }

        public override void Open()
        {
            this.transport.Open();
            this.InitBuffers();
        }

        public override int Read(byte[] buf, int off, int len)
        {
            return this.inputBuffer.Read(buf, off, len);
        }

        public override void Write(byte[] buf, int off, int len)
        {
            this.outputBuffer.Write(buf, off, len);
        }
    }
}