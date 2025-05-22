using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Text;

namespace Library.Pages
{
    public class ReaderProfileInformationModel : PageModel
    {
        public static Reader personInfo = new Reader();

        public string errorMessage = "";
        public string successMessage = "";
        private string _hashedPassword;

        public void OnPost()
        {
            personInfo.Name = Request.Form["name"] ;
            personInfo.LastName = Request.Form["lastName"];
            personInfo.Email = Request.Form["email"];
            personInfo.Telephone = Request.Form["telephone"];
            personInfo.Password = Request.Form["password"];

            string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
            Regex regex = new Regex(pattern);

            Regex validate = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[А-Я])(?=.*?[а-я])(?=.*?[#?!@$%^&*-]).{8,}$");

            if (personInfo.Name.Length == 0 || personInfo.LastName.Length == 0 || personInfo.Email.Length == 0 || personInfo.Telephone.Length == 0 || personInfo.Password.Length == 0)
            {
                errorMessage = "Всички полета са задължителни!";
                return;
            }
            else if (OftenUsedMethods.CorrectNameInput(personInfo.Name) == "Грешка" ||
                OftenUsedMethods.CorrectNameInput(personInfo.LastName) == "Грешка")
            {
                errorMessage = "Името/Фамилията трябва да е една дума!";
                return;
            }
            else if (OftenUsedMethods.CorrectPhone(personInfo.Telephone) == "Грешка")
            {
                errorMessage = "Телефонният номер трябва да започва с 0 и да не е повече от 10 цифри.";
            }
            else if (regex.IsMatch(personInfo.Email) == false)
            {
                errorMessage = "Този имейл не е валиден!";
                return;
            }
            else if (validate.IsMatch(personInfo.Password) == false)
            {
                errorMessage = "Паролата не отговаря на изискванията!";
                return;
            }
            else
            {
                personInfo.Name = OftenUsedMethods.CorrectNameInput(personInfo.Name);
                personInfo.LastName = OftenUsedMethods.CorrectNameInput(personInfo.LastName);
                personInfo.Telephone = OftenUsedMethods.CorrectPhone(personInfo.Telephone);
                _hashedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(personInfo.Password.Trim()));

                try
                {
                    if (personInfo.Name == LoginReaderModel.readerInfo.Name && personInfo.LastName == LoginReaderModel.readerInfo.LastName && personInfo.Email == LoginReaderModel.readerInfo.Email && personInfo.Telephone == LoginReaderModel.readerInfo.Telephone && personInfo.Password == LoginReaderModel.readerInfo.Password)
                    {
                        successMessage = "Не направихте промени в профила си.";
                    }
                    else
                    {
                        string connectionString = OftenUsedMethods.ConnectionString;

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            if (personInfo.Name == LoginReaderModel.readerInfo.Name && personInfo.LastName == LoginReaderModel.readerInfo.LastName && personInfo.Email == LoginReaderModel.readerInfo.Email && personInfo.Telephone == LoginReaderModel.readerInfo.Telephone)
                            {
                                string sql = $" UPDATE [dbo].[Reader] SET Password=@password WHERE ID = {LoginReaderModel.readerInfo.Id};";

                                using (SqlCommand command = new SqlCommand(sql, connection))
                                {
                                    command.Parameters.AddWithValue("@password", _hashedPassword);

                                    command.ExecuteNonQuery();
                                }

                                successMessage = "Промени ПАРОЛАТА за своя профил!";

                                LoginReaderModel.readerInfo.Password = personInfo.Password.Trim();
                            }
                            else
                            {
                                bool exist = false;

                                using (SqlCommand cmd = new SqlCommand($"Select count(*) from [dbo].[Reader] where Telephone=@telephone and ID != {LoginReaderModel.readerInfo.Id} ;", connection))
                                {
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@telephone", personInfo.Telephone.Trim());
                                    exist = (int)cmd.ExecuteScalar() > 0;

                                    if (exist == true)
                                        errorMessage = "Този телефон е на друг читател!";
                                    else
                                    {
                                        bool exist2 = false;
                                        using (SqlCommand cmd2 = new SqlCommand($"Select count(*) from [dbo].[Reader] where Email=@email and ID != {LoginReaderModel.readerInfo.Id} ;", connection))
                                        {
                                            cmd2.CommandType = CommandType.Text;
                                            cmd2.Parameters.AddWithValue("@email", personInfo.Email.Trim());
                                            exist2 = (int)cmd2.ExecuteScalar() > 0;

                                            if (exist2 == true)
                                                errorMessage = "Този имейл е на друг читател!";
                                            else
                                            {
                                                string sql = $" UPDATE [dbo].[Reader] SET Name=@name,LastName=@lastName,Email=@email,Telephone=@telephone,Password=@password WHERE ID = {LoginReaderModel.readerInfo.Id};";

                                                using (SqlCommand command = new SqlCommand(sql, connection))
                                                {
                                                    command.Parameters.AddWithValue("@name", personInfo.Name.Trim());
                                                    command.Parameters.AddWithValue("@lastName", personInfo.LastName.Trim());
                                                    command.Parameters.AddWithValue("@email", personInfo.Email.Trim());
                                                    command.Parameters.AddWithValue("@telephone", personInfo.Telephone.Trim());
                                                    command.Parameters.AddWithValue("@password", _hashedPassword);

                                                    command.ExecuteNonQuery();
                                                }
                                                successMessage = "Промени информацията за своя профил!";

                                                _hashedPassword = "";
                                                LoginReaderModel.readerInfo.Name = personInfo.Name.Trim();
                                                LoginReaderModel.readerInfo.LastName = personInfo.LastName.Trim();
                                                LoginReaderModel.readerInfo.Email = personInfo.Email.Trim();
                                                LoginReaderModel.readerInfo.Telephone = personInfo.Telephone.Trim();
                                                LoginReaderModel.readerInfo.Password = personInfo.Password.Trim();
                                            }
                                        }
                                    }
                                }
                            }
                            connection.Close();
                        }
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