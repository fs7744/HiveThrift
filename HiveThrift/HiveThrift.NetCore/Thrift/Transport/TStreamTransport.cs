using System;
using System.IO;

namespace Thrift.Transport
{
    public class TStreamTransport : TTransport
    {
        protected Stream inputStream;

        protected Stream outputStream;

        private bool _IsDisposed;

        public Stream InputStream
        {
            get
            {
                return this.inputStream;
            }
        }

        public override bool IsOpen
        {
            get
            {
                return true;
            }
        }

        public Stream OutputStream
        {
            get
            {
                return this.outputStream;
            }
        }

        public TStreamTransport()
        {
        }

        public TStreamTransport(Stream inputStream, Stream outputStream)
        {
            this.inputStream = inputStream;
            this.outputStream = outputStream;
        }

        public override void Close()
        {
            if (this.inputStream != null)
            {
                this.inputStream.Dispose();
                this.inputStream = null;
            }
            if (this.outputStream != null)
            {
                this.outputStream.Dispose();
                this.outputStream = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this._IsDisposed && disposing)
            {
                if (this.InputStream != null)
                {
                    this.InputStream.Dispose();
                }
                if (this.OutputStream != null)
                {
                    this.OutputStream.Dispose();
                }
            }
            this._IsDisposed = true;
        }

        public override void Flush()
        {
            if (this.outputStream == null)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "Cannot flush null outputstream");
            }
            this.outputStream.Flush();
        }

        public override void Open()
        {
        }

        public override int Read(byte[] buf, int off, int len)
        {
            if (this.inputStream == null)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "Cannot read from null inputstream");
            }
            return this.inputStream.Read(buf, off, len);
        }

        public override void Write(byte[] buf, int off, int len)
        {
            if (this.outputStream == null)
            {
                throw new TTransportException(TTransportException.ExceptionType.NotOpen, "Cannot write to null outputstream");
            }
            this.outputStream.Write(buf, off, len);
        }
    }
}