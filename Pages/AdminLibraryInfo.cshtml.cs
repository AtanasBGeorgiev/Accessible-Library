using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Library.Pages
{
    public class AdminLibraryInfoModel : PageModel
    {
        public static LibraryInformation libraryInfo = new LibraryInformation();

        public string errorMessage = "";
        public string successMessage = "";

        private string _userName;
        private string _cityVillage;
        private string _municipality;
        private string _district;
        private string _email;
        private string _telephone;
        private string _address;

        public void OnGet()
        {
            string id = Request.Query["id"];

            try
            {
                string connectionString = OftenUsedMethods.ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT UserName,CityVillage,Municipality,District,Email,Telephone,Address,DateOfCreation from [dbo].[Library] WHERE ID=@id;";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime date;

                                libraryInfo.Id = id;
                                _userName = reader.GetString(0);
                                libraryInfo.Name = _userName;
                                _cityVillage = reader.GetString(1);
                                libraryInfo.CityVillage = _cityVillage;
                                _municipality = reader.GetString(2);
                                libraryInfo.Municipality = _municipality;
                                _district = reader.GetString(3);
                                libraryInfo.District = _district;
                                _email = reader.GetString(4);
                                libraryInfo.Email = _email;
                                _telephone = reader.GetString(5);
                                libraryInfo.Telephone = _telephone;
                                _address = reader.GetString(6);
                                libraryInfo.Address = _address;
                                date = reader.GetDateTime(7);
                                libraryInfo.DateOfCreation = date.ToString();
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
            LibraryInformation library = new LibraryInformation();

            string id = Request.Query["id"];

            library.Name = Request.Form["name"];
            library.CityVillage = Request.Form["cityVillage"];
            library.Municipality = Request.Form["municipality"];
            library.District = Request.Form["district"];
            library.Email = Request.Form["email"];
            library.Telephone = Request.Form["telephone"];
            library.Address = Request.Form["address"];

            if (library.Name.Length == 0 || library.CityVillage.Trim().Length == 0 || 
                library.Municipality.Trim().Length == 0 || library.District.Length == 0 || 
                library.Email.Trim().Length == 0 || library.Telephone.Trim().Length == 0 ||
                library.Address.Trim().Length == 0)
            {
                errorMessage = "Всички полета са задължителни!";
                return;
            }
            else
            {
                library.Name = library.Name.Trim();
                library.CityVillage = library.CityVillage.Trim();
                library.Municipality = library.Municipality.Trim();
                library.District = library.District.Trim();
                library.Email = library.Email.Trim();
                library.Telephone = library.Telephone.Trim();
                library.Address = library.Address.Trim();

                string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
                Regex regex = new Regex(pattern);

                char[] chars = library.Name.ToCharArray();
                if (chars[0] != '"' && chars[chars.Length - 1] != '"' || chars[1] == '"' )
                {
                    errorMessage = "Името на читалището/библиотеката трябва да е в кавички!";
                    return;
                }
                else if (OftenUsedMethods.CorrectPhone(library.Telephone) == "Грешка")
                {
                    errorMessage = "Телефонният номер трябва да започва с 0 и да не е повече от 10 цифри.";
                }
                else if (regex.IsMatch(library.Email) == false)
                {
                    errorMessage = "Този имейл не е валиден!";
                    return;
                }
                else
                {
                    library.Telephone = OftenUsedMethods.CorrectPhone(library.Telephone);
                    library.CityVillage = OftenUsedMethods.CorrectCityAndMunicipalityInput(library.CityVillage);
                    library.Municipality = OftenUsedMethods.CorrectCityAndMunicipalityInput(library.Municipality);

                    if (library.Name == libraryInfo.Name && library.CityVillage == libraryInfo.CityVillage &&
                        library.Municipality == libraryInfo.Municipality && library.District == libraryInfo.District
                        && library.Email == libraryInfo.Email && library.Telephone == libraryInfo.Telephone &&
                        library.Address == libraryInfo.Address)
                    {
                        successMessage = "Не промени информацията за профила.";
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

                                if (library.Name != libraryInfo.Name || library.CityVillage != libraryInfo.CityVillage || library.Municipality != libraryInfo.Municipality)
                                {
                                    SqlCommand cmd = new SqlCommand("Select count(*) from [dbo].[Library] where UserName=@name and CityVillage=@cityVillage and Municipality=@municipality", connection);
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@name", library.Name);
                                    cmd.Parameters.AddWithValue("@cityVillage", library.CityVillage);
                                    cmd.Parameters.AddWithValue("@municipality", library.Municipality);

                                    exists = (int)cmd.ExecuteScalar() > 0;

                                    if (exists == true)
                                    {
                                        errorMessage = "Този профил вече съществува!";
                                        return;
                                    }
                                }
                                else if (library.Email != libraryInfo.Email)
                                {
                                    SqlCommand cmd2 = new SqlCommand("Select count(*) from [dbo].[Library] where Email=@email", connection);
                                    cmd2.Parameters.AddWithValue("@email", library.Email);
                                    exists = (int)cmd2.ExecuteScalar() > 0;

                                    if (exists == true)
                                    {
                                        errorMessage = "Този имейл вече е използван!";
                                        return;
                                    }
                                }
                                else if (library.Telephone != libraryInfo.Telephone)
                                {
                                    SqlCommand cmd3 = new SqlCommand("Select count(*) from [dbo].[Library] where Telephone=@telephone", connection);
                                    cmd3.Parameters.AddWithValue("@telephone", library.Telephone);
                                    exists = (int)cmd3.ExecuteScalar() > 0;

                                    if (exists == true)
                                    {
                                        errorMessage = "Този телефон вече е използван!";
                                        return;
                                    }
                                }

                                if (errorMessage.Length > 0)
                                {
                                    libraryInfo.Name = _userName;
                                    libraryInfo.CityVillage = _cityVillage;
                                    libraryInfo.Municipality = _municipality;
                                    libraryInfo.District = _district;
                                    libraryInfo.Email = _email;
                                    libraryInfo.Telephone = _telephone;
                                    libraryInfo.Address = _address;
                                    return;
                                }
                                else
                                {
                                    string sql = " UPDATE [dbo].[Library] SET UserName=@name,CityVillage=@cityVillage,Municipality=@municipality,District=@district,Email=@email,Telephone=@telephone,Address=@address WHERE ID=@id;";

                                    using (SqlCommand command = new SqlCommand(sql, connection))
                                    {
                                        command.Parameters.AddWithValue("@id", id);
                                        command.Parameters.AddWithValue("@name", library.Name);
                                        command.Parameters.AddWithValue("@cityVillage", library.CityVillage);
                                        command.Parameters.AddWithValue("@municipality", library.Municipality);
                                        command.Parameters.AddWithValue("@district", library.District);
                                        command.Parameters.AddWithValue("@email", library.Email);
                                        command.Parameters.AddWithValue("@telephone", library.Telephone);
                                        command.Parameters.AddWithValue("@address", library.Address);

                                        command.ExecuteNonQuery();
                                    }
                                    successMessage = "Информацията за този профил е обновена.";

                                    libraryInfo.Name = library.Name;
                                    libraryInfo.CityVillage = library.CityVillage;
                                    libraryInfo.Municipality = library.Municipality;
                                    libraryInfo.District = library.District;
                                    libraryInfo.Email = library.Email;
                                    libraryInfo.Telephone = library.Telephone;
                                    libraryInfo.Address = library.Address;
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
    }
}