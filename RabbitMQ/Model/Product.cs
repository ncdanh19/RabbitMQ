using System.ComponentModel;

namespace RabbitMQ.Model
{
    public class Product
    {
        public int ProductId { get; set; }

        [DefaultValue("Iphone 15 Pro Max")]
        public string ProductName { get; set; }

        [DefaultValue("Apple phone")]
        public string ProductDescription { get; set; }

        [DefaultValue(100)]
        public int ProductPrice { get; set; }

        [DefaultValue(10)]
        public int ProductStock { get; set; }
    }
}
