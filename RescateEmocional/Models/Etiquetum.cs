using System;
using System.Collections.Generic;

namespace RescateEmocional.Models;

public partial class Etiquetum
{
    public int Idetiqueta { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Organizacion> Idorganizacions { get; set; } = new List<Organizacion>();
}
