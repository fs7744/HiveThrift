using System;
using System.Text;
using Thrift.Transport;
using System.Reflection.Emit;
using System.Reflection;

namespace Thrift.Protocol
{
    public class TBinaryProtocol : TProtocol
    {
        protected const uint VERSION_MASK = 4294901760;

        protected const uint VERSION_1 = 2147549184;

        protected bool strictRead_;

        protected bool strictWrite_ = true;

        private byte[] bout = new byte[1];

        private byte[] i16out = new byte[2];

        private byte[] i32out = new byte[4];

        private byte[] i64out = new byte[8];

        private byte[] bin = new byte[1];

        private byte[] i16in = new byte[2];

        private byte[] i32in = new byte[4];

        private byte[] i64in = new byte[8];

        public TBinaryProtocol(TTransport trans) : this(trans, false, true)
        {
        }

        public TBinaryProtocol(TTransport trans, bool strictRead, bool strictWrite) : base(trans)
        {
            this.strictRead_ = strictRead;
            this.strictWrite_ = strictWrite;
        }

        private int ReadAll(byte[] buf, int off, int len)
        {
            return this.trans.ReadAll(buf, off, len);
        }

        public override byte[] ReadBinary()
        {
            int num = this.ReadI32();
            byte[] numArray = new byte[num];
            this.trans.ReadAll(numArray, 0, num);
            return numArray;
        }

        public override bool ReadBool()
        {
            return this.ReadByte() == 1;
        }

        public override sbyte ReadByte()
        {
            this.ReadAll(this.bin, 0, 1);
            return (sbyte)this.bin[0];
        }

        public override double ReadDouble()
        {
            return BitConverter.Int64BitsToDouble(this.ReadI64());
        }

        public override TField ReadFieldBegin()
        {
            TField tField = new TField()
            {
                Type = (TType)((byte)this.ReadByte())
            };
            if (tField.Type != TType.Stop)
            {
                tField.ID = this.ReadI16();
            }
            return tField;
        }

        public override void ReadFieldEnd()
        {
        }

        public override short ReadI16()
        {
            this.ReadAll(this.i16in, 0, 2);
            return (short)((this.i16in[0] & 255) << 8 | this.i16in[1] & 255);
        }

        public override int ReadI32()
        {
            this.ReadAll(this.i32in, 0, 4);
            return (this.i32in[0] & 255) << 24 | (this.i32in[1] & 255) << 16 | (this.i32in[2] & 255) << 8 | this.i32in[3] & 255;
        }

        public override long ReadI64()
        {
            this.ReadAll(this.i64in, 0, 8);
            return (long)(this.i64in[0] & 255) << 56 | (long)(this.i64in[1] & 255) << 48 | (long)(this.i64in[2] & 255) << 40 | (long)(this.i64in[3] & 255) << 32 | (long)(this.i64in[4] & 255) << 24 | (long)(this.i64in[5] & 255) << 16 | (long)(this.i64in[6] & 255) << 8 | (long)(this.i64in[7] & 255);
        }

        public override TList ReadListBegin()
        {
            TList tList = new TList()
            {
                ElementType = (TType)((byte)this.ReadByte()),
                Count = this.ReadI32()
            };
            return tList;
        }

        public override void ReadListEnd()
        {
        }

        public override TMap ReadMapBegin()
        {
            TMap tMap = new TMap()
            {
                KeyType = (TType)((byte)this.ReadByte()),
                ValueType = (TType)((byte)this.ReadByte()),
                Count = this.ReadI32()
            };
            return tMap;
        }

        public override void ReadMapEnd()
        {
        }

        public override TMessage ReadMessageBegin()
        {
            TMessage tMessage = new TMessage();
            int num = this.ReadI32();
            if (num >= 0)
            {
                if (this.strictRead_)
                {
                    throw new TProtocolException(4, "Missing version in readMessageBegin, old client?");
                }
                tMessage.Name = this.ReadStringBody(num);
                tMessage.Type = (TMessageType)this.ReadByte();
                tMessage.SeqID = this.ReadI32();
            }
            else
            {
                uint num1 = (uint)(num & -65536);
                if (Convert.ToBase64String(BitConverter.GetBytes(num1)) != Convert.ToBase64String(BitConverter.GetBytes(-2147418112)))
                {
                    throw new TProtocolException(4, string.Concat("Bad version in ReadMessageBegin: ", num1));
                }
                tMessage.Type = (TMessageType)(num & 255);
                tMessage.Name = this.ReadString();
                tMessage.SeqID = this.ReadI32();
            }
            return tMessage;
        }

        public override void ReadMessageEnd()
        {
        }

        public override TSet ReadSetBegin()
        {
            TSet tSet = new TSet()
            {
                ElementType = (TType)((byte)this.ReadByte()),
                Count = this.ReadI32()
            };
            return tSet;
        }

        public override void ReadSetEnd()
        {
        }

        private string ReadStringBody(int size)
        {
            byte[] numArray = new byte[size];
            this.trans.ReadAll(numArray, 0, size);
            return Encoding.UTF8.GetString(numArray, 0, (int)numArray.Length);
        }

        public override TStruct ReadStructBegin()
        {
            return new TStruct();
        }

        public override void ReadStructEnd()
        {
        }

        public override void WriteBinary(byte[] b)
        {
            this.WriteI32((int)b.Length);
            this.trans.Write(b, 0, (int)b.Length);
        }

        private delegate void temp(bool b);

        public override void WriteBool(bool b)

        {
            var dm = new DynamicMethod("tempWriteBool", null, new Type[] { typeof(bool) });
            var il = dm.GetILGenerator();
            var IL_0007 = new Label();
            var IL_0008 = new Label();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Brtrue_S, IL_0007);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Br_S, IL_0008);
            il.MarkLabel(IL_0007);
            il.Emit(OpCodes.Ldc_I4_1);
            il.MarkLabel(IL_0008);
            il.EmitCall(OpCodes.Callvirt, typeof(TBinaryProtocol).GetMethod("WriteByte"), new Type[] { typeof(sbyte) });
            il.Emit(OpCodes.Ret);
            var tm = (temp)(dm.CreateDelegate(typeof(TBinaryProtocol)));
            tm(b);
        }

        public override void WriteByte(sbyte b)
        {
            this.bout[0] = (byte)b;
            this.trans.Write(this.bout, 0, 1);
        }

        public override void WriteDouble(double d)
        {
            this.WriteI64(BitConverter.DoubleToInt64Bits(d));
        }

        public override void WriteFieldBegin(TField field)
        {
            this.WriteByte((sbyte)field.Type);
            this.WriteI16(field.ID);
        }

        public override void WriteFieldEnd()
        {
        }

        public override void WriteFieldStop()
        {
            this.WriteByte(0);
        }

        public override void WriteI16(short s)
        {
            this.i16out[0] = (byte)(255 & s >> 8);
            this.i16out[1] = (byte)(255 & s);
            this.trans.Write(this.i16out, 0, 2);
        }

        public override void WriteI32(int i32)
        {
            this.i32out[0] = (byte)(255 & i32 >> 24);
            this.i32out[1] = (byte)(255 & i32 >> 16);
            this.i32out[2] = (byte)(255 & i32 >> 8);
            this.i32out[3] = (byte)(255 & i32);
            this.trans.Write(this.i32out, 0, 4);
        }

        public override void WriteI64(long i64)
        {
            this.i64out[0] = (byte)((long)255 & i64 >> 56);
            this.i64out[1] = (byte)((long)255 & i64 >> 48);
            this.i64out[2] = (byte)((long)255 & i64 >> 40);
            this.i64out[3] = (byte)((long)255 & i64 >> 32);
            this.i64out[4] = (byte)((long)255 & i64 >> 24);
            this.i64out[5] = (byte)((long)255 & i64 >> 16);
            this.i64out[6] = (byte)((long)255 & i64 >> 8);
            this.i64out[7] = (byte)((long)255 & i64);
            this.trans.Write(this.i64out, 0, 8);
        }

        public override void WriteListBegin(TList list)
        {
            this.WriteByte((sbyte)list.ElementType);
            this.WriteI32(list.Count);
        }

        public override void WriteListEnd()
        {
        }

        public override void WriteMapBegin(TMap map)
        {
            this.WriteByte((sbyte)map.KeyType);
            this.WriteByte((sbyte)map.ValueType);
            this.WriteI32(map.Count);
        }

        public override void WriteMapEnd()
        {
        }

        public override void WriteMessageBegin(TMessage message)
        {
            if (!this.strictWrite_)
            {
                this.WriteString(message.Name);
                this.WriteByte((sbyte)message.Type);
                this.WriteI32(message.SeqID);
                return;
            }
            this.WriteI32(-2147418112 | (int)message.Type);
            this.WriteString(message.Name);
            this.WriteI32(message.SeqID);
        }

        public override void WriteMessageEnd()
        {
        }

        public override void WriteSetBegin(TSet set)
        {
            this.WriteByte((sbyte)set.ElementType);
            this.WriteI32(set.Count);
        }

        public override void WriteSetEnd()
        {
        }

        public override void WriteStructBegin(TStruct struc)
        {
        }

        public override void WriteStructEnd()
        {
        }

        public class Factory : TProtocolFactory
        {
            protected bool strictRead_;

            protected bool strictWrite_;

            public Factory() : this(false, true)
            {
            }

            public Factory(bool strictRead, bool strictWrite)
            {
                this.strictRead_ = strictRead;
                this.strictWrite_ = strictWrite;
            }

            public TProtocol GetProtocol(TTransport trans)
            {
                return new TBinaryProtocol(trans, this.strictRead_, this.strictWrite_);
            }
        }
    }
}