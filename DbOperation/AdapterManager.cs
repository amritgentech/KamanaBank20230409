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
    public class AdapterManager
    {
        private String ProviderName;

        public AdapterManager(String ProviderName)
        {
            this.ProviderName = ProviderName;            
        }

        internal IDataAdapter GetIDataAdapter(String sqlCommand, IDbConnection idbConnection)
        {
            return GetDataAdapter(sqlCommand, idbConnection, CommandType.Text);
        }

        internal IDataAdapter GetDataAdapter(string sqlCommand, IDbConnection connection, Parameter param, CommandType commandType)
        {
            IDataAdapter adapter = null;
            IDbCommand command = (new CommandBuilder(ProviderName)).GetCommand(sqlCommand, connection, param, commandType);

            switch (ProviderName)
            {
                case Common.SQL_SERVER_DB_PROVIDER:
                    adapter = new SqlDataAdapter((SqlCommand)command);
                    break;
                case Common.CbsSqlServerConnectionKey:
                    adapter = new SqlDataAdapter((SqlCommand)command);
                    break;
                //case Common.MY_SQL_DB_PROVIDER:
                //    adapter = new MySqlDataAdapter((MySqlCommand)command);
                //    break;
                case Common.ORACLE_DB_PROVIDER:
                    adapter = new OracleDataAdapter((OracleCommand)command);
                    break;
                case Common.CBS_ORACLE_DB_PROVIDER:
                    adapter = new OracleDataAdapter((OracleCommand)command);
                    break;
                case Common.SWITCH_ORACLE_DB_PROVIDER:
                    adapter = new OracleDataAdapter((OracleCommand)command);
                    break;
                case Common.EXCESS_DB_PROVIDER:
                    adapter = new OleDbDataAdapter((OleDbCommand)command);
                    break;
                case Common.OLE_DB_PROVIDER:
                    adapter = new OleDbDataAdapter((OleDbCommand)command);
                    break;
                case Common.ODBC_DB_PROVIDER:
                    adapter = new OdbcDataAdapter((OdbcCommand)command);
                    break;
            }
            return adapter;
        }


        internal IDataAdapter GetDataAdapter(string sqlCommand, IDbConnection connection, ParameterCollection paramCollection, CommandType commandType)
        {
            IDataAdapter adapter = null;
            IDbCommand command = (new CommandBuilder(ProviderName)).GetCommand(sqlCommand, connection, paramCollection, commandType);

            switch (ProviderName)
            {
                case Common.SQL_SERVER_DB_PROVIDER:
                    adapter = new SqlDataAdapter((SqlCommand)command);
                    break;
                case Common.CbsSqlServerConnectionKey:
                    adapter = new SqlDataAdapter((SqlCommand)command);
                    break;
                //case Common.MY_SQL_DB_PROVIDER:
                //    adapter = new MySqlDataAdapter((MySqlCommand)command);
                //    break;
                case Common.ORACLE_DB_PROVIDER:
                    adapter = new OracleDataAdapter((OracleCommand)command);
                    break;
                case Common.CBS_ORACLE_DB_PROVIDER:
                    adapter = new OracleDataAdapter((OracleCommand)command);
                    break;
                case Common.SWITCH_ORACLE_DB_PROVIDER:
                    adapter = new OracleDataAdapter((OracleCommand)command);
                    break;
                case Common.EXCESS_DB_PROVIDER:
                    adapter = new OleDbDataAdapter((OleDbCommand)command);
                    break;
                case Common.OLE_DB_PROVIDER:
                    adapter = new OleDbDataAdapter((OleDbCommand)command);
                    break;
                case Common.ODBC_DB_PROVIDER:
                    adapter = new OdbcDataAdapter((OdbcCommand)command);
                    break;
            }

            return adapter;
        }

        public IDataAdapter GetDataAdapter(string sqlCommand, IDbConnection connection, CommandType commandType)
        {
            IDataAdapter adapter = null;
            IDbCommand command = (new CommandBuilder(ProviderName)).GetCommand(sqlCommand, connection, commandType);

            switch (ProviderName)
            {
                case Common.SQL_SERVER_DB_PROVIDER:
                    adapter = new SqlDataAdapter((SqlCommand)command);
                    break;
                case Common.CbsSqlServerConnectionKey:
                    adapter = new SqlDataAdapter((SqlCommand)command);
                    break;
                //case Common.MY_SQL_DB_PROVIDER:
                //    adapter = new MySqlDataAdapter((MySqlCommand)command);
                //    break;
                case Common.ORACLE_DB_PROVIDER:
                    adapter = new OracleDataAdapter((OracleCommand)command);
                    break;
                case Common.CBS_ORACLE_DB_PROVIDER:
                    adapter = new OracleDataAdapter((OracleCommand)command);
                    break;
                case Common.SWITCH_ORACLE_DB_PROVIDER:
                    adapter = new OracleDataAdapter((OracleCommand)command);
                    break;
                case Common.EXCESS_DB_PROVIDER:
                    adapter = new OleDbDataAdapter((OleDbCommand)command);
                    break;
                case Common.OLE_DB_PROVIDER:
                    adapter = new OleDbDataAdapter((OleDbCommand)command);
                    break;
                case Common.ODBC_DB_PROVIDER:
                    adapter = new OdbcDataAdapter((OdbcCommand)command);
                    break;
            }

            return adapter;
        }
        internal DataTable GetDataTable(string sqlCommand, ParameterCollection paramCollection, IDbConnection connection, string tableName, CommandType commandType)
        {
            DataTable dt = null;

            if (tableName != string.Empty)
                dt = new DataTable(tableName);
            else
                dt = new DataTable();

            IDbCommand command = null;
            if (paramCollection != null)
            {
                if (paramCollection.Parameters.Count > 0)
                    command = (new CommandBuilder(ProviderName)).GetCommand(sqlCommand, connection, paramCollection, commandType);
                else
                    command = (new CommandBuilder(ProviderName)).GetCommand(sqlCommand, connection, commandType);
            }
            else
                command = (new CommandBuilder(ProviderName)).GetCommand(sqlCommand, connection, commandType);

            command.CommandTimeout = 1000;

            OracleDataAdapter oracleAdapter = null;
            SqlDataAdapter sqlAdapter = null;
            switch (ProviderName)
            {
                case Common.SQL_SERVER_DB_PROVIDER:
                    sqlAdapter = new SqlDataAdapter((SqlCommand)command);
                    try
                    {
                        sqlAdapter.Fill(dt);
                    }
                    catch (Exception ex1)
                    {
                        throw ex1;
                    }
                    finally
                    {
                        if (command != null)
                        {
                            command.Dispose();
                        }

                        if (sqlAdapter != null)
                        {
                            sqlAdapter.Dispose();
                        }
                    }
                    break;
                case Common.ReconContextConnectionString:
                    sqlAdapter = new SqlDataAdapter((SqlCommand)command);
                    try
                    {
                        sqlAdapter.Fill(dt);
                    }
                    catch (Exception ex1)
                    {
                        throw ex1;
                    }
                    finally
                    {
                        if (command != null)
                        {
                            command.Dispose();
                        }

                        if (sqlAdapter != null)
                        {
                            sqlAdapter.Dispose();
                        }
                    }
                    break;
                case Common.CbsSqlServerConnectionKey:
                    sqlAdapter = new SqlDataAdapter((SqlCommand)command);
                    try
                    {
                        sqlAdapter.Fill(dt);
                    }
                    catch (Exception ex1)
                    {
                        throw ex1;
                    }
                    finally
                    {
                        if (command != null)
                        {
                            command.Dispose();
                        }

                        if (sqlAdapter != null)
                        {
                            sqlAdapter.Dispose();
                        }
                    }
                    break;
                //case Common.MY_SQL_DB_PROVIDER:
                //    MySqlDataAdapter mySqlAdapter = null;
                //    mySqlAdapter = new MySqlDataAdapter((MySqlCommand)command);
                //    try
                //    {
                //        mySqlAdapter.Fill(dt);
                //    }
                //    catch (Exception ex2)
                //    {
                //        throw ex2;
                //    }
                //    finally
                //    {
                //        if (command != null)
                //        {
                //            command.Dispose();
                //        }

                //        if (mySqlAdapter != null)
                //        {
                //            mySqlAdapter.Dispose();
                //        }
                //    }
                //    break;
                case Common.ORACLE_DB_PROVIDER:                    
                    oracleAdapter = new OracleDataAdapter((OracleCommand)command);
                    try
                    {
                        oracleAdapter.Fill(dt);
                    }
                    catch (Exception ex3)
                    {
                        throw ex3;
                    }
                    finally
                    {
                        if (command != null)
                        {
                            command.Dispose();
                        }

                        if (oracleAdapter != null)
                        {
                            oracleAdapter.Dispose();
                        }
                    }
                    break;
                case Common.SWITCH_ORACLE_DB_PROVIDER:
                    oracleAdapter = new OracleDataAdapter((OracleCommand)command);
                    try
                    {
                        oracleAdapter.Fill(dt);
                    }
                    catch (Exception ex3)
                    {
                        throw ex3;
                    }
                    finally
                    {
                        if (command != null)
                        {
                            command.Dispose();
                        }

                        if (oracleAdapter != null)
                        {
                            oracleAdapter.Dispose();
                        }
                    }
                    break;
                case Common.CBS_ORACLE_DB_PROVIDER:
                    oracleAdapter = new OracleDataAdapter((OracleCommand)command);
                    try
                    {
                        oracleAdapter.Fill(dt);
                    }
                    catch (Exception ex3)
                    {
                        throw ex3;
                    }
                    finally
                    {
                        if (command != null)
                        {
                            command.Dispose();
                        }

                        if (oracleAdapter != null)
                        {
                            oracleAdapter.Dispose();
                        }
                    }
                    break;
                case Common.EXCESS_DB_PROVIDER:
                    OleDbDataAdapter oleDBAdapterAccess = null;
                    oleDBAdapterAccess = new OleDbDataAdapter((OleDbCommand)command);
                    try
                    {
                        oleDBAdapterAccess.Fill(dt);
                    }
                    catch (Exception ex4)
                    {
                        throw ex4;
                    }
                    finally
                    {
                        if (command != null)
                        {
                            command.Dispose();
                        }

                        if (oleDBAdapterAccess != null)
                        {
                            oleDBAdapterAccess.Dispose();
                        }
                    }
                    break;
                case Common.OLE_DB_PROVIDER:
                    OleDbDataAdapter oleAdapter = null;
                    oleAdapter = new OleDbDataAdapter((OleDbCommand)command);
                    try
                    {
                        oleAdapter.Fill(dt);
                    }
                    catch (Exception ex4)
                    {
                        throw ex4;
                    }
                    finally
                    {
                        if (command != null)
                        {
                            command.Dispose();
                        }

                        if (oleAdapter != null)
                        {
                            oleAdapter.Dispose();
                        }
                    }
                    break;
                case Common.ODBC_DB_PROVIDER:
                    OdbcDataAdapter odbcAdapter = null;
                    odbcAdapter = new OdbcDataAdapter((OdbcCommand)command);
                    try
                    {
                        odbcAdapter.Fill(dt);
                    }
                    catch (Exception ex4)
                    {
                        throw ex4;
                    }
                    finally
                    {
                        if (command != null)
                        {
                            command.Dispose();
                        }

                        if (odbcAdapter != null)
                        {
                            odbcAdapter.Dispose();
                        }
                    }
                    break;
                case Common.CBS_SQL_SERVER_DB_PROVIDER:
                    sqlAdapter = new SqlDataAdapter((SqlCommand)command);
                    try
                    {
                        sqlAdapter.Fill(dt);
                    }
                    catch (Exception ex1)
                    {
                        throw ex1;
                    }
                    finally
                    {
                        if (command != null)
                        {
                            command.Dispose();
                        }

                        if (sqlAdapter != null)
                        {
                            sqlAdapter.Dispose();
                        }
                    }
                    break;
            }

            return dt;
        }

        internal DataTable GetDataTable(string sqlCommand, Parameter param, IDbConnection connection, string tableName, CommandType commandType)
        {
            ParameterCollection paramCollection = new ParameterCollection();
            paramCollection.Add(param);
            return GetDataTable(sqlCommand, paramCollection, connection, tableName, commandType);
        }

        internal DataTable GetDataTable(string sqlCommand, IDbConnection connection, string tableName, CommandType commandType)
        {
            return GetDataTable(sqlCommand, new ParameterCollection(), connection, tableName, commandType);
        }

        internal DataTable GetDataTable(string sqlCommand, IDbConnection connection, CommandType commandType)
        {
            return GetDataTable(sqlCommand, new ParameterCollection(), connection, string.Empty, commandType);
        }

        internal DataTable GetDataTable(string sqlCommand, IDbConnection connection)
        {
            return GetDataTable(sqlCommand, new ParameterCollection(), connection, string.Empty, CommandType.Text);
        }

    }
}
