using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        bool pageLoaded;
        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
            txtSLNo.IsReadOnly = true;
            pageLoaded = true;
        }

        private void InitializeData()
        {
            TxtDate.Text = DateTime.Now.ToShortDateString();
            MainGrid.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Visible;
            dpexpDate.Text = DateTime.Now.ToShortDateString();
            dpRDFrom.Text = DateTime.Now.AddDays(-10).ToShortDateString();
            dpRDTo.Text = DateTime.Now.AddDays(1).ToShortDateString();
            dpREFrom.Text = DateTime.Now.AddDays(-10).ToShortDateString();
            dpRETo.Text = DateTime.Now.AddDays(1).ToShortDateString();
            dpRCFrom.Text = DateTime.Now.AddDays(-10).ToShortDateString();
            dpRCTo.Text = DateTime.Now.AddDays(1).ToShortDateString();
        }

        private void LoadData()
        {
            SQLiteDatabase db = new SQLiteDatabase();
            string expenseQuery = "Select * from Donations ORDER BY Created DESC LIMIT 10";
            // DataTable dtd = db.GetDataTable(expenseQuery);

            List<Donation> lidon = db.GetDataList<Donation>(expenseQuery); //dtd.DataTableToList<Donation>();
            grdDonations.ItemsSource = lidon;
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
            DateTime dt = DateTime.Now;
            NextNumber = NextNumber == "" ? utility.GenerateSeq() : NextNumber;//dt.Year + "" + dt.Month + "" + dt.Day + "" + dt.Hour + "" + dt.Minute + "" + dt.Millisecond.ToString().Substring(0, 2);
            txtSLNo.Text = NextNumber;
            SelectedItem = null;
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
            if (cbByType.SelectedItem != null)
            {
                if ((cbByType.SelectedItem as ComboBoxItem).Content.ToString() != "By Cash")
                {
                    MessageBox.Show("Please Enter Neft/ DD / Check Number");
                    return;
                }
            }


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
                dobj.Modified = Convert.ToString(DateTime.Now);
                dobj.ModifiedBy = LoginUser.UName;
                dobj.Created = SelectedItem.Created;
                dobj.CreatedBy = SelectedItem.CreatedBy;
                Dictionary<string, string> dic1 = GetTypePropertyValues<Donation>(dobj);
                string s = db.Update("Donations", dic1, " ReceiptNo =" + Convert.ToString(dobj.ReceiptNo) + "");
                db.ExecuteNonQuery(s);

            }
            else
            {
                dobj.Created = Convert.ToString(DateTime.Now);
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
            splDonations.Visibility = Visibility.Visible;
            stkAddDonations.Visibility = Visibility.Visible;
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

        private void LoadExpenses()
        {
            SQLiteDatabase db = new SQLiteDatabase();
            string expenseQuery = "Select * from Expenses ORDER BY Created DESC";
            DataTable dtd = db.GetDataTable(expenseQuery);
            List<Expense> liexp = dtd.DataTableToList<Expense>();
            if (liexp.Count > 0)
                grdExpenses.ItemsSource = liexp;
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {

            HideStackPanels();
            SplReport.Visibility = Visibility.Visible;
            //SQLiteDatabase db = new SQLiteDatabase();
            //string expenseQuery = "Select * from Donations where Created BETWEEN '" + dpRDFrom.Text + " 00:00:00 AM' AND '" + dpRDTo.Text + " 11:59:00 PM' ORDER BY Created DESC";
            //DataTable dtd = db.GetDataTable(expenseQuery);
            //List<Donation> lidon = dtd.DataTableToList<Donation>();
            //grdRepDonations.ItemsSource = lidon;
            btnRDLoad_Click(null, null);
            btnRELoad_Click(null, null);
        }

        void HideStackPanels()
        {
            foreach (StackPanel sp in MainGrid.Children.OfType<StackPanel>())
            {
                sp.Visibility = Visibility.Collapsed;
            }
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
            DateTime sd;
            if (txtExpName.Text == "" || txtExpAmount.Text == "" || txtEReason.Text == "" || dpexpDate.Text == "")
            {
                MessageBox.Show("Please Enter Name,Date, Amount, Reason ");
                return;
            }
            else if (!DateTime.TryParse(dpexpDate.Text, out sd))
            {
                MessageBox.Show("Please Enter Correct Date (dd/mm/yyyy)");
                return;
            }

            db = new SQLiteDatabase();
            Expense eobj = new Expense();
            eobj.Amount = Convert.ToDouble(txtExpAmount.Text);
            eobj.Name = txtExpName.Text;
            eobj.Edate = dpexpDate.Text;
            eobj.Reason = txtEReason.Text;
            eobj.Comment = txtEComment.Text;
            if (IsExpEdit)
            {
                eobj.Modified = Convert.ToString(DateTime.Now);
                eobj.ModifiedBy = LoginUser.UName;
                eobj.Created = SelectedExpenseItem.Created;
                eobj.CreatedBy = SelectedExpenseItem.CreatedBy;
                Dictionary<string, string> dic1 = GetTypePropertyValues<Expense>(eobj);
                string s = db.Update("Expenses", dic1, " ID =" + Convert.ToString(SelectedExpenseItem.ID) + "");
                db.ExecuteNonQuery(s);
            }
            else
            {
                eobj.Created = Convert.ToString(DateTime.Now);
                eobj.CreatedBy = LoginUser.UName;
                Dictionary<string, string> dic1 = GetTypePropertyValues<Expense>(eobj);
                string s = db.Insert("Expenses", dic1);
                db.ExecuteNonQuery(s);

            }
            SelectedExpenseItem = null;
            IsExpEdit = false;
            LoadExpenses();
            FillExpenseForm(new Expense());
        }

        private void FillExpenseForm(Expense eobj)
        {
            txtExpAmount.Text = Convert.ToString(eobj.Amount);
            txtExpName.Text = eobj.Name;
            dpexpDate.Text = string.IsNullOrEmpty(eobj.Edate) ? DateTime.Now.ToShortDateString() : eobj.Edate;
            txtEReason.Text = eobj.Reason;
            txtEComment.Text = eobj.Comment;
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
            // String html = "<html><title></Title><head>< style >body { height: 842px;  width: 595px; margin - left: auto; margin - right: auto; } </ style ></head><body></body></html>";
            wb = new WebBrowser();
            string tpath = Directory.GetCurrentDirectory() + @"\PrintTemplate.html";
            StreamReader sr = new StreamReader(tpath);
            string html = sr.ReadToEnd();
            SelectedItem = grdDonations.SelectedItem as Donation;
            if (SelectedItem != null)
            {
                html = html.Replace("@ReceiptNo@", Convert.ToString(SelectedItem.ReceiptNo)).Replace("@date@", SelectedItem.Ddate.Substring(0, 11)).Replace("@Name@", SelectedItem.Name)
                    .Replace("@AmountWords@", NumberToWords((int)SelectedItem.Amount) + " only").Replace("@DD@", SelectedItem.BTNo).Replace("@Amount@", Convert.ToString(SelectedItem.Amount) + "/-");
            }
            wb.NavigateToString(html);
            sr.Close();
            wb.Navigated += Wb_Navigated;

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
                words += NumberToWords(number / 100000) + " million ";
                number %= 100000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " thousand ";
                number %= 100;
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
            if (dpRDFrom.Text == "" || dpRDTo.Text == "")
            {
                MessageBox.Show("Please select date");
                return;
            }

            db = new SQLiteDatabase();
            string expenseQuery = "Select * from Donations where Created BETWEEN '" + dpRDFrom.Text + " 00:00:00 AM' AND '" + dpRDTo.Text + " 11:59:00 PM' " + (string.IsNullOrEmpty(txtRDName.Text) ? "" : "AND Name Like '%" + txtRDName.Text + "%'") + " ORDER BY Created DESC";
            // DataTable dtd = db.GetDataTable(expenseQuery);
            List<Donation> lidon = db.GetDataList<Donation>(expenseQuery); //dtd.DataTableToList<Donation>();
            grdRepDonations.ItemsSource = lidon;

            string countqry = "Select Count(*) as Count,SUM(Amount) as Total from Donations where Created BETWEEN '" + dpRDFrom.Text + " 00:00:00 AM' AND '" + dpRDTo.Text + " 11:59:00 PM' " + (string.IsNullOrEmpty(txtRDName.Text) ? "" : "AND Name Like '%" + txtRDName.Text + "%'");
            DataTable ctd = db.GetDataTable(countqry);
            if (ctd.Rows.Count > 0)
            {
                lblDrows.Content = ctd.Rows[0].Field<Int64>("Count").ToString();
                lblDTotal.Content = ctd.Rows[0].Field<Int64>("Total").ToString();
            }
            else
            {
                lblDrows.Content = lblDTotal.Content = "0";

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
                db = new SQLiteDatabase();
                string expenseQuery = "Select * from EXPENSES where Created BETWEEN '" + dpREFrom.Text + " 00:00:00 AM' AND '" + dpRETo.Text + " 11:59:00 PM' " + (string.IsNullOrEmpty(txtREName.Text) ? "" : "AND Name Like '%" + txtREName.Text + "%'") + " ORDER BY Created DESC";
                DataTable dtd = db.GetDataTable(expenseQuery);
                List<Expense> lidon = dtd.DataTableToList<Expense>();
                grdRepExpenses.ItemsSource = lidon;

                string countqry = "Select Count(*) as Count,SUM(Amount) as Total from EXPENSES where Created BETWEEN '" + dpREFrom.Text + " 00:00:00 AM' AND '" + dpRETo.Text + " 11:59:00 PM' " + (string.IsNullOrEmpty(txtREName.Text) ? "" : "AND Name Like '%" + txtREName.Text + "%'");
                DataTable ctd = db.GetDataTable(countqry);
                if (ctd.Rows.Count > 0)
                {
                    lblErows.Content = ctd.Rows[0].Field<Int64>("Count");
                    lblETotal.Content = ctd.Rows[0].Field<Double>("Total");
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
            db = new SQLiteDatabase();
            string Query = "Select ID, Edate as Date, Amount from EXPENSES where Created BETWEEN '" + dpRCFrom.Text + " 00:00:00 AM' AND '" + dpRCTo.Text + " 11:59:00 PM' ORDER BY Created DESC LIMIT 100";
            DataTable dtd = db.GetDataTable(Query);
            List<ChartData> liexp = dtd.DataTableToList<ChartData>();
            Query = "Select ReceiptNo, Ddate as Date, Amount from Donations where Created BETWEEN '" + dpRCFrom.Text + " 00:00:00 AM' AND '" + dpRCTo.Text + " 11:59:00 PM' ORDER BY Created DESC LIMIT 100";
            DataTable dte = db.GetDataTable(Query);
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
                db = new SQLiteDatabase();

                string expenseQuery = "Select * from Donations where Created BETWEEN '" + dpRDFrom.Text + " 00:00:00 AM' AND '" + dpRDTo.Text + " 11:59:00 PM' " + (string.IsNullOrEmpty(txtRDName.Text) ? "" : "AND Name Like '%" + txtRDName.Text + "%'") + " ORDER BY Created DESC LIMIT 100";
                DataTable dtd = db.GetDataTable(expenseQuery);

                List<Donation> ld = db.GetDataList<Donation>(expenseQuery);


                List<Donation> lidon = dtd.DataTableToList<Donation>();
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                // saveFileDialog.FileName= "Donation" +(new Random(1000)).Next(0, 1000).ToString() + ".csv";
                //saveFileDialog.Filter ="|*.csv";
                saveFileDialog.DefaultExt = ".csv";
                if (saveFileDialog.ShowDialog() == true)
                {
                    Utility.CreateCSVFromGenericList<Donation>(lidon, saveFileDialog.FileName);
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
                db = new SQLiteDatabase();

                string expenseQuery = "Select * from EXPENSES where Created BETWEEN '" + dpREFrom.Text + " 00:00:00 AM' AND '" + dpRETo.Text + " 11:59:00 PM' " + (string.IsNullOrEmpty(txtREName.Text) ? "" : "AND Name Like '%" + txtREName.Text + "%'") + " ORDER BY Created DESC  LIMIT 100";

                DataTable dtd = db.GetDataTable(expenseQuery);
                List<Expense> lidon = dtd.DataTableToList<Expense>();
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                // saveFileDialog.FileName = "Expense" + (new Random(1000)).Next(0,1000).ToString() + ".csv";
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
    }
}
