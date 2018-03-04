using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DonationManagement
{
   public class Utility
    {
        SQLiteDatabase db;
        public string GenerateSeq(string tabName)
        {
            string nextnumber;
            db = new SQLiteDatabase();
            string qry = "select * from Sequence where TabName='"+ tabName + "' LIMIT 1";
            DataTable Seq = db.GetDataTable(qry);
            DateTime dt = DateTime.Now;
            string todayseq = dt.Year.ToString().Substring(2,2) + dt.Month.ToString().PadLeft(2,'0') + dt.Day.ToString().PadLeft(2, '0');
            if (Convert.ToString(Seq.Rows[0].Field<Int32>("Sequence")) == todayseq)
            {
                string s = Seq.Rows[0].Field<Int32>("NextNumber").ToString();
                nextnumber = todayseq + s.PadLeft(3, '0');
                UpdateSeqWithNextNumber(s, todayseq);
            }
            else
            {
                string s = "1";
                nextnumber = todayseq + s.PadLeft(3, '0');
                UpdateSeqWithNextNumber(s, todayseq);
            }
            return nextnumber;
        }

        private void UpdateSeqWithNextNumber(string Nextnumber, string Seq)
        {
            db = new SQLiteDatabase();
            string qry = "UPDATE Sequence Set NextNumber=" + (Convert.ToInt32(Nextnumber) + 1) + ", Sequence='" + Seq + "' WHERE TabName='Donations'";
             db.ExecuteNonQuery(qry);
        }


        public static void CreateCSVFromGenericList<T>(List<T> list, string csvNameWithExt)
        {
            if (list == null || list.Count == 0) return;

            //get type from 0th member
            Type t = list[0].GetType();
            string newLine = Environment.NewLine;

            using (var sw = new StreamWriter(csvNameWithExt))
            {
                //make a new instance of the class name we figured out to get its props
                object o = Activator.CreateInstance(t);
                //gets all properties
                PropertyInfo[] props = o.GetType().GetProperties();

                //foreach of the properties in class above, write out properties
                //this is the header row
                foreach (PropertyInfo pi in props)
                {
                    sw.Write(pi.Name.ToUpper() + ",");
                }
                sw.Write(newLine);

                //this acts as datarow
                foreach (T item in list)
                {
                    //this acts as datacolumn
                    foreach (PropertyInfo pi in props)
                    {
                        //this is the row+col intersection (the value)
                        string whatToWrite =
                            Convert.ToString(item.GetType()
                                                 .GetProperty(pi.Name)
                                                 .GetValue(item, null))
                                .Replace(',', ' ') + ',';

                        sw.Write(whatToWrite);

                    }
                    sw.Write(newLine);
                }
            }
        }


        public static Dictionary<string, string> GetTypePropertyValues<T>(T o)
        {
            Dictionary<string, string> obj = new Dictionary<string, string>();
            // ItemDetails o = new ItemDetails();
            PropertyInfo[] p = o.GetType().GetProperties();

            foreach (PropertyInfo item in p)
            {
                object t = new object();
                obj.Add(Convert.ToString(item.Name), Convert.ToString(item.GetValue(o, null)));

            }

            return obj;

        }
    }
}
