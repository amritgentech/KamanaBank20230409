using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Helper.GlobalHelpers
{
    public static class GlobalHelper
    {
        public static IList<int> GetPage(IList<int> list, int page, int pageSize)
        {
            return list.Skip(page * pageSize).Take(pageSize).ToList();
        }
        public static DateTime? ChangeDateFormat(string inputDate)
        {
            DateTimeFormatInfo dateFormatInfo = new DateTimeFormatInfo();
            dateFormatInfo.ShortDatePattern = @"dd/MM/yyyy";
            DateTime? datetime = null;
            if (!string.IsNullOrEmpty(inputDate))
                datetime = Convert.ToDateTime(inputDate, dateFormatInfo);

            return datetime;
        }
        public static IList<T> DatatableToClass<T>(DataTable Table) where T : class, new()
        {
            if (Table.Rows.Count <= 0)
                return new List<T>();

            Type classType = typeof(T);
            IList<PropertyInfo> propertyList = classType.GetProperties();

            // Parameter class has no public properties.
            if (propertyList.Count == 0)
                return new List<T>();

            List<string> columnNames = Table.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList();

            List<T> result = new List<T>();
            try
            {
                foreach (DataRow row in Table.Rows)
                {
                    T classObject = new T();
                    foreach (PropertyInfo property in propertyList)
                    {
                        if (property != null && property.CanWrite)   // Make sure property isn't read only
                        {
                            if (columnNames.Contains(property.Name))  // If property is a column name
                            {
                                object propertyValue = null;
//                                if (row[property.Name] != DBNull.Value)   // Don't copy over DBNull
                                if (!string.IsNullOrEmpty(Convert.ToString(row[property.Name])))   // Don't copy over DBNull
                                {
                                    propertyValue =
                                        Convert.ChangeType(row[property.Name],
                                            Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);

                                    property.SetValue(classObject, propertyValue, null);
                                }
                            }
                        }
                    }
                    result.Add(classObject);
                }
                return result;
            }
            catch (Exception ex)
            {
                return new List<T>();
            }
        }
        public static DataTable ListToDataTable<T>(this IQueryable items)
        {
            Type type = typeof(T);

            var props = TypeDescriptor.GetProperties(type)
                .Cast<PropertyDescriptor>()
                .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                .Where(propertyInfo => propertyInfo.IsReadOnly == false)
                .ToArray();

            var table = new DataTable();

            foreach (var propertyInfo in props)
            {
                table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
            }

            foreach (var item in items)
            {
                table.Rows.Add(props.Select(property => property.GetValue(item)).ToArray());
            }

            return table;
        }
        public static string NullHelperString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "TestVal";
            }
            return value;
        }
        public static Decimal NullHelperDecimal(Decimal? value)
        {
            return value ?? 0;
        }
        public static DateTime NullHelperDate(DateTime? value)
        {
            return value ?? DateTime.Now.Date;
        }
        public static void ErrorLogging(Exception ex)
        {
            string filePath = @"C:\ReconErrorLog\Log.txt";

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                                 "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }
        public static string ChangeDateFormatWithDash(string input)
        {
            string result = null;
            
            try
            {
                var output = input.Split('/');
                result = output[2] + "-" + output[1] + "-" + output[0];
            }
            catch {
                var output = input.Split('-');
                result = output[2] + "-" + output[1] + "-" + output[0];
            }
           
            return result;
        }
        public static void StepLogging(String step)
        {
            string filePath = @"C:\ReconErrorLog\StepLog.txt";

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + step +
                                 "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }

        public static DateTime ConvertGmtToLocal(DateTime transactionDate, TimeSpan transactionTime)
        {
            DateTime newDateTime = transactionDate.Date.Add(transactionTime);
            return newDateTime.ToLocalTime();
        }

        public static bool IsEmpty(this DateTime dateTime)
        {
            return dateTime == default(DateTime);
        }
    }
    public class PropertyCopier<TParent, TChild> where TParent : class
        where TChild : class
    {
        public static void Copy(TParent parent, TChild child)
        {
            var parentProperties = parent.GetType().GetProperties();
            var childProperties = child.GetType().GetProperties();

            foreach (var parentProperty in parentProperties)
            {
                foreach (var childProperty in childProperties)
                {
                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                    {
                        childProperty.SetValue(child, parentProperty.GetValue(parent));
                        break;
                    }
                }
            }
        }
    }
}
