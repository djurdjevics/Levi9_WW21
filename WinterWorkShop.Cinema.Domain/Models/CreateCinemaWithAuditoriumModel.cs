using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class CreateCinemaWithAuditoriumModel
    {
        public string CinemaName { get; set; }
        public string AuditoriumName { get; set; }
        public int NumberOfRows { get; set; }
        public int NumberOfColumns { get; set; }
    }
}
