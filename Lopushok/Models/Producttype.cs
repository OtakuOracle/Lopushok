using System;
using System.Collections.Generic;

namespace Lopushok.Models;

public partial class Producttype
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
