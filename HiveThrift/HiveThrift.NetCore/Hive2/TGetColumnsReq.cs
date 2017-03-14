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

    public partial class TGetColumnsReq : TBase
    {
        private string _catalogName;
        private string _schemaName;
        private string _tableName;
        private string _columnName;

        public TSessionHandle SessionHandle { get; set; }

        public string CatalogName
        {
            get
            {
                return _catalogName;
            }
            set
            {
                __isset.catalogName = true;
                this._catalogName = value;
            }
        }

        public string SchemaName
        {
            get
            {
                return _schemaName;
            }
            set
            {
                __isset.schemaName = true;
                this._schemaName = value;
            }
        }

        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                __isset.tableName = true;
                this._tableName = value;
            }
        }

        public string ColumnName
        {
            get
            {
                return _columnName;
            }
            set
            {
                __isset.columnName = true;
                this._columnName = value;
            }
        }

        public Isset __isset;
#if !SILVERLIGHT

#endif

        public struct Isset
        {
            public bool catalogName;
            public bool schemaName;
            public bool tableName;
            public bool columnName;
        }

        public TGetColumnsReq()
        {
        }

        public TGetColumnsReq(TSessionHandle sessionHandle) : this()
        {
            this.SessionHandle = sessionHandle;
        }

        public void Read(TProtocol iprot)
        {
            bool isset_sessionHandle = false;
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
                        if (field.Type == TType.Struct)
                        {
                            SessionHandle = new TSessionHandle();
                            SessionHandle.Read(iprot);
                            isset_sessionHandle = true;
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 2:
                        if (field.Type == TType.String)
                        {
                            CatalogName = iprot.ReadString();
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 3:
                        if (field.Type == TType.String)
                        {
                            SchemaName = iprot.ReadString();
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 4:
                        if (field.Type == TType.String)
                        {
                            TableName = iprot.ReadString();
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 5:
                        if (field.Type == TType.String)
                        {
                            ColumnName = iprot.ReadString();
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
            if (!isset_sessionHandle)
                throw new TProtocolException(TProtocolException.INVALID_DATA);
        }

        public void Write(TProtocol oprot)
        {
            TStruct struc = new TStruct("TGetColumnsReq");
            oprot.WriteStructBegin(struc);
            TField field = new TField();
            field.Name = "sessionHandle";
            field.Type = TType.Struct;
            field.ID = 1;
            oprot.WriteFieldBegin(field);
            SessionHandle.Write(oprot);
            oprot.WriteFieldEnd();
            if (CatalogName != null && __isset.catalogName)
            {
                field.Name = "catalogName";
                field.Type = TType.String;
                field.ID = 2;
                oprot.WriteFieldBegin(field);
                oprot.WriteString(CatalogName);
                oprot.WriteFieldEnd();
            }
            if (SchemaName != null && __isset.schemaName)
            {
                field.Name = "schemaName";
                field.Type = TType.String;
                field.ID = 3;
                oprot.WriteFieldBegin(field);
                oprot.WriteString(SchemaName);
                oprot.WriteFieldEnd();
            }
            if (TableName != null && __isset.tableName)
            {
                field.Name = "tableName";
                field.Type = TType.String;
                field.ID = 4;
                oprot.WriteFieldBegin(field);
                oprot.WriteString(TableName);
                oprot.WriteFieldEnd();
            }
            if (ColumnName != null && __isset.columnName)
            {
                field.Name = "columnName";
                field.Type = TType.String;
                field.ID = 5;
                oprot.WriteFieldBegin(field);
                oprot.WriteString(ColumnName);
                oprot.WriteFieldEnd();
            }
            oprot.WriteFieldStop();
            oprot.WriteStructEnd();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("TGetColumnsReq(");
            sb.Append("SessionHandle: ");
            sb.Append(SessionHandle == null ? "<null>" : SessionHandle.ToString());
            sb.Append(",CatalogName: ");
            sb.Append(CatalogName);
            sb.Append(",SchemaName: ");
            sb.Append(SchemaName);
            sb.Append(",TableName: ");
            sb.Append(TableName);
            sb.Append(",ColumnName: ");
            sb.Append(ColumnName);
            sb.Append(")");
            return sb.ToString();
        }
    }
}