using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for Expenses.xaml
    /// </summary>
    public partial class Expenses : UserControl
    {
        public event EventHandler SaveButtonClick;
        public event EventHandler CancelButtonClick;
        Utility utility = new Utility();
        bool IsExpEdit = false;
        Expense exp = null;
        string NextExpNumber = "";
        public Expenses()
        {
            InitializeComponent();
            LoadComboboxes();
            Expense exp = new Expense();
            txtExpNo.Text= NextExpNumber == "" ? utility.GenerateSeq("Expense") : NextExpNumber;
        }

        public Expenses(Expense exp)
        {
            InitializeComponent();
            LoadComboboxes();
            this.exp = exp;
            this.DataContext = exp;
            IsExpEdit = true;
            cbExpFundType.SelectedValue = exp.FundType;
            cbTxnType.SelectedValue = exp.TxnType;
        }

        private void LoadComboboxes()
        {
            FillComboBox<FundTypes>(cbExpFundType, AppGlobalData.lifundTypes, "FundType", "FundType");
            FillComboBox<TransactionType>(cbTxnType, AppGlobalData.liTxnTypes, "TxnType", "TxnType");
        }

        private void FillComboBox<T>(ComboBox cb, List<T> data, string ValueMember, string displayMember)
        {
            cb.ItemsSource = data;
            cb.DisplayMemberPath = displayMember;
            cb.SelectedValuePath = ValueMember;
        }

        private void BtnExpSave_Click(object sender, RoutedEventArgs e)
        {
            Expense exp = this.DataContext as Expense;
            exp.ExpenseNo = txtExpNo.Text;
            SQLiteDatabase db = new SQLiteDatabase();
            Expense eobj = exp;
            eobj.FundType =Convert.ToString(cbExpFundType.SelectedValue);
            eobj.TxnType= Convert.ToString(cbTxnType.SelectedValue);
            if (IsExpEdit)
            {
                eobj.Modified = Convert.ToString(DateTime.Now);
                eobj.ModifiedBy = LoginUser.UName;
                eobj.Created = exp.Created;
                eobj.CreatedBy = exp.CreatedBy;
                Dictionary<string, string> dic1 = Utility.GetTypePropertyValues<Expense>(eobj);
                string s = db.Update("Expenses", dic1, " ID =" + Convert.ToString(exp.ID) + "");
                db.ExecuteNonQuery(s);
            }
            else
            {
                eobj.Created = Convert.ToString(DateTime.Now);
                eobj.CreatedBy = LoginUser.UName;
                Dictionary<string, string> dic1 = Utility.GetTypePropertyValues<Expense>(eobj);
                string s = db.Insert("Expenses", dic1);
                db.ExecuteNonQuery(s);

            }
          
          

            if (this.SaveButtonClick != null)
                this.SaveButtonClick(this, e);
        }

        private void btnExpCancel_Click(object sender, RoutedEventArgs e)
        {
            Expense exp = new Expense();
            this.DataContext = exp;
            if (this.CancelButtonClick != null)
                this.CancelButtonClick(this, e);
        }
    }
}
