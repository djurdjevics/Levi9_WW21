using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Data.Entities
{
    public class MovieWithTag
    {
        public Guid MovieId { get; set; }

        public virtual Movie Movie { get; set; } 
        public Guid TagId { get; set; }
        public virtual Tag Tag { get; set; } 
    }
}
