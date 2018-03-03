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
    /// Interaction logic for FundTypes.xaml
    /// </summary>
    public partial class FundTypesUC : UserControl
    {
        public event EventHandler SaveButtonClick;
        public event EventHandler CancelButtonClick;
        bool isEdit = false;
        FundTypes ftl = null;
        public FundTypesUC()
        {
            InitializeComponent();
        }
        public FundTypesUC(FundTypes ft)
        {
            InitializeComponent();
            this.DataContext = ft;
            ftl = ft;
            isEdit = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            FundTypes ft = this.DataContext as FundTypes;         
            SQLiteDatabase db = new SQLiteDatabase();
            if (isEdit)
            {
                ft.Id = ftl.Id;
                Dictionary<string, string> dic1 = Utility.GetTypePropertyValues<FundTypes>(ft);

                string s = db.Update("FundType", dic1, " ID =" + Convert.ToString(ft.Id) + "");
                db.ExecuteNonQuery(s);
            }
            else
            {
                
                Dictionary<string, string> dic1 = Utility.GetTypePropertyValues<FundTypes>(ft);
                dic1.Remove("Id");
                string s = db.Insert("FundType", dic1);
                db.ExecuteNonQuery(s);
            }
            if (this.SaveButtonClick != null)
                this.SaveButtonClick(this, e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            FundTypes exp = new FundTypes();
            this.DataContext = exp;
            if (this.CancelButtonClick != null)
                this.CancelButtonClick(this, e);
        }
    }
}
