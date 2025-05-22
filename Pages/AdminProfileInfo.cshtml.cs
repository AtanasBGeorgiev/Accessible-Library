using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace Library.Pages
{
    public class AdminProfileInfoModel : PageModel
    {
        public static AdminInformation adminInfo = new AdminInformation();
       
        public string errorMessage = "";
        public string successMessage = "";
        private string _hashedPassword;

        public void OnPost()
        {
            AdminInformation admin = new AdminInformation();

            admin.Name = Request.Form["name"];
            admin.Surname = Request.Form["surname"];
            admin.LastName = Request.Form["lastName"];
            admin.Email = Request.Form["email"];
            admin.Telephone = Request.Form["phone"];
            admin.UserName = Request.Form["userName"];
            admin.UserName = admin.UserName.Trim();
            admin.Password = Request.Form["password"];
            string check = Request.Form["check"];

            if (admin.Name.Length == 0 || admin.Surname.Length == 0 || admin.LastName.Length == 0 ||
                admin.Email.Length == 0 || admin.Telephone.Length == 0 || admin.UserName.Length == 0 || LoginLibraryModel.loggedAdmin.Password.Length == 0)
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
                else if (validate.IsMatch(admin.Password) == false)
                {
                    errorMessage = "Паролата не отговаря на изискванията!";
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
                    _hashedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(admin.Password.Trim()));

                    if (admin.Name == LoginLibraryModel.loggedAdmin.Name && admin.Surname == LoginLibraryModel.loggedAdmin.Surname && admin.LastName == LoginLibraryModel.loggedAdmin.LastName && admin.Email == LoginLibraryModel.loggedAdmin.Email &&
                admin.Telephone == LoginLibraryModel.loggedAdmin.Telephone && admin.UserName == LoginLibraryModel.loggedAdmin.UserName && admin.Password == LoginLibraryModel.loggedAdmin.Password)
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

                                if (admin.Email != LoginLibraryModel.loggedAdmin.Email)
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
                                else if (admin.Telephone != LoginLibraryModel.loggedAdmin.Telephone)
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
                                else if (admin.UserName != LoginLibraryModel.loggedAdmin.UserName)
                                {
                                    int count = 0;
                                    string query3 = "SELECT count(*) from [dbo].[Administrator] where UserName=@userName;";
                                    using (SqlCommand command3 = new SqlCommand(query3, connection))
                                    {
                                        command3.Parameters.AddWithValue("@userName", admin.UserName);
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
                                    return;
                                }
                                else
                                {
                                    string query4 = $"UPDATE [dbo].[Administrator] SET Name=@name,Surname=@surname,LastName=@lastName,Email=@email,Telephone=@phone,UserName=@userName,Password=@password WHERE ID=@id ";

                                    using (SqlCommand command4 = new SqlCommand(query4, connection))
                                    {
                                        command4.Parameters.AddWithValue("@id", LoginLibraryModel.loggedAdmin.Id);
                                        command4.Parameters.AddWithValue("@name", admin.Name);
                                        command4.Parameters.AddWithValue("@surname", admin.Surname);
                                        command4.Parameters.AddWithValue("@lastName", admin.LastName);
                                        command4.Parameters.AddWithValue("@email", admin.Email);
                                        command4.Parameters.AddWithValue("@phone", admin.Telephone);
                                        command4.Parameters.AddWithValue("@userName", admin.UserName);
                                        command4.Parameters.AddWithValue("@password", _hashedPassword);

                                        command4.ExecuteNonQuery();
                                    }
                                    successMessage = "Информацията за този профил е обновена.";

                                    LoginLibraryModel.loggedAdmin.Name = admin.Name;
                                    LoginLibraryModel.loggedAdmin.Surname = admin.Surname;
                                    LoginLibraryModel.loggedAdmin.LastName = admin.LastName;
                                    LoginLibraryModel.loggedAdmin.Email = admin.Email;
                                    LoginLibraryModel.loggedAdmin.Telephone = admin.Telephone;
                                    LoginLibraryModel.loggedAdmin.UserName = admin.UserName;
                                    LoginLibraryModel.loggedAdmin.Password = admin.Password;
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

