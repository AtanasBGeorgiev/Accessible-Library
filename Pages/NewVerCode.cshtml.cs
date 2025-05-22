using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;

namespace Library.Pages
{
    public class NewVerCodeModel : PageModel
    {
        private string _email;

        public string errorMessage = "";
        public string successMessage = "";

        private int _firstNumber;
        private int _secondNumber;
        private int _thirdNumber;
        private int _fourthNumber;
        private int _fifthNumber;
        private int _sixthNumber;

        private int _idAdmin;
        private static int _verificationCode;
        private string _hashedVerCode;

        public void OnGet()
        {
        }
        public void OnPost()
        {
            _email = Request.Form["email"];
            _email = _email.Trim();

            if (_email.Length == 0)
            {
                errorMessage = "Полето Email е задължително!";
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

                        int count = 0;
                        string query1 = "SELECT count(*) from [dbo].[Administrator] where Email=@email and IsConfirmed='False';";
                        using (SqlCommand command1 = new SqlCommand(query1, connection))
                        {
                            command1.Parameters.AddWithValue("@email", _email);
                            count = (int)command1.ExecuteScalar();
                        }

                        if (count > 0)
                        {
                            string query2 = "SELECT ID FROM [dbo].[Administrator] WHERE Email=@email;";
                            using (SqlCommand command2 = new SqlCommand(query2, connection))
                            {
                                command2.Parameters.AddWithValue("@email", _email);
                                _idAdmin = (int)command2.ExecuteScalar();

                                command2.ExecuteNonQuery();
                            }

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
                            
                            string query3 = $"UPDATE [dbo].[Administrator] SET VerCode = @verCode, VerCodeDate=@verCodeDate WHERE (ID = @id);";
                            using (SqlCommand command3 = new SqlCommand(query3, connection))
                            {
                                command3.Parameters.AddWithValue("@verCode", _hashedVerCode);
                                command3.Parameters.AddWithValue("@verCodeDate", DateTime.Now);
                                command3.Parameters.AddWithValue("@id", _idAdmin);

                                command3.ExecuteNonQuery();
                            }

                            string fromMail = "atanas23system@gmail.com";
                            string fromPassword = "yberatwebtymbxzi";

                            MailMessage message = new MailMessage();
                            message.From = new MailAddress(fromMail);
                            message.Subject = "Код за активация на профил.";
                            message.To.Add(new MailAddress(_email));
                            message.Body = $"<html><body> Твоят код за активиране на профила е: {_verificationCode} </body></html>";
                            message.IsBodyHtml = true;

                            var smtpClient = new SmtpClient("smtp.gmail.com")
                            {
                                Port = 587,
                                Credentials = new NetworkCredential(fromMail, fromPassword),
                                EnableSsl = true,
                            };

                            smtpClient.Send(message);

                            successMessage = "Генерира нов код.";
                        }
                        else
                        {
                            errorMessage = "Този администратор или не съществува,или вече е потвърден!";
                            return;
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