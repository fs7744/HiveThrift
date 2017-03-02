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

    public partial class TI64Value : TBase
    {
        private long _value;

        public long Value
        {
            get
            {
                return _value;
            }
            set
            {
                __isset.value = true;
                this._value = value;
            }
        }

        public Isset __isset;
#if !SILVERLIGHT

#endif

        public struct Isset
        {
            public bool value;
        }

        public TI64Value()
        {
        }

        public void Read(TProtocol iprot)
        {
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
                        if (field.Type == TType.I64)
                        {
                            Value = iprot.ReadI64();
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
        }

        public void Write(TProtocol oprot)
        {
            TStruct struc = new TStruct("TI64Value");
            oprot.WriteStructBegin(struc);
            TField field = new TField();
            if (__isset.value)
            {
                field.Name = "value";
                field.Type = TType.I64;
                field.ID = 1;
                oprot.WriteFieldBegin(field);
                oprot.WriteI64(Value);
                oprot.WriteFieldEnd();
            }
            oprot.WriteFieldStop();
            oprot.WriteStructEnd();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("TI64Value(");
            sb.Append("Value: ");
            sb.Append(Value);
            sb.Append(")");
            return sb.ToString();
        }
    }
}