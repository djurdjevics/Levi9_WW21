using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Data.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string TagName { get; set; }

        public virtual ICollection<MovieWithTag> MovieWithTags { get; set; }
    }
}
