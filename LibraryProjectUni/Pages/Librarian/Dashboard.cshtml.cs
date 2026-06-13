using BusinessLogicLayer;
using BusnissLogicLayer;
using LibraryProjectUni.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data;

namespace LibraryProjectUni.Pages.Librarian
{
    public class DashboardModel : ClsBaseController
    {
        public int TotalBooks { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveBorrows { get; set; }
        public int OverdueCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalFines { get; set; }
        public decimal TotalRefunded { get; set; }
        public int TotalBooksOwned { get; set; }
        public DataTable RecentBorrows { get; set; } = new();
        public DataTable RecentOverdue { get; set; } = new();

        public IActionResult OnGet()
        {
            var check = RequireLibrarian();
            if (check != null) return check;

            LoadData();
            return Page();
        }

        private void LoadData()
        {
            // Get all data
            DataTable allBooks = ClsBook.GetAllBooks();
            DataTable allUsers = ClsUser.GetAllUsers();
            DataTable allBorrows = ClsBorrow.GetAllBorrows();

            TotalBooks = allBooks.Rows.Count;
            TotalUsers = allUsers.Rows.Count;

            // Initialize counters
            ActiveBorrows = 0;
            OverdueCount = 0;
            TotalRevenue = 0;
            TotalFines = 0;
            TotalRefunded = 0;
            TotalBooksOwned = 0;

            // Create tables
            RecentBorrows = allBorrows.Clone();
            RecentOverdue = allBorrows.Clone();

            foreach (DataRow row in allBorrows.Rows)
            {
                var isPurchased = false;
                var status = row["Status"]?.ToString();

                try
                {
                    if (row["IsPurchased"] != DBNull.Value && Convert.ToBoolean(row["IsPurchased"]))
                    {
                        isPurchased = true;
                    }
                }
                catch { }

                // Count books owned by users
                if (isPurchased)
                {
                    TotalBooksOwned++;
                }

                // Count active borrows (not purchased, not returned)
                if (!isPurchased && (status == "Borrowed" || status == "Overdue"))
                {
                    ActiveBorrows++;

                    // Add to recent borrows
                    RecentBorrows.ImportRow(row);
                }

                // Count overdue
                if (status == "Overdue")
                {
                    OverdueCount++;
                    RecentOverdue.ImportRow(row);
                }

                // Calculate revenue (total paid)
                try
                {
                    if (row["TotalPaid"] != DBNull.Value && row["TotalPaid"] != null)
                    {
                        TotalRevenue += Convert.ToDecimal(row["TotalPaid"]);
                    }
                }
                catch { }

                // Calculate unpaid fines
                try
                {
                    if (row["UnpaidAmount"] != DBNull.Value && row["UnpaidAmount"] != null)
                    {
                        TotalFines += Convert.ToDecimal(row["UnpaidAmount"]);
                    }
                }
                catch { }

                // Calculate refunded
                try
                {
                    if (row["RefundedAmount"] != DBNull.Value && row["RefundedAmount"] != null)
                    {
                        TotalRefunded += Convert.ToDecimal(row["RefundedAmount"]);
                    }
                }
                catch { }
            }
        }
    }
}
