using System;
using System.Text;
using Thrift.Transport;

namespace Thrift.Protocol
{
	public abstract class TProtocol : IDisposable
	{
		protected TTransport trans;

		private bool _IsDisposed;

		public TTransport Transport
		{
			get
			{
				return this.trans;
			}
		}

		protected TProtocol(TTransport trans)
		{
			this.trans = trans;
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._IsDisposed && disposing && this.trans != null)
			{
				((IDisposable)this.trans).Dispose();
			}
			this._IsDisposed = true;
		}

		public abstract byte[] ReadBinary();

		public abstract bool ReadBool();

		public abstract sbyte ReadByte();

		public abstract double ReadDouble();

		public abstract TField ReadFieldBegin();

		public abstract void ReadFieldEnd();

		public abstract short ReadI16();

		public abstract int ReadI32();

		public abstract long ReadI64();

		public abstract TList ReadListBegin();

		public abstract void ReadListEnd();

		public abstract TMap ReadMapBegin();

		public abstract void ReadMapEnd();

		public abstract TMessage ReadMessageBegin();

		public abstract void ReadMessageEnd();

		public abstract TSet ReadSetBegin();

		public abstract void ReadSetEnd();

		public virtual string ReadString()
		{
			byte[] numArray = this.ReadBinary();
			return Encoding.UTF8.GetString(numArray, 0, (int)numArray.Length);
		}

		public abstract TStruct ReadStructBegin();

		public abstract void ReadStructEnd();

		public abstract void WriteBinary(byte[] b);

		public abstract void WriteBool(bool b);

		public abstract void WriteByte(sbyte b);

		public abstract void WriteDouble(double d);

		public abstract void WriteFieldBegin(TField field);

		public abstract void WriteFieldEnd();

		public abstract void WriteFieldStop();

		public abstract void WriteI16(short i16);

		public abstract void WriteI32(int i32);

		public abstract void WriteI64(long i64);

		public abstract void WriteListBegin(TList list);

		public abstract void WriteListEnd();

		public abstract void WriteMapBegin(TMap map);

		public abstract void WriteMapEnd();

		public abstract void WriteMessageBegin(TMessage message);

		public abstract void WriteMessageEnd();

		public abstract void WriteSetBegin(TSet set);

		public abstract void WriteSetEnd();

		public virtual void WriteString(string s)
		{
			this.WriteBinary(Encoding.UTF8.GetBytes(s));
		}

		public abstract void WriteStructBegin(TStruct struc);

		public abstract void WriteStructEnd();
	}
}