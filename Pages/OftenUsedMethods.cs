using Microsoft.Extensions.Options;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace Library.Pages
{
    //В текущата страница са имплементирани методи, които проверяват и/или обработват входни данни.
    //Тяхната цел е да се намали вероятността за некоректно въведени данни.
    public class OftenUsedMethods
    {
        private static string _connectionString = "Data Source=SQL6030.site4now.net;Initial Catalog=db_ab83a6_library25;User Id=db_ab83a6_library25_admin;Password=*";

        public static string ConnectionString
        {
            get { return _connectionString; }
        }
        public static string CorrectTopic(BookInformation bookInfo)
        {
            string topic = bookInfo.Title.Trim();
            List<char> chars = topic.ToList();
            List<int> numbers = new List<int>();

            if (chars.Count < 2)
            {
                return "Грешка";
            }
            else
            {
                if (chars[0] != '"' && chars[chars.Count - 1] != '"' || chars[1] == '"')
                {
                    return "Грешка";
                }
                else
                {
                    for (int i = 0; i < chars.Count - 1; i++)
                    {
                        if (chars[i] == ' ' && chars[i + 1] == ' ')
                        {
                            numbers.Add(i);
                        }
                    }

                    int count = 0;
                    for (int i = 0; i < numbers.Count; i++)
                    {
                        chars.RemoveAt(numbers[i] - count);
                        count++;
                    }
                    if (chars[1] == ' ')
                    {
                        chars.RemoveAt(1);
                    }
                    if (chars[chars.Count - 2] == ' ')
                    {
                        chars.RemoveAt(chars.Count - 2);
                    }

                    if (chars[0] != '"' || chars[chars.Count - 1] != '"')
                    {
                        return "Грешка";
                    }
                    else
                    {
                        topic = new string(chars.ToArray());
                        bookInfo.Title = topic;

                        return bookInfo.Title;
                    }
                }
            }
        }

        public static string CorrectNameAndFamilia(string name)
        {
            string autor = name.Trim();
            List<char> chars = autor.ToList();
            List<int> numbers = new List<int>();

            //Брои се колко празни места има в стринга
            int counter = 0;
            for (int i = 0; i < chars.Count; i++)
            {
                if (chars[i] == ' ')
                {
                    counter++;
                }
            }

            //Ако няма празни места следва,че имаме едно име,което е невалидна ситуация.
            if (counter < 1)
            {
                name = "Едно име.";
            }
            else
            {
                //Броим единичните свободни места между две букви.
                int counter2 = 0;
                for (int i = 1; i < chars.Count - 1; i++)
                {
                    if (chars[i] == ' ' && chars[i - 1] != ' ' && chars[i + 1] != ' ')
                    {
                        counter2++;
                    }
                }
                //Ако този брой е равен на броя на свободните места в стринга
                //следва,че авторът има няколко имена,което е валидна ситуация.
                if (counter2 == counter)
                {
                    //Повдига първата буква на всяко име,а останалите са малки букви.
                    chars[0] = Char.ToUpper(chars[0]);
                    for (int i = 1; i < chars.Count; i++)
                    {
                        if (chars[i] == ' ')
                        {
                            continue;
                        }
                        if (chars[i - 1] == ' ')
                        {
                            chars[i] = Char.ToUpper(chars[i]);
                        }
                        else
                        {
                            chars[i] = Char.ToLower(chars[i]);
                        }
                    }

                    autor = new string(chars.ToArray());
                    name = autor;
                }
                //След преминаването на предишните проверки остава само 
                //случаят на много празни места една до друго в стринга.
                else if (counter > 1)
                {
                    //Записва индексите на празните позиции в лист,
                    //като оставя по една свободна между думите.
                    for (int i = 0; i < chars.Count - 2; i++)
                    {
                        if (chars[i] == ' ' && chars[i + 1] == ' ' && chars[i + 2] != ' ')
                        {
                            continue;
                        }
                        else
                        {
                            if (chars[i] == ' ')
                            {
                                numbers.Add(i);
                            }
                        }
                    }

                    //Изтриват се празните места.
                    int count = 0;
                    for (int i = 0; i < numbers.Count; i++)
                    {
                        chars.RemoveAt(numbers[i] - count);
                        count++;
                    }

                    //Повдига първата буква на всяко име,а останалите са малки букви.
                    chars[0] = Char.ToUpper(chars[0]);
                    for (int i = 1; i < chars.Count; i++)
                    {
                        if (chars[i] == ' ')
                        {
                            continue;
                        }
                        if (chars[i - 1] == ' ')
                        {
                            chars[i] = Char.ToUpper(chars[i]);
                        }
                        else
                        {
                            chars[i] = Char.ToLower(chars[i]);
                        }
                    }

                    autor = new string(chars.ToArray());
                    name = autor;
                }
            }
            return name;
        }
        public static string CorrectNameInput(string name)
        {
            string word = name.Trim();

            if (word.Length != 0)
            {
                int mistake = 0;
                char[] chars = word.ToCharArray();

                for (int i = 0; i < chars.Length; i++)
                {
                    if (chars[i] == ' ')
                    {
                        mistake++;
                    }
                }
                if (mistake > 0)
                {
                    return "Грешка";
                }
                else
                {
                    for (int i = 0; i < chars.Length; i++)
                    {
                        if (i == 0)
                        {
                            chars[i] = Char.ToUpper(chars[i]);
                        }
                        else
                        {
                            if (chars[i - 1] == '-')
                            {
                                chars[i] = Char.ToUpper(chars[i]);
                            }
                            else
                            {
                                chars[i] = Char.ToLower(chars[i]);
                            }
                        }
                    }
                    word = new string(chars.ToArray());
                    name = word;
                    return name;
                }
            }
            else
            {
                return "Грешка";
            }
        }

        public static int CorrectIntInput(string str)
        {
            int number;

            bool success = int.TryParse(str, out number);
            if (success == true) 
            {
                return number;
            }
            else
            {
                return -1;
            }
        }
        public static double CorrectDoubleInput(string str)
        {
            double number;

            bool success = double.TryParse(str, out number);
            if (success == true)
            {
                return Math.Round(double.Parse(str), 2);
            }
            else
            {
                return -1;
            }
        }

        public static string CorrectCityAndMunicipalityInput(string str)
        {
            string word = str.Trim();

            if (word.Length != 0) 
            {
                List<char> chars = word.ToList();
                List<int> numbers = new List<int>();

                for (int i = 0; i < chars.Count - 1; i++)
                {
                    if (chars[i] == ' ' && chars[i + 1] == ' ')
                    {
                        numbers.Add(i);
                    }
                }

                int count = 0;
                for (int i = 0; i < numbers.Count; i++)
                {
                    chars.RemoveAt(numbers[i] - count);
                    count++;
                }

                chars[0] = Char.ToUpper(chars[0]);
                word = new string(chars.ToArray());
                str = word;

                return str;
            }
            else
            {
                return "";
            }

        }
        public static string CorrectPhone(string phone)
        {
            string telephone = phone.Trim();

            if (telephone.Length != 0) 
            {
                List<char> chars = telephone.ToList();

                for (int i = 0; i < chars.Count; i++)
                {
                    if (chars[i] == ' ')
                    {
                        return "Грешка";
                    }
                }

                if (chars[0] != '0')
                {
                    return "Грешка";
                }
                else if (chars.Count > 10)
                {
                    return "Грешка";
                }
                else
                {
                    telephone = new string(chars.ToArray());
                    phone = telephone;
                    return phone;
                }
            }
            else
            {
                return "Грешка";
            }
        }

    }
}