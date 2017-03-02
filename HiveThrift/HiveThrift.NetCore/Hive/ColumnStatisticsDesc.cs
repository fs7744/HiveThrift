/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */

using System;
using System.Text;
using Thrift.Protocol;

namespace Hive
{
    public partial class ColumnStatisticsDesc : TBase
    {
        private string _partName;
        private long _lastAnalyzed;

        public bool IsTblLevel { get; set; }

        public string DbName { get; set; }

        public string TableName { get; set; }

        public string PartName
        {
            get
            {
                return _partName;
            }
            set
            {
                __isset.partName = true;
                this._partName = value;
            }
        }

        public long LastAnalyzed
        {
            get
            {
                return _lastAnalyzed;
            }
            set
            {
                __isset.lastAnalyzed = true;
                this._lastAnalyzed = value;
            }
        }

        public Isset __isset;

        public struct Isset
        {
            public bool partName;
            public bool lastAnalyzed;
        }

        public ColumnStatisticsDesc()
        {
        }

        public ColumnStatisticsDesc(bool isTblLevel, string dbName, string tableName)
            : this()
        {
            this.IsTblLevel = isTblLevel;
            this.DbName = dbName;
            this.TableName = tableName;
        }

        public void Read(TProtocol iprot)
        {
            bool isset_isTblLevel = false;
            bool isset_dbName = false;
            bool isset_tableName = false;
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
                        if (field.Type == TType.Bool)
                        {
                            IsTblLevel = iprot.ReadBool();
                            isset_isTblLevel = true;
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 2:
                        if (field.Type == TType.String)
                        {
                            DbName = iprot.ReadString();
                            isset_dbName = true;
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 3:
                        if (field.Type == TType.String)
                        {
                            TableName = iprot.ReadString();
                            isset_tableName = true;
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 4:
                        if (field.Type == TType.String)
                        {
                            PartName = iprot.ReadString();
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 5:
                        if (field.Type == TType.I64)
                        {
                            LastAnalyzed = iprot.ReadI64();
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
            if (!isset_isTblLevel)
                throw new TProtocolException(TProtocolException.INVALID_DATA);
            if (!isset_dbName)
                throw new TProtocolException(TProtocolException.INVALID_DATA);
            if (!isset_tableName)
                throw new TProtocolException(TProtocolException.INVALID_DATA);
        }

        public void Write(TProtocol oprot)
        {
            TStruct struc = new TStruct("ColumnStatisticsDesc");
            oprot.WriteStructBegin(struc);
            TField field = new TField();
            field.Name = "isTblLevel";
            field.Type = TType.Bool;
            field.ID = 1;
            oprot.WriteFieldBegin(field);
            oprot.WriteBool(IsTblLevel);
            oprot.WriteFieldEnd();
            field.Name = "dbName";
            field.Type = TType.String;
            field.ID = 2;
            oprot.WriteFieldBegin(field);
            oprot.WriteString(DbName);
            oprot.WriteFieldEnd();
            field.Name = "tableName";
            field.Type = TType.String;
            field.ID = 3;
            oprot.WriteFieldBegin(field);
            oprot.WriteString(TableName);
            oprot.WriteFieldEnd();
            if (PartName != null && __isset.partName)
            {
                field.Name = "partName";
                field.Type = TType.String;
                field.ID = 4;
                oprot.WriteFieldBegin(field);
                oprot.WriteString(PartName);
                oprot.WriteFieldEnd();
            }
            if (__isset.lastAnalyzed)
            {
                field.Name = "lastAnalyzed";
                field.Type = TType.I64;
                field.ID = 5;
                oprot.WriteFieldBegin(field);
                oprot.WriteI64(LastAnalyzed);
                oprot.WriteFieldEnd();
            }
            oprot.WriteFieldStop();
            oprot.WriteStructEnd();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("ColumnStatisticsDesc(");
            sb.Append("IsTblLevel: ");
            sb.Append(IsTblLevel);
            sb.Append(",DbName: ");
            sb.Append(DbName);
            sb.Append(",TableName: ");
            sb.Append(TableName);
            sb.Append(",PartName: ");
            sb.Append(PartName);
            sb.Append(",LastAnalyzed: ");
            sb.Append(LastAnalyzed);
            sb.Append(")");
            return sb.ToString();
        }
    }
}