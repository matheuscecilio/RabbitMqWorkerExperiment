using System;

namespace Core.Domain
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}
