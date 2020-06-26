using System;
using System.Collections.Generic;

namespace FinalProject.Models
{
    public partial class UserPreferences
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Theme { get; set; }
        public string TechArea { get; set; }
        public string Alignment { get; set; }
        public int? Rank { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
