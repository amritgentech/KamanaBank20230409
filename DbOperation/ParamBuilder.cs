using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperations
{
    internal class ParamBuilder
    {
        private String ProviderName;
        public ParamBuilder(String ProviderName)
        {
            this.ProviderName = ProviderName;
        }

        internal IDataParameter GetParameter(Parameter parameter)
        {
            IDbDataParameter dbParam = GetParameter();
            dbParam.ParameterName = parameter.Name;
            dbParam.Value = parameter.Value;
            dbParam.Direction = parameter.ParamDirection;
            dbParam.DbType = parameter.Type;

            return dbParam;
        }

        internal List<IDataParameter> GetParameterCollection(ParameterCollection parameterCollection)
        {
            List<IDataParameter> dbParamCollection = new List<IDataParameter>();
            IDataParameter dbParam = null;
            foreach (Parameter param in parameterCollection.Parameters)
            {
                dbParam = GetParameter(param);
                dbParamCollection.Add(dbParam);
            }

            return dbParamCollection;
        }

        #region Private Methods
        private IDbDataParameter GetParameter()
        {
            IDbDataParameter dbParam = null;
            switch (ProviderName)
            {
                case Common.SQL_SERVER_DB_PROVIDER:
                    dbParam = new SqlParameter();
                    break;
                case Common.CBS_SQL_SERVER_DB_PROVIDER:
                    dbParam = new SqlParameter();
                    break;
                case Common.CbsSqlServerConnectionKey:
                    dbParam = new SqlParameter();
                    break;
                case Common.SWTICH_SQL_SERVER_DB_PROVIDER:
                    dbParam = new SqlParameter();
                    break;
                case Common.ReconContextConnectionString:
                    dbParam = new SqlParameter();
                    break;
                //case Common.MY_SQL_DB_PROVIDER:
                //    dbParam = new MySqlParameter();
                //    break;
                case Common.ORACLE_DB_PROVIDER:
                    dbParam = new OracleParameter();
                    break;
                case Common.CBS_ORACLE_DB_PROVIDER:
                    dbParam = new OracleParameter();
                    break;
                case Common.SWITCH_ORACLE_DB_PROVIDER:
                    dbParam = new OracleParameter();
                    break;
                case Common.EXCESS_DB_PROVIDER:
                    dbParam = new OleDbParameter();
                    break;
                case Common.OLE_DB_PROVIDER:
                    dbParam = new OleDbParameter();
                    break;
                case Common.ODBC_DB_PROVIDER:
                    dbParam = new OdbcParameter();
                    break;
            }
            return dbParam;
        }
        #endregion
    }
}
