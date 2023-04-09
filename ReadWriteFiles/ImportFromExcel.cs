using System;
using System.Data; 
using System.Data.OleDb ; 
using System.IO;
using System.Xml;
using System.Xml.Schema;



namespace ReadWriteFiles
{
	/// <summary>
	/// Summary description for ExcelToXML.
	/// </summary>
	public class ImportFromExcel 
	{
		#region Variable Declaration
		private OleDbConnection cn;
		private OleDbDataAdapter daAdapter; 

		private string ExcelCon = @"Provider=Microsoft.Jet.OLEDB.4.0;";
        private string ExcelsCon = @"Provider=Microsoft.ACE.OLEDB.12.0;";
		private string strConnectionString;

		private string SheetName,Range;
		#endregion

		#region Constructors
		/// <summary>
		/// Initialize ExcelXML component using the sepecifed File name, By default HDR property will be false.
		/// </summary>
		/// <param name="strFileName"></param>
		public ImportFromExcel(string strFileName)
		{
            strConnectionString = ExcelCon + "Data Source=" + strFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + "Excel 8.0;HDR=No" + Convert.ToChar(34).ToString();
			cn=new OleDbConnection(); 
			cn.ConnectionString = strConnectionString; 
		}

        public ImportFromExcel(string strFileName, int fileTypeVersion)
        {
            if (fileTypeVersion == 1)
            {
                strConnectionString = ExcelCon + "Data Source=" + strFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + "Excel 8.0;HDR=No" + Convert.ToChar(34).ToString();
            }
            else if (fileTypeVersion == 2)
            {
                strConnectionString = ExcelsCon + "Data Source=" + strFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + "Excel 8.0;HDR=No" + Convert.ToChar(34).ToString();
            }
            cn = new OleDbConnection();
            cn.ConnectionString = strConnectionString;
        }

		/// <summary>
		/// Initialize ExcelXML component using the specified File name, you can specify HDR value using _blnHeaders
		/// </summary>
		/// <param name="strFileName"></param>
		/// <param name="_blnHeaders"></param>
		public ImportFromExcel(string strFileName,Boolean _blnHeaders)
		{
			if(_blnHeaders) 
				strConnectionString=ExcelCon+"Data Source="+strFileName+";Extended Properties="+ Convert.ToChar(34).ToString()+"Excel 8.0;HDR=Yes"+Convert.ToChar(34).ToString();
			else
                strConnectionString = ExcelsCon + "Data Source=" + strFileName + ";Extended Properties=" + Convert.ToChar(34).ToString() + "Excel 12.0;HDR=Yes" + Convert.ToChar(34).ToString();
				//strConnectionString=ExcelCon+"Data Source="+strFileName+";Extended Properties="+ Convert.ToChar(34).ToString()+"Excel 8.0;HDR=Yes"+Convert.ToChar(34).ToString();

			cn=new OleDbConnection(); 
			cn.ConnectionString = strConnectionString;
		}

		#endregion

		#region Functionality

		#region XML Functionality
		public string GetXML(string strSheetName,Boolean _blnSchema)
		{
			DataSet ds=new DataSet();  			
			ds.Tables.Add(this.GetDataTable(strSheetName));  

			if(_blnSchema)
                return ds.GetXmlSchema()+ds.GetXml();  			
			else
				return ds.GetXml();  			
		}
		public string GetXMLSchema(string strSheetName)
		{
			DataSet ds=new DataSet();  
			ds.Tables.Add(this.GetDataTable(strSheetName));  
			return ds.GetXmlSchema();
		}

		public string[] GetAllXML()
		{
			string[] excelSheet=GetExcelSheetNames();
			
			DataSet dsExcelData=new DataSet(); 
			DataTable dt=new DataTable();  
			foreach(string strSheetName in excelSheet)
			{
				dsExcelData.Tables.Add(this.GetDataTable(strSheetName));  
			}
			string[] xml=new string[2];
			xml[0]=dsExcelData.GetXmlSchema();
			xml[1]=dsExcelData.GetXml();
			return xml;  
		}

		#endregion

		#region Excel File Info
		public String[] GetExcelSheetNames()
		{
			
			System.Data.DataTable dt = null;

			try
			{
                cn.Close();
                cn.Open();
				
				// Get the data table containing the schema
				dt = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
 
				if(dt == null)
				{
					return null;
				}

				String[] excelSheets = new String[dt.Rows.Count];
				int i = 0;

				// Add the sheet name to the string array.
				foreach(DataRow row in dt.Rows)
				{
					string strSheetTableName = row["TABLE_NAME"].ToString();
					excelSheets[i] = strSheetTableName.Substring(0,strSheetTableName.Length-1); 
					i++;
				}

                cn.Close();
				return excelSheets;
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message) ;
			}
			finally
			{
				// Clean up.
				cn.Close();				
			}
		}

		public DataTable GetDataTable(string strSheetName)
		{
			
			try
			{
				string strComand;
				if(strSheetName.IndexOf("|") > 0)
				{
					SheetName = strSheetName.Substring(0,strSheetName.IndexOf("|"));
					Range = strSheetName.Substring(strSheetName.IndexOf("|")+1);
					strComand="select * from ["+SheetName+"$"+Range+"]";	
				}
				else
				{
					strComand="select * from ["+strSheetName+"$]";	
				}

				
				daAdapter=new OleDbDataAdapter(strComand,cn);
				DataTable dt=new DataTable(strSheetName);
				daAdapter.FillSchema(dt,SchemaType.Source);
				daAdapter.Fill(dt); 
				cn.Close(); 
				return dt ;
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);  
			}
		}

        public DataSet GetDataSet(string strSheetName)
        {
            try
            {
                string strComand;
                if (strSheetName.IndexOf("|") > 0)
                {
                    SheetName = strSheetName.Substring(0, strSheetName.IndexOf("|"));
                    Range = strSheetName.Substring(strSheetName.IndexOf("|") + 1);
                    strComand = "select * from [" + SheetName + "$" + Range + "]";
                }
                else
                {
                    strComand = "select * from [" + strSheetName + "$]";
                }


                daAdapter = new OleDbDataAdapter(strComand, cn);
                DataSet ds = new DataSet();
                daAdapter.Fill(ds);
                cn.Close();
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
		
		#endregion

		#region Save Functionality
		public void SaveSheetXML(string strFileName,string strSheetName,Boolean _blnSchema)
		{
			try
			{
				string strFile = strFileName.Substring(strFileName.LastIndexOf("\\")+1); 
				string path = strFileName.Substring(0,strFileName.LastIndexOf("\\")); 
				strFile = strFile.Remove(strFile.IndexOf("."),4);     
				SaveFile(path,strFile,strSheetName); 
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);  
			}
		}

		private void SaveFile(string path,string strFile,string strSheetName)
		{
			FileStream file=new FileStream(path+"\\"+strFile+".xml" ,FileMode.Create);
			StreamWriter wr=new StreamWriter(file);
			wr.Write("<?xml version='1.0'?>"+this.GetXML(strSheetName,false));   
			wr.Close();
			file.Close();  
			file=new FileStream(path+"\\"+strFile+".xsd" ,FileMode.Create);
			wr=new StreamWriter(file);
			wr.Write(this.GetXMLSchema(strSheetName).Replace("utf-16","utf-8"));   
			wr.Close();
			file.Close();  
		}
		public void SaveXslXml(string strFileName,Boolean _blnSchema, Boolean _blnMulti)
		{
			string[] excelSheet=GetExcelSheetNames();
			string strFile = strFileName.Substring(strFileName.LastIndexOf("\\")+1); 
			string path = strFileName.Substring(0,strFileName.LastIndexOf("\\")); 
			strFile = strFile.Remove(strFile.IndexOf("."),4);   

			if(_blnMulti)
			{
				foreach(string strSheetName in excelSheet)
				{
					this.SaveFile(path,strFile+"_"+strSheetName,strSheetName);  
				
				}
			}
			else
			{
				string[] xml=this.GetAllXML(); 
				FileStream file=new FileStream(path+"\\"+strFile+".xml" ,FileMode.Create);
				StreamWriter wr=new StreamWriter(file);
				wr.Write("<?xml version='1.0'?>"+xml[1]);   
				wr.Close();
				file.Close();  
				file=new FileStream(path+"\\"+strFile+".xsd" ,FileMode.Create);
				wr=new StreamWriter(file);
				wr.Write(xml[0].Replace("utf-16","utf-8"));   
				wr.Close();
				file.Close();  
			}

		}

		#endregion

		#endregion

	}
}
