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
    /// <summary>
    /// Class that handles connections
    /// </summary>
    internal class ConnectionManager
    {
        private bool isLive = false;
        
        private String providerName;

        public ConnectionManager(bool isLive)
        {
            this.isLive = isLive;
        }

        public ConnectionManager(String ProviderName)
        {
            this.providerName = ProviderName;
        }
        /// <summary>
        /// returns an open idbconnection. The connection should be closed after all the required operation is finished.
        /// </summary>
        /// <returns></returns>
        internal IDbConnection GetIDbConnection()
        {
            IDbConnection idbConnection = null;
            String connectionString;
            connectionString = Configuration.GetConnectionString(providerName);
            
            switch (providerName)
            {
                case Common.SQL_SERVER_DB_PROVIDER:
                    idbConnection = new SqlConnection(connectionString);
                    break;
                case Common.ReconContextConnectionString:
                    idbConnection = new SqlConnection(connectionString);
                    break;
                case Common.CbsSqlServerConnectionKey:
                    idbConnection = new SqlConnection(connectionString);
                    break;
                case Common.CBS_SQL_SERVER_DB_PROVIDER:
                    idbConnection = new SqlConnection(connectionString);
                    break;
                case Common.ORACLE_DB_PROVIDER:
                    idbConnection = new OracleConnection(connectionString);
                    break;
                case Common.CBS_ORACLE_DB_PROVIDER:
                    idbConnection = new OracleConnection(connectionString);
                    break;
                case Common.SWITCH_ORACLE_DB_PROVIDER:
                    idbConnection = new OracleConnection(connectionString);
                    break;
                case Common.SWITCH_POSTGRE_DB_PROVIDER:
                    idbConnection = new OdbcConnection(connectionString);
                    break;
                case Common.EXCESS_DB_PROVIDER:
                    idbConnection = new OleDbConnection(connectionString);
                    break;
                case Common.ODBC_DB_PROVIDER:
                    idbConnection = new OdbcConnection(connectionString);
                    break;
                case Common.OLE_DB_PROVIDER:
                    idbConnection = new OleDbConnection(connectionString);
                    break;
            }

            try
            {
                idbConnection.Open();
            }
            catch (Exception e)
            {
                throw e;
            }

            return idbConnection;
        }
    }
}