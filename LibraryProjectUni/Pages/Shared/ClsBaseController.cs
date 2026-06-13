using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryProjectUni.Pages.Shared
{
    public class ClsBaseController : PageModel
    {
        public const int RoleLibrarian = 1;
        public const int RoleMember = 2;

        protected int SessionUserId => HttpContext.Session.GetInt32("UserID") ?? 0;
        protected string SessionFullName => HttpContext.Session.GetString("FullName") ?? "";
        protected int SessionRoleId => HttpContext.Session.GetInt32("RoleID") ?? 0;
        protected bool IsLoggedIn => SessionUserId > 0;
        protected bool IsLibrarian => SessionRoleId == RoleLibrarian;
        protected bool IsMember => SessionRoleId == RoleMember;

        protected IActionResult? RequireLogin()
        {
            if (!IsLoggedIn)
                return RedirectToPage("/Account/Login");
            return null;
        }

        protected IActionResult? RequireLibrarian()
        {
            var check = RequireLogin();
            if (check != null) return check;
            if (!IsLibrarian)
                return RedirectToPage("/Member/Dashboard");
            return null;
        }

        protected IActionResult? RequireMember()
        {
            var check = RequireLogin();
            if (check != null) return check;
            if (!IsMember)
                return RedirectToPage("/Librarian/Dashboard");
            return null;
        }

        protected void SetSession(int userId, string fullName, int roleId)
        {
            HttpContext.Session.SetInt32("UserID", userId);
            HttpContext.Session.SetString("FullName", fullName);
            HttpContext.Session.SetInt32("RoleID", roleId);
        }

        protected void ClearSession()
        {
            HttpContext.Session.Clear();
        }
    }
}
