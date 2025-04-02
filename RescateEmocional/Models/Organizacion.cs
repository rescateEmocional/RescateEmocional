using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Necesario para Data Annotations
using System.ComponentModel.DataAnnotations.Schema;

namespace RescateEmocional.Models;

public partial class Organizacion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Idorganizacion { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres.")]
    public string Descripcion { get; set; } = null!;

    [Required(ErrorMessage = "El horario es obligatorio.")]
    [StringLength(50, ErrorMessage = "El horario no puede exceder los 50 caracteres.")]
    public string Horario { get; set; } = null!;

    [Required(ErrorMessage = "La ubicación es obligatoria.")]
    [StringLength(200, ErrorMessage = "La ubicación no puede exceder los 200 caracteres.")]
    public string Ubicacion { get; set; } = null!;

    [Range(0, 1, ErrorMessage = "El estado debe ser 0 (inactivo) o 1 (activo).")]
    public byte Estado { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio.")]
    public int Idrol { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
    [StringLength(254, ErrorMessage = "El correo no puede exceder los 254 caracteres.")]
    public string CorreoElectronico { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "La contraseña debe incluir mayúsculas, minúsculas, números y caracteres especiales."
    )]
    public string Contrasena { get; set; } = null!;

    // Relaciones (no requieren validaciones adicionales aquí)
    public virtual ICollection<Conversacion> Conversacions { get; set; } = new List<Conversacion>();
    public virtual Rol? IdrolNavigation { get; set; } = null!;
    public virtual ICollection<PeticionVerificacion> PeticionVerificacions { get; set; } = new List<PeticionVerificacion>();
    public virtual ICollection<Administrador> Idadmins { get; set; } = new List<Administrador>();
    public virtual ICollection<Etiquetum> Idetiqueta { get; set; } = new List<Etiquetum>();
    public virtual ICollection<Telefono> Idtelefonos { get; set; } = new List<Telefono>();
}