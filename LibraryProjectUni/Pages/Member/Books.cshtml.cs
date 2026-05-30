using BusnissLogicLayer;
using LibraryProjectUni.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;

namespace LibraryProjectUni.Pages.Member
{
    public class BooksModel : ClsBaseController
    {
        public DataTable Books { get; set; }
        public string SearchTerm { get; set; }
        public string CategoryFilter { get; set; }
        public List<string> Categories { get; set; }
        public string Error { get; set; }
        public string Success { get; set; }

        public IActionResult OnGet(string search, string categoryFilter)
        {
            var check = RequireMember();
            if (check != null) return check;

            SearchTerm = search ?? "";
            CategoryFilter = categoryFilter ?? "";

            LoadBooks();
            return Page();
        }

        private void LoadBooks()
        {
            DataTable allBooks = ClsBook.GetAvailableBooks();

            Categories = new List<string>();
            foreach (DataRow row in allBooks.Rows)
            {
                string cat = row["Category"]?.ToString();
                if (!string.IsNullOrEmpty(cat) && !Categories.Contains(cat))
                    Categories.Add(cat);
            }

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                string safe = SearchTerm.Replace("'", "''");
                string filter = $"Title LIKE '%{safe}%' OR Author LIKE '%{safe}%' OR Category LIKE '%{safe}%'";
                DataRow[] filtered = allBooks.Select(filter);
                Books = filtered.Length > 0 ? filtered.CopyToDataTable() : allBooks.Clone();
            }
            else if (!string.IsNullOrWhiteSpace(CategoryFilter))
            {
                string safe = CategoryFilter.Replace("'", "''");
                DataRow[] filtered = allBooks.Select($"Category = '{safe}'");
                Books = filtered.Length > 0 ? filtered.CopyToDataTable() : allBooks.Clone();
            }
            else
            {
                Books = allBooks;
            }
        }

        public IActionResult OnPostBorrow(int bookId)
        {
            var check = RequireMember();
            if (check != null) return check;

            try
            {
                // Get the actual rent fee for this book
                ClsBook book = ClsBook.FindByBookId(bookId);
                decimal rentFee = book != null ? book.Price : ClsBorrow.DefaultFineAmount;

                ClsBorrow borrow = new ClsBorrow();
                borrow.UserId = SessionUserId;
                borrow.BookId = bookId;
                borrow.Amount = rentFee;

                if (borrow.Save())
                    Success = $"Book borrowed! ${rentFee:0.00} rental fee + $5.00 refundable deposit charged.";
                else
                    Error = "Failed to borrow book.";
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            LoadBooks();
            return Page();
        }

        public IActionResult OnPostBuyDirect(int bookId, decimal price)
        {
            var check = RequireMember();
            if (check != null) return check;

            try
            {
                if (ClsBook.BuyBookDirect(SessionUserId, bookId, price))
                    Success = $"Book purchased for ${price:0.00}! It's yours forever.";
                else
                    Error = "Failed to purchase book.";
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            LoadBooks();
            return Page();
        }
    }
}