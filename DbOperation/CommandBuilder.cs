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
    internal class CommandBuilder
    {
        private String ProviderName;
        public CommandBuilder(String ProviderName)
        {
            this.ProviderName = ProviderName;
            _paramBuilder = new ParamBuilder(ProviderName);
        }

        private ParamBuilder _paramBuilder;

        #region Internal Methods
        internal IDbCommand GetCommand(string commandText, IDbConnection connection)
        {
            return GetCommand(commandText, connection, CommandType.Text);
        }


        internal IDbCommand GetCommand(string commandText, IDbConnection connection, CommandType commandType)
        {
            IDbCommand command = GetCommand();
            command.CommandText = commandText;
            command.Connection = connection;
            command.CommandType = commandType;
            return command;
        }


        internal IDbCommand GetCommand(string commandText, IDbConnection connection, Parameter parameter)
        {
            return GetCommand(commandText, connection, parameter, CommandType.Text);
        }

        internal IDbCommand GetCommand(string commandText, IDbConnection connection, Parameter parameter, CommandType commandType)
        {
            IDataParameter param = _paramBuilder.GetParameter(parameter);
            IDbCommand command = GetCommand(commandText, connection, commandType);
            command.Parameters.Add(param);
            return command;
        }

        internal IDbCommand GetCommand(string commandText, IDbConnection connection, ParameterCollection parameterCollection)
        {
            return GetCommand(commandText, connection, parameterCollection, CommandType.Text);
        }

        internal IDbCommand GetCommand(string commandText, IDbConnection connection, ParameterCollection parameterCollection, CommandType commandType)
        {
            List<IDataParameter> paramArray = _paramBuilder.GetParameterCollection(parameterCollection);
            IDbCommand command = GetCommand(commandText, connection, commandType);

            foreach (IDataParameter param in paramArray)
                command.Parameters.Add(param);

            return command;
        }


        #endregion

        #region Private Methods"

        private IDbCommand GetCommand()
        {
            IDbCommand command = null;
            switch (ProviderName)
            {
                case Common.SQL_SERVER_DB_PROVIDER:
                    command = new SqlCommand();
                    break;
                case Common.CbsSqlServerConnectionKey:
                    command = new SqlCommand();
                    break;
                case Common.ReconContextConnectionString:
                    command = new SqlCommand();
                    break;
                case Common.SWTICH_SQL_SERVER_DB_PROVIDER:
                    command = new SqlCommand();
                    break;
                //case Common.MY_SQL_DB_PROVIDER:
                //    command = new MySqlCommand();
                //    break;
                case Common.ORACLE_DB_PROVIDER:
                    command = new OracleCommand();
                    break;
                case Common.CBS_ORACLE_DB_PROVIDER:
                    command = new OracleCommand();
                    break;
                case Common.SWITCH_ORACLE_DB_PROVIDER:
                    command = new OracleCommand();
                    break;
                case Common.EXCESS_DB_PROVIDER:
                    command = new OleDbCommand();
                    break;
                case Common.OLE_DB_PROVIDER:
                    command = new OleDbCommand();
                    break;
                case Common.ODBC_DB_PROVIDER:
                    command = new OdbcCommand();
                    break;
                case Common.CBS_SQL_SERVER_DB_PROVIDER:
                    command = new SqlCommand();

                    break;
                    

            }

            return command;
        }

        #endregion
    }
}
