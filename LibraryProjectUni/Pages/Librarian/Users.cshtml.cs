using Microsoft.AspNetCore.Mvc;
using LibraryProjectUni.Pages.Shared;
using BusinessLogicLayer;
using System;
using System.Data;

namespace LibraryProjectUni.Pages.Librarian
{
    public class UsersModel : ClsBaseController
    {
        public DataTable Users { get; set; } = new();
        public string Error { get; set; } = "";
        public string Success { get; set; } = "";

        public IActionResult OnGet(string? search, string? roleFilter)
        {
            // Leverages centralized authorization from ClsBaseController
            var authCheck = RequireLibrarian();
            if (authCheck != null) return authCheck;

            LoadUsers(search, roleFilter);
            return Page();
        }

        private void LoadUsers(string? search, string? roleFilter)
        {
            try
            {
                // Route directly through the business layer logic
                DataTable allUsers = ClsUser.GetAllUsers();

                if (allUsers == null)
                {
                    Users = new DataTable();
                    return;
                }

                if (!string.IsNullOrWhiteSpace(search) || !string.IsNullOrWhiteSpace(roleFilter))
                {
                    string filterExpression = "1=1";

                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        string sanitizedSearch = search.Replace("'", "''");
                        filterExpression += $" AND (FullName LIKE '%{sanitizedSearch}%' OR Email LIKE '%{sanitizedSearch}%')";
                    }

                    if (!string.IsNullOrWhiteSpace(roleFilter))
                    {
                        int targetRoleId = (roleFilter == "Librarian") ? 1 : 2;
                        filterExpression += $" AND RoleID = {targetRoleId}";
                    }

                    DataRow[] filteredRows = allUsers.Select(filterExpression, "FullName ASC");
                    Users = filteredRows.Length > 0 ? filteredRows.CopyToDataTable() : allUsers.Clone();
                }
                else
                {
                    Users = allUsers;
                }
            }
            catch (Exception ex)
            {
                Error = "Failed to load system users safely. " + ex.Message;
                Users = new DataTable();
            }
        }

        public IActionResult OnPostPromote(int userId)
        {
            var authCheck = RequireLibrarian();
            if (authCheck != null) return authCheck;

            try
            {
                // Set user to RoleID = 1 (Librarian)
                if (ClsUser.UpdateUserRole(userId, 1))
                {
                    Success = "User promoted to Librarian successfully!";
                }
                else
                {
                    Error = "Failed to promote user accounts.";
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            LoadUsers(null, null);
            return Page();
        }

        public IActionResult OnPostDemote(int userId)
        {
            var authCheck = RequireLibrarian();
            if (authCheck != null) return authCheck;

            try
            {
                // Set user to RoleID = 2 (Member)
                if (ClsUser.UpdateUserRole(userId, 2))
                {
                    Success = "User demoted to Member successfully!";
                }
                else
                {
                    Error = "Failed to demote user accounts.";
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            LoadUsers(null, null);
            return Page();
        }
    }
}
