using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Library.Pages
{
    public class ActivateAdminModel : PageModel
    {
        private string _code;
        private string _email;
        private string _password;
        private string _confirmPassword;
        private string _hashedPassword;
        private string _hashedCode;
        private DateTime _verCodeDate;

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }
        public void OnPost()
        {
            _code = Request.Form["code"];
            _email = Request.Form["email"];
            _password = Request.Form["password"];
            _confirmPassword = Request.Form["confirmPassword"];

            Regex validateAdmin = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[А-Я])(?=.*?[а-я])(?=.*?[#?!@$%^&*-]).{15,}$");

            if (_code.Length == 0 || _email.Length == 0 || _password.Length == 0 || _confirmPassword.Length == 0)
            {
                errorMessage = "Всички полета са задължителни!";
                return;
            }
            else
            {
                if (_password.Trim() != _confirmPassword.Trim())
                {
                    errorMessage = "Двете пароли не съвпадат!";
                    return;
                }
                else if (validateAdmin.IsMatch(_password) == false)
                {
                    errorMessage = "Паролата не отговаря на изискванията!";
                    return;
                }
                else
                {
                    try
                    {
                        string connectionString = OftenUsedMethods.ConnectionString;
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            int count;
                            string query1 = "SELECT count(*) FROM [dbo].[Administrator] WHERE Email=@email and IsConfirmed='False';";
                            using (SqlCommand command1 = new SqlCommand(query1, connection))
                            {
                                command1.Parameters.AddWithValue("@email", _email.Trim());

                                count = (int)command1.ExecuteScalar();
                            }

                            if (count > 0)
                            {
                                _hashedCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(_code.Trim()));

                                string query2 = "SELECT count(*) FROM [dbo].[Administrator] WHERE Email=@email and VerCode=@verCode;";
                                using (SqlCommand command2 = new SqlCommand(query2, connection))
                                {
                                    command2.Parameters.AddWithValue("@email", _email.Trim());
                                    command2.Parameters.AddWithValue("@verCode", _hashedCode);

                                    count = (int)command2.ExecuteScalar();
                                }

                                if (count > 0) 
                                {
                                    int id = 0;
                                    string query3 = "SELECT ID,VerCodeDate FROM [dbo].[Administrator] WHERE Email=@email;";
                                    using (SqlCommand command3 = new SqlCommand(query3, connection))
                                    {
                                        command3.Parameters.AddWithValue("@email", _email.Trim());

                                        using (SqlDataReader reader = command3.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                id = reader.GetInt32(0);
                                                _verCodeDate = reader.GetDateTime(1);
                                            }
                                        }
                                    }

                                    TimeSpan deadline = TimeSpan.FromMinutes(10);

                                    if (DateTime.Now - _verCodeDate > deadline)
                                    {
                                        errorMessage = "Кодът вече е невалиден.Трябва да се генерира нов.";
                                    }
                                    else
                                    {
                                        _hashedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(_confirmPassword.Trim()));

                                        string query4 = $"UPDATE [dbo].[Administrator] SET Password=@password, IsConfirmed=@isConfirmed, VerCode= NULL, VerCodeDate= NULL WHERE (ID = @id);";
                                        using (SqlCommand command4 = new SqlCommand(query4, connection))
                                        {
                                            command4.Parameters.AddWithValue("@password", _hashedPassword);
                                            command4.Parameters.AddWithValue("@isConfirmed", true);
                                            command4.Parameters.AddWithValue("@id", id);

                                            command4.ExecuteNonQuery();
                                        }
                                        successMessage = "Активира акаунта си.";
                                    }
                                }
                                else
                                {
                                    errorMessage = "Кодът за верификация е грешен.";
                                }
                            }
                            else
                            {
                                errorMessage = "Имейлът не е намерен или профилът вече е одобрен!";
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
    }
}