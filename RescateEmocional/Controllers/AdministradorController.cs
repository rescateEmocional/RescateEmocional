using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RescateEmocional.Models;

namespace RescateEmocional.Controllers
{
    // Este controlador solo puede ser accedido por usuarios con el rol "1"
    [Authorize(Roles = "1")]
    public class AdministradorController : Controller
    {
        // Contexto de base de datos para interactuar con la entidad RescateEmocional
        private readonly RescateEmocionalContext _context;

        // Constructor que recibe el contexto y lo asigna a una variable privada
        public AdministradorController(RescateEmocionalContext context)
        {
            _context = context;
        }

        // Acción que devuelve una lista de administradores según los filtros aplicados
        // Se puede filtrar por nombre, correo electrónico y limitar el número de registros (por defecto 10)
        public async Task<IActionResult> Index(Administrador administrador, int topRegistro = 10)
        {
            // Se obtiene una consulta de todos los administradores
            var query = _context.Administradors.AsQueryable();

            // Si el nombre no está vacío, se filtra por nombre
            if (!string.IsNullOrWhiteSpace(administrador.Nombre))
                query = query.Where(a => a.Nombre.Contains(administrador.Nombre));

            // Si el correo electrónico no está vacío, se filtra por correo electrónico
            if (!string.IsNullOrWhiteSpace(administrador.CorreoElectronico))
                query = query.Where(a => a.CorreoElectronico.Contains(administrador.CorreoElectronico));

            // Se ordena la consulta por Idadmin de forma descendente
            query = query.OrderByDescending(a => a.Idadmin);

            // Si se especificó un límite de registros, se toma solo esa cantidad
            if (topRegistro > 0)
                query = query.Take(topRegistro);

            // Se incluye la relación con la tabla de roles (navegación)
            query = query.Include(a => a.IdrolNavigation);

            // Se ejecuta la consulta y se pasa la lista resultante a la vista
            return View(await query.ToListAsync());
        }

        // GET: Administrador/Details/5 // Comentario que indica que la siguiente acción responde a una petición HTTP GET en la ruta "Administrador/Details/{id}", donde {id} es un parámetro.
        public async Task<IActionResult> Details(int? id) // Define una acción asíncrona llamada Details que recibe un parámetro entero nullable llamado "id" y devuelve un IActionResult.
        {
            if (id == null) // Verifica si el parámetro "id" es nulo.
            {
                return NotFound(); // Si "id" es nulo, devuelve un resultado NotFound (código de estado HTTP 404).
            }

            var administrador = await _context.Administradors // Consulta la tabla "Administradors" en la base de datos de forma asíncrona.
                .Include(a => a.IdrolNavigation) // Incluye la navegación a la entidad relacionada IdrolNavigation (probablemente la información del rol del administrador) para que esté disponible en los resultados.
                .FirstOrDefaultAsync(m => m.Idadmin == id); // Busca el primer registro donde la propiedad "Idadmin" coincida con el valor del parámetro "id".
            if (administrador == null) // Verifica si se encontró algún registro con el "id" proporcionado.
            {
                return NotFound(); // Si no se encuentra ningún administrador con ese "id", devuelve un resultado NotFound (código de estado HTTP 404).
            }

            return View(administrador); // Si se encuentra el administrador, lo pasa a la vista predeterminada asociada a esta acción para mostrar los detalles del administrador.
        }

        // GET: Administrador/Create Logica de crear
        public IActionResult Create() // Acción que responde a solicitudes HTTP GET para mostrar el formulario de creación de un nuevo administrador
        {
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Nombre"); // Llena un SelectList con los roles disponibles, usando "Idrol" como valor y "Nombre" como texto a mostrar
            return View(); // Devuelve la vista para que el usuario complete el formulario de creación
        }

        // POST: Administrador/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] // Indica que esta acción responde a solicitudes HTTP POST
        [ValidateAntiForgeryToken] // Previene ataques CSRF validando el token antifalsificación
        public async Task<IActionResult> Create([Bind("Idadmin,Nombre,CorreoElectronico,Contrasena,Idrol")] Administrador administrador) // Acción que recibe los datos del formulario para crear un nuevo administrador. Se limita el enlace de datos a las propiedades indicadas
        {
            if (ModelState.IsValid) // Verifica si el modelo cumple con todas las validaciones
            {
                try
                {
                    // Verificar si el correo electrónico ya existe en la base de datos
                    var correoExistente = await _context.Administradors
                        .FirstOrDefaultAsync(a => a.CorreoElectronico == administrador.CorreoElectronico); // Busca si ya hay un administrador con el mismo correo

                    if (correoExistente != null) // Si encuentra un correo duplicado
                    {
                        // Si el correo ya existe, agregar un error al ModelState
                        ModelState.AddModelError("CorreoElectronico", "Este correo electrónico ya está registrado."); // Agrega mensaje de error personalizado
                    }
                    else
                    {
                        // Si el correo no existe, proceder con la creación del administrador
                        administrador.Contrasena = CalcularHashMD5(administrador.Contrasena); // Hashea la contraseña antes de guardarla
                        _context.Add(administrador); // Agrega el nuevo administrador al contexto
                        await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

                        // Redirigir si todo salió bien
                        return RedirectToAction(nameof(Index)); // Redirige al índice (lista de administradores)
                    }
                }
                catch (Exception ex) // Si ocurre un error durante el proceso
                {
                    // Si ocurre algún otro error, agregar un error general al ModelState
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar el administrador."); // Muestra mensaje genérico de error
                }
            }

            // Recargar datos necesarios para la vista
            return View(administrador); // Devuelve la vista
        }


        // GET: Administrador/Edit/5
        public async Task<IActionResult> Edit(int? id) // Acción que se ejecuta cuando el usuario quiere editar un administrador
        {
            if (id == null) // Verifica si el id proporcionado es nulo
            {
                return NotFound(); // Si no hay id, devuelve error 404
            }

            var administrador = await _context.Administradors.FindAsync(id); // Busca el administrador por su id
            if (administrador == null) // Si no se encuentra
            {
                return NotFound(); // Retorna error 404
            }

            // Carga los roles disponibles y selecciona el rol actual del administrador
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Nombre", administrador.Idrol);

            return View(administrador); // Devuelve la vista con los datos del administrador cargados
        }


        // POST: Administrador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] // Indica que esta acción responde a solicitudes POST
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF
        public async Task<IActionResult> Edit(int id, [Bind("Idadmin,Nombre,CorreoElectronico,Idrol")] Administrador administrador) // Recibe los datos modificados del formulario
        {
            if (id != administrador.Idadmin) // Verifica que el ID recibido coincida con el del administrador
            {
                return NotFound(); // Si no coincide, retorna 404
            }

            // Busca el administrador actual en la base de datos
            var adminUpdate = await _context.Administradors.FirstOrDefaultAsync(m => m.Idadmin == administrador.Idadmin);
            if (adminUpdate == null) // Si no se encuentra el administrador
            {
                return NotFound(); // Retorna 404
            }

            try
            {
                // Verifica si el correo ya existe para otro administrador
                var correoExistente = await _context.Administradors
                    .FirstOrDefaultAsync(a => a.CorreoElectronico == administrador.CorreoElectronico && a.Idadmin != administrador.Idadmin);

                if (correoExistente != null) // Si el correo ya está registrado por otro
                {
                    // Agrega mensaje de error al modelo
                    ModelState.AddModelError("CorreoElectronico", "Este correo electrónico ya está registrado por otro administrador.");
                    // Recarga la lista de roles para volver a mostrar el formulario correctamente
                    ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Nombre", administrador.Idrol);
                    return View(administrador); // Devuelve la vista con errores
                }

                // Actualiza los campos del administrador
                adminUpdate.Nombre = administrador.Nombre;
                adminUpdate.CorreoElectronico = administrador.CorreoElectronico;
                adminUpdate.Idrol = administrador.Idrol;

                _context.Update(adminUpdate); // Marca la entidad como modificada
                await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos
                return RedirectToAction(nameof(Index)); // Redirige al listado
            }
            catch (DbUpdateConcurrencyException) // Captura errores de concurrencia (si alguien más modificó el registro al mismo tiempo)
            {
                if (!AdministradorExists(administrador.Idadmin)) // Verifica si el administrador aún existe
                {
                    return NotFound(); // Si no existe, retorna 404
                }
                else
                {
                    // Agrega error general al modelo
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al intentar actualizar el administrador.");
                    // Recarga lista de roles
                    ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Nombre", administrador.Idrol);
                    return View(administrador); // Devuelve la vista con el error
                }
            }
        }

        // GET: Administrador/Delete/5
        public async Task<IActionResult> Delete(int? id) // Acción GET para mostrar el formulario de confirmación de eliminación
        {
            if (id == null) // Verifica si el id proporcionado es nulo
            {
                return NotFound(); // Si no hay id, devuelve error 404
            }

            // Busca al administrador con el id proporcionado y carga también los datos relacionados del rol
            var administrador = await _context.Administradors
                .Include(a => a.IdrolNavigation) // Incluye los datos del rol asociado al administrador
                .FirstOrDefaultAsync(m => m.Idadmin == id); // Busca el administrador por su id
            if (administrador == null) // Si no se encuentra el administrador
            {
                return NotFound(); // Retorna error 404
            }

            return View(administrador); // Devuelve la vista con los datos del administrador para confirmar su eliminación
        }

        // POST: Administrador/Delete/5
        [HttpPost, ActionName("Delete")] // Indica que esta acción responde a un formulario POST con el nombre "Delete"
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF (Cross-Site Request Forgery)
        public async Task<IActionResult> DeleteConfirmed(int id) // Acción POST para confirmar la eliminación
        {
            var administrador = await _context.Administradors.FindAsync(id); // Busca el administrador por su id
            if (administrador != null) // Si se encuentra el administrador
            {
                _context.Administradors.Remove(administrador); // Elimina al administrador de la base de datos
            }

            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos
            return RedirectToAction(nameof(Index)); // Redirige al usuario a la lista de administradores (acción Index)
        }

        // Método privado que verifica si un administrador con el id dado existe en la base de datos
        private bool AdministradorExists(int id)
        {
            return _context.Administradors.Any(e => e.Idadmin == id); // Utiliza el método Any para verificar si existe un administrador con el id proporcionado
        }

        // Método privado para calcular el hash MD5 de una cadena de entrada (por ejemplo, una contraseña)
        private string CalcularHashMD5(string input)
        {
            using (MD5 md5 = MD5.Create()) // Crea una instancia de MD5 para calcular el hash
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input); // Convierte la cadena de entrada en un arreglo de bytes utilizando codificación UTF-8
                byte[] hashBytes = md5.ComputeHash(inputBytes); // Calcula el hash MD5 de los bytes de la entrada

                StringBuilder sb = new StringBuilder(); // StringBuilder para construir la cadena de salida en formato hexadecimal
                for (int i = 0; i < hashBytes.Length; i++) // Recorre cada byte del hash
                {
                    sb.Append(hashBytes[i].ToString("x2")); // "x2" convierte el byte en una cadena hexadecimal de dos caracteres.
                }
                return sb.ToString(); // Devuelve el hash como una cadena hexadecimal
            }
        }
    }
}