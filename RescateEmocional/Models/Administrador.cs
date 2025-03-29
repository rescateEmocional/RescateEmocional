using System;
using System.Collections.Generic;

namespace RescateEmocional.Models;

public partial class Administrador
{
    public int Idadmin { get; set; }

    public string Nombre { get; set; } = null!;

    public string CorreoElectronico { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public int Idrol { get; set; }

    public virtual Rol? IdrolNavigation { get; set; } = null!;

    public virtual ICollection<PeticionVerificacion> PeticionVerificacions { get; set; } = new List<PeticionVerificacion>();

    public virtual ICollection<Organizacion> Idorganizacions { get; set; } = new List<Organizacion>();

    public virtual ICollection<Usuario> Idusuarios { get; set; } = new List<Usuario>();
}
