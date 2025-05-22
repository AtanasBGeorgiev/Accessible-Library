using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Library.Pages
{
    public class LibraryProfileInformationModel : PageModel
    {
        public static LibraryInformation libraryInfo = new LibraryInformation();

        public string errorMessage = "";
        public string successMessage = "";
        public static double booksValue=0;
        private string _hashedPassword;

        public void OnGet()
        {
            booksValue = 0;

            string connectionString = OftenUsedMethods.ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = $"SELECT Price from [dbo].[Book] where IDLibrary={LoginLibraryModel.libraryInfo.Id} and Deduction='0';";
                using (SqlCommand command = new SqlCommand(query, connection))  
                {
                    using (SqlDataReader reader = command.ExecuteReader()) 
                    {
                        while (reader.Read()) 
                        {
                            booksValue += reader.GetDouble(0);
                        }
                    }
                }
            }

        }
        public void OnPost()
        {
            libraryInfo.Name = Request.Form["name"];
            libraryInfo.CityVillage = Request.Form["cityVillage"];
            libraryInfo.Municipality = Request.Form["municipality"];
            libraryInfo.District = Request.Form["district"];
            libraryInfo.Email = Request.Form["email"];
            libraryInfo.Telephone = Request.Form["telephone"];
            libraryInfo.Address = Request.Form["address"];
            libraryInfo.Password = Request.Form["password"];
            libraryInfo.Id = LoginLibraryModel.libraryInfo.Id;

            if (libraryInfo.Name.Length == 0 || libraryInfo.CityVillage.Length == 0 || libraryInfo.Municipality.Length == 0 || libraryInfo.District.Length == 0 || libraryInfo.Email.Length == 0 || libraryInfo.Telephone.Length == 0 || libraryInfo.Address.Length == 0 || libraryInfo.Password.Length == 0)
            {
                errorMessage = "Всички полета са задължителни!";
            }
            else
            {
                string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
                Regex regex = new Regex(pattern);

                Regex validate = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[А-Я])(?=.*?[а-я])(?=.*?[#?!@$%^&*-]).{8,}$");

                char[] chars = libraryInfo.Name.ToCharArray();
                if (chars[0] != '"' && chars[chars.Length - 1] != '"' || chars[1] == '"')
                {
                    errorMessage = "Името на читалището/библиотеката трябва да е в кавички!";
                    return;
                }
                else if (OftenUsedMethods.CorrectPhone(libraryInfo.Telephone) == "Грешка")
                {
                    errorMessage = "Телефонният номер трябва да започва с 0 и да не е повече от 10 цифри.";
                }
                else if (validate.IsMatch(libraryInfo.Password) == false)
                {
                    errorMessage = "Паролата не отговаря на изискванията!";
                    return;
                }
                else if (regex.IsMatch(libraryInfo.Email) == false)
                {
                    errorMessage = "Този имейл не е валиден!";
                    return;
                }
                else if (regex.IsMatch(libraryInfo.Email.Trim()) == false)
                {
                    errorMessage = "Този имейл не е валиден!";
                    return;
                }
                else if (OftenUsedMethods.CorrectCityAndMunicipalityInput(libraryInfo.CityVillage) == "" ||
                    OftenUsedMethods.CorrectCityAndMunicipalityInput(libraryInfo.Municipality) == "")
                {
                    errorMessage = "Градът и общината трябва да са думи!";
                    return;
                }
                else if (libraryInfo.Address.Trim().Length == 0)
                {
                    errorMessage = "Адресът трябва да е с думи!";
                    return;
                }
                else
                {
                    libraryInfo.Telephone = OftenUsedMethods.CorrectPhone(libraryInfo.Telephone);
                    libraryInfo.CityVillage = OftenUsedMethods.CorrectCityAndMunicipalityInput(libraryInfo.CityVillage);
                    libraryInfo.Municipality = OftenUsedMethods.CorrectCityAndMunicipalityInput(libraryInfo.Municipality);
                    _hashedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(libraryInfo.Password.Trim()));

                    try
                    {
                        if (libraryInfo.Name == LoginLibraryModel.libraryInfo.Name && libraryInfo.CityVillage == LoginLibraryModel.libraryInfo.CityVillage && 
                            libraryInfo.Municipality == LoginLibraryModel.libraryInfo.Municipality && libraryInfo.District == LoginLibraryModel.libraryInfo.District && 
                            libraryInfo.Address == LoginLibraryModel.libraryInfo.Address && libraryInfo.Email == LoginLibraryModel.libraryInfo.Email && 
                            libraryInfo.Telephone == LoginLibraryModel.libraryInfo.Telephone && libraryInfo.Password == LoginLibraryModel.libraryInfo.Password) 
                        {
                            successMessage = "Не извърши промени в профила си.";
                        }
                        else
                        {
                            string connectionString = OftenUsedMethods.ConnectionString;
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();

                                if (libraryInfo.Name == LoginLibraryModel.libraryInfo.Name && libraryInfo.CityVillage == LoginLibraryModel.libraryInfo.CityVillage && libraryInfo.Municipality == LoginLibraryModel.libraryInfo.Municipality && libraryInfo.District == LoginLibraryModel.libraryInfo.District && libraryInfo.Email == LoginLibraryModel.libraryInfo.Email && libraryInfo.Telephone == LoginLibraryModel.libraryInfo.Telephone && libraryInfo.Address == LoginLibraryModel.libraryInfo.Address) 
                                {
                                    string sql = " UPDATE [dbo].[Library] SET Password=@password WHERE (ID = @id);";

                                    using (SqlCommand command = new SqlCommand(sql, connection))
                                    {
                                        command.Parameters.AddWithValue("@id", libraryInfo.Id);
                                        command.Parameters.AddWithValue("@password", _hashedPassword);

                                        command.ExecuteNonQuery();
                                    }

                                    successMessage = "Промени ПАРОЛАТА на своя профил!";

                                    LoginLibraryModel.libraryInfo.Password = libraryInfo.Password;
                                }
                                else
                                {
                                    bool exists = false;

                                    string sql = "Select count(*) from [dbo].[Library] where UserName=@name and CityVillage=@cityVillage and Municipality=@municipality and ID!=@id;";

                                    using (SqlCommand command = new SqlCommand(sql, connection))
                                    {
                                        command.Parameters.AddWithValue("@id", libraryInfo.Id);
                                        command.Parameters.AddWithValue("@name", libraryInfo.Name.Trim());
                                        command.Parameters.AddWithValue("@cityVillage", libraryInfo.CityVillage.Trim());
                                        command.Parameters.AddWithValue("@municipality", libraryInfo.Municipality.Trim());

                                        command.ExecuteNonQuery();
                                        exists = (int)command.ExecuteScalar() > 0;
                                    }

                                    if (exists == true)
                                    {
                                        errorMessage = "В това населено място вече съществува библиотека с това име!";

                                        libraryInfo.Name = $"{LoginLibraryModel.libraryInfo.Name}";
                                        libraryInfo.CityVillage = $"{LoginLibraryModel.libraryInfo.CityVillage}";
                                        libraryInfo.Municipality = $"{LoginLibraryModel.libraryInfo.Municipality}";
                                        libraryInfo.District = $"{LoginLibraryModel.libraryInfo.District}";
                                        libraryInfo.Email = $"{LoginLibraryModel.libraryInfo.Email}";
                                        libraryInfo.Telephone = $"{LoginLibraryModel.libraryInfo.Telephone}";
                                        libraryInfo.Address = $"{LoginLibraryModel.libraryInfo.Address}";
                                        libraryInfo.Password = $"{LoginLibraryModel.libraryInfo.Password}";
                                    }
                                    else
                                    {
                                        bool exists2 = false;

                                        SqlCommand cmd2 = new SqlCommand("Select count(*) from [dbo].[Library] where Email=@email and ID!=@id;", connection);
                                        cmd2.CommandType = CommandType.Text;
                                        cmd2.Parameters.AddWithValue("@id", libraryInfo.Id);
                                        cmd2.Parameters.AddWithValue("@email", libraryInfo.Email.Trim());

                                        exists2 = (int)cmd2.ExecuteScalar() > 0;

                                        if (exists2 == true)
                                        {
                                            errorMessage = "Този имейл вече е зает!";
                                        }
                                        else
                                        {
                                            bool exists3 = false;

                                            SqlCommand cmd3 = new SqlCommand("Select count(*) from [dbo].[Library] where Telephone=@telephone and ID!=@id;", connection);
                                            cmd3.CommandType = CommandType.Text;
                                            cmd3.Parameters.AddWithValue("@id", libraryInfo.Id);
                                            cmd3.Parameters.AddWithValue("@telephone", libraryInfo.Telephone.Trim());

                                            exists3 = (int)cmd3.ExecuteScalar() > 0;

                                            if (exists3 == true)
                                            {
                                                errorMessage = "Този телефон вече е зает!";
                                            }
                                            else
                                            {
                                                string query = " UPDATE [dbo].[Library] SET UserName=@name,CityVillage=@cityVillage,Municipality=@municipality,District=@district,Email=@email,Telephone=@telephone,Address=@address,Password=@password WHERE (ID = @id);";

                                                using (SqlCommand command = new SqlCommand(query, connection))
                                                {
                                                    command.Parameters.AddWithValue("@id", libraryInfo.Id);
                                                    command.Parameters.AddWithValue("@name", libraryInfo.Name.Trim());
                                                    command.Parameters.AddWithValue("@cityVillage", libraryInfo.CityVillage.Trim());
                                                    command.Parameters.AddWithValue("@municipality", libraryInfo.Municipality.Trim());
                                                    command.Parameters.AddWithValue("@district", libraryInfo.District.Trim());
                                                    command.Parameters.AddWithValue("@email", libraryInfo.Email.Trim());
                                                    command.Parameters.AddWithValue("@telephone", libraryInfo.Telephone);
                                                    command.Parameters.AddWithValue("@address", libraryInfo.Address);
                                                    command.Parameters.AddWithValue("@password", _hashedPassword);

                                                    command.ExecuteNonQuery();
                                                }

                                                successMessage = "Промени информацията за своя профил!";

                                                _hashedPassword = "";
                                                LoginLibraryModel.libraryInfo.Name = libraryInfo.Name;
                                                LoginLibraryModel.libraryInfo.CityVillage = libraryInfo.CityVillage;
                                                LoginLibraryModel.libraryInfo.Municipality = libraryInfo.Municipality;
                                                LoginLibraryModel.libraryInfo.District = libraryInfo.District;
                                                LoginLibraryModel.libraryInfo.Email = libraryInfo.Email;
                                                LoginLibraryModel.libraryInfo.Telephone = libraryInfo.Telephone;
                                                LoginLibraryModel.libraryInfo.Address = libraryInfo.Address;
                                                LoginLibraryModel.libraryInfo.Password = libraryInfo.Password;
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
}
