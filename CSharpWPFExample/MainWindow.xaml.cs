using System.Net;
using System.Windows;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace CSharpWPFExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void search_Button_Click(object sender, RoutedEventArgs e)
        {
            // Change this to your Search Key
            string searchKey = "SEARCH KEY GOES HERE";

            // Check to see if the Search Key has been altered
            if (searchKey == "SEARCH KEY GOES HERE")
            {
                MessageBox.Show("You need to set the searchKey variable to your Search Key (available from the admin interface online)","Warning",MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }
            
            // Any search can include an optional identifier, this your own value and can be set to whatever you like for tracking purposes
            string identifier = "CSharpExample";
            
            string restPattern = "https://ws.postcoder.com/pcw/{0}/Address/UK/{1}?identifier={2}";
                        
            string searchTest = search_TextBox.Text;

            string restURL = string.Format(restPattern,searchKey,searchTest,identifier);

            // The defaults for this will return JSON, if you prefer XML then alter the accept header.
            WebClient web = new WebClient();

            // This will contain the main return from the service
            string json = "";

            try
            {
                // This is the call to the online service
                json = web.DownloadString(restURL);
            }
            catch (WebException webEx) // Something has gone wrong, 403 a parameter is incorrect. http://ws.postcoder.com/pcw/YOUR_SEARCH_KEY/status will usually show what's up.
            {
                output_TextBox.Text = webEx.ToString();
                return;
            }

            // Maps the JSON response to the .NET object (defined below)
            Address[] addresses = JsonConvert.DeserializeObject<Address[]>(json);

            // Load the forms combo box
            addresses_ComboBox.Items.Clear();
            if (addresses.Length > 0)
            {                
                foreach (Address address in addresses)
                    addresses_ComboBox.Items.Add(address);

                addresses_ComboBox.SelectedIndex = 0;
            }
                            
            // Show the Raw output in an indented format.
            output_TextBox.Text = JsonToIndentedString(json);
        }

        private void addresses_ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (addresses_ComboBox.Items.Count <= 0)
                return;

            if (addresses_ComboBox.SelectedIndex < 0)
                addresses_ComboBox.SelectedIndex = 0;

            Address address = (Address)addresses_ComboBox.SelectedItem;

            organisation_TextBox.Text = address.organisation;
            premise_TextBox.Text = address.premise;
            dependentStreet_TextBox.Text = address.dependentstreet;
            street_TextBox.Text = address.street;
            doubleDependentLocality_TextBox.Text = address.doubledependentlocality;
            locality_TextBox.Text = address.dependentlocality;
            town_TextBox.Text = address.posttown;
            county_TextBox.Text = address.county;
            postcode_TextBox.Text = address.postcode;
        }

        /// <summary>
        /// Converts a JSON string to a prettier version, cosmetic only.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string JsonToIndentedString(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
    }

    public class Address
    {
        public string organisation;
        public string premise;
        public string dependentstreet;
        public string street;
        public string doubledependentlocality;
        public string dependentlocality;
        public string posttown;
        public string county;
        public string postcode;

        public string summaryline;

        public override string ToString() { return summaryline; }
    }
}
