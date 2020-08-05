using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StayAtHome.Models
{
    public enum UrlPurpose
    {
        AutoComplete,
        Info
    }
    public class Address
    {
        private const string  _apiKey = "4449d818-fb45-46b5-8db9-7c4d71f49c86";

        public string Suburb { get; set; }
        public string Number { get; set; }
        public string NumberFirst { get; set; }
        public object NumberLast { get; set; }
        public object StreetSuffix { get; set; }
        public object StreetSuffixFull { get; set; }
        public string State { get; set; }
        public string Street { get; set; }
        public string StreetLine { get; set; }
        public object BuildingName { get; set; }
        public string StreetType { get; set; }
        public string StreetTypeFull { get; set; }
        public object UnitType { get; set; }
        public object UnitTypeFull { get; set; }
        public object UnitNumber { get; set; }
        public object LevelType { get; set; }
        public object LevelTypeFull { get; set; }
        public object LevelNumber { get; set; }
        public string Postcode { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Street_Longitude { get; set; }
        public double Street_Latitude { get; set; }
        public string Meshblock { get; set; }
        public string Meshblock2016 { get; set; }
        public string Gnaf_ID { get; set; }
        public bool Valid { get; set; }



        public static async Task<List<string>> GetAddresses(string searchTerm)
        {
            List<string> addresses = new List<string>();

            var url = GenerateUrl(searchTerm,UrlPurpose.AutoComplete);

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                addresses = JsonConvert.DeserializeObject<List<string>>(json);
            }

            return addresses;


        }

        public static async Task<Address> GetAddress(string searchTerm)
        {
            Address address = new Address();

            var url = GenerateUrl(searchTerm,UrlPurpose.Info);

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                address = JsonConvert.DeserializeObject<Address>(json);
            }

            return address;


        }

        public static string GenerateUrl(string searchTerm, UrlPurpose urlPurpose)
        {


            searchTerm = searchTerm.Replace(' ', '+');

            var url = "";

            switch (urlPurpose)
            {
                case UrlPurpose.AutoComplete:
                    
                    url =
                        $"https://api.addressify.com.au/addresspro/autocomplete?api_key={_apiKey}&term={searchTerm}&address_types=2";

                    break;
                case UrlPurpose.Info:
                    
                    url =
                        $"https://api.addressify.com.au/addresspro/info?api_key={_apiKey}&term={searchTerm}";
                    break;
            }

            

            return url;

        }
    }

}
