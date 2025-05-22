using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

namespace Library.Pages
{
    public class AddAdminModel : PageModel
    {
        AdminInformation adminInfo = new AdminInformation();

        public string errorMessage = "";
        public string successMessage = "";

        private int _firstNumber;
        private int _secondNumber;
        private int _thirdNumber;
        private int _fourthNumber;
        private int _fifthNumber;
        private int _sixthNumber;

        private static int _verificationCode;
        private string _hashedVerCode;
        public void OnGet()
        {
        }
        public void OnPost()
        {
            adminInfo.Name = Request.Form["name"];
            adminInfo.Surname = Request.Form["surname"];
            adminInfo.LastName = Request.Form["lastName"];
            adminInfo.Email = Request.Form["email"];
            adminInfo.Telephone = Request.Form["phone"];
            adminInfo.UserName = Request.Form["userName"];
            adminInfo.UserName = adminInfo.UserName.Trim();
            adminInfo.Role = Request.Form["role"];

            if (adminInfo.Name.Length == 0 || adminInfo.Surname.Length == 0 || adminInfo.LastName.Length == 0 ||
                adminInfo.Email.Length == 0 || adminInfo.Telephone.Length == 0 || adminInfo.UserName.Length == 0)
            {
                errorMessage = "Всички полета са задължителни!";
                return;
            }
            else
            {
                string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
                Regex regex = new Regex(pattern);

                if (OftenUsedMethods.CorrectNameInput(adminInfo.Name) == "Грешка")
                {
                    errorMessage = "Името трябва да е една дума.";
                }
                else if (OftenUsedMethods.CorrectNameInput(adminInfo.Surname) == "Грешка")
                {
                    errorMessage = "Презимето трябва да е една дума.";
                }
                else if (OftenUsedMethods.CorrectNameInput(adminInfo.LastName) == "Грешка")
                {
                    errorMessage = "Фамилията трябва да е една дума.";
                }
                else if (regex.IsMatch(adminInfo.Email) == false)
                {
                    errorMessage = "Този имейл не е валиден!";
                }
                else if (OftenUsedMethods.CorrectPhone(adminInfo.Telephone) == "Грешка")
                {
                    errorMessage = "Телефонът трябва да започва с 0 и да не е повече от 10 цифри!";
                }

                if (errorMessage.Length > 0)
                {
                    return;
                }
                else
                {
                    adminInfo.Name = OftenUsedMethods.CorrectNameInput(adminInfo.Name);
                    adminInfo.Surname = OftenUsedMethods.CorrectNameInput(adminInfo.Surname);
                    adminInfo.LastName = OftenUsedMethods.CorrectNameInput(adminInfo.LastName);
                    adminInfo.Telephone = OftenUsedMethods.CorrectPhone(adminInfo.Telephone);

                    try
                    {
                        string connectionString = OftenUsedMethods.ConnectionString;
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            int count = 0;
                            string query1 = "SELECT count(*) from [dbo].[Administrator] where Email=@email;";
                            using (SqlCommand command1 = new SqlCommand(query1, connection))
                            {
                                command1.Parameters.AddWithValue("@email", adminInfo.Email);
                                count = (int)command1.ExecuteScalar();
                            }
                            if (count > 0)
                            {
                                errorMessage = "Този имейл вече е използван!";
                                return;
                            }
                            else
                            {
                                string query2 = "SELECT count(*) from [dbo].[Administrator] where Telephone=@phone;";
                                using (SqlCommand command2 = new SqlCommand(query2, connection))
                                {
                                    command2.Parameters.AddWithValue("@phone", adminInfo.Telephone);
                                    count = (int)command2.ExecuteScalar();
                                }
                                if (count > 0)
                                {
                                    errorMessage = "Този телефон вече е използван!";
                                    return;
                                }
                                else
                                {
                                    string query3 = "SELECT count(*) from [dbo].[Administrator] where UserName=@userName;";
                                    using (SqlCommand command3 = new SqlCommand(query3, connection))
                                    {
                                        command3.Parameters.AddWithValue("@userName", adminInfo.UserName.Trim());
                                        count = (int)command3.ExecuteScalar();
                                    }
                                    if (count > 0)
                                    {
                                        errorMessage = "Това потребителско име вече е използвано!";
                                        return;
                                    }
                                    else
                                    {
                                        var random = new Random();
                                        _firstNumber = random.Next(100000, 290217);
                                        _secondNumber = random.Next(2740, 9241);
                                        _thirdNumber = random.Next(38000, 61682);
                                        _fourthNumber = random.Next(521243, 832192);
                                        _fifthNumber = random.Next(10241, 29836);
                                        _sixthNumber = random.Next(93482, 112836);

                                        _verificationCode = ((_firstNumber * (_thirdNumber - _fifthNumber) + (_fourthNumber * _secondNumber) / _sixthNumber));
                                        
                                        if (_verificationCode < 0)
                                        {
                                            _verificationCode = _verificationCode * -1;
                                        }

                                        _hashedVerCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(_verificationCode.ToString()));

                                        string query4 = "INSERT INTO [dbo].[Administrator] " +
                                                                "(Name,Surname,LastName,Email,Telephone,UserName,Role,IsConfirmed,VerCode,VerCodeDate) " +
                                                                "VALUES (@name,@surname,@lastName,@email,@phone,@userName,@role,@isConfirmed,@verCode,@verCodeDate)";
                                        using (SqlCommand command4 = new SqlCommand(query4, connection))
                                        {
                                            command4.Parameters.AddWithValue("@name", adminInfo.Name);
                                            command4.Parameters.AddWithValue("@surname", adminInfo.Surname);
                                            command4.Parameters.AddWithValue("@lastName", adminInfo.LastName);
                                            command4.Parameters.AddWithValue("@email", adminInfo.Email);
                                            command4.Parameters.AddWithValue("@phone", adminInfo.Telephone);
                                            command4.Parameters.AddWithValue("@userName", adminInfo.UserName.Trim());
                                            command4.Parameters.AddWithValue("@role", adminInfo.Role);
                                            command4.Parameters.AddWithValue("@isConfirmed", false);
                                            command4.Parameters.AddWithValue("@verCode", _hashedVerCode);
                                            command4.Parameters.AddWithValue("@verCodeDate", DateTime.Now);

                                            command4.ExecuteNonQuery();
                                        }
                                        successMessage = "Добави нов администратор.";
                                    }
                                }
                            }
                            connection.Close();

                            string fromMail = "atanas23system@gmail.com";
                            string fromPassword = "yberatwebtymbxzi";

                            MailMessage message = new MailMessage();
                            message.From = new MailAddress(fromMail);
                            message.Subject = "Код за активация на профил.";
                            message.To.Add(new MailAddress(adminInfo.Email));
                            message.Body = $"<html><body> Твоят код за активиране на профила е: {_verificationCode} </body></html>";
                            message.IsBodyHtml = true;

                            var smtpClient = new SmtpClient("smtp.gmail.com")
                            {
                                Port = 587,
                                Credentials = new NetworkCredential(fromMail, fromPassword),
                                EnableSsl = true,
                            };

                            smtpClient.Send(message);
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