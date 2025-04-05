using System;
using System.Collections.Generic;

namespace RescateEmocional.Models
{
    public partial class Conversacion
    {
        public int Idconversacion { get; set; }

        public int Idusuario { get; set; }

        public int Idorganizacion { get; set; }

        public DateTime FechaInicio { get; set; } = DateTime.Now;

        public string? Mensaje { get; set; }

        // Nueva propiedad agregada
        public string? Emisor { get; set; }

        // Navegación hacia el usuario que creó la conversación
        public virtual Usuario? IdusuarioNavigation { get; set; }

        // Navegación hacia la organización con la que se conversa
        public virtual Organizacion? IdorganizacionNavigation { get; set; }

        // Lista de mensajes relacionados a la conversación
        public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
    }
}
