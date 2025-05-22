using Cyrillic.Convert;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Immutable;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;

namespace Library.Pages
{
    public class AdminReaderInfoForBookModel : PageModel
    {
        public static Reader readerInfo = new Reader();

        public string errorMessage = "";
        public static string dateOfTaking = "";

        public void OnGet()
        {
            string id = Request.Query["id"];
            int idReader;
            string idPerson = "";

            try
            {
                string connectionString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT IDReader,DateOfTaking FROM Book WHERE ID=@id";
                    using (SqlCommand command1 = new SqlCommand(query, connection))
                    {
                        command1.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command1.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                idReader = reader.GetInt32(0);
                                idPerson = idReader.ToString();
                                dateOfTaking = reader.GetString(1);
                            }
                        }
                    }

                    string sql = $"SELECT * FROM Reader WHERE ID={idPerson}";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int Id;
                                Id = reader.GetInt32(0);
                                readerInfo.Id = Id.ToString();
                                readerInfo.Name = reader.GetString(1);
                                readerInfo.LastName = reader.GetString(2);
                                readerInfo.Email = reader.GetString(3);
                                readerInfo.Telephone = reader.GetString(4);
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
    }
}
