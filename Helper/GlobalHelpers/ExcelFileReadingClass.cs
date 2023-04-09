using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using OfficeOpenXml;
using System.IO;

namespace Helper.GlobalHelpers
{
    public class ExcelFileReadingClass
    {
        static List<int> columnsToAdd;
        System.Data.DataTable dtable = new System.Data.DataTable();
        System.Data.DataTable dtable2 = new System.Data.DataTable();

        public System.Data.DataTable ReadExcelFileEPPwithName(string EXCEL_PATH)
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
                            //for (int r = dim.Start.Row; r <= dim.End.Row; ++r)
                            for (int r = 1; r <= dim.End.Row; ++r)
                            {
                                //string theCell = GetRangeText(ws.Cells[ws.MergedCells[ws.GetMergeCellId(r, 19) - 1]]);

                                data = new object[dim.End.Column + 1];
                                for (int c = dim.Start.Column; c <= dim.End.Column; ++c)
                                {
                                    var a = ws.Cells[r, c].Value;
                                    //if (ws.Cells[r, c].Merge) continue;
                                    //string s;
                                    //if (ws.Cells[r, c].Merge)
                                    //{
                                    //    int i = ws.GetMergeCellId(r, c) - 1;
                                    //    s = GetRangeText(ws.Cells[ws.MergedCells[i]]);
                                    //}
                                    //else
                                    //    s = GetRangeText(ws.Cells[r, c]);
                                    //if (string.IsNullOrEmpty(s)) continue;
                                    data[c-1] = a;

                                }
                                if (addrow)
                                {
                                    //if(data[18].ToString()=="Budget Owner")
                                    FilterAndAddRow(data);
                                }
                                else
                                {
                                    for (int col = 0; col < data.Length; col++)
                                    {
                                        if (data[col] != null)
                                        {
                                            string colName = data[col].ToString().Replace("\n", " ");
                                            DataColumnCollection columns = dtable.Columns;
                                            if (!columns.Contains(colName))
                                            {
                                                dtable.Columns.Add(colName, typeof(String));
                                                columnsToAdd.Add(col);
                                            }
                                        }
                                    }
                                    addrow = true;
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
                                    for (int col = 0; col < data.Length; col++)
                                    {
                                        if (data[col] != null)
                                        {
                                            string colName = data[col].ToString().Replace("\n", " ");
                                            DataColumnCollection columns = dtable.Columns;
                                            if (!columns.Contains(colName))
                                            {
                                                dtable.Columns.Add(colName, typeof(String));
                                                columnsToAdd.Add(col);
                                            }
                                        }
                                    }
                                    addrow = true;

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

        public void AddRow(object[] data)
        {
            System.Data.DataRow drow = dtable.NewRow();
            //System.Data.DataRow drow = new System.Data.DataRow();
            for (int count = 0; count < data.Length; count++)
            {
                drow[count] = data[count];

            }
            dtable.Rows.Add(drow);

        }

        public void FilterAndAddRow(object[] data)
        {
            System.Data.DataRow drow = dtable.NewRow();
            //System.Data.DataRow drow = new System.Data.DataRow();
            //for (int count = 2; count < data.Length; count++)
            //{

            //    drow[count] = data[count];

            //}
            for (int i = 0; i < columnsToAdd.Count; i++)
                drow[i] = data[columnsToAdd[i]];
            dtable.Rows.Add(drow);

        }
    }
}