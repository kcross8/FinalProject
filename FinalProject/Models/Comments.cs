using System;
using System.Collections.Generic;

namespace FinalProject.Models
{
    public partial class Comments
    {
        public int Id { get; set; }
        public int? FavoriteId { get; set; }
        public string Comment { get; set; }
        public string CompanyName { get; set; }

        public virtual Favorite Favorite { get; set; }
    }
}
