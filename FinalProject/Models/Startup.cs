using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{

    public class RootObject
    {
        public Record[] records { get; set; }
    }

    public class Record
    {
        public string id { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public Startups startups { get; set; }
        public DateTime createdTime { get; set; }
    }

    public class Startups
    {
        [JsonProperty(PropertyName = "Company Name")]
        public string CompanyName { get; set; }
        [JsonProperty(PropertyName = "Date Added")]
        public string DateAdded { get; set; }
        public string Scout { get; set; }
        public string Source { get; set; }
        [JsonProperty(PropertyName = "Company Website")]
        public string CompanyWebsite { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        [JsonProperty(PropertyName = "Two Line Company Summary")]
        public string TwoLineCompanySummary { get; set; }
        public string Alignment { get; set; }
        [JsonProperty(PropertyName = "Theme(s)")]
        public string Themes { get; set; }
        public string Uniqueness { get; set; }
        public string Team { get; set; }
        public string Raised { get; set; }
        [JsonProperty(PropertyName = "Review Date")]
        public string ReviewDate { get; set; }
        [JsonProperty(PropertyName = "Technology Areas")]
        public string TechnologyAreas { get; set; }
        public string Landscape { get; set; }
        public string Stage { get; set; }
        [JsonProperty(PropertyName = "State/Province")]
        public string StateProvince { get; set; }

        public int Rating { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }

        public static void RateIndividual(Record r)
        {
            int uniqueness;
            int team;
            int funding;
            uniqueness = GetUniquenessRating(r);
            team = GetTeamRating(r);
            funding = GetFundingRating(r);

            r.startups.Rating = uniqueness + team + funding;

        }

        public static void RateStartups(IEnumerable<Record> startups)
        {
            
            int uniqueness;
            int team;
            int funding;
            foreach(Record s in startups)
            {
                uniqueness = GetUniquenessRating(s);
                team = GetTeamRating(s);
                funding = GetFundingRating(s);

                s.startups.Rating = uniqueness + team + funding;

            }
        }

        public static int GetUniquenessRating(Record s)
        {
            int rating = 0;
            int uniqueness = int.Parse(s.startups.Uniqueness);
            if (uniqueness == 5)
            {
                rating = 3;
            }
            else if (uniqueness == 4)
            {
                rating = 5;
            }
            else if (uniqueness == 3)
            {
                rating = 4;
            }
            else if (uniqueness == 2)
            {
                rating = 2;
            }
            else if (uniqueness == 1)
            {
                rating = 1;

            }
            return rating;
        }
        public static int GetTeamRating(Record s)
        {
            int rating = 0;
            int team = int.Parse(s.startups.Team);
            if (team == 5)
            {
                rating = 5;
            }
            else if (team == 4)
            {
                rating = 4;
            }
           else if (team == 3)
            {
                rating = 3;
            }
            else if (team == 2)
            {
                rating = 2;
            }
            else if (team == 1)
            {
                rating = 1;
            }

            return rating;
        }
        public static int GetFundingRating(Record s)
        {
            int rating = 0;
            int num;
            if (int.TryParse(s.startups.Raised, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out num))
            {
                if (num > 5000000)
                {
                    rating = 5;
                }
                else if (num <= 5000000 && num > 2000000)
                {
                    rating = 4;
                }
                else if (num <= 2000000 && num > 1000000)
                {
                    rating = 3;
                }
                else if (num <= 1000000 && num > 500000)
                {
                    rating = 2;
                }
                else if (num <= 500000)
                {
                    rating = 1;
                }
            }
            else
            {
                rating = 1;
            }
            return rating;
        }

    }

}
