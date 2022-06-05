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

        private void FindWeather(object sender, RoutedEventArgs e)
        {

            WeatherData Data = new WeatherData(inputLocation.Text);
            Data.CheckWeather();
            locationName.Text = Data.City;
            temp.Text = Math.Round(Data.Temp).ToString() + "°C";
            //weatherDescription.Text = Data.Description;
            // coś tutaj nie działa czeba to sprawdzić później
            string iconUrl = "http://openweathermap.org/img/wn/" + Data.Icon + "@2x.png";
            smallIcon.Source = new BitmapImage(new Uri(iconUrl));
            time.Text = DateTime.Now.ToString();
            
            //if (Data.Temp > 15)
            //{
            //    this.Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "Images/warm.png")));
            //}

        }
        
        private void SendToDatabase(object sender, RoutedEventArgs e)
        {

        }


    }
}

