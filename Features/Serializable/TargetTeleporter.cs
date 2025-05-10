using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMER.Features.Serializable;

public class TargetTeleporter
{
    public TargetTeleporter()
    {

    }

    public TargetTeleporter(int id, float chance)
    {
        Id = id;
        Chance = chance;
    }

    public int Id { get; set; }

    public float Chance { get; set; } = 100.0f;
}

