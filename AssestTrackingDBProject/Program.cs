// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Intrinsics.X86;

Console.WriteLine("Asset Traking DB Project");
Console.WriteLine("=================");


SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
builder.DataSource = "localhost";
builder.UserID = "sa";
builder.Password = "";
builder.InitialCatalog = "DevicesDB";
SqlConnection connection = new SqlConnection(builder.ConnectionString);

while (true)
{
    List<Device> devices = new List<Device>();

    //getDataFromDB(devices, connection);
    GetingDataToDevicesList(devices, connection);
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write(
       $">> You have {devices.Count()} Devices \n" +
       ">> Pick an option: \n" +
       ">> (1) Show All Devices \n" +
       ">> (2) Add New Device \n" +
       ">> (3) Update Device Information \n" +
       ">> (4) Delete Device \n"+
       ">> (q) Quit \n" +
       ">>>>");

    
    string option = Console.ReadLine();

    if (option == "1")
    {
        getDataFromDB(devices, connection);
    }else if (option == "2")
    {
        insertToDB(devices,connection);
    }else if(option == "3")
    {
        updateDB(devices,connection);
    }else if(option == "4")
    {
        deleteFromDB(devices, connection);
    }else if(option == "q")
    {
        break;
    }
    else
    {
        Console.WriteLine("Error.... Not Valid....Please Try Again...");
    }


}


static void getDataFromDB(List<Device> devices,SqlConnection connection)
{
    string SqlText = "";
    bool enterOrder = true;
    while (enterOrder)
    {
        Console.Write("Pick Order Options....\n" +
        "(1) Order By ID...\n" +
        "(2) Order By Office Then Purchase Date...\n" +
        "(3) Order By Office Then Name(Computer Then Mobile....) Then Purchase Date...\n" +
        ">>>>");
        string orderBy = Console.ReadLine();
        if (orderBy == "1")
        {
            SqlText = "SELECT * FROM Devices order by ID";
            enterOrder = false;
        }
        else if (orderBy == "2")
        {
            SqlText = "SELECT * FROM Devices order by Office,Purchase";
            enterOrder = false;

        }
        else if (orderBy == "3")
        {
            SqlText = "SELECT * FROM Devices order by Office,CASE When name='computer' then 1 when name ='mobil' then 2 else 3 END, convert(datetime, Purchase, 101)\n";
            enterOrder = false;
        }
        else
        {
            Console.WriteLine("Error....Invalid Choice....");
        }
        if (!enterOrder)
        {
            connection.Open();
            SqlCommand sqlCommand = new SqlCommand(SqlText, connection);

            Console.WriteLine("ID".PadRight(10) + "Name".PadRight(20) + "Brand".PadRight(20) + "Model".PadRight(20) + "Office".PadRight(20) + "purchase".PadRight(20) + "ExexpiredOfDays".PadRight(20) + "price".PadRight(20) + "LocalPrice".PadRight(20) + "Currency");
            var Result = sqlCommand.ExecuteReader();
            while (Result.Read())
            {

                var id = Result.GetValue(0).ToString();
                var name = Result.GetValue(1).ToString();
                var brand = Result.GetValue(2).ToString();
                var model = Result.GetValue(3).ToString();
                var office = Result.GetValue(4).ToString();
                var purchase = Result.GetValue(5).ToString();
                var price = Result.GetValue(6).ToString();

                devices.Add(new Device(id, name, brand, model, office, purchase, price));

                if (checkingDate(purchase))
                {
                    Console.WriteLine(id.PadRight(10) + name.PadRight(20) + brand.PadRight(20) + model.PadRight(20) + office.PadRight(20) + purchase.PadRight(20) + ExexpiredDays(purchase).PadRight(20) + price.PadRight(20) + gettingLocalPrice(useCurrency(office), price).PadRight(20) + useCurrency(office));
                }
            }
            connection.Close();
        }
        
    }
}


static void GetingDataToDevicesList (List<Device> devices, SqlConnection connection)
{
    connection.Open();  

    SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Devices order by Office,CASE When name='computer' then 1 when name ='mobil' then 2 else 3 END, convert(datetime, Purchase, 101)\n", connection);

    var Result = sqlCommand.ExecuteReader();

    while (Result.Read())
    {

        var id = Result.GetValue(0).ToString();
        var name = Result.GetValue(1).ToString();
        var brand = Result.GetValue(2).ToString();
        var model = Result.GetValue(3).ToString();
        var office = Result.GetValue(4).ToString();
        var purchase = Result.GetValue(5).ToString();
        var price = Result.GetValue(6).ToString();

        devices.Add(new Device(id, name, brand, model, office, purchase, price));
    }
    connection.Close();
}

static void insertToDB(List<Device> devices, SqlConnection connection)
{
    bool enterID = false;
    bool enterName = false;
    bool enterBrand = false;
    bool enterModel = false;
    bool enterOffice = false;
    bool enterPurchase = false;
    bool enterPrice = false;

    string id = "";
    string name = "";
    string brand = "";
    string model = "";
    string office = "";
    string purchase = "";
    string price = "";

    
    while (true)
    {
        if (!enterID)
        {
            Console.Write("Enter ID >>>");
            id = Console.ReadLine();
            if (IsNumber(id)&&IsEmpty(id))
            {
                enterID = !CheckingId(devices, id.ToString());
            }
           
        }

        if (!enterName && enterID)
        {
            Console.Write("Enter Name >>>");
            name = Console.ReadLine();
            enterName = IsEmpty(name);
            
            
        }

        if (!enterBrand && enterName)
        {
            Console.Write("Enter Brand >>>");
            brand = Console.ReadLine();

            enterBrand = IsEmpty(brand);
        }

        if (!enterModel && enterBrand)
        {
            Console.Write("Enter Model >>>");
            model = Console.ReadLine();
            enterModel = IsEmpty(model);
        }

        if (!enterOffice && enterModel)
        {
            Console.Write("Enter Office Europ / Sweden / USA >>>");
            office = Console.ReadLine();
            enterOffice = IsEmpty(office);
        }

        if (!enterPurchase && enterOffice)
        {
            Console.Write("Enter Purchase Date MM/DD/YYYY >>>");
            purchase = Console.ReadLine();
            enterPurchase = IsEmpty(purchase);
        }

        if (!enterPrice && enterPurchase)
        {
            Console.Write("Enter Price >>>");
            price = Console.ReadLine();
            if (IsNumber(price) && IsEmpty(price))
            {
                enterPrice = IsNumber(price);
            }
            else
            {
                enterPrice = false;
            }
           
        }


        if(enterID && enterName && enterBrand && enterModel && enterOffice && enterPurchase && enterPrice)
        {
            string sql = "Insert INTO [Devices] (ID,Name,Brand,Model,Office,Purchase,Price) values(@ID,@Name,@Brand,@Model,@Office,@Purchase,@Price)";
            connection.Open();
            SqlCommand sqlCommand = new SqlCommand(sql, connection);

            sqlCommand.Parameters.AddWithValue("@ID", id);
            sqlCommand.Parameters.AddWithValue("@Name",FirstCharToUpper( name));
            sqlCommand.Parameters.AddWithValue("@Brand", brand);
            sqlCommand.Parameters.AddWithValue("@Model", model);
            sqlCommand.Parameters.AddWithValue("@Office", FirstCharToUpper(office));
            sqlCommand.Parameters.AddWithValue("@Purchase", purchase);
            sqlCommand.Parameters.AddWithValue("@Price", price);

            sqlCommand.ExecuteNonQuery();
            connection.Close();

            break;

            enterID = true;
            enterName = true;
            enterBrand = true;
            enterModel = true;
            enterOffice = true;
            enterPurchase = true;
            enterPrice = true;
        }
       
    }
    
}
static void updateDB(List<Device>devices,SqlConnection connection)
{
    bool enterID = false;
    bool enterKeyName = false;
    bool enterNewValue= false;

    string id = "";
    string keyName = "";
    string newValue = "";
    while (true)
    {

        if (!enterID)
        {
            Console.Write("Enter Id Update >>>");
            id = Console.ReadLine();
            if (IsNumber(id) && IsEmpty(id))
            {
                enterID = CheckingId(devices, id);
            }
           
        }
       if(enterID && !enterKeyName)
        {
            Console.Write("Enter Name of key >>>");
            keyName = Console.ReadLine();
            if (CheckingKeyName(keyName, connection))
            {
                enterKeyName = IsEmpty(keyName);
            }
            else
            {
                enterKeyName = false;
            }
            
        }

       if(enterKeyName && !enterNewValue)
        {
                Console.Write("Enter The New Value >>>");
                newValue = Console.ReadLine();
            if (keyName.ToLower().Trim() == "purchase")
            {
                if (IsValidDate(newValue)){
                    enterNewValue = IsValidDate(newValue);
                }
                else
                {
                    Console.WriteLine("Error... Not Valid Date >>>>MM/DD/YYYY");
                }
            }else if (keyName.ToLower().Trim()=="id")
            {
                if (CheckingId(devices, newValue))
                {
                    Console.WriteLine("ID IS Already Exsit..");
                    enterNewValue = false;
                }
                else
                {
                    enterNewValue = true;
                }
            }else if(keyName.ToLower().Trim() == "name"|| keyName.ToLower().Trim() == "office")
            {
                newValue = FirstCharToUpper(newValue);
                enterNewValue = IsEmpty(newValue);
            }
            else
            {
                enterNewValue = IsEmpty(newValue);
            }
        }

        if (enterID && enterKeyName && enterNewValue)
        {
            connection.Open();
            SqlCommand sqlCommand = new SqlCommand("Update Devices set " + keyName + "= '" + newValue + "' Where ID = '" + id + " '", connection); ;
            sqlCommand.ExecuteReader();
            connection.Close();


            enterID = false;
            enterKeyName = false;
            enterNewValue = false;
            break;

        }
    }
}



static void deleteFromDB(List<Device>devices,SqlConnection connection)
{
    bool enterDeleteID = false;

    Console.Write("Enter Id Delete >>>");
    string id = Console.ReadLine();
    if (CheckingId(devices, id) && IsNumber(id))
    {
        connection.Open();
        SqlCommand sqlCommand = new SqlCommand("Delete FROM Devices Where ID = '" + id + " '", connection); ;
        sqlCommand.ExecuteReader();
        connection.Close();
    }
    
}

static bool CheckingKeyName(string input, SqlConnection connection)
{
    input = input.ToLower();
    List<string> keyNames = new List<string>();
    if (IsEmpty(input))
    {
        connection.Open();
        SqlCommand sqlCommand = new SqlCommand("select c.name from sys.columns c\ninner join sys.tables t \non t.object_id = c.object_id\nand t.name = 'Devices' and t.type = 'U'", connection);
        var Result = sqlCommand.ExecuteReader();
        while (Result.Read())
        {
            keyNames.Add(Result.GetString(0).ToLower());
        }
        connection.Close();
        
    }
    return keyNames.Contains(input);
}



static string useCurrency(string office)
{
    string currency = "";
    string inputValue = office.ToLower().Trim();
    if (inputValue == "usa")
    {
        currency = "USD";
    }
    else if (inputValue == "sweden")
    {
        currency = "SEK";
    }
    else if (inputValue == "europ")
    {
        currency = "EUR";
    }
    else
    {
        currency = "OPS....Office NOt Found!";
    }
    return currency;
}

static string gettingLocalPrice(string currency, string price)
{
    double rate = 1;
    currency = currency.ToLower().Trim();
    if (currency == "sek")
    {
        rate = 10.7;
    }
    else if (currency == "eur")
    {
        rate = 0.99;
    }
    else if (currency == "usd")
    {
        rate = 1;
    }
    double localPrice = Convert.ToDouble(price) * rate;
    return localPrice.ToString();
}


static bool checkingDate(string input)
{
    DateTime newDate = new DateTime();
    newDate = DateTime.Now;
    DateTime inputValue = Convert.ToDateTime(input);
    DateTime afterThreeYears = inputValue.AddYears(3);
    DateTime finishThreeMonths = afterThreeYears.AddMonths(-3);
    DateTime finishSixMonths = afterThreeYears.AddMonths(-6);

    if (DateTime.Compare(newDate, afterThreeYears) > 0)
    {
        Console.ForegroundColor = ConsoleColor.White;
        return false;
    }
    if (DateTime.Compare(newDate, finishThreeMonths) > 0)
    {
        Console.ForegroundColor = ConsoleColor.Red;
    }
    else if (DateTime.Compare(newDate, finishSixMonths) > 0)
    {

        Console.ForegroundColor = ConsoleColor.Yellow;
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.White;
    }
    return true;
}


static bool IsValidDate(string input)
{
    DateTime tempObject; ;
    return DateTime.TryParse(input, out tempObject);
}

static string ExexpiredDays(string input)
{
    DateTime newDate = new DateTime();
    newDate = DateTime.Now;
    DateTime inputValue = Convert.ToDateTime(input);
    DateTime afterThreeYears = inputValue.AddYears(3);
    string ex = ((afterThreeYears.Date - newDate.Date).Days).ToString();
    return ex;
}

static bool CheckingId(List<Device>devices, string id)
{

    List<string> ids = new List<string>();
    foreach (Device device in devices)
    {

        ids.Add(device.Id);
    }
    if (IsEmpty(id) && IsNumber(id)&& ids.Contains(id))
    {
        return true;
    }
    else {
      
        return false; }

}

static string FirstCharToUpper(string input)
{

    return input.First().ToString().ToUpper() + input.Substring(1);
}

static bool IsNumber(string id)
{
    if (id.All(char.IsDigit))
    {
        return true;
    }
    else
    {
        Console.WriteLine("It Is NOT Number!!!!");
        return false;
    }
}

static bool IsEmpty(string input)
{
    if (String.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine("Error... Empty Field...");
        return false;
    }
    return true;

}



Console.ReadLine();

class Device
{

    public Device() { }
    public Device(string id,string name, string brand, string model, string office, string purchase, string price)
    {
        Id = id;
        Name = name;
        Brand = brand;
        Model = model;
        Office = office;
        Purchase = purchase;
        Price = price;
    }



    public string Id { get; set; }
    public string Name { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string Office { get; set; }
    public string Purchase { get; set; }
    public string Price { get; set; }
}
