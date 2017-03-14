using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Transport;

namespace Thrift.Transport
{
    class TMemoryInputTransport : TTransport
    {
        private byte[] buf_;
        private int pos_;
        private int endPos_;

        public TMemoryInputTransport()
        {
        }

        public TMemoryInputTransport(byte[] buf)
        {
            Reset(buf);
        }

        public TMemoryInputTransport(byte[] buf, int offset, int length)
        {
            Reset(buf, offset, length);
        }

        public void Reset(byte[] buf)
        {
            Reset(buf, 0, buf.Length);
        }

        public void Reset(byte[] buf, int offset, int length)
        {
            buf_ = buf;
            pos_ = offset;
            endPos_ = offset + length;
        }

        public void Clear()
        {
            buf_ = null;
        }

        public override void Close()
        {
        }

        public override bool IsOpen
        {
            get { return true; }
        }

        public override void Open()
        {
        }

        public override int Read(byte[] buf, int off, int len)
        {
            int bytesRemaining = GetBytesRemainingInBuffer;
            int amtToRead = (len > bytesRemaining ? bytesRemaining : len);
            if (amtToRead > 0)
            {
                Array.Copy(buf_, pos_, buf, off, amtToRead);
                ConsumeBuffer(amtToRead);
            }
            return amtToRead;
        }

        public override void Write(byte[] buf, int off, int len)
        {
        }

        public byte[] GetBuffer()
        {
            return buf_;
        }

        public int GetBufferPosition
        {
            get { return pos_; }
        }

        public int GetBytesRemainingInBuffer
        {
            get { return endPos_ - pos_; }
        }

        public void ConsumeBuffer(int len)
        {
            pos_ += len;
        }

        protected override void Dispose(bool disposing)
        {
            Dispose();
        }
    }
}