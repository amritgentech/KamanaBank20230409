using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.Odbc;

namespace ReadWriteFiles
{
    public class ImportFromCSV
    {

        #region Variable Declaration
        
        private OdbcConnection cn;
        private OdbcDataAdapter daAdapter;
        private string strConnectionString;
        string sql_select;
        private string strFormat = string.Empty;
               
        #endregion

        public ImportFromCSV()
		{
    
		}

        public string Format(string format, string delimited)
        {            
            try
            {
                if (format == "CSV Delimited")
                    strFormat = "CSVDelimited";
                else if (format == "Tab Delimited")
                    strFormat = "TabDelimited";
                else if (format == "Custom Delimited")
                    strFormat = "Delimited(" + delimited + ")";
                else
                    strFormat = "Delimited(;)";

            }
            catch (Exception)
            {
                throw;
            }
            return strFormat;
        }

        # region Create schema.ini
        /*Schema.ini File (Text File Driver)

		When the Text driver is used, the format of the text file is determined by using a
		schema information file. The schema information file, which is always named Schema.ini
		and always kept in the same directory as the text data source, provides the IISAM 
		with information about the general format of the file, the column name and data type
		information, and a number of other data characteristics*/

        private void writeSchema(string path, string file, string format, string delimiter)
        {
            try
            {
                string returnFormat = Format(format, delimiter);

                FileStream fsOutput = new FileStream(path + "\\schema.ini", FileMode.Create, FileAccess.Write);
                StreamWriter srOutput = new StreamWriter(fsOutput);
                string s1, s2, s3, s4, s5;
                s1 = "[" + file + "]";
                s2 = "ColNameHeader= true";
                s3 = "Format=" + returnFormat;
                s4 = "MaxScanRows=250";
                s5 = "CharacterSet=OEM";
                srOutput.WriteLine(s1.ToString() + '\n' + s2.ToString() + '\n' + s3.ToString() + '\n' + s4.ToString() + '\n' + s5.ToString());
                srOutput.Close();
                fsOutput.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            { }
        }
        #endregion


        # region Function For Importing Data From CSV File

        public DataTable ConvertToCVS(string path, string file, string format, string delimiter)
        {
            writeSchema(path, file, format, delimiter);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                // You can get connected to driver either by using DSN or connection string

                // Create a connection string as below, if you want to use DSN less connection. The DBQ attribute sets the path of directory which contains CSV files
                strConnectionString = "Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + path.Trim() + ";Extensions=asc,csv,tab,txt;Persist Security Info=False";
                           
                //Create connection to CSV file
                cn = new OdbcConnection(strConnectionString.Trim());

                // For creating a connection using DSN, use following line
                //conn	=	new System.Data.Odbc.OdbcConnection(DSN="MyDSN");
                                
                cn.Open();
                
                //Fetch records from CSV
                sql_select = "select * from [" + file + "]";

                daAdapter = new System.Data.Odbc.OdbcDataAdapter(sql_select, cn);
                daAdapter.Fill(ds, "CustomerInfo");
                //Fill datatable with the records from CSV file
                daAdapter.Fill(dt);

                                        
                //Close Connection to CSV file
                cn.Close();
            }
            catch (Exception) //Error
            {
                throw;
            }
            return dt;
        }

        # endregion
        				
    }
}
