using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DbOperations
{
    /// <summary>
    /// Configuration class. This class initializes the configuration. Specially Connection String.
    /// Please view CONFIG.class for ser of default connection string names.
    /// Created BY: Mubin Shrestha
    /// </summary>
    internal static class Configuration
    {
        private static string connectionStringKey = Config.DEFAULT_SQL_SERVER_CONNECTION_KEY;
        private static string liveConnectionStringKey = Config.SQL_SERVER_LIVE_CONNECTION_KEY;

        /**
         * Edited for Laxmi Kiosk to support multiple db connection
         */
        private static string cbsSqlServerConnectionStringKey = Config.CBS_SQL_SERVER_CONNECTION_KEY;
        private static string cbsOracleConnectionStringKey = Config.CBS_ORACLE_CONNECTION_KEY;
        private static string cbsPostGreConnectionStringKey = Config.CBS_POSTGRE_CONNECTION_KEY;
        private static string swtichOracleConnectionStringKey = Config.SWITCH_ORACLE_CONNECTION_KEY;
        private static string switchSqlServerConnectionStringKey = Config.SWITCH_SQL_SERVER_CONNECTION_KEY;
        private static string switchPostGreConnectionStringKey = Config.SWITCH_POSTGRE_CONNECTION_KEY;

        public static String DefaultConnection
        {
            get
            {
                return ConfigurationManager.AppSettings[connectionStringKey];
            }
        }

        public static string LiveConnection
        {
            get
            {
                return ConfigurationManager.AppSettings[liveConnectionStringKey];
            }
        }

        public static string connectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[DefaultConnection].ConnectionString;
            }
        }

        public static string liveConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[LiveConnection].ConnectionString;
            }
        }

        public static String providerName
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[DefaultConnection].ProviderName.ToUpper();
            }
        }

        internal static String GetConnectionString(string providerName)
        {            
            switch (providerName)
            {
                case Common.CbsSqlServerConnectionKey:
                    return ConfigurationManager.ConnectionStrings[Config.CbsSqlServerConnectionKey].ConnectionString;
                case Common.ReconContextConnectionString:
                    return ConfigurationManager.ConnectionStrings[Config.ReconContextConnectionString].ConnectionString;
                case Common.CBS_SQL_SERVER_DB_PROVIDER:
                    return ConfigurationManager.ConnectionStrings[Config.CBS_SQL_SERVER_CONNECTION_KEY].ConnectionString;
                case Common.SWTICH_SQL_SERVER_DB_PROVIDER:
                    return ConfigurationManager.ConnectionStrings[Config.CBS_SQL_SERVER_CONNECTION_KEY].ConnectionString;
                case Common.CBS_ORACLE_DB_PROVIDER:
                    return ConfigurationManager.ConnectionStrings[Config.CBS_ORACLE_CONNECTION_KEY].ConnectionString;
                case Common.SWITCH_ORACLE_DB_PROVIDER:
                    return ConfigurationManager.ConnectionStrings[Config.SWITCH_ORACLE_CONNECTION_KEY].ConnectionString;
                case Common.ORACLE_DB_PROVIDER:
                    return ConfigurationManager.ConnectionStrings[Config.SWITCH_ORACLE_CONNECTION_KEY].ConnectionString;
                case Common.CBS_POSTGRE_DB_PROVIDER:
                    return ConfigurationManager.ConnectionStrings[Config.CBS_POSTGRE_CONNECTION_KEY].ConnectionString;
                case Common.SWITCH_POSTGRE_DB_PROVIDER:
                    return ConfigurationManager.ConnectionStrings[Config.SWITCH_POSTGRE_CONNECTION_KEY].ConnectionString;
                default:
                    return null;
            }
        }
    }
}
