using System;
using System.Collections.Generic;

namespace Lopushok.Models;

public partial class Materialsupplier
{
    public int Materialsupplierid { get; set; }

    public int Materialid { get; set; }

    public int Supplierid { get; set; }

    public virtual Material Material { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;
}
