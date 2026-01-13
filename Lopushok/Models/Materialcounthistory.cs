using System;
using System.Collections.Generic;

namespace Lopushok.Models;

public partial class Materialcounthistory
{
    public int Id { get; set; }

    public int Materialid { get; set; }

    public DateTime Changedate { get; set; }

    public double Countvalue { get; set; }

    public virtual Material Material { get; set; } = null!;
}
