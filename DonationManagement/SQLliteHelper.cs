using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace DonationManagement
{
    public class SQLiteDatabase
    {
        String dbConnection;


        ///
        ///     Default Constructor for SQLiteDatabase Class.
        ///
        public SQLiteDatabase()
        {
            dbConnection = @"Data Source=Donations.db;Version=3;new=False;datetimeformat=CurrentCulture;";
        }

        ///
        ///     Single Param Constructor for specifying the DB file.
        ///
        /// The File containing the DB
        public SQLiteDatabase(String inputFile)
        {
            dbConnection = String.Format("Data Source={0};Version=3;", inputFile);
        }

        ///
        ///     Single Param Constructor for specifying advanced connection options.
        ///
        /// A dictionary containing all desired options and their values
        public SQLiteDatabase(Dictionary<string, string> connectionOpts)
        {
            String str = "";
            foreach (KeyValuePair<string, string> row in connectionOpts)
            {
                str += String.Format("{0}={1}; ", row.Key, row.Value);
            }
            str = str.Trim().Substring(0, str.Length - 1);
            dbConnection = str;
        }

        ///
        ///     Allows the programmer to run a query against the Database.
        ///
        /// The SQL to run
        /// A DataTable containing the result set.
        public DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection(dbConnection))
                {

                    cnn.Open();
                    using (SQLiteCommand mycommand = new SQLiteCommand(sql, cnn))
                    {
                        using (SQLiteDataReader reader = mycommand.ExecuteReader())
                        {
                            dt.Load(reader);
                            reader.Close();
                        }
                        cnn.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return dt;
        }
        ///
        ///     Allows the programmer to interact with the database for purposes other than a query.
        ///
        /// The SQL to be run.
        /// An Integer containing the number of rows updated.
        public int ExecuteNonQuery(string sql)
        {
            int rowsUpdated;
            using (SQLiteConnection cnn = new SQLiteConnection(dbConnection))
            {
                cnn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, cnn))
                {
                    rowsUpdated = cmd.ExecuteNonQuery();
                }
                cnn.Close();
            }
            return rowsUpdated;
        }
        ///
        ///     Allows the programmer to retrieve single items from the DB.
        ///
        /// The query to run.
        /// A string.
        public string ExecuteScalar(string sql)
        {
            object value = null;
            using (SQLiteConnection cnn = new SQLiteConnection(dbConnection))
            {
                cnn.Open();
                using (SQLiteCommand mycommand = new SQLiteCommand(sql, cnn))
                {
                    value = mycommand.ExecuteScalar();
                }
                cnn.Close();
            }
            if (value != null)
            {
                return value.ToString();
            }
            return "";
        }

        ///
        ///     Allows the programmer to easily update rows in the DB.
        ///
        /// The table to update.
        /// A dictionary containing Column names and their new values.
        /// The where clause for the update statement.
        /// A boolean true or false to signify success or failure.
        public string Update(String tableName, Dictionary<string, string> data, String where)
        {
            String vals = "";
            Boolean returnCode = true;
            if (data.Count >= 1)
            {
                int i = 0;
                foreach (KeyValuePair<string, string> val in data)
                {
                    if (i != 0)
                    {
                        if (val.Value != "")
                        {
                            vals += String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());

                        }
                    }
                    i++;
                }

                vals = vals.Substring(0, vals.Length - 1);

            }
            try
            {
                // this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
            }
            catch
            {
                returnCode = false;
            }
            return String.Format("update {0} set {1} WHERE {2};", tableName, vals, where);
        }

        ///
        ///     Allows the programmer to easily delete rows from the DB.
        ///
        /// The table from which to delete.
        /// The where clause for the delete.
        /// A boolean true or false to signify success or failure.
        public bool Delete(String tableName, String where)
        {
            Boolean returnCode = true;
            try
            {
                this.ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        public string InsertOrUpdate(String tableName, Dictionary<string, string> data)
        {
            // this.Insert(tableName, data);
            string str = null;
            string Query = "select * from " + tableName + " where OppNo='" + data["OppNo"] + "' and RotaLine_NO='" + data["RotaLine_NO"] + "'";
            string val = this.ExecuteScalar(Query);
            if (val == "")
            {
                str = this.Insert(tableName, data);
            }
            else
            {
                str = this.UpdateRotationPlan(tableName, data);
            }
            return str;
        }

        ///
        ///     Allows the programmer to easily insert into the DB
        ///
        /// The table into which we insert the data.
        /// A dictionary containing the column names and data for the insert.
        /// A boolean true or false to signify success or failure.
        public string Insert(String tableName, Dictionary<string, string> data)
        {
            String columns = "";
            String values = "";
            Boolean returnCode = true;
            foreach (KeyValuePair<string, string> val in data)
            {
                if (val.Key != "ID")
                {
                    if (val.Value != "")
                    {
                        columns += String.Format(" {0},", val.Key.ToString());
                        values += String.Format(" '{0}',", val.Value);
                    }
                }
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            return String.Format("insert into {0}({1}) values({2});", tableName, columns, values);
        }

        public string UpdateRotationPlan(String tableName, Dictionary<string, string> data)
        {
            String query = "";
            foreach (KeyValuePair<string, string> val in data)
            {
                query += String.Format(" {0} ='{1}',", val.Key.ToString(), val.Value);
            }
            query = query.Substring(0, query.Length - 1);
            return String.Format("Update {0} set {1} where OppNo='" + data["OppNo"] + "' and RotaLine_NO='" + data["RotaLine_NO"] + "'", tableName, query, query);
        }

        public string UpdateManagers(String tableName, Dictionary<string, string> data)
        {
            String query = "";
            foreach (KeyValuePair<string, string> val in data)
            {
                query += String.Format(" {0} ='{1}',", val.Key.ToString(), val.Value);
            }
            query = query.Substring(0, query.Length - 1);
            return String.Format("Update {0} set {1} where OppNo='" + data["OppNo"] + "'", tableName, query, query);
        }

        ///
        ///     Allows the programmer to easily delete all data from the DB.
        ///
        /// A boolean true or false to signify success or failure.
        public bool ClearDB()
        {
            DataTable tables;
            try
            {
                tables = this.GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");
                foreach (DataRow table in tables.Rows)
                {
                    this.ClearTable(table["NAME"].ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        ///
        ///     Allows the user to easily clear all data from a specific table.
        ///
        /// The name of the table to clear.
        /// A boolean true or false to signify success or failure.
        public bool ClearTable(String table)
        {
            try
            {
                this.ExecuteNonQuery(String.Format("delete from {0};", table));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string CreateTable(Dictionary<string, string> prop, string tableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("create table IF NOT EXISTS " + tableName + "(");
            int i = 0;
            foreach (var item in prop)
            {
                sb.Append(item.Key + " " + GetColumnType(item.Value, i == 0 ? true : false) + ",");
                i++;
            }
            sb.Append(")");
            sb.Replace(",)", ")");
            return sb.ToString();
        }

        private string GetColumnType(string Type, bool isPk = false)
        {
            string str = "";
            switch (Type)
            {
                case "String":
                    str = "Varchar(200)";
                    break;
                case "object":
                    str = "Varchar(1000)";
                    break;
                case "Int32":
                    str = "INTEGER";
                    break;
                case "Decimal":
                    str = "DECIMAL(10,10)";
                    break;
                case "DateTime":
                    str = "Varchar(100)";
                    break;
                default:
                    str = "Varchar(200)";
                    break;
            }
            return isPk ? str + " PRIMARY KEY " : str;
        }

        public string CreateSelect(Dictionary<string, string> prop, string tableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select ");
            foreach (var item in prop)
            {
                if (item.Key.Contains("CW"))
                {
                    sb.Append(" SUM(CAST(" + item.Key + " as decimal)) as " + item.Key + ",");
                }
                else
                {
                    sb.Append(item.Key + ",");
                }

            }
            // sb.Append(")");
            sb.Remove(sb.Length - 1, 1);
            sb.Append(" from  ResourcePlan group by ResourceName");
            return sb.ToString();
        }
    }

    public static class Helper
    {
        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    //PropertyInfo[] p = obj.GetType().GetProperties();
                    //Parallel.ForEach<PropertyInfo>(p, (prop) =>
                    //{
                    //    try
                    //    {
                    //        PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                    //        propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                    //    }
                    //    catch
                    //    {
                    //        // continue;
                    //    }
                    //});

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }


        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }


        public static List<T> DtToList<T>(this DataTable dt)
        {
            List<T> data = new List<T>();
            Type temp = typeof(T);
            PropertyInfo[] p = temp.GetProperties();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row, p);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr, PropertyInfo[] p)
        {

            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in p)
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, Convert.ChangeType(dr[column.ColumnName], pro.PropertyType), null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}
