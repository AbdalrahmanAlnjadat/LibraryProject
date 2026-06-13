using BusinessLogicLayer;
using BusnissLogicLayer;
using LibraryProjectUni.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data;

namespace LibraryProjectUni.Pages.Librarian
{
    public class BorrowsModel : ClsBaseController
    {
        public DataTable Borrows { get; set; } = new();
        public string Success { get; set; } = "";
        public int TotalBorrows { get; set; }
        public int TotalOwned { get; set; }
        public int ActiveBorrows { get; set; }
        public decimal TotalFines { get; set; }

        public IActionResult OnGet()
        {
            var check = RequireLibrarian();
            if (check != null) return check;

            LoadData();
            return Page();
        }

        private void LoadData()
        {
            Borrows = ClsBorrow.GetAllBorrows();

            TotalBorrows = Borrows.Rows.Count;
            TotalOwned = 0;
            ActiveBorrows = 0;
            TotalFines = 0;

            foreach (DataRow row in Borrows.Rows)
            {
                var isPurchased = false;

                try
                {
                    if (row["IsPurchased"] != DBNull.Value && Convert.ToInt32(row["IsPurchased"]) == 1)
                    {
                        isPurchased = true;
                    }
                }
                catch
                {
                    isPurchased = false;
                }

                if (isPurchased)
                {
                    TotalOwned++;
                }

                var status = row["Status"]?.ToString();
                if (!isPurchased && (status == "Borrowed" || status == "Overdue"))
                {
                    ActiveBorrows++;
                }

                try
                {
                    if (row["UnpaidAmount"] != DBNull.Value && row["UnpaidAmount"] != null)
                    {
                        TotalFines += Convert.ToDecimal(row["UnpaidAmount"]);
                    }
                }
                catch { }
            }
        }

        public IActionResult OnPostReturn(int borrowId)
        {
            var check = RequireLibrarian();
            if (check != null) return check;

            try
            {
                if (ClsBorrow.ReturnBook(borrowId))
                {
                    Success = "Book returned safely and inventory count updated!";
                }
                else
                {
                    Success = "Failed to process book return. Verify database entry status.";
                }
            }
            catch (Exception ex)
            {
                Success = "Error: " + ex.Message;
            }

            LoadData();
            return Page();
        }

        public IActionResult OnPostPayFine(int borrowId)
        {
            var check = RequireLibrarian();
            if (check != null) return check;

            try
            {
                if (ClsBorrow.PayFine(borrowId))
                {
                    Success = "Fine settlement processed successfully!";
                }
                else
                {
                    Success = "Failed to clear the pending fine balance.";
                }
            }
            catch (Exception ex)
            {
                Success = "Error: " + ex.Message;
            }

            LoadData();
            return Page();
        }
    }
}
