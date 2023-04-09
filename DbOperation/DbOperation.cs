using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Design.PluralizationServices;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperations
{
    public class DbOperation
    {
        #region "Private Variables"
        ConnectionManager _connectionManager;
        CommandBuilder _commandBuilder;
        private AdapterManager _adapterManager;
        String ProviderName;
        #endregion

        public DbOperation()
        {
            _connectionManager = new ConnectionManager(false);
        }

        public DbOperation(bool isForLiveConnection)
        {
            _connectionManager = new ConnectionManager(isForLiveConnection);
        }

        public DbOperation(String ProviderName)
        {
            _commandBuilder = new CommandBuilder(ProviderName);
            _adapterManager = new AdapterManager(ProviderName);
            this.ProviderName = ProviderName;
            _connectionManager = new ConnectionManager(ProviderName);
        }

        #region "Postgre"
        public String ExecuteScalar(String sql, String cardNumber)
        {
            return "11111111111";
            //return "1234567890123";
            NpgsqlConnection connection = new NpgsqlConnection(Configuration.GetConnectionString(ProviderName));
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand(sql);
            command.Connection = connection;
            command.Parameters.AddWithValue("@CardNumber", cardNumber);

            String AccountNumner = (String)command.ExecuteScalar();
            command.Dispose();
            connection.Close();
            connection.Dispose();
            return AccountNumner;
        }
        #endregion

        #region "Execute Scalar"

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, CommandType commandType)
        {
            object objScalar = null;
            IDbConnection connection = _connectionManager.GetIDbConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, commandType);

            try
            {
                objScalar = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                // incase of power failures and unexpected shutdowns
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                if (command != null)
                    command.Dispose();

                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                if (command != null)
                    command.Dispose();
            }
            return objScalar;
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="param">Parameter to be associated</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, Parameter param, CommandType commandType)
        {
            object objScalar = null;
            IDbConnection connection = _connectionManager.GetIDbConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, param, commandType);

            try
            {
                objScalar = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                if (command != null)
                    command.Dispose();

                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                if (command != null)
                    command.Dispose();
            }
            return objScalar;
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="paramCollection">Parameter collection to be associated</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, ParameterCollection paramCollection, CommandType commandType)
        {
            object objScalar = null;
            IDbConnection connection = _connectionManager.GetIDbConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, paramCollection, commandType);

            try
            {
                objScalar = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                if (command != null)
                    command.Dispose();
                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                if (command != null)
                    command.Dispose();
            }
            return objScalar;
        }

        /// <summary>
        /// Executes the Sql Command and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(commandText, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="param">Parameter to be associated</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, Parameter param)
        {
            return ExecuteScalar(commandText, param, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command and returns result.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="paramCollection">Parameter collection to be associated.</param>
        /// <returns>A single value. (First row's first cell value, if more than one row and column is returned.)</returns>
        public object ExecuteScalar(string commandText, ParameterCollection paramCollection)
        {
            return ExecuteScalar(commandText, paramCollection, CommandType.Text);
        }
        #endregion ExecuteScalar

        #region ExecuteNonQuery

        /// <summary>
        /// Executes Sql Command or Stored procedure and returns number of rows effected.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Number of rows effected.</returns>
        public int ExecuteNonQuery(string commandText, CommandType commandType)
        {
            int iRowsEffected = 0;
            IDbConnection connection = _connectionManager.GetIDbConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, commandType);
            command.CommandTimeout = 500;
            try
            {
                command.CommandTimeout = 0;
                iRowsEffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                if (command != null)
                    command.Dispose();
            }




            return iRowsEffected;
        }

        /// <summary>
        /// Executes Sql Command or Stored procedure and returns number of rows effected.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="param">Parameter to be associated with the command</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Number of rows effected.</returns>
        public int ExecuteNonQuery(string commandText, Parameter param, CommandType commandType)
        {
            int iRowsEffected = 0;
            IDbConnection connection = _connectionManager.GetIDbConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, param, commandType);

            try
            {
                iRowsEffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                if (command != null)
                    command.Dispose();
            }
            return iRowsEffected;
        }

        /// <summary>
        /// Executes Sql Command or Stored procedure and returns number of rows effected.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="paramCollection">Parameter collection to be associated with the command</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Number of rows effected.</returns>
        public int ExecuteNonQuery(string commandText, ParameterCollection paramCollection, CommandType commandType)
        {
            int iRowsEffected = 0;
            IDbConnection connection = _connectionManager.GetIDbConnection();
            IDbCommand command = _commandBuilder.GetCommand(commandText, connection, paramCollection, commandType);

            try
            {
                iRowsEffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                if (command != null)
                    command.Dispose(); 
                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                if (command != null)
                    command.Dispose();
            }
            return iRowsEffected;
        }

        /// <summary>
        /// Executes Sql Command and returns number of rows effected.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <returns>Number of rows effected.</returns>
        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(commandText, CommandType.Text);
        }

        /// <summary>
        /// Executes Sql Command and returns number of rows effected.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="param">Parameter to be associated with the command</param>
        /// <returns>Number of rows effected.</returns>
        public int ExecuteNonQuery(string commandText, Parameter param)
        {
            return ExecuteNonQuery(commandText, param, CommandType.Text);
        }

        /// <summary>
        /// Executes Sql Command and returns number of rows effected.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="paramCollection">Parameter Collection to be associated with the command</param>
        /// <returns>Number of rows effected.</returns>
        public int ExecuteNonQuery(string commandText, ParameterCollection paramCollection)
        {
            return ExecuteNonQuery(commandText, paramCollection, CommandType.Text);
        }
        #endregion

        #region GetDataSet

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataSet.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="param">Parameter to be associated with the command</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataSet</returns>
        public DataSet ExecuteDataSet(string commandText, Parameter param, CommandType commandType)
        {
            DataSet daReturn = new DataSet();
            IDbConnection connection = _connectionManager.GetIDbConnection();

            IDataAdapter adapter = (new AdapterManager(ProviderName)).GetDataAdapter(commandText, connection, param, commandType);
            try
            {
                adapter.Fill(daReturn);
            }
            catch (Exception ex)
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

            }
            return daReturn;
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataSet.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="paramCollection">Parameter collection to be associated with the command</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataSet</returns>
        public DataSet ExecuteDataSet(string commandText, ParameterCollection paramCollection, CommandType commandType)
        {
            DataSet daReturn = new DataSet();
            IDbConnection connection = _connectionManager.GetIDbConnection();

            IDataAdapter adapter = _adapterManager.GetDataAdapter(commandText, connection,paramCollection, commandType);
            try
            {
                adapter.Fill(daReturn);
            }
            catch (Exception ex)
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

            }
            return daReturn;
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataSet.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataSet</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType commandType)
        {
            DataSet daReturn = new DataSet();
            IDbConnection connection = _connectionManager.GetIDbConnection();

            IDataAdapter adapter = _adapterManager.GetDataAdapter(commandText, connection, commandType);
            try
            {
                adapter.Fill(daReturn);
            }
            catch (Exception ex)
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return daReturn;
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataSet.
        /// </summary>
        /// <param name="commandText">Sql Command </param>
        /// <returns>Result in the form of DataSet</returns>
        public DataSet ExecuteDataSet(string commandText)
        {
            return ExecuteDataSet(commandText, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataSet.
        /// </summary>
        /// <param name="commandText">Sql Command </param>
        /// <param name="param">Parameter to be associated with the command</param>
        /// <returns>Result in the form of DataSet</returns>
        public DataSet ExecuteDataSet(string commandText, Parameter param)
        {
            return ExecuteDataSet(commandText, param, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataSet.
        /// </summary>
        /// <param name="commandText">Sql Command </param>
        /// <param name="paramCollection">Parameter collection to be associated with the command</param>
        /// <returns>Result in the form of DataSet</returns>
        public DataSet ExecuteDataSet(string commandText, ParameterCollection paramCollection)
        {
            return ExecuteDataSet(commandText, paramCollection, CommandType.Text);
        }
        #endregion

        #region "ExecuteDataTable"
        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure name</param>
        /// <param name="tableName">Table name</param>
        /// <param name="paramCollection">Parameter collection to be associated with the Command or Stored Procedure.</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, string tableName, ParameterCollection paramCollection, CommandType commandType)
        {
            DataTable dtReturn = new DataTable();
            IDbConnection connection = null;
            try
            {
                connection = _connectionManager.GetIDbConnection();
                dtReturn = _adapterManager.GetDataTable(commandText, paramCollection, connection, tableName, commandType);
            }
            catch (Exception ex)
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return dtReturn;
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command8 or Stored Procedure name</param>
        /// <param name="paramCollection">Parameter collection to be associated with the Command or Stored Procedure.</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, ParameterCollection paramCollection, CommandType commandType)
        {
            return ExecuteDataTable(commandText, string.Empty, paramCollection, commandType);
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="tableName">Table name</param>
        /// <param name="paramCollection">Parameter collection to be associated with the Command.</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, string tableName, ParameterCollection paramCollection)
        {
            return ExecuteDataTable(commandText, tableName, paramCollection, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command</param>
        /// <param name="paramCollection">Parameter collection to be associated with the Command.</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, ParameterCollection paramCollection)
        {
            return ExecuteDataTable(commandText, string.Empty, paramCollection, CommandType.Text);
        }


        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>
        /// <param name="tableName">Table name</param>
        /// <param name="param">Parameter to be associated with the Command.</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, string tableName, Parameter param, CommandType commandType)
        {
            return ExecuteDataTable(commandText, tableName, param, commandType);
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>        
        /// <param name="param">Parameter to be associated with the Command.</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, Parameter param, CommandType commandType)
        {
            return ExecuteDataTable(commandText, string.Empty, param, commandType);
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command</param>        
        /// <param name="tableName">Table name</param>
        /// <param name="param">Parameter to be associated with the Command.</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, string tableName, Parameter param)
        {
            return ExecuteDataTable(commandText, tableName, param, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command</param>        
        /// <param name="param">Parameter to be associated with the Command.</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, Parameter param)
        {
            return ExecuteDataTable(commandText, string.Empty, param, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>        
        /// <param name="tableName">Table name</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, string tableName, CommandType commandType)
        {
            return ExecuteDataTable(commandText, tableName, new ParameterCollection(), commandType);
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>        
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, CommandType commandType)
        {
            return ExecuteDataTable(commandText, string.Empty, commandType);
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command</param>        
        /// <param name="tableName">Table name</param>
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, string tableName)
        {
            return ExecuteDataTable(commandText, tableName, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command and return resultset in the form of DataTable.
        /// </summary>
        /// <param name="commandText">Sql Command</param>   
        /// <returns>Result in the form of DataTable</returns>
        public DataTable ExecuteDataTable(string commandText)
        {
            return ExecuteDataTable(commandText, string.Empty, CommandType.Text);
        }
        #endregion

        #region "ExecuteReader"
        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns the DataReader.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>        
        /// <param name="con">Database Connection object (DBHelper.GetConnObject() may be used)</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, IDbConnection con, CommandType commandType)
        {
            IDataReader dataReader = null;
            IDbCommand command = _commandBuilder.GetCommand(commandText, con, commandType);
            dataReader = command.ExecuteReader();
            command.Dispose();
            return dataReader;
        }

        /// <summary>
        /// Executes the Sql Command and returns the DataReader.
        /// </summary>
        /// <param name="commandText">Sql Command</param>        
        /// <param name="con">Database Connection object (DBHelper.GetConnObject() may be used)</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, IDbConnection con)
        {
            return ExecuteDataReader(commandText, con, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns the DataReader.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>        
        /// <param name="con">Database Connection object (DBHelper.GetConnObject() may be used)</param>
        /// <param name="param">Parameter to be associated with the Sql Command or Stored Procedure.</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, IDbConnection con, Parameter param, CommandType commandType)
        {
            IDataReader dataReader = null;
            IDbCommand command = _commandBuilder.GetCommand(commandText, con, param, commandType);
            dataReader = command.ExecuteReader();
            command.Dispose();
            return dataReader;
        }

        /// <summary>
        /// Executes the Sql Command and returns the DataReader.
        /// </summary>
        /// <param name="commandText">Sql Command</param>        
        /// <param name="con">Database Connection object (DBHelper.GetConnObject() may be used)</param>
        /// <param name="param">Parameter to be associated with the Sql Command.</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, IDbConnection con, Parameter param)
        {
            return ExecuteDataReader(commandText, con, param, CommandType.Text);
        }

        /// <summary>
        /// Executes the Sql Command or Stored Procedure and returns the DataReader.
        /// </summary>
        /// <param name="commandText">Sql Command or Stored Procedure Name</param>        
        /// <param name="con">Database Connection object (DBHelper.GetConnObject() may be used)</param>
        /// <param name="paramCollection">Parameter to be associated with the Sql Command or Stored Procedure.</param>
        /// <param name="commandType">Type of command (i.e. Sql Command/ Stored Procedure name/ Table Direct)</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, IDbConnection con, ParameterCollection paramCollection, CommandType commandType)
        {
            IDataReader dataReader = null;
            IDbCommand command = _commandBuilder.GetCommand(commandText, con, paramCollection, commandType);
            dataReader = command.ExecuteReader();
            command.Dispose();
            return dataReader;
        }

        /// <summary>
        /// Executes the Sql Command and returns the DataReader.
        /// </summary>
        /// <param name="commandText">Sql Command</param>        
        /// <param name="con">Database Connection object (DBHelper.GetConnObject() may be used)</param>
        /// <param name="paramCollection">Parameter to be associated with the Sql Command or Stored Procedure.</param>
        /// <returns>DataReader</returns>
        public IDataReader ExecuteDataReader(string commandText, IDbConnection con, ParameterCollection paramCollection)
        {
            return ExecuteDataReader(commandText, con, paramCollection, CommandType.Text);
        }


        #endregion

        public string[] GetDataInArray(string Query)
        {
            string[] DBResult = new string[20];
            DataSet ds = new DataSet();
            ds = ExecuteDataSet(Query);
            for (int i = 0; i < 20; i++)
                DBResult[i] = string.Empty;

            for (int count = 0; count < ds.Tables[0].Columns.Count; count++)
            {
                if (ds.Tables[0].Rows[0][count] != null)
                    DBResult[count] = ds.Tables[0].Rows[0][count].ToString().Trim();
                else
                    DBResult[count] = string.Empty;
            }

            return DBResult;
        }

        public ArrayList GetDataInArrayList(string sqlCommand)
        {
            ArrayList DBResult = new ArrayList();
            DataSet ds = new DataSet();
            ds = ExecuteDataSet(sqlCommand);

            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int count = 0; count < ds.Tables[0].Columns.Count; count++)
                {
                    if (ds.Tables[0].Rows[0][count] != null)
                        if (ds.Tables[0].Rows[0][count].ToString() != string.Empty)
                            DBResult.Add(ds.Tables[0].Rows[0][count].ToString());
                }
            }
            return DBResult;
        }

        /// <summary>
        /// Creates and opens the database connection for the connection parameters specified into web.config or App.config file.
        /// </summary>
        /// <returns>Database connection object in the opened state. </returns>
        public IDbConnection GetConnObject()
        {
            return _connectionManager.GetIDbConnection();
        }

        public DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public int BulkInsert<T>(IList<T> lists)
        {
            if (lists.Count < 1)
            {
                return -1;
            }

            String tableName = PluralizationService.CreateService(new CultureInfo("en-US"))
                                                   .Pluralize(typeof(T).Name);
            DataTable dataTable = ToDataTable(lists);
            using (var connection = new SqlConnection(_connectionManager.GetIDbConnection().ConnectionString))
            {
                SqlTransaction transaction = null;
                connection.Open();
                try
                {
                    transaction = connection.BeginTransaction();
                    using (var sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction))
                    {
                        sqlBulkCopy.DestinationTableName = tableName;
                        String[] columns = Columns(tableName);
                        String[] columnsInDatatable = dataTable.Columns.Cast<DataColumn>()
                                                        .Select(x => x.ColumnName)
                                                        .ToArray();
                        columns = columns.Intersect(columnsInDatatable).ToArray();
                        foreach (var col in columns)
                        {
                            sqlBulkCopy.ColumnMappings.Add(col, col);
                        }
                        sqlBulkCopy.WriteToServer(dataTable);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
            return dataTable.Rows.Count;
        }

        public string[] Columns(String tableName)
        {            
            String sql = "select distinct COLUMN_NAME " +
                         "from   INFORMATION_SCHEMA.COLUMNS  " +
                         "where  TABLE_NAME = @TableName";
            ParameterCollection paramCollection = new ParameterCollection();
            paramCollection.Add(new Parameter("@TableName", tableName));
            DataTable dataTable = ExecuteDataTable(sql, paramCollection);
            List<String> cols = new List<string>();
            foreach(DataRow dr in dataTable.Rows)
            {
                cols.Add(dr["COLUMN_NAME"].ToString());
            }
            return cols.ToArray();
        }
    }
}
