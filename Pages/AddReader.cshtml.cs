using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Text;

namespace Library.Pages
{
    public class AddReaderModel : PageModel
    {
        public static Reader readerInfo = new Reader();

        public string errorMessage = "";
        public string successMessage = "";

        private string _confirmPassword;
        private string _hashedPassword;

        public void OnGet()
        {
        }
        public void OnPost()
        {
            readerInfo.Name = Request.Form["name"];
            readerInfo.LastName = Request.Form["lastName"];
            readerInfo.Email = Request.Form["email"];
            readerInfo.Telephone = Request.Form["telephone"];
            readerInfo.Password = Request.Form["password"];
            _confirmPassword = Request.Form["confirmPassword"];

            string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
            Regex regex = new Regex(pattern);

            Regex validate = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[А-Я])(?=.*?[а-я])(?=.*?[#?!@$%^&*-]).{8,}$");

            if (readerInfo.Name.Length == 0 || readerInfo.LastName.Length == 0 || readerInfo.Email.Length == 0 || readerInfo.Telephone.Length == 0 || readerInfo.Password.Length == 0)
            {
                errorMessage = "Всички полета са задължителни!";
                return;
            }
            else if (readerInfo.Password.Trim() != _confirmPassword.Trim())
            {
                errorMessage = "Двете пароли не съвпадат!";
                return;
            }
            else if (OftenUsedMethods.CorrectNameInput(readerInfo.Name) == "Грешка" || 
                OftenUsedMethods.CorrectNameInput(readerInfo.LastName) == "Грешка") 
            {
                errorMessage = "Името/Фамилията трябва да е една дума!";
                return;
            }
            else if (OftenUsedMethods.CorrectPhone(readerInfo.Telephone) == "Грешка") 
            {
                errorMessage = "Телефонният номер трябва да започва с 0 и да не е повече от 10 цифри.";
            }          
            else if (regex.IsMatch(readerInfo.Email) == false)
            {
                errorMessage = "Този имейл не е валиден!";
                return;
            }
            else if (validate.IsMatch(readerInfo.Password) == false)
            {
                errorMessage = "Паролата не отговаря на изискванията!";
                return;
            }
            else
            {
                readerInfo.Name = OftenUsedMethods.CorrectNameInput(readerInfo.Name);
                readerInfo.LastName = OftenUsedMethods.CorrectNameInput(readerInfo.LastName);
                readerInfo.Telephone = OftenUsedMethods.CorrectPhone(readerInfo.Telephone);
                _hashedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(readerInfo.Password.Trim()));

                try
                {
                    string connectionString = OftenUsedMethods.ConnectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        bool exists = false;

                        SqlCommand cmd = new SqlCommand("Select count(*) from [dbo].[Reader] where Email=@email", connection);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@email", readerInfo.Email.Trim());

                        exists = (int)cmd.ExecuteScalar() > 0;

                        if (exists == true)
                        {
                            errorMessage = "Този имейл вече е използван!";
                            return;
                        }
                        else
                        {
                            SqlCommand cmd2 = new SqlCommand("Select count(*) from [dbo].[Reader] where Telephone=@telephone", connection);
                            cmd2.CommandType = CommandType.Text;
                            cmd2.Parameters.AddWithValue("@telephone", readerInfo.Telephone.Trim());

                            exists = (int)cmd2.ExecuteScalar() > 0;

                            if (exists == true)
                            {
                                errorMessage = "Този телефон вече е използван!";
                                return;
                            }
                            else
                            {
                                using (SqlCommand command = new SqlCommand("INSERT INTO [dbo].[Reader] (Name,LastName,Email,Telephone,Password) VALUES (@name,@lastName,@email,@telephone,@password);", connection))
                                {
                                    command.Parameters.AddWithValue("@name", readerInfo.Name.Trim());
                                    command.Parameters.AddWithValue("@lastName", readerInfo.LastName.Trim());
                                    command.Parameters.AddWithValue("@email", readerInfo.Email.Trim());
                                    command.Parameters.AddWithValue("@telephone", readerInfo.Telephone.Trim());
                                    command.Parameters.AddWithValue("@password", _hashedPassword);

                                    command.ExecuteNonQuery();

                                    successMessage = "Нов читател е добавен успешно.";
                                }
                            }

                        }
                        connection.Close();
                    }
                    _hashedPassword = "";
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return;
                }

                readerInfo.Name = "";
                readerInfo.LastName = "";
                readerInfo.Email = "";
                readerInfo.Telephone = "";
                readerInfo.Password = "";
            }

        }

    }
}