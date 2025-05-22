using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Library.Pages
{
    public class ReaderInfoForAdminModel : PageModel
    {
        public static Reader readerInfo = new Reader();
        private string _name;
        private string _lastName;
        private string _email;
        private string _telephone;

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

                    string query = "SELECT Name,LastName,Email,Telephone,DateOfCreation from [dbo].[Reader] where ID=@id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime date;

                                readerInfo.Id = id;
                                _name = reader.GetString(0);
                                readerInfo.Name = _name;                                
                                _lastName = reader.GetString(1);
                                readerInfo.LastName = _lastName;
                                _email = reader.GetString(2);
                                readerInfo.Email = _email;
                                _telephone = reader.GetString(3);
                                readerInfo.Telephone = _telephone; 
                                date = reader.GetDateTime(4);
                                readerInfo.DateOfCreation = date.ToString();
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
            Reader reader = new Reader();

            string id = Request.Query["id"];

            reader.Name = Request.Form["name"];
            reader.LastName = Request.Form["lastName"];
            reader.Email = Request.Form["email"];
            reader.Telephone = Request.Form["phone"];

            if (reader.Name.Length == 0 || reader.LastName.Length == 0 ||
                reader.Email.Length == 0 || reader.Telephone.Length == 0 )
            {
                errorMessage = "Всички полета са задължителни!";
                return;
            }
            else
            {
                reader.Email = reader.Email.Trim();

                string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
                Regex regex = new Regex(pattern);

                if (OftenUsedMethods.CorrectNameInput(reader.Name) == "Грешка")
                {
                    errorMessage = "Името трябва да е една дума.";
                }
                else if (OftenUsedMethods.CorrectNameInput(reader.LastName) == "Грешка")
                {
                    errorMessage = "Фамилията трябва да е една дума.";
                }
                else if (regex.IsMatch(reader.Email) == false)
                {
                    errorMessage = "Този имейл не е валиден!";
                }
                else if (OftenUsedMethods.CorrectPhone(reader.Telephone) == "Грешка")
                {
                    errorMessage = "Телефонът трябва да започва с 0 и да не е повече от 10 цифри!";
                }

                if (errorMessage.Length > 0)
                {
                    return;
                }
                else
                {
                    reader.Name = OftenUsedMethods.CorrectNameInput(reader.Name);
                    reader.LastName = OftenUsedMethods.CorrectNameInput(reader.LastName);
                    reader.Telephone = OftenUsedMethods.CorrectPhone(reader.Telephone);

                    if (reader.Name == readerInfo.Name && reader.LastName == readerInfo.LastName && 
                        reader.Email == readerInfo.Email && reader.Telephone == readerInfo.Telephone )
                    {
                        successMessage = "Не промени информацията за профила.";
                    }
                    else
                    {
                        try
                        {
                            string connectionStrong = OftenUsedMethods.ConnectionString;
                            using (SqlConnection connection = new SqlConnection(connectionStrong))
                            {
                                connection.Open();

                                if (reader.Email != readerInfo.Email)
                                {
                                    int count = 0;
                                    string query1 = "SELECT count(*) from [dbo].[Reader] where Email=@email;";
                                    using (SqlCommand command1 = new SqlCommand(query1, connection))
                                    {
                                        command1.Parameters.AddWithValue("@email", reader.Email);
                                        count = (int)command1.ExecuteScalar();
                                    }
                                    if (count > 0)
                                    {
                                        errorMessage = "Този имейл вече е използван!";
                                        return;
                                    }
                                }
                                else if (reader.Telephone != readerInfo.Telephone)
                                {
                                    int count = 0;
                                    string query2 = "SELECT count(*) from [dbo].[Reader] where Telephone=@phone;";
                                    using (SqlCommand command2 = new SqlCommand(query2, connection))
                                    {
                                        command2.Parameters.AddWithValue("@phone", reader.Telephone);
                                        count = (int)command2.ExecuteScalar();
                                    }
                                    if (count > 0)
                                    {
                                        errorMessage = "Този телефон вече е използван!";
                                        return;
                                    }
                                }
                                
                                if (errorMessage.Length > 0)
                                {
                                    readerInfo.Name = _name;
                                    readerInfo.LastName = _lastName;
                                    readerInfo.Email = _email;
                                    readerInfo.Telephone = _telephone;
                                    return;
                                }
                                else
                                {
                                    string query4 = $"UPDATE [dbo].[Reader] SET Name=@name,LastName=@lastName,Email=@email,Telephone=@phone WHERE ID=@id ";

                                    using (SqlCommand command4 = new SqlCommand(query4, connection))
                                    {
                                        command4.Parameters.AddWithValue("@id", id);
                                        command4.Parameters.AddWithValue("@name", reader.Name);
                                        command4.Parameters.AddWithValue("@lastName", reader.LastName);
                                        command4.Parameters.AddWithValue("@email", reader.Email);
                                        command4.Parameters.AddWithValue("@phone", reader.Telephone);

                                        command4.ExecuteNonQuery();
                                    }
                                    successMessage = "Обнови информацията за този профил.";

                                    readerInfo.Name = reader.Name;
                                    readerInfo.LastName = reader.LastName;
                                    readerInfo.Email = reader.Email;
                                    readerInfo.Telephone = reader.Telephone;
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
        }
    }
}
