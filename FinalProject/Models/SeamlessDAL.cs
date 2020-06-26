using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class SeamlessDAL
    {
        private readonly string APIKey;
        public SeamlessDAL(IConfiguration configuration)
        {
            APIKey = configuration.GetSection("ApiKeys")["AirtableAPI"];
        }
        public string GetAPIString()
        {
            string url = $"https://api.airtable.com/v0/appFo187B73tuYhyg/Master%20List?api_key={APIKey}";
            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string output = rd.ReadToEnd();
            return output;
        }
       
        public string GetStartupString(string id)
        {
            string url = $"https://api.airtable.com/v0/appFo187B73tuYhyg/Master%20List/{id}?api_key={APIKey}";
            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string output = rd.ReadToEnd();
            return output;
        }
        public Record GetRecord(string id)
        {
            string output = GetStartupString(id);
            Record r = JsonConvert.DeserializeObject<Record>(output);
            return r;
        }
        public string GetPost()
        {
            string output = GetAPIString();            
            return output;
        }
        public RootObject getStart()
        {
            string output = GetAPIString();
            RootObject s = JsonConvert.DeserializeObject<RootObject>(output);
            return s;
        }
        public HttpClient GetHttpClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.airtable.com/v0/appFo187B73tuYhyg/");
            return client;
        }
        public async Task<List<Startup>> GetStartups()
        {
            var client = GetHttpClient();
            var response = await client.GetAsync($"Master%20List?api_key={APIKey}");
            //install-package Microsoft.AspNet.WebAPI.Client
            var startups = await response.Content.ReadAsAsync<List<Startup>>();
            
            return startups;                
        }

    }
}
