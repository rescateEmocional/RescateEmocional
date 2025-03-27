using System;
using System.Collections.Generic;

namespace RescateEmocional.Models;

public partial class PeticionVerificacion
{
    public int Idpeticion { get; set; }

    public int Idorganizacion { get; set; }

    public byte Estado { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public string? Comentarios { get; set; }

    public int Idadmin { get; set; }

    public virtual Administrador IdadminNavigation { get; set; } = null!;

    public virtual Organizacion IdorganizacionNavigation { get; set; } = null!;
}
