using System;
using System.Collections.Generic;

namespace RescateEmocional.Models;

// Clase parcial que representa una organización en el sistema RescateEmocional.
// Esta clase forma parte del modelo de datos, probablemente generada por Entity Framework Core,
// y define la estructura de una entidad "Organización" en la base de datos.
public partial class Organizacion
{
    // Identificador único de la organización. Actúa como la clave primaria en la base de datos.
    public int Idorganizacion { get; set; }

    // Nombre de la organización. Es un campo obligatorio (no puede ser nulo).
    public string Nombre { get; set; } = null!;

    // Descripción detallada de la organización. También es un campo obligatorio.
    public string Descripcion { get; set; } = null!;

    // Horario de la organización, como las horas de operación o disponibilidad. Campo obligatorio.
    public string Horario { get; set; } = null!;

    // Ubicación de la organización, ya sea física o virtual. Campo obligatorio.
    public string Ubicacion { get; set; } = null!;

    // Estado de la organización, representado como un byte.
    // Por ejemplo, podría ser 0 para inactivo y 1 para activo.
    public byte Estado { get; set; }

    // Clave foránea que referencia el rol de la organización en el sistema.
    // Relaciona esta organización con una entrada en la tabla de roles.
    public int Idrol { get; set; }

    // Colección de conversaciones asociadas con esta organización.
    // Representa una relación uno-a-muchos con la entidad Conversacion.
    // Inicializada como una lista vacía para evitar referencias nulas.
    public virtual ICollection<Conversacion> Conversacions { get; set; } = new List<Conversacion>();

    // Propiedad de navegación que permite acceder al rol asociado con esta organización.
    // Relaciona directamente esta entidad con un objeto de la clase Rol.
    // Puede ser nulo si no se ha cargado el rol relacionado.
    public virtual Rol? IdrolNavigation { get; set; } = null!;

    // Colección de peticiones de verificación relacionadas con esta organización.
    // Representa una relación uno-a-muchos con la entidad PeticionVerificacion.
    public virtual ICollection<PeticionVerificacion> PeticionVerificacions { get; set; } = new List<PeticionVerificacion>();

    // Colección de administradores asociados con esta organización.
    // Representa una relación uno-a-muchos con la entidad Administrador.
    public virtual ICollection<Administrador> Idadmins { get; set; } = new List<Administrador>();

    // Colección de etiquetas relacionadas con esta organización.
    // Representa una relación uno-a-muchos con la entidad Etiquetum.
    public virtual ICollection<Etiquetum> Idetiqueta { get; set; } = new List<Etiquetum>();

    // Colección de teléfonos asociados con esta organización.
    // Representa una relación uno-a-muchos con la entidad Telefono.
    public virtual ICollection<Telefono> Idtelefonos { get; set; } = new List<Telefono>();
}