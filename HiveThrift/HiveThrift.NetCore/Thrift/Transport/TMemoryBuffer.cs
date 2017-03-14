using System;
using System.IO;
using System.Reflection;
using Thrift.Protocol;

namespace Thrift.Transport
{
	public class TMemoryBuffer : TTransport
	{
		private readonly MemoryStream byteStream;

		private bool _IsDisposed;

		public override bool IsOpen
		{
			get
			{
				return true;
			}
		}

		public TMemoryBuffer()
		{
			this.byteStream = new MemoryStream();
		}

		public TMemoryBuffer(byte[] buf)
		{
			this.byteStream = new MemoryStream(buf);
		}

		public override void Close()
		{
		}

		public static T DeSerialize<T>(byte[] buf)
		where T : TAbstractBase
		{
			TBinaryProtocol tBinaryProtocol = new TBinaryProtocol(new TMemoryBuffer(buf));
			if (!typeof(TBase).IsAssignableFrom(typeof(T)))
			{
				MethodInfo method = typeof(T).GetMethod("Read", BindingFlags.Static | BindingFlags.Public);
				object[] objArray = new object[] { tBinaryProtocol };
				return (T)method.Invoke(null, objArray);
			}
			MethodInfo methodInfo = typeof(T).GetMethod("Read", BindingFlags.Instance | BindingFlags.Public);
			T t = Activator.CreateInstance<T>();
			object obj = t;
			object[] objArray1 = new object[] { tBinaryProtocol };
			methodInfo.Invoke(obj, objArray1);
			return t;
		}

		protected override void Dispose(bool disposing)
		{
			if (!this._IsDisposed && disposing && this.byteStream != null)
			{
				this.byteStream.Dispose();
			}
			this._IsDisposed = true;
		}

		public byte[] GetBuffer()
		{
			return this.byteStream.ToArray();
		}

		public override void Open()
		{
		}

		public override int Read(byte[] buf, int off, int len)
		{
			return this.byteStream.Read(buf, off, len);
		}

		public static byte[] Serialize(TAbstractBase s)
		{
			TMemoryBuffer tMemoryBuffer = new TMemoryBuffer();
			s.Write(new TBinaryProtocol(tMemoryBuffer));
			return tMemoryBuffer.GetBuffer();
		}

		public override void Write(byte[] buf, int off, int len)
		{
			this.byteStream.Write(buf, off, len);
		}
	}
}