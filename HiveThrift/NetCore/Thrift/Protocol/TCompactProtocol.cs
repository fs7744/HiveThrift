using System;
using System.Collections.Generic;
using System.Text;
using Thrift.Transport;

namespace Thrift.Protocol
{
    public class TCompactProtocol : TProtocol
    {
        private const byte PROTOCOL_ID = 130;

        private const byte VERSION = 1;

        private const byte VERSION_MASK = 31;

        private const byte TYPE_MASK = 224;

        private const int TYPE_SHIFT_AMOUNT = 5;

        private static TStruct ANONYMOUS_STRUCT;

        private static TField TSTOP;

        private static byte[] ttypeToCompactType;

        private Stack<short> lastField_ = new Stack<short>(15);

        private short lastFieldId_;

        private TField? booleanField_;

        private bool? boolValue_;

        private byte[] byteDirectBuffer = new byte[1];

        private byte[] i32buf = new byte[5];

        private byte[] varint64out = new byte[10];

        private byte[] byteRawBuf = new byte[1];

        static TCompactProtocol()
        {
            TCompactProtocol.ANONYMOUS_STRUCT = new TStruct("");
            TCompactProtocol.TSTOP = new TField("", TType.Stop, 0);
            TCompactProtocol.ttypeToCompactType = new byte[16];
        }

        public TCompactProtocol(TTransport trans) : base(trans)
        {
            TCompactProtocol.ttypeToCompactType[0] = 0;
            TCompactProtocol.ttypeToCompactType[2] = 1;
            TCompactProtocol.ttypeToCompactType[3] = 3;
            TCompactProtocol.ttypeToCompactType[6] = 4;
            TCompactProtocol.ttypeToCompactType[8] = 5;
            TCompactProtocol.ttypeToCompactType[10] = 6;
            TCompactProtocol.ttypeToCompactType[4] = 7;
            TCompactProtocol.ttypeToCompactType[11] = 8;
            TCompactProtocol.ttypeToCompactType[15] = 9;
            TCompactProtocol.ttypeToCompactType[14] = 10;
            TCompactProtocol.ttypeToCompactType[13] = 11;
            TCompactProtocol.ttypeToCompactType[12] = 12;
        }

        private long bytesToLong(byte[] bytes)
        {
            return (long)(((ulong)bytes[7] & (long)255) << 56 | ((ulong)bytes[6] & (long)255) << 48 | ((ulong)bytes[5] & (long)255) << 40 | ((ulong)bytes[4] & (long)255) << 32 | ((ulong)bytes[3] & (long)255) << 24 | ((ulong)bytes[2] & (long)255) << 16 | ((ulong)bytes[1] & (long)255) << 8 | (ulong)bytes[0] & (long)255);
        }

        private void fixedLongToBytes(long n, byte[] buf, int off)
        {
            buf[off] = (byte)(n & (long)255);
            buf[off + 1] = (byte)(n >> 8 & (long)255);
            buf[off + 2] = (byte)(n >> 16 & (long)255);
            buf[off + 3] = (byte)(n >> 24 & (long)255);
            buf[off + 4] = (byte)(n >> 32 & (long)255);
            buf[off + 5] = (byte)(n >> 40 & (long)255);
            buf[off + 6] = (byte)(n >> 48 & (long)255);
            buf[off + 7] = (byte)(n >> 56 & (long)255);
        }

        private byte getCompactType(TType ttype)
        {
            return TCompactProtocol.ttypeToCompactType[(int)ttype];
        }

        private TType getTType(byte type)
        {
            switch ((byte)(type & 15))
            {
                case 0:
                    {
                        return TType.Stop;
                    }
                case 1:
                case 2:
                    {
                        return TType.Bool;
                    }
                case 3:
                    {
                        return TType.Byte;
                    }
                case 4:
                    {
                        return TType.I16;
                    }
                case 5:
                    {
                        return TType.I32;
                    }
                case 6:
                    {
                        return TType.I64;
                    }
                case 7:
                    {
                        return TType.Double;
                    }
                case 8:
                    {
                        return TType.String;
                    }
                case 9:
                    {
                        return TType.List;
                    }
                case 10:
                    {
                        return TType.Set;
                    }
                case 11:
                    {
                        return TType.Map;
                    }
                case 12:
                    {
                        return TType.Struct;
                    }
            }
            throw new TProtocolException(string.Concat("don't know what type: ", (byte)(type & 15)));
        }

        private uint intToZigZag(int n)
        {
            return (uint)(n << 1 ^ n >> 31);
        }

        private bool isBoolType(byte b)
        {
            int num = b & 15;
            if (num == 1)
            {
                return true;
            }
            return num == 2;
        }

        private ulong longToZigzag(long n)
        {
            return (ulong)(n << 1 ^ n >> 63);
        }

        public override byte[] ReadBinary()
        {
            int num = (int)this.ReadVarint32();
            if (num == 0)
            {
                return new byte[0];
            }
            byte[] numArray = new byte[num];
            this.trans.ReadAll(numArray, 0, num);
            return numArray;
        }

        private byte[] ReadBinary(int length)
        {
            if (length == 0)
            {
                return new byte[0];
            }
            byte[] numArray = new byte[length];
            this.trans.ReadAll(numArray, 0, length);
            return numArray;
        }

        public override bool ReadBool()
        {
            if (!this.boolValue_.HasValue)
            {
                return this.ReadByte() == 1;
            }
            bool value = this.boolValue_.Value;
            this.boolValue_ = null;
            return value;
        }

        public override sbyte ReadByte()
        {
            this.trans.ReadAll(this.byteRawBuf, 0, 1);
            return (sbyte)this.byteRawBuf[0];
        }

        public override double ReadDouble()
        {
            byte[] numArray = new byte[8];
            this.trans.ReadAll(numArray, 0, 8);
            return BitConverter.Int64BitsToDouble(this.bytesToLong(numArray));
        }

        public override TField ReadFieldBegin()
        {
            short num;
            byte num1 = (byte)this.ReadByte();
            if (num1 == 0)
            {
                return TCompactProtocol.TSTOP;
            }
            short num2 = (short)((num1 & 240) >> 4);
            num = (num2 != 0 ? (short)(this.lastFieldId_ + num2) : this.ReadI16());
            TField tField = new TField("", this.getTType((byte)(num1 & 15)), num);
            if (this.isBoolType(num1))
            {
                this.boolValue_ = new bool?(((byte)(num1 & 15) == 1 ? true : false));
            }
            this.lastFieldId_ = tField.ID;
            return tField;
        }

        public override void ReadFieldEnd()
        {
        }

        public override short ReadI16()
        {
            return (short)this.zigzagToInt(this.ReadVarint32());
        }

        public override int ReadI32()
        {
            return this.zigzagToInt(this.ReadVarint32());
        }

        public override long ReadI64()
        {
            return this.zigzagToLong(this.ReadVarint64());
        }

        public override TList ReadListBegin()
        {
            byte num = (byte)this.ReadByte();
            int num1 = num >> 4 & 15;
            if (num1 == 15)
            {
                num1 = (int)this.ReadVarint32();
            }
            return new TList(this.getTType(num), num1);
        }

        public override void ReadListEnd()
        {
        }

        public override TMap ReadMapBegin()
        {
            byte num;
            int num1 = (int)this.ReadVarint32();
            if (num1 == 0)
            {
                num = 0;
            }
            else
            {
                num = (byte)this.ReadByte();
            }
            byte num2 = num;
            return new TMap(this.getTType((byte)(num2 >> 4)), this.getTType((byte)(num2 & 15)), num1);
        }

        public override void ReadMapEnd()
        {
        }

        public override TMessage ReadMessageBegin()
        {
            byte num = (byte)this.ReadByte();
            if (num != 130)
            {
                byte num1 = 130;
                throw new TProtocolException(string.Concat("Expected protocol id ", num1.ToString("X"), " but got ", num.ToString("X")));
            }
            byte num2 = (byte)this.ReadByte();
            byte num3 = (byte)(num2 & 31);
            if (num3 != 1)
            {
                object[] objArray = new object[] { "Expected version ", (byte)1, " but got ", num3 };
                throw new TProtocolException(string.Concat(objArray));
            }
            byte num4 = (byte)(num2 >> 5 & 3);
            int num5 = (int)this.ReadVarint32();
            return new TMessage(this.ReadString(), (TMessageType)num4, num5);
        }

        public override void ReadMessageEnd()
        {
        }

        public override TSet ReadSetBegin()
        {
            return new TSet(this.ReadListBegin());
        }

        public override void ReadSetEnd()
        {
        }

        public override string ReadString()
        {
            int num = (int)this.ReadVarint32();
            if (num == 0)
            {
                return "";
            }
            return Encoding.UTF8.GetString(this.ReadBinary(num));
        }

        public override TStruct ReadStructBegin()
        {
            this.lastField_.Push(this.lastFieldId_);
            this.lastFieldId_ = 0;
            return TCompactProtocol.ANONYMOUS_STRUCT;
        }

        public override void ReadStructEnd()
        {
            this.lastFieldId_ = this.lastField_.Pop();
        }

        private uint ReadVarint32()
        {
            uint num = 0;
            int num1 = 0;
            while (true)
            {
                byte num2 = (byte)this.ReadByte();
                unchecked
                {
                    num = num | (uint)((num2 & 127) << (num1 & 31));
                }
                if ((num2 & 128) != 128)
                {
                    break;
                }
                num1 = num1 + 7;
            }
            return num;
        }

        private ulong ReadVarint64()
        {
            int num = 0;
            ulong num1 = (ulong)0;
            while (true)
            {
                byte num2 = (byte)this.ReadByte();
                unchecked
                {
                    num1 = num1 | (ulong)(num2 & 127) << (num & 63);
                }
                if ((num2 & 128) != 128)
                {
                    break;
                }
                num = num + 7;
            }
            return num1;
        }

        public void reset()
        {
            this.lastField_.Clear();
            this.lastFieldId_ = 0;
        }

        public override void WriteBinary(byte[] bin)
        {
            this.WriteBinary(bin, 0, (int)bin.Length);
        }

        private void WriteBinary(byte[] buf, int offset, int length)
        {
            this.WriteVarint32((uint)length);
            this.trans.Write(buf, offset, length);
        }

        public override void WriteBool(bool b)
        {
            if (!this.booleanField_.HasValue)
            {
                this.WriteByteDirect((byte)((b ? 1 : 2)));
                return;
            }
            this.WriteFieldBeginInternal(this.booleanField_.Value, (byte)((b ? 1 : 2)));
            this.booleanField_ = null;
        }

        public override void WriteByte(sbyte b)
        {
            this.WriteByteDirect((byte)b);
        }

        private void WriteByteDirect(byte b)
        {
            this.byteDirectBuffer[0] = b;
            this.trans.Write(this.byteDirectBuffer);
        }

        private void WriteByteDirect(int n)
        {
            this.WriteByteDirect((byte)n);
        }

        protected void WriteCollectionBegin(TType elemType, int size)
        {
            if (size <= 14)
            {
                this.WriteByteDirect(size << 4 | this.getCompactType(elemType));
                return;
            }
            this.WriteByteDirect(240 | this.getCompactType(elemType));
            this.WriteVarint32((uint)size);
        }

        public override void WriteDouble(double dub)
        {
            byte[] numArray = new byte[8];
            this.fixedLongToBytes(BitConverter.DoubleToInt64Bits(dub), numArray, 0);
            this.trans.Write(numArray);
        }

        public override void WriteFieldBegin(TField field)
        {
            if (field.Type == TType.Bool)
            {
                this.booleanField_ = new TField?(field);
                return;
            }
            this.WriteFieldBeginInternal(field, 255);
        }

        private void WriteFieldBeginInternal(TField field, byte typeOverride)
        {
            byte num = (typeOverride == 255 ? this.getCompactType(field.Type) : typeOverride);
            if (field.ID <= this.lastFieldId_ || field.ID - this.lastFieldId_ > 15)
            {
                this.WriteByteDirect(num);
                this.WriteI16(field.ID);
            }
            else
            {
                this.WriteByteDirect(field.ID - this.lastFieldId_ << 4 | num);
            }
            this.lastFieldId_ = field.ID;
        }

        public override void WriteFieldEnd()
        {
        }

        public override void WriteFieldStop()
        {
            this.WriteByteDirect((byte)0);
        }

        public override void WriteI16(short i16)
        {
            this.WriteVarint32(this.intToZigZag(i16));
        }

        public override void WriteI32(int i32)
        {
            this.WriteVarint32(this.intToZigZag(i32));
        }

        public override void WriteI64(long i64)
        {
            this.WriteVarint64(this.longToZigzag(i64));
        }

        public override void WriteListBegin(TList list)
        {
            this.WriteCollectionBegin(list.ElementType, list.Count);
        }

        public override void WriteListEnd()
        {
        }

        public override void WriteMapBegin(TMap map)
        {
            if (map.Count == 0)
            {
                this.WriteByteDirect(0);
                return;
            }
            this.WriteVarint32((uint)map.Count);
            this.WriteByteDirect(this.getCompactType(map.KeyType) << 4 | this.getCompactType(map.ValueType));
        }

        public override void WriteMapEnd()
        {
        }

        public override void WriteMessageBegin(TMessage message)
        {
            this.WriteByteDirect((byte)130);
            this.WriteByteDirect((byte)((int)TMessageType.Call | (int)message.Type << (int)(TMessageType.Call | TMessageType.Oneway) & 224));
            this.WriteVarint32((uint)message.SeqID);
            this.WriteString(message.Name);
        }

        public override void WriteMessageEnd()
        {
        }

        public override void WriteSetBegin(TSet set)
        {
            this.WriteCollectionBegin(set.ElementType, set.Count);
        }

        public override void WriteSetEnd()
        {
        }

        public override void WriteString(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            this.WriteBinary(bytes, 0, (int)bytes.Length);
        }

        public override void WriteStructBegin(TStruct strct)
        {
            this.lastField_.Push(this.lastFieldId_);
            this.lastFieldId_ = 0;
        }

        public override void WriteStructEnd()
        {
            this.lastFieldId_ = this.lastField_.Pop();
        }

        private void WriteVarint32(uint n)
        {
            int num = 0;
            unchecked
            {
                while (((ulong)n & (ulong)-128) != (long)0)
                {
                    int num1 = num;
                    num = num1 + 1;
                    this.i32buf[num1] = (byte)(n & 127 | 128);
                    n = n >> 7;
                }
            }
            int num2 = num;
            num = num2 + 1;
            this.i32buf[num2] = (byte)n;
            this.trans.Write(this.i32buf, 0, num);
        }

        private void WriteVarint64(ulong n)
        {
            int num = 0;
            unchecked
            {
                while ((n & (ulong)-128) != (long)0)
                {
                    int num1 = num;
                    num = num1 + 1;
                    this.varint64out[num1] = (byte)(n & (long)127 | (long)128);
                    n = n >> 7;
                }
            }
            int num2 = num;
            num = num2 + 1;
            this.varint64out[num2] = (byte)n;
            this.trans.Write(this.varint64out, 0, num);
        }

        private int zigzagToInt(uint n)
        {
            return (int)(n >> 1 ^ -(n & 1));
        }

        private long zigzagToLong(ulong n)
        {
            return ((long)n >> 1 ^ -((long)n & (long)1));
        }

        public class Factory : TProtocolFactory
        {
            public Factory()
            {
            }

            public TProtocol GetProtocol(TTransport trans)
            {
                return new TCompactProtocol(trans);
            }
        }

        private static class Types
        {
            public const byte STOP = 0;

            public const byte BOOLEAN_TRUE = 1;

            public const byte BOOLEAN_FALSE = 2;

            public const byte BYTE = 3;

            public const byte I16 = 4;

            public const byte I32 = 5;

            public const byte I64 = 6;

            public const byte DOUBLE = 7;

            public const byte BINARY = 8;

            public const byte LIST = 9;

            public const byte SET = 10;

            public const byte MAP = 11;

            public const byte STRUCT = 12;
        }
    }
}