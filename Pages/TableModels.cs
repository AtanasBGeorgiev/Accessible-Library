namespace Library.Pages
{
    //В текущата страница са създадени четири класа, всеки от които отговаря на таблица в базата от данни.
    public class TableModels
    {
    }

    public class LibraryInformation
    {
        private string _id;
        private string _name;
        private string _cityVillage;
        private string _municipality;
        private string _district;
        private string _email;
        private string _telephone;
        private string _address;
        private string _password;
        private string _dateOfCreation;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string CityVillage
        {
            get { return _cityVillage; }
            set { _cityVillage = value; }
        }
        public string Municipality
        {
            get { return _municipality; }
            set { _municipality = value; }
        }
        public string District
        {
            get { return _district; }
            set { _district = value; }
        }
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        public string Telephone
        {
            get { return _telephone; }
            set { _telephone = value; }
        }
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public string DateOfCreation
        {
            get { return _dateOfCreation; }
            set { _dateOfCreation = value; }
        }
    }

    public class Reader
    {
        private string _id;
        private string _name;
        private string _lastName;
        private string _email;
        private string _telephone;
        private string _password;
        private string _dateOfCreation;

        //Следва имплементация на свойствата за достъп до полетата 
        //с ниво на достъп public 

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        public string Telephone
        {
            get { return _telephone; }
            set { _telephone = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public string DateOfCreation
        {
            get { return _dateOfCreation; }
            set { _dateOfCreation = value; }
        }
    }

    public class BookInformation
    {
        private string _id;
        private string _inventoryNum;
        private string _title;
        private string _author;
        private string _year;
        private string _price;
        private string _signature;
        private string _idLibrary;
        private string _isAvaiable;
        private string _idReader;
        private string _dateOfTaking;
        private string _inventory;
        private string _deduction;
        private string _dateOfCreation;
        private string _sentEmail;
        private string _category;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string InventoryNum
        {
            get { return _inventoryNum; }
            set { _inventoryNum = value; }
        }
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }
        public string Year
        {
            get { return _year; }
            set { _year = value; }
        }
        public string Price
        {
            get { return _price; }
            set { _price = value; }
        }
        public string Signature
        {
            get { return _signature; }
            set { _signature = value; }
        }
        public string IdLibrary
        {
            get { return _idLibrary; }
            set { _idLibrary = value; }
        }
        public string IsAvaiable
        {
            get { return _isAvaiable; }
            set { _isAvaiable = value; }
        }
        public string IdReader
        {
            get { return _idReader; }
            set { _idReader = value; }
        }
        public string DateOfTaking
        {
            get { return _dateOfTaking; }
            set { _dateOfTaking = value; }
        }
        public string Inventory
        {
            get { return _inventory; }
            set { _inventory = value; }
        }
        public string Deduction
        {
            get { return _deduction; }
            set { _deduction = value; }
        }
        public string DateOfCreation
        {
            get { return _dateOfCreation; }
            set { _dateOfCreation = value; }
        }
        public string SentEmail
        {
            get { return _sentEmail; }
            set { _sentEmail = value; }
        }
        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }
    }

    public class AdminInformation
    {
        private string _id;
        private string _name;
        private string _surname;
        private string _lastName;
        private string _email;
        private string _telephone;
        private string _userName;
        private string _password;
        private string _role;
        private bool _isConfirmed;
        private string _dateOfCreation;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Surname
        {
            get { return _surname; }
            set { _surname = value; }
        }
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        public string Telephone
        {
            get { return _telephone; }
            set { _telephone = value; }
        }
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public string Role
        {
            get { return _role; }
            set { _role = value; }
        }
        public bool IsConfirmed
        {
            get { return _isConfirmed; }
            set { _isConfirmed = value; }
        }
        public string DateOfCreation
        {
            get { return _dateOfCreation; }
            set { _dateOfCreation = value; }
        }
    }
}