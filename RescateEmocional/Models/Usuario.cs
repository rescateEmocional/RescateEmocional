using System;
using System.Collections.Generic;

namespace RescateEmocional.Models;

public partial class Usuario
{
    public int Idusuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string CorreoElectronico { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public byte Estado { get; set; }

    public int Idrol { get; set; }

    public virtual ICollection<Conversacion> Conversacions { get; set; } = new List<Conversacion>();

    public virtual ICollection<Diario> Diarios { get; set; } = new List<Diario>();

    public virtual Rol? IdrolNavigation { get; set; } = null!;

    public virtual ICollection<Administrador> Idadmins { get; set; } = new List<Administrador>();
}
