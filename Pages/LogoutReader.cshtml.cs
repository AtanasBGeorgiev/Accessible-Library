using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Library.Pages
{
    public class LogoutReaderModel : PageModel
    {
        public void OnGet()
        {
            LoginReaderModel.readerInfo.Id = "0";
            HttpContext.Session.Clear();
            Response.Redirect("/Index");
        }
    }
}
