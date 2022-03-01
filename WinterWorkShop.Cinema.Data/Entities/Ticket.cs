using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data.Entities
{
    [Table("ticket")]
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SeatId { get; set; }
        public Guid ProjectionId { get; set; }
        public double Price { get; set; }
        public User User { get; set; }
        public Seat Seat { get; set; }
        public Projection Projection { get; set; }
    }
}
