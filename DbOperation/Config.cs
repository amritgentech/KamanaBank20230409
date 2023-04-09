using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperations
{
    public class Config
    {
        /// <summary>
        /// Constant variable storing Default Sql Server Connection String.
        /// Sql Server Default Connection String = "DefaultSQLServerConnectionString".
        /// If the sql server has default connection string in app.config or web.config,
        /// just call the public empty constructor of Configuration class.
        /// </summary>
        public const String DEFAULT_SQL_SERVER_CONNECTION_KEY = "defaultConnection";
        
        /// <summary>
        /// Constant variable storing Default Oracle Connection String.
        /// Oracle Default Connection String = "DefaultSQLServerConnectionString".
        /// If the sql server has default connection string in app.config or web.config,
        /// just call the public empty constructor of Configuration class.
        /// </summary>
        public const String DEFAULT_ORACLE_CONNECTION_KEY = "DefaultOracleConnectionString";


        /// <summary>
        /// To be used when more than two connection string are required.
        /// </summary>
        public const String SQL_SERVER_LIVE_CONNECTION_KEY = "LiveConnectionKey";

        /**
         * Edited for Laxmi Kiosk
         *
         */

        public const String CbsSqlServerConnectionKey = "CbsSqlServerConnectionKey";
        public const String ReconContextConnectionString = "ReconContextConnectionString";
        public const String KIOSK_SQL_SERVER_CONNECTION_KEY = "KioskSqlServerConnectionKey";
        public const String CBS_SQL_SERVER_CONNECTION_KEY = "CbsSqlServerConnectionKey";
        public const String CBS_ORACLE_CONNECTION_KEY = "CbsOracleConnectionKey";
        public const String CBS_POSTGRE_CONNECTION_KEY = "CbsPostGreConnectionKey";
        public const String SWITCH_SQL_SERVER_CONNECTION_KEY = "SwitchSqlServerConnectionKey";
        public const String SWITCH_ORACLE_CONNECTION_KEY = "SwitchOracleConnectionKey";
        public const String SWITCH_POSTGRE_CONNECTION_KEY = "SwitchPostGreConnectionKey";
    }
}
