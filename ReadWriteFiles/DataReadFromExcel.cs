using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace ReadWriteFiles
{
    public class DataReadFromExcel
    {
        //public static System.Data.DataTable GetDataTableFromExcel(Stream fileStream, string fileName)
        //{
        //    IExcelDataReader excelReader = null;
        //    string ExcelFileType_97_2003 = ".xls";
        //    string ExcelFileType_2007 = ".xlsx";

        //    string extension = Path.GetExtension(fileName);


        //    if (extension == ExcelFileType_2007)
        //    {
        //        excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
        //    }
        //    else if (extension == ExcelFileType_97_2003)
        //    {
        //        excelReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
        //    }
        //    else
        //        throw new Exception("File Format is invalid");

        //    if (excelReader.IsValid == false)
        //    {
        //        throw new Exception(excelReader.ExceptionMessage);
        //    }
        //    DataSet excelDataSet = excelReader.AsDataSet();
        //    if (excelDataSet.Tables.Count == 0)
        //    {
        //        while (excelReader.Read())
        //        {
        //            string a = excelReader.GetString(0);
        //        }
        //    }
        //    excelReader.Close();
        //    return excelDataSet.Tables[0];
        //}

//        public static System.Data.DataTable ReadExcelFile(string file)
//        {
//            int columnCount = 0;
//            int maxColumn = 0;
//            ReadWriteFiles.ImportFromExcel obj = new ReadWriteFiles.ImportFromExcel(file, false);
//            System.Data.DataTable excelDataTableExcel = obj.GetDataTable("Sheet1");
//            System.Data.DataTable excelDataTable = CreateDataTable(excelDataTableExcel.Columns.Count, "Range", "Sheet1");
//            for (int i = 0; i < excelDataTableExcel.Rows.Count; i++)
//            {
//                DataRow dr = null;
//                columnCount = 0;
//                for (int j = 1; j < excelDataTableExcel.Columns.Count; j++)
//                {
//                    DataRow dataRowExcel = excelDataTableExcel.Rows[i];
//                    object objExcel = dataRowExcel[j];
//                    if (!String.IsNullOrEmpty(objExcel.ToString()))
//                    {
//                        if (dr == null)
//                        {
//                            dr = excelDataTable.NewRow();
//                        }
//                        dr[columnCount] = objExcel;
//                        columnCount++;
//                        if (maxColumn < columnCount)
//                        {
//                            maxColumn = columnCount;
//                        }
//                    }
//                }
//                if (dr != null)
//                {
//                    excelDataTable.Rows.Add(dr);
//                }
//            }
//
//            System.Data.DataTable filterDataTable = new DataView(excelDataTable).ToTable(false, getSelectedColumnName(maxColumn));
//
//            DataRow[] dataRowList = filterDataTable.Select("Range_" + maxColumn.ToString() + " is not NULL", "Range_1 asc");
//
//            System.Data.DataTable cloneDataTable = filterDataTable.Clone();
//            AddDataRow(cloneDataTable, dataRowList);
//
//            return cloneDataTable;
//        }

        public static System.Data.DataTable ReadExcelFileCBS(string file) {
            ReadWriteFiles.ImportFromExcel obj = new ReadWriteFiles.ImportFromExcel(file, false);
            System.Data.DataTable excelDataTableExcel = obj.GetDataTable("Sheet1");
            return excelDataTableExcel;
        }

        public static System.Data.DataTable ReadExcelFileSWITCH(string file)
        {
            ReadWriteFiles.ImportFromExcel obj = new ReadWriteFiles.ImportFromExcel(file, false);
            System.Data.DataTable excelDataTableExcel = obj.GetDataTable("Sheet1");
            return excelDataTableExcel;
        }

        public static System.Data.DataTable ReadExcelFileSwitch(string file, string sheetName)
        {
            ReadWriteFiles.ImportFromExcel obj = new ReadWriteFiles.ImportFromExcel(file, false);
            System.Data.DataTable excelDataTableExcel = obj.GetDataTable(sheetName);
            return excelDataTableExcel;
        }

        public static System.Data.DataTable ReadExcelFileNPN(string file, string sheetName)
        {
            ReadWriteFiles.ImportFromExcel obj = new ReadWriteFiles.ImportFromExcel(file, false);
            System.Data.DataTable excelDataTableExcel = obj.GetDataTable(sheetName);
            return excelDataTableExcel;
        }

        public static System.Data.DataTable ReadExcelFileCreditPayment(string file)
        {
            ReadWriteFiles.ImportFromExcel obj = new ReadWriteFiles.ImportFromExcel(file, false);
            string[] sheetName = obj.GetExcelSheetNames();

            System.Data.DataTable excelDataTableExcel = obj.GetDataTable(sheetName[0]);
            return excelDataTableExcel;
        }

        public static System.Data.DataTable ReadExcelFile(string file)
        {
            ReadWriteFiles.ImportFromExcel obj = new ReadWriteFiles.ImportFromExcel(file, false);
            string[] sheetName = obj.GetExcelSheetNames();
            string orisheetName = "";
            if (sheetName.Length > 0) {
                orisheetName = sheetName[0];
                orisheetName = orisheetName.Substring(1, orisheetName.Length - 2);
            }

            System.Data.DataTable excelDataTableExcel = obj.GetDataTable(orisheetName);
            return excelDataTableExcel;
        }


        //public static System.Data.DataTable ReadExcelFile(string file, int isCBSOrSCT)
        //{
        //    object Missing = Type.Missing;
        //    Application excel = new Application();

        //    Workbook workbook = excel.Workbooks.Open(file, Missing, false, Missing, Missing, Missing, Missing, Missing, Missing, Missing, Missing, Missing, Missing, Missing,
        //        Missing);

        //    Worksheet worksheet = (Worksheet)workbook.ActiveSheet;
        //    Range workrange1;

        //    Range last = worksheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell, Type.Missing);

        //    System.Data.DataTable excelDataTable = CreateDataTable(last.Column, "Range", worksheet.Name);
        //    int columnCount = 0;
        //    int maxColumn = 0;
        //    for (int i = 1; i <= last.Row; i++)
        //    {
        //        DataRow dr = null;
        //        columnCount = 0;
        //        for (int j = 1; j <= last.Column; j++)
        //        {
        //            workrange1 = (Range)worksheet.Cells[i, j];
        //            if (workrange1.Value != null)
        //            {
        //                if (dr == null)
        //                {
        //                    dr = excelDataTable.NewRow();
        //                }
        //                dr[columnCount] = workrange1.Value;
        //                columnCount++;
        //                if (maxColumn < columnCount)
        //                {
        //                    maxColumn = columnCount;
        //                }
        //            }
        //        }
        //        if (dr != null)
        //        {
        //            excelDataTable.Rows.Add(dr);
        //        }
        //    }
        //    workbook.Close();

        //    System.Data.DataTable filterDataTable = new DataView(excelDataTable).ToTable(false, getSelectedColumnName(maxColumn));
        //    if (isCBSOrSCT == 1)
        //    {
        //        DataRow[] dataRowList = filterDataTable.Select("Range_" + maxColumn.ToString() + " is not NULL", "Range_1 asc");

        //        System.Data.DataTable cloneDataTable = filterDataTable.Clone();
        //        AddDataRow(cloneDataTable, dataRowList);

        //        return cloneDataTable;
        //    }
        //    else
        //    {
        //        return filterDataTable;
        //    }
        //}

        protected static System.Data.DataTable CreateDataTable(int columnTotal, string columnName, string tableName) 
        {
            System.Data.DataTable dt = new System.Data.DataTable(tableName);
            for (int i = 1; i <= columnTotal; i++) 
            {
                dt.Columns.Add(columnName + "_" + i.ToString());
            }
            return dt;
        }

        protected static string[] getSelectedColumnName(int maxColumn) 
        {
            string[] columnNameList = new string[maxColumn];
            for (int i = 1; i <= maxColumn; i++) 
            {
                string columnName = "Range_" + i.ToString();
                columnNameList[i - 1] = columnName;
            }
            return columnNameList;
        }

        public static void AddDataRow(System.Data.DataTable dt, DataRow[] drList) 
        {
            foreach (DataRow dr in drList) 
            {
                if (dr != drList[drList.Length - 1])
                {
                    dt.ImportRow(dr);
                }
            }
        }

        public static System.Data.DataTable ReadCVSFileCBS(string file, string path)
        {
            ReadWriteFiles.ImportFromCSV obj = new ReadWriteFiles.ImportFromCSV();

            System.Data.DataTable excelDataTableExcel = obj.ConvertToCVS(path, file, "CSVDelimited", ",");
            return excelDataTableExcel;
        }
    }
}