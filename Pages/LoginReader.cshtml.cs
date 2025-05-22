using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace Library.Pages
{
    public class LoginReaderModel : PageModel
    {
        public static Reader readerInfo = new Reader();
        public string errorMessage = "";
        private string _hashedPassword;

        public void OnPost()
        {
            readerInfo.Email = Request.Form["email"];
            readerInfo.Password = Request.Form["password"];

            if (readerInfo.Email.Length == 0 || readerInfo.Password.Length == 0)
            {
                errorMessage = "Всички полета са задължителни!";
                return;
            }
            else
            {
                _hashedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(readerInfo.Password.Trim()));
                try
                {
                    string connectionString = OftenUsedMethods.ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        int count = 0;
                        string query1 = "SELECT count(*) from [dbo].[Reader] where Email=@email and Password=@password;";
                        using (SqlCommand command1 = new SqlCommand(query1, connection))
                        {
                            command1.Parameters.AddWithValue("@email", readerInfo.Email.Trim());
                            command1.Parameters.AddWithValue("@password", _hashedPassword);
                            count = (int)command1.ExecuteScalar();
                        }
                        if (count == 0)
                        {
                            errorMessage = "Този профил не съществува или паролата е грешно въведена!";
                            return;
                        }
                        else
                        {
                            string query2 = "SELECT * FROM [dbo].[Reader] where Email=@email and Password=@password;";
                            using (SqlCommand command2 = new SqlCommand(query2, connection))
                            {
                                command2.Parameters.AddWithValue("@email", readerInfo.Email.Trim());
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
                                        readerInfo.Id = id.ToString();
                                        readerInfo.Name = reader.GetString(1);
                                        readerInfo.LastName = reader.GetString(2);
                                        readerInfo.Email = reader.GetString(3);
                                        readerInfo.Telephone = reader.GetString(4);
                                        hashedPassword = reader.GetString(5);
                                        readerInfo.Password = Encoding.UTF8.GetString(Convert.FromBase64String(hashedPassword));
                                        date = reader.GetDateTime(6);
                                        readerInfo.DateOfCreation = date.ToString();

                                        HttpContext.Session.SetString("ReaderId", readerInfo.Id);
                                        Response.Redirect("/TakenBooks");
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