using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RescateEmocional.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace RescateEmocional.Controllers
{
    [Authorize(Roles = "3")] // Aplica un filtro de autorización que requiere que el usuario tenga el rol "3" para acceder a las acciones de este controlador.
    public class DiarioController : Controller // Define la clase DiarioController, que hereda de la clase base Controller de ASP.NET Core MVC.
    {
        private readonly RescateEmocionalContext _context; // Declara un campo privado de solo lectura para almacenar el contexto de la base de datos RescateEmocional.

        public DiarioController(RescateEmocionalContext context) // Define el constructor de la clase DiarioController, que recibe una instancia de RescateEmocionalContext a través de la inyección de dependencias.
        {
            _context = context; // Asigna la instancia de RescateEmocionalContext recibida al campo privado _context.
        }

        public async Task<IActionResult> Index(Diario diario, int topRegistro = 10) // Define una acción asíncrona llamada Index que recibe un objeto Diario y un entero opcional topRegistro (con valor predeterminado de 10) como parámetros, y devuelve un IActionResult.
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value); // Obtiene el ID del usuario autenticado a partir de sus Claims (identidad) y lo convierte a un entero.

            var rescateEmocionalContext = _context.Diarios // Accede a la tabla Diarios a través del contexto de la base de datos.
                .Where(d => d.Idusuario == userId) // Filtra los diarios para obtener solo aquellos que pertenecen al usuario autenticado (comparando la propiedad Idusuario con el userId obtenido).
                .Include(d => d.IdusuarioNavigation); // Incluye la navegación a la entidad relacionada IdusuarioNavigation (probablemente la información del usuario) para que esté disponible en los resultados.

            var query = _context.Diarios.AsQueryable(); // Crea una variable de tipo IQueryable para construir la consulta de forma fluida. Inicialmente, representa todos los diarios.
            if (!string.IsNullOrWhiteSpace(diario.Titulo)) // Verifica si la propiedad Titulo del objeto Diario recibido no es nula ni contiene solo espacios en blanco.
                query = query.Where(d => d.Titulo.Contains(diario.Titulo)); // Si el título tiene valor, agrega una condición a la consulta para filtrar los diarios cuyo título contenga el valor proporcionado.
            if (diario.FechaCreacion != default(DateTime)) // Verifica si la propiedad FechaCreacion del objeto Diario recibido no tiene el valor predeterminado de DateTime (lo que indica que se proporcionó una fecha).
                query = query.Where(d => d.FechaCreacion.Date == diario.FechaCreacion.Date); // Si se proporcionó una fecha de creación, agrega una condición a la consulta para filtrar los diarios cuya fecha de creación coincida con la fecha proporcionada (ignorando la hora).

            query = query.OrderByDescending(d => d.Iddiario); // Ordena la consulta de forma descendente según la propiedad Iddiario, lo que mostrará los diarios más recientes primero.

            if (topRegistro > 0) // Verifica si el valor de topRegistro es mayor que cero.
                query = query.Take(topRegistro); // Si topRegistro es positivo, limita el número de resultados devueltos por la consulta a la cantidad especificada en topRegistro.

            return View(await query.ToListAsync()); // Ejecuta la consulta de forma asíncrona para obtener los resultados como una lista y los pasa a la vista predeterminada asociada a la acción Index.
        }

        // GET: Diario/Details/5 // Comentario que indica que la siguiente acción responde a una petición HTTP GET en la ruta "Diario/Details/{id}", donde {id} es un parámetro.
        public async Task<IActionResult> Details(int? id) // Define una acción asíncrona llamada Details que recibe un parámetro entero nullable llamado "id" y devuelve un IActionResult.
        {
            if (id == null) // Verifica si el parámetro "id" es nulo.
            {
                return NotFound(); // Si "id" es nulo, devuelve un resultado NotFound (código de estado HTTP 404).
            }

            var diario = await _context.Diarios // Consulta la tabla "Diarios" en la base de datos de forma asíncrona.
                .Include(d => d.IdusuarioNavigation) // Incluye la navegación a la entidad relacionada IdusuarioNavigation (probablemente la información del usuario) para que esté disponible en los resultados.
                .FirstOrDefaultAsync(m => m.Iddiario == id); // Busca el primer registro donde la propiedad "Iddiario" coincida con el valor del parámetro "id".
            if (diario == null) // Verifica si se encontró algún registro con el "id" proporcionado.
            {
                return NotFound(); // Si no se encuentra ningún diario con ese "id", devuelve un resultado NotFound (código de estado HTTP 404).
            }

            return View(diario); // Si se encuentra el diario, lo pasa a la vista predeterminada asociada a esta acción para mostrar los detalles del diario.
        }

        // GET: Diario/Create // Comentario que indica que la siguiente acción responde a una petición HTTP GET en la ruta "Diario/Create".
        public IActionResult Create() // Define una acción síncrona llamada Create que devuelve un IActionResult.
        {
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Idusuario"); // Prepara un SelectList para la propiedad Idusuario que se utilizará en la vista para un dropdown. Los datos se obtienen de la tabla Usuarios, utilizando "Idusuario" como valor y texto a mostrar.
            return View(); // Devuelve la vista predeterminada asociada a la acción Create, que probablemente contendrá un formulario para crear un nuevo diario.
        }

        // POST: Diario/Create // Comentario que indica que la siguiente acción responde a una petición HTTP POST en la ruta "Diario/Create".
        // To protect from overposting attacks, enable the specific properties you want to bind to. // Comentario que advierte sobre la protección contra ataques de sobrepublicación.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598. // Enlace a documentación de Microsoft sobre cómo prevenir la sobrepublicación.
        [HttpPost] // Atributo que especifica que esta acción solo responde a peticiones HTTP POST.
        [ValidateAntiForgeryToken] // Atributo que agrega una validación anti-falsificación de solicitudes para proteger contra ataques CSRF (Cross-Site Request Forgery).
        public async Task<IActionResult> Create([Bind("Iddiario,Idusuario,Titulo,Contenido,FechaCreacion")] Diario diario) // Define una acción asíncrona llamada Create que recibe un objeto Diario como parámetro. El atributo [Bind] especifica las propiedades del modelo Diario que se deben incluir en el enlace de datos desde el formulario.
        {
            if (ModelState.IsValid) // Verifica si el modelo Diario recibido es válido según las reglas de validación definidas en la clase Diario.
            {
                // Verificar si el usuario está autenticado // Comentario que explica el siguiente bloque de código.
                if (User.Identity.IsAuthenticated) // Verifica si el usuario actual está autenticado.
                {
                    // Obtener el Id del usuario autenticado desde los claims // Comentario que explica el siguiente bloque de código.
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value); // Obtiene el valor del claim con el tipo NameIdentifier (que usualmente contiene el ID del usuario) del usuario autenticado y lo convierte a un entero.

                    // Asignar el Idusuario al diario // Comentario que explica el siguiente bloque de código.
                    diario.Idusuario = userId; // Asigna el ID del usuario autenticado a la propiedad Idusuario del objeto Diario.
                }
                else
                {
                    // Si no está autenticado, redirigir al login // Comentario que explica el siguiente bloque de código.
                    return RedirectToAction("Login", "Account"); // Si el usuario no está autenticado, lo redirige a la acción "Login" del controlador "Account".
                }

                // Agregar el diario a la base de datos // Comentario que explica el siguiente bloque de código.
                _context.Add(diario); // Agrega la entidad Diario al contexto de la base de datos para que se rastree para su posterior inserción.
                await _context.SaveChangesAsync(); // Guarda de forma asíncrona todos los cambios realizados en el contexto de la base de datos, lo que en este caso insertará el nuevo diario en la tabla correspondiente.
                return RedirectToAction(nameof(Index)); // Redirige al usuario a la acción Index del mismo controlador (DiarioController), que probablemente mostrará la lista de diarios.
            }
            return View(diario); // Si el modelo no es válido, devuelve la vista asociada a la acción Create, pasando el objeto Diario para que se puedan mostrar los errores de validación en el formulario.
        }

        // GET: Diario/Edit/5 // Comentario que indica que la siguiente acción responde a una petición HTTP GET en la ruta "Diario/Edit/{id}", donde {id} es un parámetro.
        public async Task<IActionResult> Edit(int? id) // Define una acción asíncrona llamada Edit que recibe un parámetro entero nullable llamado "id" y devuelve un IActionResult.
        {
            if (id == null) // Verifica si el parámetro "id" es nulo.
            {
                return NotFound(); // Si "id" es nulo, devuelve un resultado NotFound (código de estado HTTP 404).
            }

            var diario = await _context.Diarios.FindAsync(id); // Busca de forma asíncrona un registro en la tabla "Diarios" cuya clave primaria coincida con el valor del parámetro "id".
            if (diario == null) // Verifica si se encontró algún registro con el "id" proporcionado.
            {
                return NotFound(); // Si no se encuentra ningún diario con ese "id", devuelve un resultado NotFound (código de estado HTTP 404).
            }
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Idusuario", diario.Idusuario); // Prepara un SelectList para la propiedad Idusuario que se utilizará en la vista para un dropdown. Los datos se obtienen de la tabla Usuarios, utilizando "Idusuario" como valor y texto a mostrar, y selecciona el valor actual de diario.Idusuario.
            return View(diario); // Devuelve la vista predeterminada asociada a la acción Edit, pasando el objeto Diario para que se puedan editar sus datos.
        }

        // POST: Diario/Edit/5 // Comentario que indica que la siguiente acción responde a una petición HTTP POST en la ruta "Diario/Edit/{id}", donde {id} es un parámetro.
        // To protect from overposting attacks, enable the specific properties you want to bind to. // Comentario que advierte sobre la protección contra ataques de sobrepublicación.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598. // Enlace a documentación de Microsoft sobre cómo prevenir la sobrepublicación.
        [HttpPost] // Atributo que especifica que esta acción solo responde a peticiones HTTP POST.
        [ValidateAntiForgeryToken] // Atributo que agrega una validación anti-falsificación de solicitudes para proteger contra ataques CSRF (Cross-Site Request Forgery).
        public async Task<IActionResult> Edit(int id, [Bind("Iddiario,Idusuario,Titulo,Contenido,FechaCreacion")] Diario diario) // Define una acción asíncrona llamada Edit que recibe un parámetro entero llamado "id" y un objeto Diario como parámetro. El atributo [Bind] especifica las propiedades del modelo Diario que se deben incluir en el enlace de datos desde el formulario.
        {
            if (id != diario.Iddiario) // Verifica si el valor del parámetro "id" coincide con la propiedad "Iddiario" del objeto Diario recibido. Esto es una medida de seguridad para evitar la manipulación del ID en la petición.
            {
                return NotFound(); // Si los IDs no coinciden, devuelve un resultado NotFound (código de estado HTTP 404).
            }

            if (ModelState.IsValid) // Verifica si el modelo Diario recibido es válido según las reglas de validación definidas en la clase Diario.
            {
                try // Inicia un bloque try para manejar posibles excepciones durante la actualización de la base de datos.
                {
                    _context.Update(diario); // Marca la entidad Diario en el contexto de la base de datos como modificada.
                    await _context.SaveChangesAsync(); // Guarda de forma asíncrona todos los cambios realizados en el contexto de la base de datos, lo que en este caso actualizará el registro del diario en la tabla correspondiente.
                }
                catch (DbUpdateConcurrencyException) // Captura una excepción que ocurre cuando varios usuarios intentan modificar el mismo registro simultáneamente.
                {
                    if (!DiarioExists(diario.Iddiario)) // Llama a un método privado (que se definirá más adelante) para verificar si el diario con el ID proporcionado todavía existe en la base de datos.
                    {
                        return NotFound(); // Si el diario ya no existe, devuelve un resultado NotFound (código de estado HTTP 404).
                    }
                    else
                    {
                        throw; // Si el diario existe pero ocurrió otra excepción de concurrencia, relanza la excepción para que sea manejada por un nivel superior.
                    }
                }
                return RedirectToAction(nameof(Index)); // Si la actualización se realiza con éxito, redirige al usuario a la acción Index del mismo controlador.
            }
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Idusuario", diario.Idusuario); // Si el modelo no es válido, vuelve a preparar el SelectList para Idusuario y lo asigna al ViewData.
            return View(diario); // Si el modelo no es válido, devuelve la vista asociada a la acción Edit, pasando el objeto Diario para que se puedan mostrar los errores de validación en el formulario.
        }

        // GET: Diario/Delete/5
        public async Task<IActionResult> Delete(int? id) // Define una acción asíncrona llamada Delete que recibe un parámetro entero nullable llamado "id" y devuelve un IActionResult.
        {
            if (id == null) // Verifica si el parámetro "id" es nulo.
            {
                return NotFound(); // Si "id" es nulo, devuelve un resultado NotFound (código de estado HTTP 404).
            }

            var diario = await _context.Diarios // Consulta la tabla "Diarios" en la base de datos de forma asíncrona.
                .Include(d => d.IdusuarioNavigation) // Incluye la navegación a la entidad relacionada IdusuarioNavigation (probablemente la información del usuario) para que esté disponible en los resultados.
                .FirstOrDefaultAsync(m => m.Iddiario == id); // Busca el primer registro donde la propiedad "Iddiario" coincida con el valor del parámetro "id".
            if (diario == null) // Verifica si se encontró algún registro con el "id" proporcionado.
            {
                return NotFound(); // Si no se encuentra ningún diario con ese "id", devuelve un resultado NotFound (código de estado HTTP 404).
            }

            return View(diario); // Si se encuentra el diario, lo pasa a la vista predeterminada asociada a la acción Delete, que probablemente mostrará una confirmación antes de eliminar el diario.
        }

        // POST: Diario/Delete/5 // Comentario que indica que la siguiente acción responde a una petición HTTP POST en la ruta "Diario/Delete/{id}", donde {id} es un parámetro.
        [HttpPost, ActionName("Delete")] // Atributo que especifica que esta acción solo responde a peticiones HTTP POST y que su nombre de acción es "Delete" (útil cuando el nombre del método es diferente, como "DeleteConfirmed").
        [ValidateAntiForgeryToken] // Atributo que agrega una validación anti-falsificación de solicitudes para proteger contra ataques CSRF (Cross-Site Request Forgery).
        public async Task<IActionResult> DeleteConfirmed(int id) // Define una acción asíncrona llamada DeleteConfirmed que recibe un parámetro entero llamado "id" y devuelve un IActionResult. Este método se llama después de que el usuario confirma la eliminación.
        {
            var diario = await _context.Diarios.FindAsync(id); // Busca de forma asíncrona un registro en la tabla "Diarios" cuya clave primaria coincida con el valor del parámetro "id".
            if (diario != null) // Verifica si se encontró el diario con el "id" proporcionado.
            {
                _context.Diarios.Remove(diario); // Marca la entidad Diario en el contexto de la base de datos para su posterior eliminación.
            }

            await _context.SaveChangesAsync(); // Guarda de forma asíncrona todos los cambios realizados en el contexto de la base de datos, lo que en este caso eliminará el registro del diario de la tabla correspondiente.
            return RedirectToAction(nameof(Index)); // Redirige al usuario a la acción Index del mismo controlador, que probablemente mostrará la lista actualizada de diarios.
        }

        private bool DiarioExists(int id) // Define un método privado que toma un entero "id" como parámetro y devuelve un valor booleano.
        {
            return _context.Diarios.Any(e => e.Iddiario == id); // Utiliza LINQ para verificar si existe algún registro en la tabla "Diarios" donde la propiedad "Iddiario" coincida con el valor del parámetro "id". Devuelve true si existe al menos un registro, y false en caso contrario.
        }
    }
}