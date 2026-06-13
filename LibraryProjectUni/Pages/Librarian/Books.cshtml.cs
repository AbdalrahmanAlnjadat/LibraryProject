using BusnissLogicLayer;
using LibraryProjectUni.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;

namespace LibraryProjectUni.Pages.Librarian
{
    public class BooksModel : ClsBaseController
    {
        public DataTable Books { get; set; } = new();
        public string SearchTerm { get; set; } = "";
        public string CategoryFilter { get; set; } = "";
        public List<string> Categories { get; set; } = new();
        public string Success { get; set; } = "";
        public string Error { get; set; } = "";

        public IActionResult OnGet(string search, string categoryFilter)
        {
            var check = RequireLibrarian();
            if (check != null) return check;

            SearchTerm = search ?? "";
            CategoryFilter = categoryFilter ?? "";

            LoadBooks();
            return Page();
        }

        private void LoadBooks()
        {
            DataTable allBooks = ClsBook.GetAllBooks();

            // Get unique categories
            Categories = new List<string>();
            foreach (DataRow row in allBooks.Rows)
            {
                string cat = row["Category"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(cat) && !Categories.Contains(cat))
                    Categories.Add(cat);
            }

            // Apply filters safely — sanitize input before using in DataTable.Select
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                string safe = SearchTerm.Replace("'", "''");
                string filter = $"Title LIKE '%{safe}%' OR Author LIKE '%{safe}%'";
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

        public IActionResult OnPostDelete(int bookId)
        {
            var check = RequireLibrarian();
            if (check != null) return check;

            try
            {
                if (ClsBook.DeleteBook(bookId))
                    Success = "Book deleted successfully!";
                else
                    Error = "Failed to delete book.";
            }
            catch (InvalidOperationException ex)
            {
                Error = ex.Message;
            }
            catch (Exception)
            {
                Error = "Cannot delete this book — it may have active borrows.";
            }

            LoadBooks();
            return Page();
        }
    }
}
