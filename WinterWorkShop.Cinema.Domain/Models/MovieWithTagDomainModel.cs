using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class MovieWithTagDomainModel
    {
        public Guid TagId { get; set; }
        public string TagName { get; set; }
        public Guid MovieId { get; set; }
        public string MovieTitle { get; set; }
    }
}
