using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Net.Mail;

namespace Library.Pages
{
    public class ForgottenPasswordModel : PageModel
    {
        public string errorMessage = "";

        public static string id;
        public static string database;

        public static int verificationCode;
        public static DateTime startTime;

        public void OnGet()
        {

        }
        public void OnPost()
        {
            string email = Request.Form["email"];
            email = email.Trim();

            if (email.Length == 0)
            {
                errorMessage = "Полето е задължително!";
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

                        bool exists = false;

                        SqlCommand cmd = new SqlCommand("Select count(*) from [dbo].[Library] where Email=@email", connection);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@email", email);

                        exists = (int)cmd.ExecuteScalar() > 0;

                        if (exists == true)
                        {
                            string command = "SELECT ID FROM [dbo].[Library] where Email=@email";
                            SqlCommand cmd2 = new SqlCommand(command, connection);
                            cmd2.CommandType = CommandType.Text;
                            cmd2.Parameters.AddWithValue("@email", email);
                            
                            id = cmd2.ExecuteScalar().ToString();
                            database = "Library";

                            Response.Redirect("/ForgottenPassword2");
                        }
                        else
                        {
                            SqlCommand cmd3 = new SqlCommand("Select count(*) from [dbo].[Reader] where Email=@email", connection);
                            cmd3.CommandType = CommandType.Text;
                            cmd3.Parameters.AddWithValue("@email", email);

                            exists = (int)cmd3.ExecuteScalar() > 0;

                            if (exists == true)
                            {
                                string command = "SELECT ID FROM [dbo].[Reader] where Email=@email";
                                SqlCommand cmd4 = new SqlCommand(command, connection);
                                cmd4.CommandType = CommandType.Text;
                                cmd4.Parameters.AddWithValue("@email", email);

                                id = cmd4.ExecuteScalar().ToString();
                                database = "Reader";

                                Response.Redirect("/ForgottenPassword2");
                            }
                            else
                            {
                                SqlCommand cmd5 = new SqlCommand("Select count(*) from [dbo].[Administrator] where Email=@email", connection);
                                cmd5.CommandType = CommandType.Text;
                                cmd5.Parameters.AddWithValue("@email", email);

                                exists = (int)cmd5.ExecuteScalar() > 0;

                                if (exists == true)
                                {
                                    string command = "SELECT ID FROM [dbo].[Administrator] where Email=@email";
                                    SqlCommand cmd6 = new SqlCommand(command, connection);
                                    cmd6.CommandType = CommandType.Text;
                                    cmd6.Parameters.AddWithValue("@email", email);

                                    id = cmd6.ExecuteScalar().ToString();
                                    database = "Administrators";

                                    Response.Redirect("/ForgottenPassword2");
                                }
                                else
                                {
                                    errorMessage = "Профилът не е намерен!";
                                }
                            }                         
                        }

                        if (errorMessage.Length == 0)
                        {
                            var random = new Random();
                            int first = random.Next(100, 1392);
                            int second = random.Next(3827, 5925);
                            int third = random.Next(35750, 92501);

                            verificationCode = (second + third) - first;
                            startTime = DateTime.Now;

                            string fromMail = "atanas23system@gmail.com";
                            string fromPassword = "yberatwebtymbxzi";

                            MailMessage message = new MailMessage();
                            message.From = new MailAddress(fromMail);
                            message.Subject = "Смяна на парола.";
                            message.To.Add(new MailAddress(email));
                            message.Body = $"<html><body> Твоят код за потвърждение е: {verificationCode} </body></html>";
                            message.IsBodyHtml = true;

                            var smtpClient = new SmtpClient("smtp.gmail.com")
                            {
                                Port = 587,
                                Credentials = new NetworkCredential(fromMail, fromPassword),
                                EnableSsl = true,
                            };

                            smtpClient.Send(message);
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
