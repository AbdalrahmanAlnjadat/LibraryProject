using BusinessLogicLayer;
using BusnissLogicLayer;
using LibraryProjectUni.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data;

namespace LibraryProjectUni.Pages.Librarian
{
    public class UserDetailsModel : ClsBaseController
    {
        public ClsUser User { get; set; }
        public string RoleName { get; set; }
        public int TotalBorrowed { get; set; }
        public int ActiveBorrows { get; set; }
        public decimal UnpaidFines { get; set; }
        public DataTable History { get; set; }

        public IActionResult OnGet(int id)
        {
            var check = RequireLibrarian();
            if (check != null) return check;

            User = ClsUser.FindByUserId(id);
            if (User == null)
                return RedirectToPage("/Librarian/Users");

            RoleName = User.RoleId == 1 ? "Librarian" : "Member";

            History = ClsBorrow.GetUserBorrowHistory(id);
            TotalBorrowed = History.Rows.Count;

            ActiveBorrows = 0;
            UnpaidFines = 0;

            foreach (DataRow row in History.Rows)
            {
                var status = row["Status"]?.ToString();

                // Count active borrows (not purchased, not returned)
                var isPurchased = false;
                try
                {
                    if (row["IsPurchased"] != DBNull.Value && Convert.ToBoolean(row["IsPurchased"]))
                    {
                        isPurchased = true;
                    }
                }
                catch { }

                if (!isPurchased && (status == "Borrowed" || status == "Overdue"))
                {
                    ActiveBorrows++;
                }

                // Calculate unpaid fines - use UnpaidAmount column
                if (row["UnpaidAmount"] != DBNull.Value && row["UnpaidAmount"] != null)
                {
                    UnpaidFines += Convert.ToDecimal(row["UnpaidAmount"]);
                }
            }

            return Page();
        }
    }
}