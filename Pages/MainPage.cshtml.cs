using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace Library.Pages
{
    public class MainPageModel : PageModel
    {
        public static List<BookInformation> listBooks = new List<BookInformation>();
        public static List<BookInformation> listDates = new List<BookInformation>();
        private string _email = "";

        public string errorMessage = "";

        public void OnGet()
        {
            listBooks.Clear();
            listDates.Clear();

            try
            {
                string connectionString = OftenUsedMethods.ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query1 = $"SELECT DateOfTaking from [dbo].[Book] WHERE IDLibrary={LoginLibraryModel.libraryInfo.Id} and IsAvaiable='НЕ';";
                    using (SqlCommand command1 = new SqlCommand(query1, connection))
                    {
                        using (SqlDataReader reader = command1.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                BookInformation bookDateOfTaking = new BookInformation();
                                
                                bookDateOfTaking.DateOfTaking = reader.GetString(0);

                                TimeSpan days = DateTime.Now - Convert.ToDateTime(bookDateOfTaking.DateOfTaking);

                                listDates.Add(bookDateOfTaking);
                            }
                        }
                    }

                    int counterBooks = 0;
                    TimeSpan ts;
                    for (int i = 0; i < listDates.Count; i++)
                    {
                        ts = DateTime.Now - Convert.ToDateTime(listDates[i].DateOfTaking);
                        if (ts.TotalDays > 30)
                        {
                            counterBooks++;
                        }
                    }

                    if (counterBooks > 0) 
                    {
                        errorMessage = $"Има {counterBooks} книги, които са взети преди повече от 30 дни!";
                    }
                    

                    int count = 0;
                    string query2 = "SELECT count(*) FROM [dbo].[Book] WHERE IsAvaiable='НЕ';";
                    using (SqlCommand command2 = new SqlCommand(query2, connection))
                    {
                        count = (int)command2.ExecuteScalar();
                    }

                    if (count > 0)
                    {
                        string query3 = $"SELECT ID,Title,Author,IDReader,DateOfTaking,SentEmail FROM [dbo].[Book] WHERE IDLibrary={LoginLibraryModel.libraryInfo.Id} and IsAvaiable='НЕ' and SentEmail != 2;";
                        using (SqlCommand command3 = new SqlCommand(query3, connection))
                        {
                            using (SqlDataReader reader = command3.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    BookInformation book = new BookInformation();

                                    book.Id = reader.GetInt32(0).ToString();
                                    book.Title = reader.GetString(1);
                                    book.Author = reader.GetString(2);
                                    book.IdReader = reader.GetInt32(3).ToString();
                                    book.DateOfTaking = reader.GetString(4);
                                    book.SentEmail = reader.GetString(5);

                                    listBooks.Add(book);
                                }
                            }

                            for (int i = 0; i < listBooks.Count; i++)
                            {
                                string query4 = $"SELECT Email FROM [dbo].[Reader] WHERE ID={listBooks[i].IdReader};";
                                using (SqlCommand command4 = new SqlCommand(query4, connection))
                                {
                                    _email = command4.ExecuteScalar().ToString();
                                }

                                TimeSpan days = DateTime.Now - Convert.ToDateTime(listBooks[i].DateOfTaking);
                                DateTime deadline = Convert.ToDateTime(listBooks[i].DateOfTaking).AddDays(30);


                                string fromMail = "atanas23system@gmail.com";
                                string fromPassword = "yberatwebtymbxzi";

                                MailMessage message = new MailMessage();
                                message.From = new MailAddress(fromMail);
                                message.Subject = "Взета книга.";
                                message.To.Add(new MailAddress(_email));

                                if (days.Days > 30 && listBooks[i].SentEmail == "1")
                                {
                                    message.Body = $"<html><body> Взел си книгата {listBooks[i].Title} от {listBooks[i].Author} " +
                                        $"на {listBooks[i].DateOfTaking}.Изминали са повече от 30 дни." +
                                        $"Трябва да я върнеш. </body></html>";

                                    string query5 = $"UPDATE [dbo].[Book] SET SentEmail = @sentEmail WHERE (ID = @id);";
                                    using (SqlCommand command5 = new SqlCommand(query5, connection))
                                    {
                                        command5.Parameters.AddWithValue("@sentEmail", 2);
                                        command5.Parameters.AddWithValue("@id", listBooks[i].Id);

                                        command5.ExecuteNonQuery();
                                    }
                                }
                                else if (days.Days > 14 && listBooks[i].SentEmail == "0")
                                {
                                    message.Body = $"<html><body> Взел си книгата {listBooks[i].Title} от {listBooks[i].Author} " +
                                            $"на {listBooks[i].DateOfTaking}." +
                                            $"Трябва да я върнеш до {deadline}. </body></html>";


                                    string query5 = $"UPDATE [dbo].[Book] SET SentEmail = @sentEmail WHERE (ID = @id);";
                                    using (SqlCommand command5 = new SqlCommand(query5, connection))
                                    {
                                        command5.Parameters.AddWithValue("@sentEmail", 1);
                                        command5.Parameters.AddWithValue("@id", listBooks[i].Id);

                                        command5.ExecuteNonQuery();
                                    }
                                }

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

                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}
