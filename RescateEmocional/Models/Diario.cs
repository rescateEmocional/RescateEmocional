using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RescateEmocional.Models;

public partial class Diario
{
    public int Iddiario { get; set; }

    public int Idusuario { get; set; }

    [Required(ErrorMessage = "El titulo es obligatorio.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = null!;

    [MaxLength(5000)]
    [Required(ErrorMessage = "El contenido es obligatorio.")]
    public string Contenido { get; set; } = null!;

    [Display(Name = "Fecha")]
    [Required(ErrorMessage = "La fecha es obligatorio.")]
    public DateTime FechaCreacion { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; } = null!;
}
