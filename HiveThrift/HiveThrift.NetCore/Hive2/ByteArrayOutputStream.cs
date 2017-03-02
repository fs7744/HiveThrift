using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thrift.Transport
{
    public class ByteArrayOutputStream
    {
        protected byte[] _buf;
        protected int Count { get; private set; }
        public ByteArrayOutputStream()
            : this(32)
        { }

        public ByteArrayOutputStream(int size)
        {
            if (size > 0)
            {
                _buf = new byte[size];
            }
            else
                throw new ArgumentException();
        }

        public void Write(byte[] b, int off, int len)
        {
            if ((off < 0) || (off > b.Length) || (len < 0) || ((off + len) > b.Length) || ((off + len) < 0))
            {
                throw new IndexOutOfRangeException();
            }
            else if (len == 0) { return; }

            int newCount = Count + len;
            if (newCount > _buf.Length)
            {
                byte[] newBuf = new byte[Math.Max(_buf.Length << 1, newCount)];
                Array.Copy(_buf, 0, newBuf, 0, Count);
                _buf = newBuf;
            }
            Array.Copy(b, off, _buf, Count, len);
            Count = newCount;
        }

        public void Reset()
        {
            Count = 0;
        }

        public byte[] GetBytes()
        {
            byte[] newBuf = new byte[Count];
            Array.Copy(_buf, 0, newBuf, 0, Count);
            return newBuf;
        }
    }

}