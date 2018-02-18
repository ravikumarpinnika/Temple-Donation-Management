using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DonationManagement
{
    public class Donation
    {
        public string ReceiptNo { get; set; }
        public string Ddate { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Place { get; set; }
        public string FundType { get; set; }
        public string PaymentType { get; set; }
        public string BTNo { get; set; }
        public Decimal Amount { get; set; }
        public string AmountType { get; set; }
        public string Comment { get; set; }
        public string Address { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class Expense1
    {
        public int ID { get; set; }
        public int ExpenseNo { get; set; }
        public String Edate { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class User
    {
        public int ID { get; set; }
        public string UName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class LoginUser
    {
        public static int ID { get; set; }
        public static string UName { get; set; }
        public static string Role { get; set; }
    }
    public class ChartData
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int Amount { get; set; }
    }

   public class Expense
    {

        public int ID { get; set; }
        public string ExpenseNo { get; set; }
        public string VendorName { get; set; }
        public string ExpDate { get; set; }
        public decimal AmountPaid { get; set; }
        public string Reason { get; set; }
        public string VocherNo { get; set; }
        public string FundType { get; set; }
        //public FundTypes FType { get; set; }
        public string TxnType { get; set; }
        public string TxnRefNo { get; set; }
        public string VendorBillNo { get; set; }
        public string VendorBillDate { get; set; }
        public string Comment { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool isSelected { get; set; }

    }

    public class TransactionType
    {
        public int Id { get; set; }
        public string TxnType { get; set; }
        public string Comments { get; set; }

    }

    public class FundTypes
    {
        public int Id { get; set; }
        public string FundType { get; set; }
        public string AccountNo { get; set; }
        public string Comments { get; set; }
        public bool IsDefault { get; set; }

    }

   public static class AppGlobalData
    {
        public static List<FundTypes> lifundTypes { get; set; }
        public static List<TransactionType> liTxnTypes { get; set; }
    }
}
