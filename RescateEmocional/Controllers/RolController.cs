using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RescateEmocional.Models;

namespace RescateEmocional.Controllers // Define el espacio de nombres donde reside este controlador.
{
    [Authorize(Roles = "1")] // Aplica un filtro de autorización que requiere que el usuario tenga el rol "1" para acceder a las acciones de este controlador.
    public class RolController : Controller // Define la clase RolController, que hereda de la clase base Controller de ASP.NET Core MVC.
    {
        private readonly RescateEmocionalContext _context; // Declara un campo privado de solo lectura para almacenar el contexto de la base de datos RescateEmocional.

        public RolController(RescateEmocionalContext context) // Define el constructor de la clase RolController, que recibe una instancia de RescateEmocionalContext a través de la inyección de dependencias.
        {
            _context = context; // Asigna la instancia de RescateEmocionalContext recibida al campo privado _context.
        }

        // GET: Rol // Comentario que indica que la siguiente acción responde a una petición HTTP GET en la ruta base del controlador "Rol".
        public async Task<IActionResult> Index() // Define una acción asíncrona llamada Index que devuelve un IActionResult. ver los roles
        {
            return View(await _context.Rols.ToListAsync()); // Recupera todos los registros de la tabla "Rols" de la base de datos de forma asíncrona y los pasa a la vista predeterminada asociada a esta acción.
        }

        // GET: Rol/Details/5 // Comentario que indica que la siguiente acción responde a una petición HTTP GET en la ruta "Rol/Details/{id}", donde {id} es un parámetro.
        public async Task<IActionResult> Details(int? id) // Define una acción asíncrona llamada Details que recibe un parámetro entero nullable llamado "id" y devuelve un IActionResult.
        {
            if (id == null) // Verifica si el parámetro "id" es nulo.
            {
                return NotFound(); // Si "id" es nulo, devuelve un resultado NotFound (código de estado HTTP 404).
            }

            var rol = await _context.Rols // Consulta la tabla "Rols" en la base de datos de forma asíncrona.
                .FirstOrDefaultAsync(m => m.Idrol == id); // Busca el primer registro donde la propiedad "Idrol" coincida con el valor del parámetro "id".
            if (rol == null) // Verifica si se encontró algún registro con el "id" proporcionado.
            {
                return NotFound(); // Si no se encuentra ningún rol con ese "id", devuelve un resultado NotFound (código de estado HTTP 404).
            }

            return View(rol); // Si se encuentra el rol, lo pasa a la vista predeterminada asociada a esta acción para mostrar los detalles.
        }

        // GET: Rol/Create // Comentario que indica que la siguiente acción responde a una petición HTTP GET en la ruta "Rol/Create".
        public IActionResult Create() // Define una acción síncrona llamada Create que devuelve un IActionResult.
        {
            return View(); // Devuelve la vista predeterminada asociada a la acción Create, que probablemente contendrá un formulario para crear un nuevo rol.
        }

        // POST: Rol/Create // Comentario que indica que la siguiente acción responde a una petición HTTP POST en la ruta "Rol/Create".
        // To protect from overposting attacks, enable the specific properties you want to bind to. // Comentario que advierte sobre la protección contra ataques de sobrepublicación.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598. // Enlace a documentación de Microsoft sobre cómo prevenir la sobrepublicación.
        [HttpPost] // Atributo que especifica que esta acción solo responde a peticiones HTTP POST.
        [ValidateAntiForgeryToken] // Atributo que agrega una validación anti-falsificación de solicitudes para proteger contra ataques CSRF (Cross-Site Request Forgery).
        public async Task<IActionResult> Create([Bind("Idrol,Nombre,Descripcion")] Rol rol) // Define una acción asíncrona llamada Create que recibe un objeto Rol como parámetro. El atributo [Bind] especifica las propiedades del modelo Rol que se deben incluir en el enlace de datos desde el formulario.
        {
            if (ModelState.IsValid) // Verifica si el modelo Rol recibido es válido según las reglas de validación definidas en la clase Rol (por ejemplo, mediante Data Annotations).
            {
                _context.Add(rol); // Agrega la entidad Rol al contexto de la base de datos para que se rastree para su posterior inserción.
                await _context.SaveChangesAsync(); // Guarda de forma asíncrona todos los cambios realizados en el contexto de la base de datos, lo que en este caso insertará el nuevo rol en la tabla correspondiente.
                return RedirectToAction(nameof(Index)); // Redirige al usuario a la acción Index del mismo controlador (RolController), que probablemente mostrará la lista de roles.
            }
            return View(rol); // Si el modelo no es válido, devuelve la vista asociada a la acción Create, pasando el objeto Rol para que se puedan mostrar los errores de validación en el formulario.
        }

        // GET: Rol/Edit/5 // Comentario que indica que la siguiente acción responde a una petición HTTP GET en la ruta "Rol/Edit/{id}", donde {id} es un parámetro. editar rol
        public async Task<IActionResult> Edit(int? id) // Define una acción asíncrona llamada Edit que recibe un parámetro entero nullable llamado "id" y devuelve un IActionResult.
        {
            if (id == null) // Verifica si el parámetro "id" es nulo.
            {
                return NotFound(); // Si "id" es nulo, devuelve un resultado NotFound (código de estado HTTP 404).
            }

            var rol = await _context.Rols.FindAsync(id); // Busca de forma asíncrona un registro en la tabla "Rols" cuya clave primaria coincida con el valor del parámetro "id".
            if (rol == null) // Verifica si se encontró algún registro con el "id" proporcionado.
            {
                return NotFound(); // Si no se encuentra ningún rol con ese "id", devuelve un resultado NotFound (código de estado HTTP 404).
            }
            return View(rol); // Si se encuentra el rol, lo pasa a la vista predeterminada asociada a la acción Edit, que probablemente contendrá un formulario para editar los datos del rol.
        }

        // POST: Rol/Edit/5 // Comentario que indica que la siguiente acción responde a una petición HTTP POST en la ruta "Rol/Edit/{id}", donde {id} es un parámetro.
        // To protect from overposting attacks, enable the specific properties you want to bind to. // Comentario que advierte sobre la protección contra ataques de sobrepublicación.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598. // Enlace a documentación de Microsoft sobre cómo prevenir la sobrepublicación.
        [HttpPost] // Atributo que especifica que esta acción solo responde a peticiones HTTP POST.
        [ValidateAntiForgeryToken] // Atributo que agrega una validación anti-falsificación de solicitudes para proteger contra ataques CSRF (Cross-Site Request Forgery).
        public async Task<IActionResult> Edit(int id, [Bind("Idrol,Nombre,Descripcion")] Rol rol) // Define una acción asíncrona llamada Edit que recibe un parámetro entero llamado "id" y un objeto Rol como parámetro. El atributo [Bind] especifica las propiedades del modelo Rol que se deben incluir en el enlace de datos desde el formulario.
        {
            if (id != rol.Idrol) // Verifica si el valor del parámetro "id" coincide con la propiedad "Idrol" del objeto Rol recibido. Esto es una medida de seguridad para evitar la manipulación del ID en la petición.
            {
                return NotFound(); // Si los IDs no coinciden, devuelve un resultado NotFound (código de estado HTTP 404).
            }

            if (ModelState.IsValid) // Verifica si el modelo Rol recibido es válido según las reglas de validación definidas en la clase Rol.
            {
                try // Inicia un bloque try para manejar posibles excepciones durante la actualización de la base de datos.
                {
                    _context.Update(rol); // Marca la entidad Rol en el contexto de la base de datos como modificada.
                    await _context.SaveChangesAsync(); // Guarda de forma asíncrona todos los cambios realizados en el contexto de la base de datos, lo que en este caso actualizará el registro del rol en la tabla correspondiente.
                }
                catch (DbUpdateConcurrencyException) // Captura una excepción que ocurre cuando varios usuarios intentan modificar el mismo registro simultáneamente.
                {
                    if (!RolExists(rol.Idrol)) // Llama a un método privado (que se definirá más adelante) para verificar si el rol con el ID proporcionado todavía existe en la base de datos.
                    {
                        return NotFound(); // Si el rol ya no existe, devuelve un resultado NotFound (código de estado HTTP 404).
                    }
                    else
                    {
                        throw; // Si el rol existe pero ocurrió otra excepción de concurrencia, relanza la excepción para que sea manejada por un nivel superior.
                    }
                }
                return RedirectToAction(nameof(Index)); // Si la actualización se realiza con éxito, redirige al usuario a la acción Index del mismo controlador.
            }
            return View(rol); // Si el modelo no es válido, devuelve la vista asociada a la acción Edit, pasando el objeto Rol para que se puedan mostrar los errores de validación en el formulario.
        }
        // GET: Rol/Delete/5 // Comentario que indica que la siguiente acción responde a una petición HTTP GET en la ruta "Rol/Delete/{id}", donde {id} es un parámetro. Eliminar
        public async Task<IActionResult> Delete(int? id) // Define una acción asíncrona llamada Delete que recibe un parámetro entero nullable llamado "id" y devuelve un IActionResult.
        {
            if (id == null) // Verifica si el parámetro "id" es nulo.
            {
                return NotFound(); // Si "id" es nulo, devuelve un resultado NotFound (código de estado HTTP 404).
            }

            var rol = await _context.Rols // Consulta la tabla "Rols" en la base de datos de forma asíncrona.
                .FirstOrDefaultAsync(m => m.Idrol == id); // Busca el primer registro donde la propiedad "Idrol" coincida con el valor del parámetro "id".
            if (rol == null) // Verifica si se encontró algún registro con el "id" proporcionado.
            {
                return NotFound(); // Si no se encuentra ningún rol con ese "id", devuelve un resultado NotFound (código de estado HTTP 404).
            }

            return View(rol); // Si se encuentra el rol, lo pasa a la vista predeterminada asociada a la acción Delete, que probablemente mostrará una confirmación antes de eliminar el rol.
        }

        // POST: Rol/Delete/5 // Comentario que indica que la siguiente acción responde a una petición HTTP POST en la ruta "Rol/Delete/{id}", donde {id} es un parámetro.
        [HttpPost, ActionName("Delete")] // Atributo que especifica que esta acción solo responde a peticiones HTTP POST y que su nombre de acción es "Delete" (útil cuando el nombre del método es diferente, como "DeleteConfirmed").
        [ValidateAntiForgeryToken] // Atributo que agrega una validación anti-falsificación de solicitudes para proteger contra ataques CSRF (Cross-Site Request Forgery).
        public async Task<IActionResult> DeleteConfirmed(int id) // Define una acción asíncrona llamada DeleteConfirmed que recibe un parámetro entero llamado "id" y devuelve un IActionResult. Este método se llama después de que el usuario confirma la eliminación.
        {
            var rol = await _context.Rols.FindAsync(id); // Busca de forma asíncrona un registro en la tabla "Rols" cuya clave primaria coincida con el valor del parámetro "id".
            if (rol != null) // Verifica si se encontró el rol con el "id" proporcionado.
            {
                _context.Rols.Remove(rol); // Marca la entidad Rol en el contexto de la base de datos para su posterior eliminación.
            }

            await _context.SaveChangesAsync(); // Guarda de forma asíncrona todos los cambios realizados en el contexto de la base de datos, lo que en este caso eliminará el registro del rol de la tabla correspondiente.
            return RedirectToAction(nameof(Index)); // Redirige al usuario a la acción Index del mismo controlador, que probablemente mostrará la lista actualizada de roles.
        }

        private bool RolExists(int id) // Define un método privado que toma un entero "id" como parámetro y devuelve un valor booleano.
        {
            return _context.Rols.Any(e => e.Idrol == id); // Utiliza LINQ para verificar si existe algún registro en la tabla "Rols" donde la propiedad "Idrol" coincida con el valor del parámetro "id". Devuelve true si existe al menos un registro, y false en caso contrario.
        }
    }
}