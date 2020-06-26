using System;
using System.Collections.Generic;

namespace FinalProject.Models
{
    public partial class Favorite
    {
        public Favorite()
        {
            Comments = new HashSet<Comments>();
        }

        public int Id { get; set; }
        public string StartupName { get; set; }
        public int? Rank { get; set; }
        public string UserId { get; set; }
        public string StartupId { get; set; }
        public string PrivateComments { get; set; }

        public virtual AspNetUsers User { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }
    }
}
