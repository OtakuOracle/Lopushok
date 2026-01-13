using System;
using System.Collections.Generic;

namespace Lopushok.Models;

public partial class Agentpriorityhistory
{
    public int Id { get; set; }

    public int Agentid { get; set; }

    public DateTime Changedate { get; set; }

    public int Priorityvalue { get; set; }

    public virtual Agent Agent { get; set; } = null!;
}
