using System;
using System.Collections.Generic;

namespace RescateEmocional.Models;

public partial class Conversacion
{
    public int Idconversacion { get; set; }

    public int Idusuario { get; set; }

    public int Idorganizacion { get; set; }

    public DateTime FechaInicio { get; set; }

    public string? Mensaje { get; set; }

    public virtual Organizacion? IdorganizacionNavigation { get; set; } = null!;

    public virtual Usuario? IdusuarioNavigation { get; set; } = null!;

    public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
}
