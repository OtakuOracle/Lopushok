using System;
using System.Collections.Generic;

namespace Lopushok.Models;

public partial class Materialtype
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
