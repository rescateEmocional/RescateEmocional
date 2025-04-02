using System;
using System.Collections.Generic;

namespace RescateEmocional.Models;

public partial class Diario
{
    public int Iddiario { get; set; }

    public int Idusuario { get; set; }

    public string Titulo { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; } = null!;
}
