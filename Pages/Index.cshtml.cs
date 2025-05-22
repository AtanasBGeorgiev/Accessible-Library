using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Library.Pages
{
    public class IndexModel : PageModel
    {
        public static List<AdminInformation> listAdmins = new List<AdminInformation>();
        
        public string errorMessage = "";

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            listAdmins.Clear();

            try
            {
                int countAdmins = 0;
                string connectionString = OftenUsedMethods.ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Email,Telephone from [dbo].[Administrator] where IsConfirmed='True';";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (countAdmins == 3)
                                {
                                    break;
                                }
                                else
                                {
                                    AdminInformation admin = new AdminInformation();

                                    admin.Email = reader.GetString(0);
                                    admin.Telephone = reader.GetString(1);

                                    listAdmins.Add(admin);

                                    countAdmins++;
                                }
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}