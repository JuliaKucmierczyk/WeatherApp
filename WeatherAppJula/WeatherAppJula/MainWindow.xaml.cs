using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Data.SQLite;
using System.Data.Common;

namespace WeatherAppJula
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        class WeatherData
        {
            public WeatherData(string City)
            {
                city = City;
            }
            private string city;
            private float temp;
            private string description;
            private string icon;

            public void CheckWeather()
            {
                WeatherAPI DataAPI = new WeatherAPI(City);
                temp = DataAPI.GetTemp();
                description = DataAPI.GetDescription();
                icon = DataAPI.GetIcon();
            }

            public string City { get => city; set => city = value; }
            public float Temp { get => temp; set => temp = value; }
            public string Description { get => description; set => description = value; }
            public string Icon { get => icon; set => icon = value; }            

        }


        class WeatherAPI
        {
            public WeatherAPI(string city)
            {
                SetCurrentURL(city);
                xmlDocument = GetXML(CurrentURL);
            }

            public float GetTemp()
            {
                XmlNode temp_node = xmlDocument.SelectSingleNode("//temperature");
                XmlAttribute temp_value = temp_node.Attributes["value"];
                string temp_string = temp_value.Value;
                float number = float.Parse(temp_string, CultureInfo.InvariantCulture);
                return number;
            }

            public string GetDescription()
            {
                XmlNode description_node = xmlDocument.SelectSingleNode("//weather");
                XmlAttribute description_value = description_node.Attributes["value"];
                string description_string = description_value.Value;
                return description_string;
            }
            public string GetIcon()
            {
                XmlNode icon_node = xmlDocument.SelectSingleNode("//weather");
                XmlAttribute icon_value = icon_node.Attributes["value"];
                string icon_string = icon_value.Value;
                return icon_string;
            }

            private const string APIKEY = "5e67e2f428065a10d8791a98720540ee";
            private string CurrentURL;
            private XmlDocument xmlDocument;

            private void SetCurrentURL(string location)
            {
                CurrentURL = "http://api.openweathermap.org/data/2.5/weather?q="
                    + location + "&mode=xml&units=metric&APPID=" + APIKEY;
            }


            private XmlDocument GetXML(string CurrentURL)
            {
                using (WebClient client = new WebClient())
                {
                    string xmlContent = client.DownloadString(CurrentURL);
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(xmlContent);
                    return xmlDocument;
                }
            }

        }


        class DataBase
        {
        
            public static SQLiteConnection CreateConnection()
            {

                SQLiteConnection sqlite_conn;
                sqlite_conn = new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True; ");
                try
                {
                    sqlite_conn.Open();
                }
                catch (Exception ex)
                {

                }
                return sqlite_conn;
            }

            public static void CreateTable(SQLiteConnection conn, string TableName)
            {
                SQLiteCommand sqlite_cmd;
                string sql = "CREATE TABLE IF NOT EXISTS "+TableName+"(city TEXT, temp TEXT, description TEXT, date TEXT);";
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = sql;
                sqlite_cmd.ExecuteNonQuery();

            }

            public static void InsertData(SQLiteConnection conn, string city, string temp, string description, string date, string TableName)
            {
                SQLiteCommand sqlite_cmd;
                string sql = "INSERT INTO "+TableName+" (city, temp, description, date) VALUES ('" + city + "', '" + temp + "', '" + description + "', '" + date + "');";
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = sql;
                sqlite_cmd.ExecuteNonQuery();
            }

            public static string ReadData(SQLiteConnection conn, string TableName)
            {
                SQLiteDataReader sqlite_datareader;
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = "SELECT * FROM "+TableName;
                sqlite_datareader = sqlite_cmd.ExecuteReader();

                List<string> listaZrekordami = new List<string>();

                foreach (DbDataRecord record in sqlite_datareader)
                {
                    listaZrekordami.Add(record["city"].ToString() + " " + record["temp"].ToString() + " " + record["description"].ToString() + " " + record["date"].ToString());
                }
                conn.Close();
                string nazwaTabeli = "Nazwa tabeli: "+TableName+"\n";
                return nazwaTabeli+string.Join("\n", listaZrekordami);
            }
        }

        private void FindWeather(object sender, RoutedEventArgs e)
        {
            if (inputLocation.Text == "" || inputLocation.Text == "Nazwa miasta")
            {
                MessageBox.Show("Wprowadź nazwę miasta");
            }
                if (databaseRecords.Visibility == Visibility.Visible)
            {
                databaseRecords.Visibility = Visibility.Hidden;
                locationName.Visibility = Visibility.Visible;
                temp.Visibility = Visibility.Visible;
                weatherDescription.Visibility = Visibility.Visible;
                time.Visibility = Visibility.Visible;
            }
                WeatherData Data = new WeatherData(inputLocation.Text);
                Data.CheckWeather();
                locationName.Text = Data.City;
                temp.Text = Math.Round(Data.Temp).ToString() + "°C";
                weatherDescription.Text = Data.Description;
                time.Text = DateTime.Now.ToString();
        }

        private void SendToDatabase(object sender, RoutedEventArgs e)
        { 
            SQLiteConnection conn = DataBase.CreateConnection();
            DataBase.CreateTable(conn, TableName.Text);
            DataBase.InsertData(conn, locationName.Text, temp.Text, weatherDescription.Text, time.Text, TableName.Text);
            databaseText.Text = "Przesłano"; 
        }

        private void ShowRecords(object sender, RoutedEventArgs e)
        {
            if (TableName.Text == "Nazwa tabeli" || TableName.Text == "")
            {
                MessageBox.Show("Wprowadź nazwę tabeli");
            }
            else
            {
                locationName.Visibility = Visibility.Hidden;
                temp.Visibility = Visibility.Hidden;
                weatherDescription.Visibility = Visibility.Hidden;
                time.Visibility = Visibility.Hidden;
                databaseText.Visibility = Visibility.Hidden;
                databaseRecords.Visibility = Visibility.Visible;
                databaseRecords.Text = DataBase.ReadData(DataBase.CreateConnection(), TableName.Text);
            }
            
        }
    }
}

