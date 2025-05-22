using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace Library.Pages
{
    public class AddLibraryModel : PageModel
    {
        public static LibraryInformation libraryInfo = new LibraryInformation();

        public string errorMessage = "";
        public string successMessage = "";

        private string _confirmPassword;
        private string _hashedPassword;

        public void OnGet()
        {
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
            _confirmPassword = Request.Form["confirmPassword"];

            if (libraryInfo.Name.Length == 0 || libraryInfo.CityVillage.Length == 0 || libraryInfo.Municipality.Length == 0 || 
                libraryInfo.District.Length == 0 || libraryInfo.Email.Length == 0 || libraryInfo.Telephone.Length == 0 || 
                libraryInfo.Address.Length == 0 || libraryInfo.Password.Length == 0 || _confirmPassword.Length == 0) 
            {
                errorMessage = "Всички полета са задължителни!";
                return;
            }
            else
            {
                Regex validate = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[А-Я])(?=.*?[а-я])(?=.*?[#?!@$%^&*-]).{8,}$");
                
                string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
                Regex regex = new Regex(pattern);

                char[] chars = libraryInfo.Name.ToCharArray();
                if (chars[0] != '"' && chars[chars.Length - 1] != '"' || chars[1] == '"')
                {
                    errorMessage = "Името на читалището/библиотеката трябва да е в кавички!";
                    return;
                }
                else if (libraryInfo.Password.Trim() != _confirmPassword.Trim())
                {
                    errorMessage = "Двете пароли не съвпадат!";
                    return;
                }
                else if (OftenUsedMethods.CorrectPhone(libraryInfo.Telephone) == "Грешка")
                {
                    errorMessage = "Телефонният номер трябва да започва с 0 и да не е повече от 10 цифри!";
                }
                else if (validate.IsMatch(libraryInfo.Password.Trim()) == false)
                {
                    errorMessage = "Паролата не отговаря на изискванията!";
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
                        string connectionString = OftenUsedMethods.ConnectionString;
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            bool exists = false;

                            SqlCommand cmd = new SqlCommand("Select count(*) from [dbo].[Library] where UserName=@name and CityVillage=@cityVillage and Municipality=@municipality", connection);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@name", libraryInfo.Name.Trim());
                            cmd.Parameters.AddWithValue("@cityVillage", libraryInfo.CityVillage);
                            cmd.Parameters.AddWithValue("@municipality", libraryInfo.Municipality);

                            exists = (int)cmd.ExecuteScalar() > 0;

                            if (exists == true)
                            {
                                errorMessage = "Този профил вече съществува!";
                                return;
                            }
                            else
                            {
                                SqlCommand cmd2 = new SqlCommand("Select count(*) from [dbo].[Library] where Email=@email", connection);
                                cmd2.Parameters.AddWithValue("@email", libraryInfo.Email);
                                exists = (int)cmd2.ExecuteScalar() > 0;

                                if (exists == true)
                                {
                                    errorMessage = "Този имейл вече е използван!";
                                }
                                else
                                {
                                    SqlCommand cmd3 = new SqlCommand("Select count(*) from [dbo].[Library] where Telephone=@telephone", connection);
                                    cmd3.Parameters.AddWithValue("@telephone", libraryInfo.Telephone);
                                    exists = (int)cmd3.ExecuteScalar() > 0;

                                    if (exists == true)
                                    {
                                        errorMessage = "Този телефон вече е използван!";
                                    }
                                    else
                                    {
                                        string sql = " INSERT INTO [dbo].[Library] " +
                                        "(UserName,CityVillage,Municipality,District,Email,Telephone,Address,Password) VALUES " +
                                        "(@name,@cityVillage,@municipality,@district,@email,@telephone,@address,@password);";

                                        using (SqlCommand command = new SqlCommand(sql, connection))
                                        {
                                            command.Parameters.AddWithValue("@name", libraryInfo.Name.Trim());
                                            command.Parameters.AddWithValue("@cityVillage", libraryInfo.CityVillage);
                                            command.Parameters.AddWithValue("@municipality", libraryInfo.Municipality);
                                            command.Parameters.AddWithValue("@district", libraryInfo.District);
                                            command.Parameters.AddWithValue("@email", libraryInfo.Email);
                                            command.Parameters.AddWithValue("@telephone", libraryInfo.Telephone);
                                            command.Parameters.AddWithValue("@address", libraryInfo.Address.Trim());
                                            command.Parameters.AddWithValue("@password", _hashedPassword);

                                            command.ExecuteNonQuery();
                                        }

                                        successMessage = "Нова библиотека е добавена успешно.";
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

                    libraryInfo.Name = "";
                    libraryInfo.CityVillage = "";
                    libraryInfo.Municipality = "";
                    libraryInfo.District = "";
                    libraryInfo.Email = "";
                    libraryInfo.Telephone = "";
                    libraryInfo.Address = "";
                    libraryInfo.Password = "";
                }            
            }
        }
    }
}