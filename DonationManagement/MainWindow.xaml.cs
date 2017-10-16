using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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
        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
            LoadData();
        }

        private void InitializeData()
        {
            TxtDate.Text = DateTime.Now.ToShortDateString();
            MainGrid.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Visible;
        }

        private void LoadData()
        {


            SQLiteDatabase db = new SQLiteDatabase();
            string expenseQuery = "Select * from Donations ORDER BY Created DESC";
            DataTable dtd = db.GetDataTable(expenseQuery);

            List<Donation> lidon = dtd.DataTableToList<Donation>();
            grdDonations.ItemsSource = lidon;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddEditDonation addobj = new AddEditDonation();
            addobj.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                try { SQLiteConnection.CreateFile("Donations.sqlite"); } catch (Exception ex) { }
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
            //  stkAddDonations.Visibility = Visibility.Collapsed;
            splAddDonation.Visibility = Visibility.Visible;
            splDonations.Visibility = Visibility.Collapsed;
            DateTime dt = DateTime.Now;
            txtSLNo.Text = dt.Year + "" + dt.Month + "" + dt.Day + "" + dt.Hour + "" + dt.Minute + "" + dt.Millisecond.ToString().Substring(0,2);
        }

        private void btnSaveDonation_Click(object sender, RoutedEventArgs e)
        {
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
            db = new SQLiteDatabase();
            Donation dobj = new Donation();
            dobj.ReceiptNo = Convert.ToInt64(txtSLNo.Text);
            dobj.Ddate = TxtDate.Text;
            dobj.Name = txtName.Text;
            dobj.Gender = (bool)rbfeMale.IsChecked ? "Female" : "Male";
            dobj.Email = txtEmail.Text;
            dobj.Phone = txtPhone.Text;
            dobj.Place = txtPlace.Text;
            dobj.FundType = txtFundType.Text;
            dobj.Amount = Convert.ToDecimal((!string.IsNullOrWhiteSpace(txtAmount.Text) ? txtAmount.Text : "0"));
            dobj.BTNo = txtBNo.Text;
            dobj.Address = txtAddress.Text;
            dobj.Comment = txtComment.Text;
            dobj.AmountType = txtByType.Text;
            dobj.Place = txtPlace.Text;
            if (IsDonationEdit)
            {
                dobj.Modified = Convert.ToString(DateTime.Now);
                dobj.ModifiedBy = LoginUser.UName;
                dobj.Created = SelectedItem.Created;
                dobj.CreatedBy= SelectedItem.CreatedBy;
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
            }

            splAddDonation.Visibility = Visibility.Collapsed;
            splDonations.Visibility = Visibility.Visible;
            LoadData();
            IsDonationEdit = false;
            FillDoantionForm(new Donation());
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
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnSqlSettings_Click(object sender, RoutedEventArgs e)
        {
            splQueryeditor.Visibility = Visibility.Visible;
            stkAddDonations.Visibility = Visibility.Collapsed;
        }

        private void btnDonationHome_Click(object sender, RoutedEventArgs e)
        {
            splQueryeditor.Visibility = Visibility.Collapsed;
            stkAddDonations.Visibility = Visibility.Visible;
            splAddDonation.Visibility = Visibility.Collapsed;
            splDonations.Visibility = Visibility.Visible;
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
                MainGrid.Visibility = Visibility.Visible;
                LoginGrid.Visibility = Visibility.Collapsed;
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
            TxtDate.Text = dobj.Ddate;
            txtName.Text = dobj.Name;
            if (dobj.Gender == "Female")
            {
                rbfeMale.IsChecked = true;
            }
            else { rbMale.IsChecked = true; };
            txtEmail.Text = dobj.Email;
            txtPhone.Text = dobj.Phone;
            txtPlace.Text = dobj.Place;
            txtFundType.SelectedValue = dobj.FundType;
            txtAmount.Text = Convert.ToString(dobj.Amount);
            txtBNo.Text = dobj.BTNo;
            txtAddress.Text = dobj.Address;
            txtComment.Text = dobj.Comment;
            txtByType.SelectedValue = dobj.AmountType;
            txtPlace.Text = dobj.Place;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnDonationHome_Click(null,null);
            FillDoantionForm(new Donation());
        }
    }
}
