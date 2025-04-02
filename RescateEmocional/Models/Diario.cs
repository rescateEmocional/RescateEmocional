using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RescateEmocional.Models;

public partial class Diario
{
    public int Iddiario { get; set; }

    public int Idusuario { get; set; }

    [Required(ErrorMessage = "El titulo es obligatorio.")]

    public string Titulo { get; set; } = null!;

    [MaxLength(2000)]
    [Required(ErrorMessage = "El contenido es obligatorio.")]
    public string Contenido { get; set; } = null!;

    [Required(ErrorMessage = "La fecha es obligatorio.")]
    public DateTime FechaCreacion { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; } = null!;
}
