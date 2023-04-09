using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ReconUi.UiHelpers
{
    public static class UIHelper
    {
        public static object GetPropertyValue(object obj, string property)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
        }

        public static T GetEnumValue<T>(int intValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }
            T val = ((T[])Enum.GetValues(typeof(T)))[0];

            foreach (T enumValue in (T[])Enum.GetValues(typeof(T)))
            {
                if (Convert.ToInt32(enumValue).Equals(intValue))
                {
                    val = enumValue;
                    break;
                }
            }
            return val;
        }
        public static DataTable ToDataTable<T>(IList<T> data)// T is any generic type
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
}