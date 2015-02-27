using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Hive2
{
    public class Cursor : IDisposable
    {
        private TSessionHandle m_Session;
        private TCLIService.Client m_Client;
        private TOperationHandle m_Operation;
        private TTableSchema m_LastSchema;

        public Cursor(TSessionHandle m_Session, TCLIService.Client m_Client)
        {
            this.m_Session = m_Session;
            this.m_Client = m_Client;
        }

        ~Cursor()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseOperation();
                m_LastSchema = null;
                m_Client = null;
                m_Session = null;
            }
        }

        public void Execute(string statement)
        {
            CloseOperation();
            var execReq = new TExecuteStatementReq()
            {
                SessionHandle = m_Session,
                Statement = statement,
            };
            m_LastSchema = null;
            var execResp = m_Client.ExecuteStatement(execReq);
            execResp.Status.CheckStatus();
            m_Operation = execResp.OperationHandle;
        }

        public List<ExpandoObject> FetchMany(int size)
        {
            var result = new List<ExpandoObject>();
            var names = GetColumnNames();
            var rowSet = Fetch(size);
            if (rowSet != null)
            {
                foreach (var row in rowSet.Rows)
                {
                    var obj = new ExpandoObject();
                    for (int i = 0; i < row.ColVals.Count; i++)
                    {
                        ((IDictionary<string, object>)obj).Add(names[i], GetrValue(row.ColVals[i]));
                    }
                    result.Add(obj);
                }
            }
            return result;
        }

        public ExpandoObject FetchOne()
        {
            return FetchMany(1).FirstOrDefault();
        }

        private object GetrValue(TColumnValue value)
        {
            if (value.__isset.stringVal)
                return value.StringVal.Value;
            else if (value.__isset.i32Val)
                return value.I32Val.Value;
            else if (value.__isset.boolVal)
                return value.BoolVal.Value;
            else if (value.__isset.doubleVal)
                return value.DoubleVal.Value;
            else if (value.__isset.byteVal)
                return value.ByteVal.Value;
            else if (value.__isset.i64Val)
                return value.I64Val.Value;
            else if (value.__isset.i16Val)
                return value.I16Val.Value;
            else
                return null;
        }

        public TRowSet Fetch(int count = int.MaxValue)
        {
            if (m_Operation == null) return null;
            var req = new TFetchResultsReq()
            {
                MaxRows = count,
                Orientation = TFetchOrientation.FETCH_NEXT,
                OperationHandle = m_Operation,
            };
            var resultsResp = m_Client.FetchResults(req);
            resultsResp.Status.CheckStatus();
            return resultsResp.Results;
        }

        public List<string> GetColumnNames()
        {
            var schema = GetSchema();
            return schema == null ? null
                : schema.Columns.Select(i => i.ColumnName).ToList();
        }

        public TTableSchema GetSchema()
        {
            if (m_Operation == null) return null;
            else if (m_LastSchema == null)
            {
                var req = new TGetResultSetMetadataReq(m_Operation);
                var resp = m_Client.GetResultSetMetadata(req);
                resp.Status.CheckStatus();
                m_LastSchema = resp.Schema;
            }
            return m_LastSchema;
        }

        private void CloseOperation()
        {
            if (m_Operation != null)
            {
                TCloseOperationReq closeReq = new TCloseOperationReq();
                closeReq.OperationHandle = m_Operation;
                TCloseOperationResp closeOperationResp = m_Client.CloseOperation(closeReq);
                closeOperationResp.Status.CheckStatus();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}