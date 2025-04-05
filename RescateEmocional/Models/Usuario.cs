using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RescateEmocional.Models;

public partial class Usuario
{
    public int Idusuario { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Debe proporcionar un correo electrónico válido.")]
    public string CorreoElectronico { get; set; } = null!;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [Phone(ErrorMessage = "Debe proporcionar un número de teléfono válido.")]
    public string Telefono { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(50, ErrorMessage = "La contraseña no puede tener más de 50 caracteres.")]
    public string Contrasena { get; set; } = null!;

    public byte Estado { get; set; }

    public int Idrol { get; set; }

    public virtual ICollection<Conversacion> Conversacions { get; set; } = new List<Conversacion>();

    public virtual ICollection<Diario> Diarios { get; set; } = new List<Diario>();

    public virtual Rol? IdrolNavigation { get; set; } = null!;

    public virtual ICollection<Administrador> Idadmins { get; set; } = new List<Administrador>();
}
