using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Library.Pages
{
    public class AdminsModel : PageModel
    {
        public static List<AdminInformation> listAdmins = new List<AdminInformation>();

        public string errorMessage = "";

        public void OnGet()
        {
            listAdmins.Clear();

            try
            {
                string connectionString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = $"SELECT ID,Name,Surname,LastName,Email,Telephone,UserName,DateOfCreation from [dbo].[Administrator] WHERE IsConfirmed='True' and Role='0';";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AdminInformation adminInfo = new AdminInformation();
                                int id;
                                DateTime date;

                                id = reader.GetInt32(0);
                                adminInfo.Id = id.ToString();
                                adminInfo.Name = reader.GetString(1);
                                adminInfo.Surname = reader.GetString(2);
                                adminInfo.LastName = reader.GetString(3);
                                adminInfo.Email = reader.GetString(4);
                                adminInfo.Telephone = reader.GetString(5);
                                adminInfo.UserName = reader.GetString(6);
                                date = reader.GetDateTime(7);
                                adminInfo.DateOfCreation = date.ToString();

                                listAdmins.Add(adminInfo);
                            }
                        }
                    }
                    connection.Close();
                }
                if (listAdmins.Count == 0)
                {
                    errorMessage = "Ти си единственият администратор.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}
