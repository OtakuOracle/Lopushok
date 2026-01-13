using System;
using System.Collections.Generic;

namespace Lopushok.Models;

public partial class Productmaterial
{
    public int Productid { get; set; }

    public int Materialid { get; set; }

    public int? Quantity { get; set; }

    public virtual Material Material { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
