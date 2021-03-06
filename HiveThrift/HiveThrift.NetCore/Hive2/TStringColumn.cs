/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;

using Thrift.Protocol;
using Thrift.Transport;

namespace Hive2
{
#if !SILVERLIGHT

#endif

    public partial class TStringColumn : TBase
    {
        public List<string> Values { get; set; }

        public byte[] Nulls { get; set; }

        public TStringColumn()
        {
        }

        public TStringColumn(List<string> values, byte[] nulls) : this()
        {
            this.Values = values;
            this.Nulls = nulls;
        }

        public void Read(TProtocol iprot)
        {
            bool isset_values = false;
            bool isset_nulls = false;
            TField field;
            iprot.ReadStructBegin();
            while (true)
            {
                field = iprot.ReadFieldBegin();
                if (field.Type == TType.Stop)
                {
                    break;
                }
                switch (field.ID)
                {
                    case 1:
                        if (field.Type == TType.List)
                        {
                            {
                                Values = new List<string>();
                                TList _list51 = iprot.ReadListBegin();
                                for (int _i52 = 0; _i52 < _list51.Count; ++_i52)
                                {
                                    string _elem53 = null;
                                    _elem53 = iprot.ReadString();
                                    Values.Add(_elem53);
                                }
                                iprot.ReadListEnd();
                            }
                            isset_values = true;
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 2:
                        if (field.Type == TType.String)
                        {
                            Nulls = iprot.ReadBinary();
                            isset_nulls = true;
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    default:
                        TProtocolUtil.Skip(iprot, field.Type);
                        break;
                }
                iprot.ReadFieldEnd();
            }
            iprot.ReadStructEnd();
            if (!isset_values)
                throw new TProtocolException(TProtocolException.INVALID_DATA);
            if (!isset_nulls)
                throw new TProtocolException(TProtocolException.INVALID_DATA);
        }

        public void Write(TProtocol oprot)
        {
            TStruct struc = new TStruct("TStringColumn");
            oprot.WriteStructBegin(struc);
            TField field = new TField();
            field.Name = "values";
            field.Type = TType.List;
            field.ID = 1;
            oprot.WriteFieldBegin(field);
            {
                oprot.WriteListBegin(new TList(TType.String, Values.Count));
                foreach (string _iter54 in Values)
                {
                    oprot.WriteString(_iter54);
                }
                oprot.WriteListEnd();
            }
            oprot.WriteFieldEnd();
            field.Name = "nulls";
            field.Type = TType.String;
            field.ID = 2;
            oprot.WriteFieldBegin(field);
            oprot.WriteBinary(Nulls);
            oprot.WriteFieldEnd();
            oprot.WriteFieldStop();
            oprot.WriteStructEnd();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("TStringColumn(");
            sb.Append("Values: ");
            sb.Append(Values);
            sb.Append(",Nulls: ");
            sb.Append(Nulls);
            sb.Append(")");
            return sb.ToString();
        }
    }
}