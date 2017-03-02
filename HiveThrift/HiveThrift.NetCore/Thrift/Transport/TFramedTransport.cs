using System;
using System.IO;

namespace Thrift.Transport
{
    public class TFramedTransport : TTransport, IDisposable
    {
        private const int header_size = 4;

        protected TTransport transport;

        protected MemoryStream writeBuffer;

        protected MemoryStream readBuffer;

        private static byte[] header_dummy;

        private bool _IsDisposed;

        public override bool IsOpen
        {
            get
            {
                return this.transport.IsOpen;
            }
        }

        static TFramedTransport()
        {
            TFramedTransport.header_dummy = new byte[4];
        }

        public TFramedTransport()
        {
            this.InitWriteBuffer();
        }

        public TFramedTransport(TTransport transport) : this()
        {
            this.transport = transport;
        }

        public override void Close()
        {
            this.transport.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (!this._IsDisposed && disposing && this.readBuffer != null)
            {
                this.readBuffer.Dispose();
            }
            this._IsDisposed = true;
        }

        public override void Flush()
        {
            ArraySegment<byte> buffer;
            this.writeBuffer.TryGetBuffer(out buffer);
            int length = (int)this.writeBuffer.Length;
            int num = length - 4;
            if (num < 0)
            {
                throw new InvalidOperationException();
            }
            this.InitWriteBuffer();
            buffer.Array[0] = (byte)(255 & num >> 24);
            buffer.Array[1] = (byte)(255 & num >> 16);
            buffer.Array[2] = (byte)(255 & num >> 8);
            buffer.Array[3] = (byte)(255 & num);
            this.transport.Write(buffer.Array, 0, length);
            this.transport.Flush();
        }

        private void InitWriteBuffer()
        {
            this.writeBuffer = new MemoryStream(1024);
            this.writeBuffer.Write(TFramedTransport.header_dummy, 0, 4);
        }

        public override void Open()
        {
            this.transport.Open();
        }

        public override int Read(byte[] buf, int off, int len)
        {
            if (this.readBuffer != null)
            {
                int num = this.readBuffer.Read(buf, off, len);
                if (num > 0)
                {
                    return num;
                }
            }
            this.ReadFrame();
            return this.readBuffer.Read(buf, off, len);
        }

        private void ReadFrame()
        {
            byte[] numArray = new byte[4];
            this.transport.ReadAll(numArray, 0, 4);
            int num = (numArray[0] & 255) << 24 | (numArray[1] & 255) << 16 | (numArray[2] & 255) << 8 | numArray[3] & 255;
            byte[] numArray1 = new byte[num];
            this.transport.ReadAll(numArray1, 0, num);
            this.readBuffer = new MemoryStream(numArray1);
        }

        public override void Write(byte[] buf, int off, int len)
        {
            this.writeBuffer.Write(buf, off, len);
        }

        public class Factory : TTransportFactory
        {
            public Factory()
            {
            }

            public override TTransport GetTransport(TTransport trans)
            {
                return new TFramedTransport(trans);
            }
        }
    }
}