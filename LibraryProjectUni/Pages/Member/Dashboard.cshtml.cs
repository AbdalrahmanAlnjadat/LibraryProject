using BusinessLogicLayer;
using BusnissLogicLayer;
using LibraryProjectUni.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data;

namespace LibraryProjectUni.Pages.Member
{
    public class DashboardModel : ClsBaseController
    {
        public int ActiveBorrows { get; set; }
        public int BooksAvailable { get; set; }
        public int TotalBorrowed { get; set; }
        public decimal TotalFines { get; set; }
        public decimal TotalSpent { get; set; }
        public int BooksOwned { get; set; }
        public decimal TotalRefunded { get; set; }
        public DataTable MyBorrows { get; set; } = new();
        public DataTable ActiveBorrowsList { get; set; } = new();

        public IActionResult OnGet()
        {
            var check = RequireMember();
            if (check != null) return check;

            LoadData();
            return Page();
        }

        private void LoadData()
        {
            MyBorrows = ClsBorrow.GetUserBorrowHistory(SessionUserId);
            DataTable available = ClsBook.GetAvailableBooks();

            BooksAvailable = available.Rows.Count;
            TotalBorrowed = MyBorrows.Rows.Count;

            // Filter: Active borrows only (NOT purchased, NOT returned)
            ActiveBorrowsList = new DataTable();
            TotalFines = 0;
            TotalSpent = 0;
            BooksOwned = 0;
            TotalRefunded = 0;
            ActiveBorrows = 0;

            // Create table for active borrows
            ActiveBorrowsList = MyBorrows.Clone();

            foreach (DataRow row in MyBorrows.Rows)
            {
                // Check if purchased
                var isPurchased = false;
                try
                {
                    if (row["IsPurchased"] != DBNull.Value && Convert.ToBoolean(row["IsPurchased"]))
                    {
                        isPurchased = true;
                    }
                }
                catch { }

                // Check status
                var status = row["Status"]?.ToString();

                // Count stats
                if (row["TotalPaid"] != DBNull.Value && row["TotalPaid"] != null)
                {
                    TotalSpent += Convert.ToDecimal(row["TotalPaid"]);
                }

                if (isPurchased)
                {
                    BooksOwned++;
                }

                if (row["RefundedAmount"] != DBNull.Value && row["RefundedAmount"] != null)
                {
                    TotalRefunded += Convert.ToDecimal(row["RefundedAmount"]);
                }

                if (row["UnpaidAmount"] != DBNull.Value && row["UnpaidAmount"] != null)
                {
                    TotalFines += Convert.ToDecimal(row["UnpaidAmount"]);
                }

                // Add ONLY active borrows (not purchased, not returned)
                if (!isPurchased && (status == "Borrowed" || status == "Overdue"))
                {
                    ActiveBorrowsList.ImportRow(row);
                    ActiveBorrows++;
                }
            }
        }
    }
}
