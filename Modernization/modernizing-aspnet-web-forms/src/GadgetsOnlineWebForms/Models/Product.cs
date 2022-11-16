using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GadgetsOnline.Models
{
    public class Product
    {
        [ScaffoldColumn(false)]
        public int ProductId { get; set; }

        [DisplayName("Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00")]
        public decimal Price { get; set; }

        [DisplayName("Product Art URL")]
        [StringLength(1024)]
        public string ProductArtUrl { get; set; }

        public virtual Category Category { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; }
    }
}