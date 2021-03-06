﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DonationManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SQLiteDatabase db;
        bool IsDonationEdit = false;
        Donation SelectedItem;
        Expense SelectedExpenseItem;
        bool IsExpEdit = false;
        Utility utility = new Utility();
        string NextNumber = "";
        string dateFormat = string.Empty;
        bool pageLoaded;

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
            txtSLNo.IsReadOnly = true;
            pageLoaded = true;
            AppSettingsReader rdr = new AppSettingsReader();
            dateFormat = rdr.GetValue("DateFormat", typeof(string)).ToString();
        }

        private void InitializeData()
        {
            TxtDate.Text = DateTime.Now.ToShortDateString();
            MainGrid.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Visible;
            // dpexpDate.Text = DateTime.Now.ToShortDateString();
            dtDonFrom.Text = dpRDFrom.Text = DateTime.Now.AddDays(-10).ToShortDateString();
            dtDonTo.Text = dpRDTo.Text = DateTime.Now.AddDays(1).ToShortDateString();
            dtExpFrom.Text = dpREFrom.Text = DateTime.Now.AddDays(-10).ToShortDateString();
            dtExpTo.Text = dpRETo.Text = DateTime.Now.AddDays(1).ToShortDateString();
            dpRCFrom.Text = DateTime.Now.AddDays(-10).ToShortDateString();
            dpRCTo.Text = DateTime.Now.AddDays(1).ToShortDateString();

        }

        private void InitializeConsts()
        {
            SQLiteDatabase db = new SQLiteDatabase();
            string fundQuery = "Select * from FundType";
            AppGlobalData.lifundTypes = db.GetDataList<FundTypes>(fundQuery);
            FillComboBox<FundTypes>(cbFundType, AppGlobalData.lifundTypes, "FundType", "FundType");
            string TxnQuery = "Select * from TransactionType";
            AppGlobalData.liTxnTypes = db.GetDataList<TransactionType>(TxnQuery);
            FillComboBox<TransactionType>(cbByType, AppGlobalData.liTxnTypes, "TxnType", "TxnType");
        }

        private void LoadData()
        {
            try
            {
                LoadSearchDonData(); ;
                return;
                if (dtDonFrom.Text == "" || dtDonTo.Text == "")
                {
                    return;
                }
                string dtfrom = Convert.ToDateTime(dtDonFrom.Text).ToSqlLiteDatetime();
                string dtto = Convert.ToDateTime(dtDonTo.Text + " 23:59:59").ToSqlLiteDatetime();
                splDonCommands.Visibility = Visibility.Visible;
                SQLiteDatabase db = new SQLiteDatabase();
                // string expenseQuery = "Select * from Donations ORDER BY Created DESC LiMIT 1000";
                // DataTable dtd = db.GetDataTable(expenseQuery);
                string expenseQuery = "Select * from Donations where Created BETWEEN '" + dtfrom + "' AND '" + dtto + "' " + (string.IsNullOrEmpty(txtDonSearch.Text) ? "" : "AND Name Like '%" + txtREName.Text + "%'") + " ORDER BY Created DESC Limit 1000";
                List<Donation> lidon = db.GetDataList<Donation>(expenseQuery); //dtd.DataTableToList<Donation>();
                grdDonations.ItemsSource = lidon;
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter Valid Values", "Error");
            }
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // try { SQLiteConnection.CreateFile("Donations.db"); } catch (Exception ex) { }
                SQLiteDatabase db = new SQLiteDatabase();
                db.ExecuteNonQuery(txtQuery.Text);
                txtOutcome.Text = "Executed Successfully";
            }
            catch (Exception ex)
            {
                txtOutcome.Text = ex.Message;
            }
        }

        private void btnSchema_Click(object sender, RoutedEventArgs e)
        {
            //Donations
            db = new SQLiteDatabase();
            Donation dobj = new Donation();
            Dictionary<string, string> dic = GetTypeProperties<Donation>(dobj);
            string query = db.CreateTable(dic, "Donations");

            //Exenses
            Expense eobj = new Expense();
            Dictionary<string, string> dicexpense = GetTypeProperties<Expense>(eobj);
            string qry = db.CreateTable(dicexpense, "Expenses");
            // txtQuery.Text = query + "; \n " + qry + ";";

            User uobj = new User();
            Dictionary<string, string> udic = GetTypeProperties<User>(uobj);
            string uquery = db.CreateTable(udic, "Users");
            txtQuery.Text = query + "; \n " + qry + ";" + "\n" + uquery;
        }

        private void btnAddDonation_Click(object sender, RoutedEventArgs e)
        {
            splAddDonation.Visibility = Visibility.Visible;
            splDonations.Visibility = Visibility.Collapsed;
            splDonCommands.Visibility = Visibility.Collapsed;
            DateTime dt = DateTime.Now;
            NextNumber = NextNumber == "" ? utility.GenerateSeq("Donations") : NextNumber;//dt.Year + "" + dt.Month + "" + dt.Day + "" + dt.Hour + "" + dt.Minute + "" + dt.Millisecond.ToString().Substring(0, 2);
            txtSLNo.Text = NextNumber;
            SelectedItem = null;
            IsDonationEdit = false;
        }

        private void btnSaveDonation_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "" || txtAmount.Text == "")
            {
                MessageBox.Show("Please enter Name and Amount.");
                return;
            }
            if (txtEmail.Text != "")
            {
                if (!Regex.IsMatch(txtEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
                {
                    MessageBox.Show("Please enter correct Email");
                    return;
                }
            }
            if (txtPhone.Text != "")
            {
                if (!Regex.IsMatch(txtPhone.Text, @"[^0-9]+"))
                {
                    MessageBox.Show("Please enter correct phone number");
                    return;
                }
            }
            /*
            if (cbByType.SelectedItem != null)
            {
                if ((cbByType.SelectedItem as ComboBoxItem).Content.ToString() != "By Cash")
                {
                    MessageBox.Show("Please Enter Neft/ DD / Check Number");
                    return;
                }
            }
            */

            db = new SQLiteDatabase();
            Donation dobj = new Donation();
            dobj.ReceiptNo = txtSLNo.Text;//
            dobj.Ddate = TxtDate.Text;
            dobj.Name = txtName.Text;
            dobj.Gender = (bool)rbfeMale.IsChecked ? "Female" : "Male";
            dobj.Email = txtEmail.Text;
            dobj.Phone = txtPhone.Text;
            dobj.Place = txtPlace.Text;
            dobj.FundType = cbFundType.Text;
            dobj.Amount = Convert.ToDecimal((!string.IsNullOrWhiteSpace(txtAmount.Text) ? txtAmount.Text : "0"));
            dobj.BTNo = txtBNo.Text;
            dobj.Address = txtAddress.Text;
            dobj.Comment = txtComment.Text;
            dobj.AmountType = cbByType.Text;
            dobj.Place = txtPlace.Text;
            if (IsDonationEdit)
            {
                dobj.Modified = DateTime.Now.ToSqlLiteDatetime();
                dobj.ModifiedBy = LoginUser.UName;
                dobj.Created = Convert.ToDateTime(SelectedItem.Created).ToSqlLiteDatetime();
                dobj.CreatedBy = SelectedItem.CreatedBy;
                Dictionary<string, string> dic1 = GetTypePropertyValues<Donation>(dobj);
                string s = db.Update("Donations", dic1, " ReceiptNo =" + Convert.ToString(dobj.ReceiptNo) + "");
                db.ExecuteNonQuery(s);

            }
            else
            {
                dobj.Created = DateTime.Now.ToSqlLiteDatetime();
                dobj.CreatedBy = LoginUser.UName;
                Dictionary<string, string> dic1 = GetTypePropertyValues<Donation>(dobj);
                string s = db.Insert("Donations", dic1);
                db.ExecuteNonQuery(s);
                NextNumber = "";
            }

            splAddDonation.Visibility = Visibility.Collapsed;
            splDonations.Visibility = Visibility.Visible;
            LoadData();
            IsDonationEdit = false;
            FillDoantionForm(new Donation());
            SelectedItem = null;


        }

        #region MyRegion
        public Dictionary<string, string> GetTypeProperties<T>(T o)
        {
            Dictionary<string, string> obj = new Dictionary<string, string>();
            // ItemDetails o = new ItemDetails();
            PropertyInfo[] p = o.GetType().GetProperties();

            foreach (var item in p)
            {
                obj.Add(Convert.ToString(item.Name), item.PropertyType.Name);
            }

            return obj;

        }

        public Dictionary<string, string> GetTypePropertyValues<T>(T o)
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

        #endregion

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            string text = (sender as TextBox).Text + e.Text;
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
            int count = 0;
            foreach (char c in text)
            {
                if (c == '.')
                    count++;
            }
            if (count > 1)
                e.Handled = true;
        }

        private void btnSqlSettings_Click(object sender, RoutedEventArgs e)
        {

            HideStackPanels();
            splQueryeditor.Visibility = Visibility.Visible;
        }

        private void btnDonationHome_Click(object sender, RoutedEventArgs e)
        {
            HideStackPanels();
            Misc.Visibility = Visibility.Collapsed;
            splDonations.Visibility = Visibility.Visible;
            stkAddDonations.Visibility = Visibility.Visible;
            splAddDonation.Visibility = Visibility.Collapsed;
            splDonCommands.Visibility = Visibility.Visible;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            SQLiteDatabase db = new SQLiteDatabase();
            string expenseQuery = "Select * from Users where Uname='" + txtUname.Text + "' and Password='" + txtPassword.Password + "'";
            DataTable dtd = db.GetDataTable(expenseQuery);
            if (dtd.Rows.Count > 0)
            {
                LoginUser.UName = dtd.Rows[0].Field<string>("Uname");
                LoginUser.Role = dtd.Rows[0].Field<string>("Role");
                LoginGrid.Visibility = Visibility.Collapsed;
                HideStackPanels();
                MainGrid.Visibility = Visibility.Visible;
                stkAddDonations.Visibility = Visibility.Visible;
                LoadData();
                btnSqlSettings.IsEnabled = (LoginUser.Role == "Admin" ? true : false);
                txtPassword.Password = "";
                InitializeConsts();
                var Logindet = @"INSERT INTO LoginActivity (LoginName,LoginDate)VALUES ('"+ LoginUser.UName + "', datetime('now'));";
               string ret= db.ExecuteScalar(Logindet);
            }
            else
            {
                MessageBox.Show("Please Enetr Valid User Name Password.");
            }
        }

        private void txtEmail_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void EditDonation_Click(object sender, RoutedEventArgs e)
        {
            if (grdDonations.SelectedItem == null)
            {
                MessageBox.Show("Please select item to edit");
                return;
            }
            splAddDonation.Visibility = Visibility.Visible;
            splDonations.Visibility = Visibility.Collapsed;
            IsDonationEdit = true;
            Donation dobj = grdDonations.SelectedItem as Donation;
            if (dobj != null)
            {
                SelectedItem = dobj;
                FillDoantionForm(SelectedItem);
            }
            splDonCommands.Visibility = Visibility.Collapsed;
        }

        void FillDoantionForm(Donation dobj)
        {
            //Donation dobj = new Donation();
            txtSLNo.Text = Convert.ToString(dobj.ReceiptNo);
            TxtDate.Text = string.IsNullOrEmpty(dobj.Ddate) ? DateTime.Now.ToShortDateString() : dobj.Ddate;
            txtName.Text = dobj.Name;
            if (dobj.Gender == "Female")
            {
                rbfeMale.IsChecked = true;
            }
            else { rbMale.IsChecked = true; };
            txtEmail.Text = dobj.Email;
            txtPhone.Text = dobj.Phone;
            txtPlace.Text = dobj.Place;
            cbFundType.SelectedValue = dobj.FundType;
            txtAmount.Text = Convert.ToString(dobj.Amount);
            txtBNo.Text = dobj.BTNo;
            txtAddress.Text = dobj.Address;
            txtComment.Text = dobj.Comment;
            cbByType.SelectedValue = dobj.AmountType;
            txtPlace.Text = dobj.Place;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnDonationHome_Click(null, null);
            FillDoantionForm(new Donation());
            splAddDonation.Visibility = Visibility.Collapsed;
            splDonations.Visibility = Visibility.Visible;
        }

        private void btnExpenses_Click(object sender, RoutedEventArgs e)
        {
            HideStackPanels();
            SplExpences.Visibility = Visibility.Visible;
            LoadExpenses();
        }



        private void btnReports_Click(object sender, RoutedEventArgs e)
        {

            HideStackPanels();
            SplReport.Visibility = Visibility.Visible;
            btnRDLoad_Click(null, null);
            btnRELoad_Click(null, null);
        }

        void HideStackPanels()
        {
            foreach (StackPanel sp in MainGrid.Children.OfType<StackPanel>())
            {
                sp.Visibility = Visibility.Collapsed;
            }
            Misc.Visibility = Visibility.Collapsed;
            stpMisc.Visibility = Visibility.Collapsed;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Visible;
        }

        private void btnDeleteDonation_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Are you sure you want to delete the Donation +", "Delete Donation", MessageBoxButton.YesNo);

            if (dialogResult == MessageBoxResult.Yes)
            {
                SelectedItem = grdDonations.SelectedItem as Donation;
                if (SelectedItem != null)
                {
                    string sql = "DELETE FROM DONATIONS where ReceiptNo ='" + SelectedItem.ReceiptNo + "'";
                    db = new SQLiteDatabase();
                    db.ExecuteNonQuery(sql);
                    MessageBox.Show("Donation deleted");
                    LoadData();
                }
            }
        }

        private void BtnExpenseSave_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void FillExpenseForm(Expense eobj)
        {
           
        }

        private void btnExpCancel_Click(object sender, RoutedEventArgs e)
        {
            FlowDocument obj = new FlowDocument();
            IsExpEdit = false;
            SelectedExpenseItem = null;
            FillExpenseForm(new Expense());
        }
        WebBrowser wb;
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wb = new WebBrowser();
                string excdir = Directory.GetCurrentDirectory() + @"\Templates";
                string tpath = excdir + @"\PrintTemplate.html";
                string imgPath = excdir + @"\Receipt.png";
                SelectedItem = grdDonations.SelectedItem as Donation;
                if (SelectedItem != null)
                {
                    if (SelectedItem.Amount <= 1000)
                    {
                        tpath = excdir + @"\smallVocher.html";
                        imgPath = excdir + @"\smallVocher.png";
                    }

                    StreamReader sr = new StreamReader(tpath);
                    string html = sr.ReadToEnd();
                    html = html.Replace("@img@", imgPath);

                    html = html.Replace("@ReceiptNo@", Convert.ToString(SelectedItem.ReceiptNo)).Replace("@date@", SelectedItem.Ddate.Substring(0, 8)).Replace("@Name@", SelectedItem.Name)
                        .Replace("@AmountWords@", NumberToWords((int)SelectedItem.Amount) + " only").Replace("@DD@", SelectedItem.BTNo).Replace("@Amount@", Convert.ToString(SelectedItem.Amount) + "/-");
                    wb.NavigateToString(html);
                    sr.Close();
                    wb.Navigated += Wb_Navigated;
                }
                else
                {
                    wb = null;
                    MessageBox.Show("Pleases select item to print", "Print");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error Happened :" + ex.Message, "Print");
            }
        }

        private void btnPrintExp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wb = new WebBrowser();
                string excdir = Directory.GetCurrentDirectory() + @"\Templates";
                string tpath = excdir + @"\VocherTemplate.html";
                string imgPath = excdir + @"\Vocher.png";
                StreamReader sr = new StreamReader(tpath);
                string html = sr.ReadToEnd();
                html = html.Replace("@img@", imgPath);
                Expense exp = grdExpenses.SelectedItem as Expense;
                SQLiteDatabase db = new SQLiteDatabase();
                string expenseQuery = "Select * from EXPENSES e inner join FundType f on e.FundType=f.Fundtype where e.ID=" + exp.ID;
                DataTable dtd = db.GetDataTable(expenseQuery);
                if (exp != null)
                {
                    html = html.Replace("@ReceiptNo@", Convert.ToString(exp.ExpenseNo)).Replace("@date@", exp.ExpDate.Substring(0, 9)).Replace("@Name@", exp.Reason)
                        .Replace("@AmountWords@", NumberToWords((int)exp.AmountPaid) + " only").Replace("@DD@", exp.TxnRefNo).Replace("@Amount@", Convert.ToString(exp.AmountPaid) + "/-");
                    html = html.Replace("@FUNDTYPE@", GetField(dtd.Rows[0], "FundType"));
                    html = html.Replace("@Bank@", GetField(dtd.Rows[0], "BankName"));
                    html = html.Replace("@Dated@", exp.ExpDate.Substring(0, 10));
                    html = html.Replace("@Debit@", GetField(dtd.Rows[0], "AccountNo"));
                    wb.NavigateToString(html);
                    sr.Close();
                    wb.Navigated += Wb_Navigated;
                }
                else
                {
                    wb = null;
                    MessageBox.Show("Pleases select item to print", "Print");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Happened :" + ex.Message, "Print");
            }
        }

        public string GetField(DataRow dr, string columnName)
        {
            return Convert.ToString(dr.Field<string>(columnName));
        }

        private void Wb_Navigated(object sender, NavigationEventArgs e)
        {
            System.Threading.Thread.Sleep(100);
            mshtml.IHTMLDocument2 doc = wb.Document as mshtml.IHTMLDocument2;
            doc.execCommand("Print", true, null);
        }



        public string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 100000) > 0)
            {
                words += NumberToWords(number / 100000) + " lacks ";
                number %= 100000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

        private void GrdExpEditButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedExpenseItem = new Expense();
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    SelectedExpenseItem = row.Item as Expense;
                    FillExpenseForm(SelectedExpenseItem);
                    IsExpEdit = true;
                    break;
                }
        }

        private void GrdExpDeleteButton_Click(object sender, RoutedEventArgs e)
        {

            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    SelectedExpenseItem = row.Item as Expense;
                    MessageBoxResult dialogResult = MessageBox.Show("Are you sure you want to delete the Expense +", "Delete Expense", MessageBoxButton.YesNo);

                    if (dialogResult == MessageBoxResult.Yes)
                    {

                        if (SelectedExpenseItem != null)
                        {
                            string sql = "DELETE FROM EXPENSES where ID ='" + SelectedExpenseItem.ID + "'";
                            db = new SQLiteDatabase();
                            db.ExecuteNonQuery(sql);
                            MessageBox.Show("Expense deleted");
                            SelectedExpenseItem = null;
                            LoadExpenses();
                            FillExpenseForm(new Expense());
                        }
                    }
                    break;
                }
        }

        private void btnRDLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dpRDFrom.Text == "" || dpRDTo.Text == "")
                {
                    MessageBox.Show("Please select date");
                    return;
                }
                string dtfrom = Convert.ToDateTime(dpRDFrom.Text).ToSqlLiteDatetime();//.ToString(dateFormat);
                string dtto = Convert.ToDateTime(dpRDTo.Text + " 23:59:00").ToSqlLiteDatetime();//.ToString(dateFormat);

                db = new SQLiteDatabase();
                string expenseQuery = "Select * from Donations where Created BETWEEN '" + dtfrom + "' AND '" + dtto + "' " + (string.IsNullOrEmpty(txtRDName.Text) ? "" : "AND Name Like '%" + txtRDName.Text + "%'") + " ORDER BY Created DESC";
                // DataTable dtd = db.GetDataTable(expenseQuery);
                List<Donation> lidon = db.GetDataList<Donation>(expenseQuery); //dtd.DataTableToList<Donation>();
                grdRepDonations.ItemsSource = lidon;

                string countqry = "Select Count(*) as Count,SUM(Amount) as Total from Donations where Created BETWEEN '" + dtfrom + "' AND '" + dtto + "' " + (string.IsNullOrEmpty(txtRDName.Text) ? "" : "AND Name Like '%" + txtRDName.Text + "%'");
                DataTable ctd = db.GetDataTable(countqry);
                if (ctd.Rows.Count > 0)
                {
                    lblDrows.Content = Convert.ToString(ctd.Rows[0]["Count"] != DBNull.Value ? ctd.Rows[0].Field<Int64>("Count") : 0);
                    lblDTotal.Content = Convert.ToString(ctd.Rows[0]["Total"] != DBNull.Value ? ctd.Rows[0].Field<Int64>("Total") : 0);
                }
                else
                {
                    lblDrows.Content = lblDTotal.Content = "0";

                }
            }
            catch (Exception ex)
            {


            }
        }

        private void btnRELoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dpREFrom.Text == "" || dpRETo.Text == "")
                {
                    MessageBox.Show("Please select date");
                    return;
                }
                string dtfrom = Convert.ToDateTime(dpREFrom.Text).ToSqlLiteDatetime();
                string dtto = Convert.ToDateTime(dpRETo.Text + " 23:59:59").ToSqlLiteDatetime();
                db = new SQLiteDatabase();
                string expenseQuery = "Select * from EXPENSES where Created BETWEEN '" + dtfrom + "' AND '" + dtto + "' " + (string.IsNullOrEmpty(txtREName.Text) ? "" : "AND VendorName Like '%" + txtREName.Text + "%'") + " ORDER BY Created DESC";
                DataTable dtd = db.GetDataTable(expenseQuery);
                List<Expense> lidon = dtd.DataTableToList<Expense>();
                grdRepExpenses.ItemsSource = lidon;

                string countqry = "Select Count(*) as Count,SUM(Amount) as Total from EXPENSES where Created BETWEEN '" + dtfrom + "' AND '" + dtto + "' " + (string.IsNullOrEmpty(txtREName.Text) ? "" : "AND VendorName Like '%" + txtREName.Text + "%'");
                DataTable ctd = db.GetDataTable(countqry);
                if (ctd.Rows.Count > 0)
                {
                    lblErows.Content = Convert.ToString(ctd.Rows[0]["Count"] != DBNull.Value ? ctd.Rows[0].Field<Int64>("Count") : 0);
                    lblETotal.Content = Convert.ToString(ctd.Rows[0]["Total"] != DBNull.Value ? ctd.Rows[0].Field<Int64>("Total") : 0);
                }
                else
                {
                    lblErows.Content = lblETotal.Content = "0";

                }
            }
            catch (Exception)
            {


            }
        }

        private void txtRDName_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnRDLoad_Click(null, null);
        }

        private void txtREName_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnRELoad_Click(null, null);
        }



        private void BtnChart_Click(object sender, RoutedEventArgs e)
        {
            if (dpRCFrom.Text == "" || dpRCTo.Text == "")
            {
                MessageBox.Show("Please select date");
                return;
            }
            gdChart.Children.Clear();
            string dtfrom = Convert.ToDateTime(dpRCFrom.Text).ToSqlLiteDatetime();
            string dtto = Convert.ToDateTime(dpRCTo.Text + " 23:59:59").ToSqlLiteDatetime();
            db = new SQLiteDatabase();
            string Query = "Select ID, ExpDate as Date, AmountPaid as Amount from EXPENSES where Created BETWEEN '" + dtfrom + "' AND '" + dtto + "' ORDER BY Created DESC";
            DataTable dte = db.GetDataTable(Query);
            List<ChartData> liexp = dte.DataTableToList<ChartData>();
            Query = "Select ReceiptNo, Ddate as Date, Amount from Donations where Created BETWEEN '" + dtfrom + "' AND '" + dtto + "' ORDER BY Created DESC";
            DataTable dtd = db.GetDataTable(Query);
            List<ChartData> lidon = dtd.DataTableToList<ChartData>();

            Chart crt = new Chart();
            crt.Margin = new Thickness(0, 0, 0, 0);
            PieSeries ls = new PieSeries();
            ls.IndependentValuePath = "Date";
            ls.DependentValuePath = "Amount";
            ls.ItemsSource = liexp;
            crt.Series.Add(ls);
            PieSeries lsd = new PieSeries();
            lsd.IndependentValuePath = "Date";
            lsd.DependentValuePath = "Amount";
            lsd.ItemsSource = lidon;
            crt.Series.Add(lsd);
            gdChart.Children.Add(crt);
        }

        private void btnDExpExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (dpRDFrom.Text == "" || dpRDTo.Text == "")
                {
                    MessageBox.Show("Please select date");
                    return;
                }

                string dtfrom = Convert.ToDateTime(dpRDFrom.Text).ToSqlLiteDatetime();
                string dtto = Convert.ToDateTime(dpRDTo.Text + " 23:59:59").ToSqlLiteDatetime();
                db = new SQLiteDatabase();
                string expenseQuery = "Select * from Donations where Created BETWEEN '" + dtfrom + "' AND '" + dtto + "' " + (string.IsNullOrEmpty(txtRDName.Text) ? "" : "AND VendorName Like '%" + txtRDName.Text + "%'") + " ORDER BY Created DESC";
                DataTable dtd = db.GetDataTable(expenseQuery);
                List<Donation> ld = db.GetDataList<Donation>(expenseQuery);
                List<Donation> lidon = dtd.DataTableToList<Donation>();
                SaveFileDialog saveFileDialog = new SaveFileDialog();     
                saveFileDialog.FileName = "Donation_" + (new Random(1000)).Next(0, 1000).ToString() + ".csv";
                saveFileDialog.Filter = "|*.csv";
                saveFileDialog.DefaultExt = ".csv";
                if (saveFileDialog.ShowDialog() == true)
                {
                    Utility.CreateCSVFromGenericList<Donation>(lidon,  saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void btnEExpExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dpREFrom.Text == "" || dpRETo.Text == "")
                {
                    MessageBox.Show("Please select date");
                    return;
                }

                string dtfrom = Convert.ToDateTime(dpREFrom.Text).ToSqlLiteDatetime();
                string dtto = Convert.ToDateTime(dpRETo.Text + " 23:59:59").ToSqlLiteDatetime();
                db = new SQLiteDatabase();
                string expenseQuery = "Select * from EXPENSES where Created BETWEEN '" + dtfrom + "' AND '" + dtto + "' " + (string.IsNullOrEmpty(txtREName.Text) ? "" : "AND VendorName Like '%" + txtREName.Text + "%'") + " ORDER BY Created DESC";
                DataTable dtd = db.GetDataTable(expenseQuery);
                List<Expense> lidon = dtd.DataTableToList<Expense>();
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                 saveFileDialog.FileName = "Expense" + (new Random(1000)).Next(0,1000).ToString() + ".csv";
                saveFileDialog.DefaultExt = ".csv";
                if (saveFileDialog.ShowDialog() == true)
                {
                    Utility.CreateCSVFromGenericList<Expense>(lidon, saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }


        private void cbByType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            return;
            if (pageLoaded)
            {
                ComboBoxItem cb = ((sender as ComboBox).SelectedItem as ComboBoxItem);
                if (cb != null)
                {
                    if (cb.Content.ToString() == "By Cash")
                    {
                        txtBNo.IsEnabled = false;
                        lblNo.IsEnabled = false;
                    }
                    else
                    {
                        txtBNo.IsEnabled = true;
                        lblNo.IsEnabled = true;
                    }
                }
            }
        }

        private void btnClean_Click(object sender, RoutedEventArgs e)
        {
            txtQuery.Text = "VACUUM;";
        }

        private void FillComboBox<T>(ComboBox cb, List<T> data, string ValueMember, string displayMember)
        {
            cb.ItemsSource = data;
            cb.DisplayMemberPath = displayMember;
            cb.SelectedValuePath = ValueMember;
        }
        private void btnAddExp_Click(object sender, RoutedEventArgs e)
        {
            AddExpenses.Children.Clear();
            AddExpenses.Background = new SolidColorBrush(Colors.AliceBlue);
            AddExpenses.Visibility = Visibility.Visible;
            Expenses expnses = new Expenses();
            expnses.SaveButtonClick += Expnses_SaveButtonClick;
            expnses.CancelButtonClick += Expnses_CancelButtonClick;
            AddExpenses.Children.Add(expnses);

        }

        private void Expnses_CancelButtonClick(object sender, EventArgs e)
        {
            AddExpenses.Visibility = Visibility.Collapsed;
            AddExpenses.Children.Clear();
        }

        private void Expnses_SaveButtonClick(object sender, EventArgs e)
        {
            AddExpenses.Visibility = Visibility.Collapsed;
            Expense exp = ((sender as Expenses).DataContext as Expense);
            LoadExpenses();
            AddExpenses.Children.Clear();
        }

        private void EditExp_Click(object sender, RoutedEventArgs e)
        {
            AddExpenses.Background = new SolidColorBrush(Colors.AliceBlue);
            if (grdExpenses.SelectedItem != null)
            {
                AddExpenses.Children.Clear();
                AddExpenses.Visibility = Visibility.Visible;
                Expenses expnses = new Expenses(grdExpenses.SelectedItem as Expense);
                expnses.SaveButtonClick += Expnses_SaveButtonClick;
                expnses.CancelButtonClick += Expnses_CancelButtonClick;
                AddExpenses.Children.Add(expnses);

            }
        }


        private void btnDeleteExp_Click(object sender, RoutedEventArgs e)
        {
            if (grdExpenses.SelectedItem != null)
            {
                SelectedExpenseItem = grdExpenses.SelectedItem as Expense;
                MessageBoxResult dialogResult = MessageBox.Show("Are you sure you want to delete the Expense?", "Delete Expense", MessageBoxButton.YesNo);

                if (dialogResult == MessageBoxResult.Yes)
                {

                    if (SelectedExpenseItem != null)
                    {
                        string sql = "DELETE FROM EXPENSES where ID ='" + SelectedExpenseItem.ID + "'";
                        db = new SQLiteDatabase();
                        db.ExecuteNonQuery(sql);
                        MessageBox.Show("Expense deleted");
                        SelectedExpenseItem = null;
                        LoadExpenses();
                    }
                }
            }
        }

        private void btnPrintExp_Click1(object sender, RoutedEventArgs e)
        {
            string tpath = Directory.GetCurrentDirectory() + @"\ExpenseVocher.html";
            StreamReader sr = new StreamReader(tpath);
            string html = sr.ReadToEnd();
            string FinalHtml = string.Empty;
            string printhtml = @"<tr style='border: 1px solid black'>
                 <td colspan = '4' style = ''><span> @Ttitle </span></td>    
                    <td colspan = '2' ><span> @Debit </span></td>     
                     <td colspan = '2'><span> @Credit </span></td>      
                  </tr>";
            List<string> strFundTypeArr = new List<string>();
            decimal totalamt = 0;
            if (grdExpenses.SelectedItems.Count > 0)
            {
                List<Expense> expitems = new List<Expense>();
                foreach (var item in grdExpenses.SelectedItems)
                {
                    Expense exp = item as Expense;
                    expitems.Add(exp);
                    if (strFundTypeArr.IndexOf(exp.FundType) <= -1)
                        strFundTypeArr.Add(exp.FundType);
                }

                foreach (var Ftype in strFundTypeArr)
                {
                    decimal amount = 0;
                    foreach (Expense item in expitems)
                    {
                        if (Ftype == item.FundType)
                        {
                            FinalHtml += printhtml.Replace("@Credit", Convert.ToString(item.AmountPaid)).Replace("@Ttitle", Convert.ToString(item.Reason)).Replace("@Debit", "");
                            amount += item.AmountPaid;
                        }
                    }
                    totalamt += amount;
                    FinalHtml += printhtml.Replace("@Debit", Convert.ToString(amount)).Replace("@Ttitle", Convert.ToString(Ftype)).Replace("@Credit", ""); ;
                }
                string totalline = printhtml.Replace("@Debit", Convert.ToString(totalamt)).Replace("@Ttitle", "<b>Total Amount:</b>").Replace("@Credit", Convert.ToString(totalamt)); ;
                html = html.Replace(" @LineItems", FinalHtml);
                html = html.Replace(" @TotalLine", totalline);

            }
            html = html.Replace("@Amountinwards", NumberToWords((int)totalamt));
            html = html.Replace("@UserId", LoginUser.UName);


            //string tpath = Directory.GetCurrentDirectory() + @"\ExpenseVocher.html";
            //StreamReader sr = new StreamReader(tpath);
            //string html = sr.ReadToEnd();
            // SelectedItem = //grdDonations.SelectedItem as Donation;
            if (true)
            {
                wb = new WebBrowser();
                //    html = html.Replace("@ReceiptNo@", Convert.ToString(SelectedItem.ReceiptNo)).Replace("@date@", SelectedItem.Ddate.Substring(0, 11)).Replace("@Name@", SelectedItem.Name)
                //        .Replace("@AmountWords@", NumberToWords((int)SelectedItem.Amount) + " only").Replace("@DD@", SelectedItem.BTNo).Replace("@Amount@", Convert.ToString(SelectedItem.Amount) + "/-");
                wb.NavigateToString(html);
                sr.Close();
                wb.Navigated += Wb_Navigated;
            }
            else
            {
                wb = null;
                MessageBox.Show("Pleases select item to print", "Print");
            }



        }

        private void txtPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (Convert.ToInt16(e.Key) == 6)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void btnMisc_Click(object sender, RoutedEventArgs e)
        {
            foreach (var x in MainGrid.Children.OfType<StackPanel>())
            {
                x.Visibility = Visibility.Collapsed;
            }
            Misc.Visibility = Visibility.Visible;
            string sql = "Select * from FundType";
            db = new SQLiteDatabase();
            List<FundTypes> ft = db.GetDataList<FundTypes>(sql);
            grdFundtype.ItemsSource = ft;
            stpMisc.Children.Clear();
            stpMisc.Visibility = Visibility.Collapsed;
        }

        private void btnAddFund_Click(object sender, RoutedEventArgs e)
        {
            stpMisc.Visibility = Visibility.Visible;
            FundTypesUC fuc = new FundTypesUC();
            stpMisc.Children.Add(fuc);
            Misc.Visibility = Visibility.Collapsed;
            fuc.SaveButtonClick += Fuc_SaveButtonClick;
            fuc.CancelButtonClick += Fuc_CancelButtonClick;
        }

        private void btnEditFund_Click(object sender, RoutedEventArgs e)
        {
            FundTypes ft = grdFundtype.SelectedItem as FundTypes;
            if (ft != null)
            {
                stpMisc.Visibility = Visibility.Visible;
                FundTypesUC fuc = new FundTypesUC(ft);
                stpMisc.Children.Add(fuc);
                Misc.Visibility = Visibility.Collapsed;
                fuc.SaveButtonClick += Fuc_SaveButtonClick;
                fuc.CancelButtonClick += Fuc_CancelButtonClick;
            }
            else
            {
                MessageBox.Show("Please select Fund type to Edit");
            }
        }

        private void Fuc_CancelButtonClick(object sender, EventArgs e)
        {
            btnMisc_Click(null, null);
        }

        private void Fuc_SaveButtonClick(object sender, EventArgs e)
        {
            btnMisc_Click(null, null);
        }

        private void btnDeleteFund_Click(object sender, RoutedEventArgs e)
        {
            FundTypes ft = grdFundtype.SelectedItem as FundTypes;
            if (ft != null)
            {
                MessageBoxResult dialogResult = MessageBox.Show("Are you sure you want to delete the Fund Type?", "Delete Fund Type", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {


                    string sql = "DELETE FROM FundType where ID ='" + ft.Id + "'";
                    db = new SQLiteDatabase();
                    db.ExecuteNonQuery(sql);
                    MessageBox.Show("FundType deleted");

                    btnMisc_Click(null, null);
                }
            }
            else
            {
                MessageBox.Show("Please select Fund type to delete");
            }
        }

        private void btnDonReset_Click(object sender, RoutedEventArgs e)
        {
            txtDonSearch.Text = "";
        }

        private void btnExpReset_Click(object sender, RoutedEventArgs e)
        {
            txtExpSearch.Text = "";
            LoadExpenses();
        }

        private void txtExpSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadExpenses();
        }

        private void txtDonSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

            LoadSearchDonData();
        }

        private void btnDonSearch_Click(object sender, RoutedEventArgs e)
        {
            txtDonSearch.Text = "";
            LoadSearchDonData();
        }

        private void LoadSearchDonData()
        {
            try
            {
                if (dtDonFrom.Text == "" || dtDonTo.Text == "")
                {
                    return;
                }
                string dtfrom = Convert.ToDateTime(dtDonFrom.Text).ToSqlLiteDatetime();
                string dtto = Convert.ToDateTime(dtDonTo.Text + " 23:59:59").ToSqlLiteDatetime();
                SQLiteDatabase db = new SQLiteDatabase();
                string expenseQuery = "Select * from Donations where Created BETWEEN '" + dtfrom + "' AND '" + dtto + "' " + (string.IsNullOrEmpty(txtDonSearch.Text) ? "" : "AND Name Like '%" + txtDonSearch.Text + "%'") + " ORDER BY Created DESC";
                splDonCommands.Visibility = Visibility.Visible;
                List<Donation> lidon = db.GetDataList<Donation>(expenseQuery);
                grdDonations.ItemsSource = lidon;
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter Valid Values", "Error");
            }
        }
        private void LoadExpenses()
        {
            try
            {
                if (dtExpFrom.Text == "" || dtExpTo.Text == "")
                {
                    MessageBox.Show("Please select date");
                    return;
                }
                string dtfrom = Convert.ToDateTime(dtExpFrom.Text).ToSqlLiteDatetime();
                string dtto = Convert.ToDateTime(dtExpTo.Text + " 23:59:59").ToSqlLiteDatetime();
                SQLiteDatabase db = new SQLiteDatabase();
                string expenseQuery = "Select * from EXPENSES where Created BETWEEN '" + dtfrom + "' AND '" + dtto + "' " + (string.IsNullOrEmpty(txtExpSearch.Text) ? "" : "AND VendorName Like '%" + txtExpSearch.Text + "%'") + " ORDER BY Created DESC";
                DataTable dtd = db.GetDataTable(expenseQuery);
                List<Expense> liexp = dtd.DataTableToList<Expense>();
                grdExpenses.ItemsSource = liexp;

                //    SQLiteDatabase db = new SQLiteDatabase();
                //string expenseQuery = "Select * from Expenses ORDER BY Created DESC";
                //DataTable dtd = db.GetDataTable(expenseQuery);
                //List<Expense> liexp = dtd.DataTableToList<Expense>();
                //grdExpenses.ItemsSource = liexp;
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter Valid Values", "Error");
            }
        }

        private void LoadSearchExpData()
        {

        }

        private void btnExpSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadExpenses();
        }

    }
}
