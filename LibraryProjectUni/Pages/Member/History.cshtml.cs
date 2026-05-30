using BusinessLogicLayer;
using BusnissLogicLayer;
using LibraryProjectUni.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data;

namespace LibraryProjectUni.Pages.Member
{
    public class HistoryModel : ClsBaseController
    {
        public DataTable History { get; set; }
        public string Error { get; set; }
        public string Success { get; set; }
        public decimal TotalSpent { get; set; }
        public int BooksOwned { get; set; }
        public decimal TotalRefunded { get; set; }

        public IActionResult OnGet()
        {
            var check = RequireMember();
            if (check != null) return check;

            LoadData();
            return Page();
        }

        private void LoadData()
        {
            History = ClsBorrow.GetUserBorrowHistory(SessionUserId);

            TotalSpent = 0;
            BooksOwned = 0;
            TotalRefunded = 0;

            foreach (DataRow row in History.Rows)
            {
                if (row["TotalPaid"] != DBNull.Value && row["TotalPaid"] != null)
                {
                    TotalSpent += Convert.ToDecimal(row["TotalPaid"]);
                }

                if (row["IsPurchased"] != DBNull.Value && Convert.ToBoolean(row["IsPurchased"]))
                {
                    BooksOwned++;
                }

                if (row["RefundedAmount"] != DBNull.Value && row["RefundedAmount"] != null)
                {
                    TotalRefunded += Convert.ToDecimal(row["RefundedAmount"]);
                }
            }
        }
    }
}