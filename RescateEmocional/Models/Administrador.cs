using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace RescateEmocional.Models;

public partial class Administrador
{

    public int Idadmin { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = null!;

    [Display(Name = "Correo electrónico")]
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    public string CorreoElectronico { get; set; } = null!;

    [Required(ErrorMessage = "El contraseña es obligatorio.")]
    [DataType(DataType.Password)]
    [StringLength(255, MinimumLength = 5, ErrorMessage = "El passowrd debe tener entre 5 y 255 caracteres.")]
    public string Contrasena { get; set; } = null!;
    [Display(Name = "Rol")]
    [Required(ErrorMessage = "El Rol es obligatorio.")]
    public int Idrol { get; set; }
    [Display(Name = "Rol")]
    public virtual Rol? IdrolNavigation { get; set; } = null!;

    public virtual ICollection<PeticionVerificacion> PeticionVerificacions { get; set; } = new List<PeticionVerificacion>();

    public virtual ICollection<Organizacion> Idorganizacions { get; set; } = new List<Organizacion>();

    public virtual ICollection<Usuario> Idusuarios { get; set; } = new List<Usuario>();
}
