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

    public partial class TPrimitiveTypeEntry : TBase
    {
        private TTypeQualifiers _typeQualifiers;

        /// <summary>
        ///
        /// <seealso cref="TTypeId"/>
        /// </summary>
        public TTypeId Type { get; set; }

        public TTypeQualifiers TypeQualifiers
        {
            get
            {
                return _typeQualifiers;
            }
            set
            {
                __isset.typeQualifiers = true;
                this._typeQualifiers = value;
            }
        }

        public Isset __isset;
#if !SILVERLIGHT

#endif

        public struct Isset
        {
            public bool typeQualifiers;
        }

        public TPrimitiveTypeEntry()
        {
        }

        public TPrimitiveTypeEntry(TTypeId type) : this()
        {
            this.Type = type;
        }

        public void Read(TProtocol iprot)
        {
            bool isset_type = false;
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
                        if (field.Type == TType.I32)
                        {
                            Type = (TTypeId)iprot.ReadI32();
                            isset_type = true;
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 2:
                        if (field.Type == TType.Struct)
                        {
                            TypeQualifiers = new TTypeQualifiers();
                            TypeQualifiers.Read(iprot);
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
            if (!isset_type)
                throw new TProtocolException(TProtocolException.INVALID_DATA);
        }

        public void Write(TProtocol oprot)
        {
            TStruct struc = new TStruct("TPrimitiveTypeEntry");
            oprot.WriteStructBegin(struc);
            TField field = new TField();
            field.Name = "type";
            field.Type = TType.I32;
            field.ID = 1;
            oprot.WriteFieldBegin(field);
            oprot.WriteI32((int)Type);
            oprot.WriteFieldEnd();
            if (TypeQualifiers != null && __isset.typeQualifiers)
            {
                field.Name = "typeQualifiers";
                field.Type = TType.Struct;
                field.ID = 2;
                oprot.WriteFieldBegin(field);
                TypeQualifiers.Write(oprot);
                oprot.WriteFieldEnd();
            }
            oprot.WriteFieldStop();
            oprot.WriteStructEnd();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("TPrimitiveTypeEntry(");
            sb.Append("Type: ");
            sb.Append(Type);
            sb.Append(",TypeQualifiers: ");
            sb.Append(TypeQualifiers == null ? "<null>" : TypeQualifiers.ToString());
            sb.Append(")");
            return sb.ToString();
        }
    }
}