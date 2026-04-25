using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Worker.Models
{
    public class OrderMessage
    {
        public Guid OrderId { get; set; }
        public string Product { get; set; } = "";
        public int Quantity { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
