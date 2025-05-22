using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Library.Pages
{
    public class AdminInfoModel : PageModel
    {
        public static AdminInformation adminInfo = new AdminInformation();
        private static string _name;
        private static string _surname;
        private static string _lastName;
        private static string _email;
        private static string _telephone;
        private static string _userName;

        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
            string id = Request.Query["id"];

            try
            {
                string connectionString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Name,Surname,LastName,Email,Telephone,UserName,DateOfCreation from [dbo].[Administrator] where ID=@id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                       
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime date;

                                adminInfo.Id = id;
                                _name = reader.GetString(0);
                                adminInfo.Name = _name;
                                _surname = reader.GetString(1);
                                adminInfo.Surname = _surname;
                                _lastName = reader.GetString(2);
                                adminInfo.LastName = _lastName;
                                _email = reader.GetString(3);
                                adminInfo.Email = _email;
                                _telephone = reader.GetString(4);
                                adminInfo.Telephone = _telephone;
                                _userName = reader.GetString(5);
                                adminInfo.UserName = _userName;
                                date = reader.GetDateTime(6);
                                adminInfo.DateOfCreation = date.ToString();
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

        public void OnPost()
        {
            AdminInformation admin = new AdminInformation();

            string id = Request.Query["id"];

            admin.Name = Request.Form["name"];
            admin.Surname = Request.Form["surname"];
            admin.LastName = Request.Form["lastName"];
            admin.Email = Request.Form["email"];
            admin.Telephone = Request.Form["phone"];
            admin.UserName = Request.Form["userName"];
            admin.UserName = admin.UserName.Trim();

            if (admin.Name.Length == 0 || admin.Surname.Length == 0 || admin.LastName.Length == 0 ||
                admin.Email.Length == 0 || admin.Telephone.Length == 0 || admin.UserName.Length == 0)
            {
                errorMessage = "Всички полета са задължителни!";
                return;
            }
            else
            {
                admin.Email = admin.Email.Trim();

                string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
                Regex regex = new Regex(pattern);

                Regex validate = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[А-Я])(?=.*?[а-я])(?=.*?[#?!@$%^&*-]).{8,}$");

                if (OftenUsedMethods.CorrectNameInput(admin.Name) == "Грешка")
                {
                    errorMessage = "Името трябва да е една дума.";
                }
                else if (OftenUsedMethods.CorrectNameInput(admin.Surname) == "Грешка")
                {
                    errorMessage = "Презимето трябва да е една дума.";
                }
                else if (OftenUsedMethods.CorrectNameInput(admin.LastName) == "Грешка")
                {
                    errorMessage = "Фамилията трябва да е една дума.";
                }
                else if (regex.IsMatch(admin.Email) == false)
                {
                    errorMessage = "Този имейл не е валиден!";
                }
                else if (OftenUsedMethods.CorrectPhone(admin.Telephone) == "Грешка")
                {
                    errorMessage = "Телефонът трябва да започва с 0 и да не е повече от 10 цифри!";
                }

                if (errorMessage.Length > 0)
                {
                    return;
                }
                else
                {
                    admin.Name = OftenUsedMethods.CorrectNameInput(admin.Name);
                    admin.Surname = OftenUsedMethods.CorrectNameInput(admin.Surname);
                    admin.LastName = OftenUsedMethods.CorrectNameInput(admin.LastName);
                    admin.Telephone = OftenUsedMethods.CorrectPhone(admin.Telephone);
                    admin.UserName = admin.UserName.Trim();

                    if (admin.Name == adminInfo.Name && admin.Surname == adminInfo.Surname && admin.LastName == adminInfo.LastName && admin.Email == adminInfo.Email &&
                admin.Telephone == adminInfo.Telephone && admin.UserName == adminInfo.UserName)
                    {
                        successMessage = "Не промени информацията за профила.";
                    }
                    else
                    {
                        try
                        {
                            string connectionString = OftenUsedMethods.ConnectionString;
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();

                                if (admin.Email != adminInfo.Email)
                                {
                                    int count = 0;
                                    string query1 = "SELECT count(*) from [dbo].[Administrator] where Email=@email;";
                                    using (SqlCommand command1 = new SqlCommand(query1, connection))
                                    {
                                        command1.Parameters.AddWithValue("@email", admin.Email);
                                        count = (int)command1.ExecuteScalar();
                                    }
                                    if (count > 0)
                                    {
                                        errorMessage = "Този имейл вече е използван!";
                                        return;
                                    }
                                }
                                else if (admin.Telephone != adminInfo.Telephone)
                                {
                                    int count = 0;
                                    string query2 = "SELECT count(*) from [dbo].[Administrator] where Telephone=@phone;";
                                    using (SqlCommand command2 = new SqlCommand(query2, connection))
                                    {
                                        command2.Parameters.AddWithValue("@phone", admin.Telephone);
                                        count = (int)command2.ExecuteScalar();
                                    }
                                    if (count > 0)
                                    {
                                        errorMessage = "Този телефон вече е използван!";
                                        return;
                                    }
                                }
                                else if (admin.UserName != adminInfo.UserName)
                                {
                                    int count = 0;
                                    string query3 = "SELECT count(*) from [dbo].[Administrator] where UserName=@userName;";
                                    using (SqlCommand command3 = new SqlCommand(query3, connection))
                                    {
                                        command3.Parameters.AddWithValue("@userName", admin.UserName.Trim());
                                        count = (int)command3.ExecuteScalar();
                                    }
                                    if (count > 0)
                                    {
                                        errorMessage = "Това потребителско име вече е използвано!";
                                        return;
                                    }
                                }
                                if (errorMessage.Length > 0)
                                {
                                    adminInfo.Name = _name;
                                    adminInfo.Surname = _surname;
                                    adminInfo.LastName = _lastName;
                                    adminInfo.Email = _email;
                                    adminInfo.Telephone = _telephone;
                                    adminInfo.UserName = _userName;
                                    return;
                                }
                                else
                                {
                                    string query4 = $"UPDATE [dbo].[Administrator] SET Name=@name,Surname=@surname,LastName=@lastName,Email=@email,Telephone=@phone,UserName=@userName WHERE ID=@id ";

                                    using (SqlCommand command4 = new SqlCommand(query4, connection))
                                    {
                                        command4.Parameters.AddWithValue("@id", id);
                                        command4.Parameters.AddWithValue("@name", admin.Name);
                                        command4.Parameters.AddWithValue("@surname", admin.Surname);
                                        command4.Parameters.AddWithValue("@lastName", admin.LastName);
                                        command4.Parameters.AddWithValue("@email", admin.Email);
                                        command4.Parameters.AddWithValue("@phone", admin.Telephone);
                                        command4.Parameters.AddWithValue("@userName", admin.UserName.Trim());

                                        command4.ExecuteNonQuery();
                                    }
                                    successMessage = "Информацията за този профил е обновена.";

                                    adminInfo.Name = admin.Name;
                                    adminInfo.Surname = admin.Surname;
                                    adminInfo.LastName = admin.LastName;
                                    adminInfo.Email = admin.Email;
                                    adminInfo.Telephone = admin.Telephone;
                                    adminInfo.UserName = admin.UserName;
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

}  

