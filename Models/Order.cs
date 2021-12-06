using System;

namespace OpticsStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }

        public int GlassesFrameId { get; set; }
        public GlassesFrame GlassesFrame { get; set; }

        public string UserRecipe { get; set; }
        public decimal Price { get; set; }

        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; }
    }
}