using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Library.Pages
{
    public class LogoutAdminModel : PageModel
    {
        public void OnGet()
        {
            LoginLibraryModel.loggedAdmin.Id = "0";
            HttpContext.Session.Clear();
            Response.Redirect("/Index");
        }
    }
}
