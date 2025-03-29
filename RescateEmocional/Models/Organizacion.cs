using System;
using System.Collections.Generic;

namespace RescateEmocional.Models;

public partial class Organizacion
{
    public int Idorganizacion { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string Horario { get; set; } = null!;

    public string Ubicacion { get; set; } = null!;

    public byte Estado { get; set; }

    public int Idrol { get; set; }

    public string CorreoElectronico { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public virtual ICollection<Conversacion> Conversacions { get; set; } = new List<Conversacion>();

    public virtual Rol? IdrolNavigation { get; set; } = null!;

    public virtual ICollection<PeticionVerificacion> PeticionVerificacions { get; set; } = new List<PeticionVerificacion>();

    public virtual ICollection<Administrador> Idadmins { get; set; } = new List<Administrador>();

    public virtual ICollection<Etiquetum> Idetiqueta { get; set; } = new List<Etiquetum>();

    public virtual ICollection<Telefono> Idtelefonos { get; set; } = new List<Telefono>();
}
