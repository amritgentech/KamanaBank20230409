using Db.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReconParser.App_Code.Recon.DigitalBanking
{
    public class DigitalBanking : EBase
    {
        System.Data.DataTable dtable = new System.Data.DataTable();
        static List<int> columnsToAdd;
        public DigitalBanking(String FileName, int FileCount)
                 : base(FileName, FileCount)
        {
            _Source = Source.DigitalBanking();
        }

        public System.Data.DataTable ReadExcelFileEPPUnmerged(string EXCEL_PATH)
        {
            try
            {
                dtable = new System.Data.DataTable();
                // Get the file we are going to process
                var existingFile = new FileInfo(EXCEL_PATH);
                // Open and read the XlSX file.
                columnsToAdd = new List<int>();
                using (var pck = new ExcelPackage(existingFile))
                {
                    var workBook = pck.Workbook;
                    if (workBook != null)
                    {
                        if (workBook.Worksheets.Count > 0)
                        {
                            // Get the first worksheet
                            var ws = workBook.Worksheets[1];
                            var dim = ws.Dimension;
                            // first loop through all non-merged cells
                            object[] data = null;
                            bool addrow = false;
                            for (int r = dim.Start.Row; r <= dim.End.Row; ++r)
                            {
                                data = new object[dim.End.Column + 1];
                                for (int c = dim.Start.Column; c <= dim.End.Column; ++c)
                                {
                                    string s = GetRangeText(ws.Cells[r, c]);
                                    if (string.IsNullOrEmpty(s)) continue;
                                    data[c - 1] = s;

                                }
                                if (addrow)
                                {
                                    FilterAndAddRow(data);
                                }
                                else
                                {
                                    List<string> columnlist = new List<string>();
                                    for (int col = 0; col < data.Length; col++)
                                    {
                                        if (data[col] != null)
                                        {
                                            string colName = data[col].ToString().Replace("\n", " ");
                                            DataColumnCollection columns = dtable.Columns;
                                            if (!columns.Contains(colName))
                                            {
                                                columnlist.Add(colName);
                                                columnsToAdd.Add(col);
                                            }

                                        }
                                    }
                                    if (columnsToAdd.Count < 6)
                                        columnsToAdd.Clear();
                                    else
                                        foreach (var columnname in columnlist)
                                        {
                                            dtable.Columns.Add(columnname, typeof(String));
                                            addrow = true;
                                        }
                                }
                            }
                        }
                    }
                }
                return dtable;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        private string GetRangeText(ExcelRangeBase range)
        {
            var val = range.Value;
            string s = val as string;
            if (string.IsNullOrEmpty(s))
            {
                object[,] arr = val as object[,];
                if (arr != null && arr.GetLength(0) > 0 && arr.GetLength(1) > 0)
                    s = arr[0, 0].ToString();
            }
            if (string.IsNullOrEmpty(s) && val != null)
                s = val.ToString();
            return s;
        }
        public void FilterAndAddRow(object[] data)
        {
            System.Data.DataRow drow = dtable.NewRow();

            for (int i = 0; i < columnsToAdd.Count; i++)
                drow[i] = data[columnsToAdd[i]];
            dtable.Rows.Add(drow);

        }
    }
}
