using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Thrift.Transport;

namespace Thrift.Protocol
{
    public class TJSONProtocol : TProtocol
    {
        private const long VERSION = 1L;

        private const int DEF_STRING_SIZE = 16;

        private static byte[] COMMA;

        private static byte[] COLON;

        private static byte[] LBRACE;

        private static byte[] RBRACE;

        private static byte[] LBRACKET;

        private static byte[] RBRACKET;

        private static byte[] QUOTE;

        private static byte[] BACKSLASH;

        private static byte[] ZERO;

        private byte[] ESCSEQ = new byte[] { 92, 117, 48, 48 };

        private byte[] JSON_CHAR_TABLE = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 98, 116, 110, 0, 102, 114, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 34, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        private char[] ESCAPE_CHARS = "\"\\bfnrt".ToCharArray();

        private byte[] ESCAPE_CHAR_VALS = new byte[] { 34, 92, 8, 12, 10, 13, 9 };

        private static byte[] NAME_BOOL;

        private static byte[] NAME_BYTE;

        private static byte[] NAME_I16;

        private static byte[] NAME_I32;

        private static byte[] NAME_I64;

        private static byte[] NAME_DOUBLE;

        private static byte[] NAME_STRUCT;

        private static byte[] NAME_STRING;

        private static byte[] NAME_MAP;

        private static byte[] NAME_LIST;

        private static byte[] NAME_SET;

        protected Encoding utf8Encoding = Encoding.UTF8;

        protected Stack<TJSONProtocol.JSONBaseContext> contextStack = new Stack<TJSONProtocol.JSONBaseContext>();

        protected TJSONProtocol.JSONBaseContext context;

        protected TJSONProtocol.LookaheadReader reader;

        private byte[] tempBuffer = new byte[4];

        static TJSONProtocol()
        {
            TJSONProtocol.COMMA = new byte[] { 44 };
            TJSONProtocol.COLON = new byte[] { 58 };
            TJSONProtocol.LBRACE = new byte[] { 123 };
            TJSONProtocol.RBRACE = new byte[] { 125 };
            TJSONProtocol.LBRACKET = new byte[] { 91 };
            TJSONProtocol.RBRACKET = new byte[] { 93 };
            TJSONProtocol.QUOTE = new byte[] { 34 };
            TJSONProtocol.BACKSLASH = new byte[] { 92 };
            TJSONProtocol.ZERO = new byte[] { 48 };
            TJSONProtocol.NAME_BOOL = new byte[] { 116, 102 };
            TJSONProtocol.NAME_BYTE = new byte[] { 105, 56 };
            TJSONProtocol.NAME_I16 = new byte[] { 105, 49, 54 };
            TJSONProtocol.NAME_I32 = new byte[] { 105, 51, 50 };
            TJSONProtocol.NAME_I64 = new byte[] { 105, 54, 52 };
            TJSONProtocol.NAME_DOUBLE = new byte[] { 100, 98, 108 };
            TJSONProtocol.NAME_STRUCT = new byte[] { 114, 101, 99 };
            TJSONProtocol.NAME_STRING = new byte[] { 115, 116, 114 };
            TJSONProtocol.NAME_MAP = new byte[] { 109, 97, 112 };
            TJSONProtocol.NAME_LIST = new byte[] { 108, 115, 116 };
            TJSONProtocol.NAME_SET = new byte[] { 115, 101, 116 };
        }

        public TJSONProtocol(TTransport trans) : base(trans)
        {
            this.context = new TJSONProtocol.JSONBaseContext(this);
            this.reader = new TJSONProtocol.LookaheadReader(this);
        }

        private static TType GetTypeIDForTypeName(byte[] name)
        {
            TType tType = TType.Stop;
            if ((int)name.Length > 1)
            {
                byte num = name[0];
                if (num == 100)
                {
                    tType = TType.Double;
                }
                else
                {
                    switch (num)
                    {
                        case 105:
                            {
                                byte num1 = name[1];
                                switch (num1)
                                {
                                    case 49:
                                        {
                                            tType = TType.I16;
                                            break;
                                        }
                                    case 50:
                                        {
                                            break;
                                        }
                                    case 51:
                                        {
                                            tType = TType.I32;
                                            break;
                                        }
                                    default:
                                        {
                                            switch (num1)
                                            {
                                                case 54:
                                                    {
                                                        tType = TType.I64;
                                                        break;
                                                    }
                                                case 56:
                                                    {
                                                        tType = TType.Byte;
                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case 106:
                        case 107:
                            {
                                break;
                            }
                        case 108:
                            {
                                tType = TType.List;
                                break;
                            }
                        case 109:
                            {
                                tType = TType.Map;
                                break;
                            }
                        default:
                            {
                                switch (num)
                                {
                                    case 114:
                                        {
                                            tType = TType.Struct;
                                            break;
                                        }
                                    case 115:
                                        {
                                            if (name[1] != 116)
                                            {
                                                if (name[1] != 101)
                                                {
                                                    if (tType == TType.Stop)
                                                    {
                                                        throw new TProtocolException(5, "Unrecognized type");
                                                    }
                                                    return tType;
                                                }
                                                tType = TType.Set;
                                            }
                                            else
                                            {
                                                tType = TType.String;
                                            }
                                            break;
                                        }
                                    case 116:
                                        {
                                            tType = TType.Bool;
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                }
            }
            if (tType == TType.Stop)
            {
                throw new TProtocolException(5, "Unrecognized type");
            }
            return tType;
        }

        private static byte[] GetTypeNameForTypeID(TType typeID)
        {
            switch (typeID)
            {
                case TType.Bool:
                    {
                        return TJSONProtocol.NAME_BOOL;
                    }
                case TType.Byte:
                    {
                        return TJSONProtocol.NAME_BYTE;
                    }
                case TType.Double:
                    {
                        return TJSONProtocol.NAME_DOUBLE;
                    }
                case TType.Void | TType.Double:
                case TType.Void | TType.Bool | TType.Byte | TType.Double | TType.I16:
                case TType.Void | TType.I32:
                    {
                        throw new TProtocolException(5, "Unrecognized type");
                    }
                case TType.I16:
                    {
                        return TJSONProtocol.NAME_I16;
                    }
                case TType.I32:
                    {
                        return TJSONProtocol.NAME_I32;
                    }
                case TType.I64:
                    {
                        return TJSONProtocol.NAME_I64;
                    }
                case TType.String:
                    {
                        return TJSONProtocol.NAME_STRING;
                    }
                case TType.Struct:
                    {
                        return TJSONProtocol.NAME_STRUCT;
                    }
                case TType.Map:
                    {
                        return TJSONProtocol.NAME_MAP;
                    }
                case TType.Set:
                    {
                        return TJSONProtocol.NAME_SET;
                    }
                case TType.List:
                    {
                        return TJSONProtocol.NAME_LIST;
                    }
                default:
                    {
                        throw new TProtocolException(5, "Unrecognized type");
                    }
            }
        }

        private static byte HexChar(byte val)
        {
            val = (byte)(val & 15);
            if (val < 10)
            {
                return (byte)(val + 48);
            }
            val = (byte)(val - 10);
            return (byte)(val + 97);
        }

        private static byte HexVal(byte ch)
        {
            if (ch >= 48 && ch <= 57)
            {
                return (byte)(ch - 48);
            }
            if (ch < 97 || ch > 102)
            {
                throw new TProtocolException(1, "Expected hex character");
            }
            ch = (byte)(ch + 10);
            return (byte)(ch - 97);
        }

        private bool IsJSONNumeric(byte b)
        {
            byte num = b;
            switch (num)
            {
                case 43:
                case 45:
                case 46:
                case 48:
                case 49:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                case 69:
                    {
                        return true;
                    }
                case 44:
                case 47:
                case 58:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                case 66:
                case 67:
                case 68:
                    {
                        return false;
                    }
                default:
                    {
                        if (num != 101)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
            }
        }

        protected void PopContext()
        {
            this.context = this.contextStack.Pop();
        }

        protected void PushContext(TJSONProtocol.JSONBaseContext c)
        {
            this.contextStack.Push(this.context);
            this.context = c;
        }

        public override byte[] ReadBinary()
        {
            return this.ReadJSONBase64();
        }

        public override bool ReadBool()
        {
            if (this.ReadJSONInteger() != (long)0)
            {
                return true;
            }
            return false;
        }

        public override sbyte ReadByte()
        {
            return (sbyte)this.ReadJSONInteger();
        }

        public override double ReadDouble()
        {
            return this.ReadJSONDouble();
        }

        public override TField ReadFieldBegin()
        {
            TField typeIDForTypeName = new TField();
            if (this.reader.Peek() != TJSONProtocol.RBRACE[0])
            {
                typeIDForTypeName.ID = (short)this.ReadJSONInteger();
                this.ReadJSONObjectStart();
                typeIDForTypeName.Type = TJSONProtocol.GetTypeIDForTypeName(this.ReadJSONString(false));
            }
            else
            {
                typeIDForTypeName.Type = TType.Stop;
            }
            return typeIDForTypeName;
        }

        public override void ReadFieldEnd()
        {
            this.ReadJSONObjectEnd();
        }

        public override short ReadI16()
        {
            return (short)this.ReadJSONInteger();
        }

        public override int ReadI32()
        {
            return (int)this.ReadJSONInteger();
        }

        public override long ReadI64()
        {
            return this.ReadJSONInteger();
        }

        private void ReadJSONArrayEnd()
        {
            this.ReadJSONSyntaxChar(TJSONProtocol.RBRACKET);
            this.PopContext();
        }

        private void ReadJSONArrayStart()
        {
            this.context.Read();
            this.ReadJSONSyntaxChar(TJSONProtocol.LBRACKET);
            this.PushContext(new TJSONProtocol.JSONListContext(this));
        }

        private byte[] ReadJSONBase64()
        {
            byte[] numArray = this.ReadJSONString(false);
            int length = (int)numArray.Length;
            int num = 0;
            int num1 = 0;
            while (length >= 4)
            {
                TBase64Utils.decode(numArray, num, 4, numArray, num1);
                num = num + 4;
                length = length - 4;
                num1 = num1 + 3;
            }
            if (length > 1)
            {
                TBase64Utils.decode(numArray, num, length, numArray, num1);
                num1 = num1 + (length - 1);
            }
            byte[] numArray1 = new byte[num1];
            Array.Copy(numArray, 0, numArray1, 0, num1);
            return numArray1;
        }

        private double ReadJSONDouble()
        {
            double num;
            this.context.Read();
            if (this.reader.Peek() != TJSONProtocol.QUOTE[0])
            {
                if (this.context.EscapeNumbers())
                {
                    this.ReadJSONSyntaxChar(TJSONProtocol.QUOTE);
                }
                try
                {
                    num = double.Parse(this.ReadJSONNumericChars());
                }
                catch (FormatException formatException)
                {
                    throw new TProtocolException(1, "Bad data encounted in numeric data");
                }
                return num;
            }
            byte[] numArray = this.ReadJSONString(true);
            double num1 = double.Parse(this.utf8Encoding.GetString(numArray, 0, (int)numArray.Length), CultureInfo.InvariantCulture);
            if (!this.context.EscapeNumbers() && !double.IsNaN(num1) && !double.IsInfinity(num1))
            {
                throw new TProtocolException(1, "Numeric data unexpectedly quoted");
            }
            return num1;
        }

        private long ReadJSONInteger()
        {
            long num;
            this.context.Read();
            if (this.context.EscapeNumbers())
            {
                this.ReadJSONSyntaxChar(TJSONProtocol.QUOTE);
            }
            string str = this.ReadJSONNumericChars();
            if (this.context.EscapeNumbers())
            {
                this.ReadJSONSyntaxChar(TJSONProtocol.QUOTE);
            }
            try
            {
                num = long.Parse(str);
            }
            catch (FormatException formatException)
            {
                throw new TProtocolException(1, "Bad data encounted in numeric data");
            }
            return num;
        }

        private string ReadJSONNumericChars()
        {
            StringBuilder stringBuilder = new StringBuilder();
            while (this.IsJSONNumeric(this.reader.Peek()))
            {
                stringBuilder.Append((char)this.reader.Read());
            }
            return stringBuilder.ToString();
        }

        private void ReadJSONObjectEnd()
        {
            this.ReadJSONSyntaxChar(TJSONProtocol.RBRACE);
            this.PopContext();
        }

        private void ReadJSONObjectStart()
        {
            this.context.Read();
            this.ReadJSONSyntaxChar(TJSONProtocol.LBRACE);
            this.PushContext(new TJSONProtocol.JSONPairContext(this));
        }

        private byte[] ReadJSONString(bool skipContext)
        {
            MemoryStream memoryStream = new MemoryStream();
            if (!skipContext)
            {
                this.context.Read();
            }
            this.ReadJSONSyntaxChar(TJSONProtocol.QUOTE);
            while (true)
            {
                byte eSCAPECHARVALS = this.reader.Read();
                if (eSCAPECHARVALS == TJSONProtocol.QUOTE[0])
                {
                    break;
                }
                if (eSCAPECHARVALS == this.ESCSEQ[0])
                {
                    eSCAPECHARVALS = this.reader.Read();
                    if (eSCAPECHARVALS != this.ESCSEQ[1])
                    {
                        int num = Array.IndexOf<char>(this.ESCAPE_CHARS, Convert.ToChar(eSCAPECHARVALS));
                        if (num == -1)
                        {
                            throw new TProtocolException(1, "Expected control char");
                        }
                        eSCAPECHARVALS = this.ESCAPE_CHAR_VALS[num];
                    }
                    else
                    {
                        this.ReadJSONSyntaxChar(TJSONProtocol.ZERO);
                        this.ReadJSONSyntaxChar(TJSONProtocol.ZERO);
                        this.trans.ReadAll(this.tempBuffer, 0, 2);
                        eSCAPECHARVALS = (byte)((TJSONProtocol.HexVal(this.tempBuffer[0]) << 4) + TJSONProtocol.HexVal(this.tempBuffer[1]));
                    }
                }
                memoryStream.Write(new byte[] { eSCAPECHARVALS }, 0, 1);
            }
            return memoryStream.ToArray();
        }

        protected void ReadJSONSyntaxChar(byte[] b)
        {
            byte num = this.reader.Read();
            if (num != b[0])
            {
                throw new TProtocolException(1, string.Concat("Unexpected character:", (char)num));
            }
        }

        public override TList ReadListBegin()
        {
            TList typeIDForTypeName = new TList();
            this.ReadJSONArrayStart();
            typeIDForTypeName.ElementType = TJSONProtocol.GetTypeIDForTypeName(this.ReadJSONString(false));
            typeIDForTypeName.Count = (int)this.ReadJSONInteger();
            return typeIDForTypeName;
        }

        public override void ReadListEnd()
        {
            this.ReadJSONArrayEnd();
        }

        public override TMap ReadMapBegin()
        {
            TMap typeIDForTypeName = new TMap();
            this.ReadJSONArrayStart();
            typeIDForTypeName.KeyType = TJSONProtocol.GetTypeIDForTypeName(this.ReadJSONString(false));
            typeIDForTypeName.ValueType = TJSONProtocol.GetTypeIDForTypeName(this.ReadJSONString(false));
            typeIDForTypeName.Count = (int)this.ReadJSONInteger();
            this.ReadJSONObjectStart();
            return typeIDForTypeName;
        }

        public override void ReadMapEnd()
        {
            this.ReadJSONObjectEnd();
            this.ReadJSONArrayEnd();
        }

        public override TMessage ReadMessageBegin()
        {
            TMessage str = new TMessage();
            this.ReadJSONArrayStart();
            if (this.ReadJSONInteger() != (long)1)
            {
                throw new TProtocolException(4, "Message contained bad version.");
            }
            byte[] numArray = this.ReadJSONString(false);
            str.Name = this.utf8Encoding.GetString(numArray, 0, (int)numArray.Length);
            str.Type = (TMessageType)((int)this.ReadJSONInteger());
            str.SeqID = (int)this.ReadJSONInteger();
            return str;
        }

        public override void ReadMessageEnd()
        {
            this.ReadJSONArrayEnd();
        }

        public override TSet ReadSetBegin()
        {
            TSet typeIDForTypeName = new TSet();
            this.ReadJSONArrayStart();
            typeIDForTypeName.ElementType = TJSONProtocol.GetTypeIDForTypeName(this.ReadJSONString(false));
            typeIDForTypeName.Count = (int)this.ReadJSONInteger();
            return typeIDForTypeName;
        }

        public override void ReadSetEnd()
        {
            this.ReadJSONArrayEnd();
        }

        public override string ReadString()
        {
            byte[] numArray = this.ReadJSONString(false);
            return this.utf8Encoding.GetString(numArray, 0, (int)numArray.Length);
        }

        public override TStruct ReadStructBegin()
        {
            this.ReadJSONObjectStart();
            return new TStruct();
        }

        public override void ReadStructEnd()
        {
            this.ReadJSONObjectEnd();
        }

        public override void WriteBinary(byte[] bin)
        {
            this.WriteJSONBase64(bin);
        }

        public override void WriteBool(bool b)
        {
            this.WriteJSONInteger((b ? (long)1 : (long)0));
        }

        public override void WriteByte(sbyte b)
        {
            this.WriteJSONInteger((long)b);
        }

        public override void WriteDouble(double dub)
        {
            this.WriteJSONDouble(dub);
        }

        public override void WriteFieldBegin(TField field)
        {
            this.WriteJSONInteger((long)field.ID);
            this.WriteJSONObjectStart();
            this.WriteJSONString(TJSONProtocol.GetTypeNameForTypeID(field.Type));
        }

        public override void WriteFieldEnd()
        {
            this.WriteJSONObjectEnd();
        }

        public override void WriteFieldStop()
        {
        }

        public override void WriteI16(short i16)
        {
            this.WriteJSONInteger((long)i16);
        }

        public override void WriteI32(int i32)
        {
            this.WriteJSONInteger((long)i32);
        }

        public override void WriteI64(long i64)
        {
            this.WriteJSONInteger(i64);
        }

        private void WriteJSONArrayEnd()
        {
            this.PopContext();
            this.trans.Write(TJSONProtocol.RBRACKET);
        }

        private void WriteJSONArrayStart()
        {
            this.context.Write();
            this.trans.Write(TJSONProtocol.LBRACKET);
            this.PushContext(new TJSONProtocol.JSONListContext(this));
        }

        private void WriteJSONBase64(byte[] b)
        {
            this.context.Write();
            this.trans.Write(TJSONProtocol.QUOTE);
            int length = (int)b.Length;
            int num = 0;
            while (length >= 3)
            {
                TBase64Utils.encode(b, num, 3, this.tempBuffer, 0);
                this.trans.Write(this.tempBuffer, 0, 4);
                num = num + 3;
                length = length - 3;
            }
            if (length > 0)
            {
                TBase64Utils.encode(b, num, length, this.tempBuffer, 0);
                this.trans.Write(this.tempBuffer, 0, length + 1);
            }
            this.trans.Write(TJSONProtocol.QUOTE);
        }

        private void WriteJSONDouble(double num)
        {
            this.context.Write();
            string str = num.ToString(CultureInfo.InvariantCulture);
            bool flag = false;
            char chr = str[0];
            if (chr != '-')
            {
                if (chr == 'I' || chr == 'N')
                {
                    flag = true;
                }
            }
            else if (str[1] == 'I')
            {
                flag = true;
            }
            bool flag1 = (flag ? true : this.context.EscapeNumbers());
            if (flag1)
            {
                this.trans.Write(TJSONProtocol.QUOTE);
            }
            this.trans.Write(this.utf8Encoding.GetBytes(str));
            if (flag1)
            {
                this.trans.Write(TJSONProtocol.QUOTE);
            }
        }

        private void WriteJSONInteger(long num)
        {
            this.context.Write();
            string str = num.ToString();
            bool flag = this.context.EscapeNumbers();
            if (flag)
            {
                this.trans.Write(TJSONProtocol.QUOTE);
            }
            this.trans.Write(this.utf8Encoding.GetBytes(str));
            if (flag)
            {
                this.trans.Write(TJSONProtocol.QUOTE);
            }
        }

        private void WriteJSONObjectEnd()
        {
            this.PopContext();
            this.trans.Write(TJSONProtocol.RBRACE);
        }

        private void WriteJSONObjectStart()
        {
            this.context.Write();
            this.trans.Write(TJSONProtocol.LBRACE);
            this.PushContext(new TJSONProtocol.JSONPairContext(this));
        }

        private void WriteJSONString(byte[] b)
        {
            this.context.Write();
            this.trans.Write(TJSONProtocol.QUOTE);
            int length = (int)b.Length;
            for (int i = 0; i < length; i++)
            {
                if ((b[i] & 255) < 48)
                {
                    this.tempBuffer[0] = this.JSON_CHAR_TABLE[b[i]];
                    if (this.tempBuffer[0] == 1)
                    {
                        this.trans.Write(b, i, 1);
                    }
                    else if (this.tempBuffer[0] <= 1)
                    {
                        this.trans.Write(this.ESCSEQ);
                        this.tempBuffer[0] = TJSONProtocol.HexChar((byte)(b[i] >> 4));
                        this.tempBuffer[1] = TJSONProtocol.HexChar(b[i]);
                        this.trans.Write(this.tempBuffer, 0, 2);
                    }
                    else
                    {
                        this.trans.Write(TJSONProtocol.BACKSLASH);
                        this.trans.Write(this.tempBuffer, 0, 1);
                    }
                }
                else if (b[i] != TJSONProtocol.BACKSLASH[0])
                {
                    this.trans.Write(b, i, 1);
                }
                else
                {
                    this.trans.Write(TJSONProtocol.BACKSLASH);
                    this.trans.Write(TJSONProtocol.BACKSLASH);
                }
            }
            this.trans.Write(TJSONProtocol.QUOTE);
        }

        public override void WriteListBegin(TList list)
        {
            this.WriteJSONArrayStart();
            this.WriteJSONString(TJSONProtocol.GetTypeNameForTypeID(list.ElementType));
            this.WriteJSONInteger((long)list.Count);
        }

        public override void WriteListEnd()
        {
            this.WriteJSONArrayEnd();
        }

        public override void WriteMapBegin(TMap map)
        {
            this.WriteJSONArrayStart();
            this.WriteJSONString(TJSONProtocol.GetTypeNameForTypeID(map.KeyType));
            this.WriteJSONString(TJSONProtocol.GetTypeNameForTypeID(map.ValueType));
            this.WriteJSONInteger((long)map.Count);
            this.WriteJSONObjectStart();
        }

        public override void WriteMapEnd()
        {
            this.WriteJSONObjectEnd();
            this.WriteJSONArrayEnd();
        }

        public override void WriteMessageBegin(TMessage message)
        {
            this.WriteJSONArrayStart();
            this.WriteJSONInteger((long)1);
            this.WriteJSONString(this.utf8Encoding.GetBytes(message.Name));
            this.WriteJSONInteger((long)message.Type);
            this.WriteJSONInteger((long)message.SeqID);
        }

        public override void WriteMessageEnd()
        {
            this.WriteJSONArrayEnd();
        }

        public override void WriteSetBegin(TSet set)
        {
            this.WriteJSONArrayStart();
            this.WriteJSONString(TJSONProtocol.GetTypeNameForTypeID(set.ElementType));
            this.WriteJSONInteger((long)set.Count);
        }

        public override void WriteSetEnd()
        {
            this.WriteJSONArrayEnd();
        }

        public override void WriteString(string str)
        {
            this.WriteJSONString(this.utf8Encoding.GetBytes(str));
        }

        public override void WriteStructBegin(TStruct str)
        {
            this.WriteJSONObjectStart();
        }

        public override void WriteStructEnd()
        {
            this.WriteJSONObjectEnd();
        }

        public class Factory : TProtocolFactory
        {
            public Factory()
            {
            }

            public TProtocol GetProtocol(TTransport trans)
            {
                return new TJSONProtocol(trans);
            }
        }

        protected class JSONBaseContext
        {
            protected TJSONProtocol proto;

            public JSONBaseContext(TJSONProtocol proto)
            {
                this.proto = proto;
            }

            public virtual bool EscapeNumbers()
            {
                return false;
            }

            public virtual void Read()
            {
            }

            public virtual void Write()
            {
            }
        }

        protected class JSONListContext : TJSONProtocol.JSONBaseContext
        {
            private bool first;

            public JSONListContext(TJSONProtocol protocol) : base(protocol)
            {
            }

            public override void Read()
            {
                if (this.first)
                {
                    this.first = false;
                    return;
                }
                this.proto.ReadJSONSyntaxChar(TJSONProtocol.COMMA);
            }

            public override void Write()
            {
                if (this.first)
                {
                    this.first = false;
                    return;
                }
                this.proto.trans.Write(TJSONProtocol.COMMA);
            }
        }

        protected class JSONPairContext : TJSONProtocol.JSONBaseContext
        {
            private bool first;

            private bool colon;

            public JSONPairContext(TJSONProtocol proto) : base(proto)
            {
            }

            public override bool EscapeNumbers()
            {
                return this.colon;
            }

            public override void Read()
            {
                if (this.first)
                {
                    this.first = false;
                    this.colon = true;
                    return;
                }
                this.proto.ReadJSONSyntaxChar((this.colon ? TJSONProtocol.COLON : TJSONProtocol.COMMA));
                this.colon = !this.colon;
            }

            public override void Write()
            {
                if (this.first)
                {
                    this.first = false;
                    this.colon = true;
                    return;
                }
                this.proto.trans.Write((this.colon ? TJSONProtocol.COLON : TJSONProtocol.COMMA));
                this.colon = !this.colon;
            }
        }

        protected class LookaheadReader
        {
            protected TJSONProtocol proto;

            private bool hasData;

            private byte[] data;

            public LookaheadReader(TJSONProtocol proto)
            {
                this.proto = proto;
            }

            public byte Peek()
            {
                if (!this.hasData)
                {
                    this.proto.trans.ReadAll(this.data, 0, 1);
                }
                this.hasData = true;
                return this.data[0];
            }

            public byte Read()
            {
                if (!this.hasData)
                {
                    this.proto.trans.ReadAll(this.data, 0, 1);
                }
                else
                {
                    this.hasData = false;
                }
                return this.data[0];
            }
        }
    }
}