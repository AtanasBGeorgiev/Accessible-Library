using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace Library.Pages
{
    public class LoginLibraryModel : PageModel
    {
        public static LibraryInformation libraryInfo = new LibraryInformation();
        public static AdminInformation loggedAdmin = new AdminInformation();

        public string errorMessage = "";
        private string _hashedPassword = "";

        public void OnGet()
        {
        }
        public void OnPost()
        {
            libraryInfo.Email = Request.Form["email"];
            libraryInfo.Password = Request.Form["password"];

            if (libraryInfo.Email.Length == 0 || libraryInfo.Password.Length == 0)
            {
                errorMessage = "Всички полета са задължителни!";
                return;
            }
            else
            {
                _hashedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(libraryInfo.Password.Trim()));
                try
                {
                    string connectionString = OftenUsedMethods.ConnectionString;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        int count1, count2 = 0;
                        string query1 = "SELECT count(*) from [dbo].[Library] where Email=@email and Password=@password;";
                        using (SqlCommand command1 = new SqlCommand(query1, connection))
                        {
                            command1.Parameters.AddWithValue("@email", libraryInfo.Email.Trim());
                            command1.Parameters.AddWithValue("@password", _hashedPassword);
                            count1 = (int)command1.ExecuteScalar();
                        }
                        if (count1 == 0)
                        {
                            string query3 = "SELECT count(*) from [dbo].[Administrator] where UserName=@userName and Password=@password;";
                            using (SqlCommand command3 = new SqlCommand(query3, connection))
                            {
                                command3.Parameters.AddWithValue("@userName", libraryInfo.Email.Trim());
                                command3.Parameters.AddWithValue("@password", _hashedPassword);
                                count2 = (int)command3.ExecuteScalar();
                            }
                            if (count2 == 0)
                            {
                                errorMessage = "Този профил не е намерен!";
                                return;
                            }
                            else
                            {
                                string query4 = "SELECT IsConfirmed from [dbo].[Administrator] where UserName=@userName and Password=@password;";
                                using (SqlCommand command4 = new SqlCommand(query4, connection))
                                {
                                    command4.Parameters.AddWithValue("@userName", libraryInfo.Email.Trim());
                                    command4.Parameters.AddWithValue("@password", _hashedPassword);
                                    loggedAdmin.IsConfirmed = (bool)command4.ExecuteScalar();
                                }
                                if (loggedAdmin.IsConfirmed == false)
                                {
                                    errorMessage = "Този профил не е намерен!";
                                    return;
                                }
                                else
                                {
                                    string query5 = "SELECT ID,Name,Surname,LastName,Email,Telephone,UserName,Password,Role,IsConfirmed,DateOfCreation from [dbo].[Administrator] where UserName=@userName and Password=@password;";
                                    using (SqlCommand command5 = new SqlCommand(query5, connection))
                                    {
                                        command5.Parameters.AddWithValue("@userName", libraryInfo.Email.Trim());
                                        command5.Parameters.AddWithValue("@password", _hashedPassword);

                                        using (SqlDataReader reader = command5.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                DateTime date;
                                                int id;
                                                string hashedPassword;

                                                id = reader.GetInt32(0);
                                                loggedAdmin.Id = id.ToString();
                                                loggedAdmin.Name = reader.GetString(1);
                                                loggedAdmin.Surname = reader.GetString(2);
                                                loggedAdmin.LastName = reader.GetString(3);
                                                loggedAdmin.Email = reader.GetString(4);
                                                loggedAdmin.Telephone = reader.GetString(5);
                                                loggedAdmin.UserName = reader.GetString(6);
                                                hashedPassword = reader.GetString(7);
                                                loggedAdmin.Password = Encoding.UTF8.GetString(Convert.FromBase64String(hashedPassword));
                                                loggedAdmin.Role = reader.GetString(8);
                                                loggedAdmin.IsConfirmed = reader.GetBoolean(9);
                                                date = reader.GetDateTime(10);
                                                loggedAdmin.DateOfCreation = date.ToString();

                                                if (loggedAdmin.Role == "1")
                                                {
                                                    HttpContext.Session.SetString("SystemAdminId", loggedAdmin.Id);
                                                    Response.Redirect("/AdminLibraries");
                                                }
                                                else
                                                {
                                                    HttpContext.Session.SetString("AdminId", loggedAdmin.Id);
                                                    Response.Redirect("/AdminLibraries");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            string query2 = "SELECT * FROM [dbo].[Library] where Email=@email and Password=@password;";
                            using (SqlCommand command2 = new SqlCommand(query2, connection))
                            {
                                command2.Parameters.AddWithValue("@email", libraryInfo.Email.Trim());
                                command2.Parameters.AddWithValue("@password", _hashedPassword);
                                _hashedPassword = "";

                                using (SqlDataReader reader = command2.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int id = 0;
                                        DateTime date;
                                        string hashedPassword;

                                        id = reader.GetInt32(0);
                                        libraryInfo.Id = id.ToString();
                                        libraryInfo.Name = reader.GetString(1);
                                        libraryInfo.CityVillage = reader.GetString(2);
                                        libraryInfo.Municipality = reader.GetString(3);
                                        libraryInfo.District = reader.GetString(4);
                                        hashedPassword = reader.GetString(5);
                                        libraryInfo.Password = Encoding.UTF8.GetString(Convert.FromBase64String(hashedPassword));
                                        libraryInfo.Email = reader.GetString(6);
                                        libraryInfo.Telephone = reader.GetString(7);
                                        libraryInfo.Address = reader.GetString(8);
                                        date = reader.GetDateTime(9);
                                        libraryInfo.DateOfCreation = date.ToString();

                                        HttpContext.Session.SetString("LibraryId", libraryInfo.Id);
                                        Response.Redirect("/MainPage");
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
                    return;
                }
            }
        }
    }
}