using System;
using System.Collections.Generic;

namespace Lopushok.Models;

public partial class Material
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int Countinpack { get; set; }

    public string Unit { get; set; } = null!;

    public int? Countinstock { get; set; }

    public int Mincount { get; set; }

    public decimal Cost { get; set; }

    public int Materialtypeid { get; set; }

    public virtual ICollection<Materialcounthistory> Materialcounthistories { get; set; } = new List<Materialcounthistory>();

    public virtual Materialtype Materialtype { get; set; } = null!;

    public virtual ICollection<Productmaterial> Productmaterials { get; set; } = new List<Productmaterial>();

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}
