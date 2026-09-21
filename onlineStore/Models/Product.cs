using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onlineStore.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category Category { get; set; } = null!;

    [NotMapped]
    [Required(ErrorMessage = "Please upload a product image.")]
    public virtual IFormFile? ImageFile { get; set; }
}
