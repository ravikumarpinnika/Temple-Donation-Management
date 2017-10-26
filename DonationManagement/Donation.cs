using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DonationManagement
{
    public class Donation
    {
        public long ReceiptNo { get; set; }
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

    public class Expense
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
        public  int ID { get; set; }
        public  DateTime Date  { get; set; }
        public  int Amount { get; set; }
    }
}
