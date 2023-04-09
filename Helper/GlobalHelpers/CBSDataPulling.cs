using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.GlobalHelpers
{
    public class CBSDataPulling
    {
        public static DataTable GetT24CBSData(string dateFrom,string dateTo)
        {
            try
            {
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();

               string strConn = ConfigurationManager.ConnectionStrings["sqlT24CBSConnectionString"].ToString();
               string selectStatement = "SELECT Id,Transaction_type,credit_amount,debit_acct_no,Company,debit_value_date,ATM_Term_Id,At_Unique_Id,Card_Number,credit_acct_no,record_status,Network_id,TXN_RR,[Date],[Time] FROM vw_CardDetails ";
               selectStatement += " where Date between '" + dateFrom + "' and '" + dateTo + "';";

               SqlConnection sqlcn = new SqlConnection(strConn);

               try
               {
                   sqlcn.Open();
                   SqlDataAdapter da = new SqlDataAdapter(selectStatement, sqlcn);
                   da.SelectCommand.CommandTimeout = 120;
                   da.Fill(ds);
                   dt = ds.Tables[0];
               }
               catch { throw; }
               finally { sqlcn.Close(); }

               return dt;

            }
            catch  {
                return new DataTable();
            }
        }

        public static DataTable GetPumoriCBSData(string dateFrom, string dateTo)
        {
            try
            {
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();

                string strConn = ConfigurationManager.ConnectionStrings["sqlPumoriCBSConnectionString"].ToString();
                string selectStatement = "SELECT TranDate,[TimeStamp],TerminalID,CardNo,Amount,traceNo,RESPONSE_CODE,Type_Of_NT,TYPE_OF_TERMINAL,ACQUIR_ID,DR_CR,Currency,AuthCode, IS_FINANCIAL FROM v_CardDetails ";
                selectStatement += " where Date between '" + dateFrom + "' and '" + dateTo + "';";

                SqlConnection sqlcn = new SqlConnection(strConn);

                try
                {
                    sqlcn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(selectStatement, sqlcn);
                    da.SelectCommand.CommandTimeout = 120;
                    da.Fill(ds);
                    dt = ds.Tables[0];
                }
                catch { throw; }
                finally { sqlcn.Close(); }

                return dt;

            }
            catch
            {
                return new DataTable();
            }
        }
    }
}
