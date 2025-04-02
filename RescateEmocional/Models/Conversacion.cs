using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RescateEmocional.Models
{
    public partial class Conversacion
    {
        public int Idconversacion { get; set; }
        public int Idusuario { get; set; }
        public int Idorganizacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public string? Mensaje { get; set; }

        // Nueva propiedad para identificar el remitente
        [Required]
        public string Emisor { get; set; } = string.Empty;

        public virtual Organizacion? IdorganizacionNavigation { get; set; }
        public virtual Usuario? IdusuarioNavigation { get; set; }
        public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
    }
}
