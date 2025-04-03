using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RescateEmocional.Models;

public partial class Rol
{
    public int Idrol { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = null!;

    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    public virtual ICollection<Administrador> Administradors { get; set; } = new List<Administrador>();

    public virtual ICollection<Organizacion> Organizacions { get; set; } = new List<Organizacion>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
