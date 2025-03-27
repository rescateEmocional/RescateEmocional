using System;
using System.Collections.Generic;

namespace RescateEmocional.Models;

public partial class Telefono
{
    public int Idtelefono { get; set; }

    public string TipoDeNumero { get; set; } = null!;

    public virtual ICollection<Organizacion> Idorganizacions { get; set; } = new List<Organizacion>();
}
