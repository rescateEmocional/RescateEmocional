using System;
using System.Collections.Generic;

namespace RescateEmocional.Models;

public partial class Mensaje
{
    public int Idmensaje { get; set; }

    public int Idconversacion { get; set; }

    public string Contenido { get; set; } = null!;

    public DateTime FechaHora { get; set; }

    public byte Estado { get; set; }

    public virtual Conversacion IdconversacionNavigation { get; set; } = null!;
}
