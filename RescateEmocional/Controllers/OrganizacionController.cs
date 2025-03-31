using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RescateEmocional.Models;

// Namespace que contiene el controlador para las operaciones CRUD de Organizacion
namespace RescateEmocional.Controllers
{
    // Controlador para manejar las operaciones relacionadas con Organizacion
    public class OrganizacionController : Controller
    {
        // Contexto de la base de datos inyectado a través del constructor
        private readonly RescateEmocionalContext _context;

        // Constructor que recibe el contexto de la base de datos
        public OrganizacionController(RescateEmocionalContext context)
        {
            _context = context;
        }

        // GET: Organizacion
        // Acción que devuelve la vista con la lista de todas las organizaciones
        public async Task<IActionResult> Index()
        {
            // Incluye la navegación a IdrolNavigation para mostrar datos relacionados
            var rescateEmocionalContext = _context.Organizacions.Include(o => o.IdrolNavigation);
            // Retorna la vista con la lista de organizaciones
            return View(await rescateEmocionalContext.ToListAsync());
        }

        // GET: Organizacion/Details/5
        // Acción que muestra los detalles de una organización específica
        public async Task<IActionResult> Details(int? id)
        {
            // Verifica si el id es nulo
            if (id == null)
            {
                return NotFound();
            }

            // Busca la organización por su id, incluyendo la navegación a IdrolNavigation
            var organizacion = await _context.Organizacions
                .Include(o => o.IdrolNavigation)
                .FirstOrDefaultAsync(m => m.Idorganizacion == id);
            // Si no se encuentra la organización, devuelve NotFound
            if (organizacion == null)
            {
                return NotFound();
            }

            // Retorna la vista con los detalles de la organización
            return View(organizacion);
        }

        // GET: Organizacion/Create
        // Acción que prepara la vista para crear una nueva organización
        public IActionResult Create()
        {
            // Prepara el ViewData con una lista de roles para el dropdown
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Idrol");
            // Retorna la vista para crear una nueva organización
            return View();
        }

        // POST: Organizacion/Create
        // Acción que procesa el formulario para crear una nueva organización
        // Protege contra ataques de overposting al especificar las propiedades permitidas en Bind
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idorganizacion,Nombre,Descripcion,Horario,Ubicacion,Estado,Idrol")] Organizacion organizacion)
        {
            // Verifica si el modelo es válido
            if (ModelState.IsValid)
            {
                // Agrega la nueva organización al contexto
                _context.Add(organizacion);
                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();
                // Redirige a la acción Index
                return RedirectToAction(nameof(Index));
            }
            // Si el modelo no es válido, vuelve a preparar el ViewData y retorna la vista con los errores
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Idrol", organizacion.Idrol);
            return View(organizacion);
        }

        // GET: Organizacion/Edit/5
        // Acción que prepara la vista para editar una organización existente
        public async Task<IActionResult> Edit(int? id)
        {
            // Verifica si el id es nulo
            if (id == null)
            {
                return NotFound();
            }

            // Busca la organización por su id
            var organizacion = await _context.Organizacions.FindAsync(id);
            // Si no se encuentra la organización, devuelve NotFound
            if (organizacion == null)
            {
                return NotFound();
            }
            // Prepara el ViewData con la lista de roles para el dropdown, seleccionando el rol actual
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Idrol", organizacion.Idrol);
            // Retorna la vista con la organización a editar
            return View(organizacion);
        }

        // POST: Organizacion/Edit/5
        // Acción que procesa el formulario para actualizar una organización
        // Protege contra ataques de overposting al especificar las propiedades permitidas en Bind
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idorganizacion,Nombre,Descripcion,Horario,Ubicacion,Estado,Idrol")] Organizacion organizacion)
        {
            // Verifica si el id coincide con el de la organización
            if (id != organizacion.Idorganizacion)
            {
                return NotFound();
            }

            // Verifica si el modelo es válido
            if (ModelState.IsValid)
            {
                try
                {
                    // Actualiza la organización en el contexto
                    _context.Update(organizacion);
                    // Guarda los cambios en la base de datos
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Maneja la excepción de concurrencia verificando si la organización aún existe
                    if (!OrganizacionExists(organizacion.Idorganizacion))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Redirige a la acción Index
                return RedirectToAction(nameof(Index));
            }
            // Si el modelo no es válido, vuelve a preparar el ViewData y retorna la vista con los errores
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Idrol", organizacion.Idrol);
            return View(organizacion);
        }

        // GET: Organizacion/Delete/5
        // Acción que muestra la vista de confirmación para eliminar una organización
        public async Task<IActionResult> Delete(int? id)
        {
            // Verifica si el id es nulo
            if (id == null)
            {
                return NotFound();
            }

            // Busca la organización por su id, incluyendo la navegación a IdrolNavigation
            var organizacion = await _context.Organizacions
                .Include(o => o.IdrolNavigation)
                .FirstOrDefaultAsync(m => m.Idorganizacion == id);
            // Si no se encuentra la organización, devuelve NotFound
            if (organizacion == null)
            {
                return NotFound();
            }

            // Retorna la vista de confirmación con la organización a eliminar
            return View(organizacion);
        }

        // POST: Organizacion/Delete/5
        // Acción que procesa la eliminación de la organización confirmada
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Busca la organización por su id
            var organizacion = await _context.Organizacions.FindAsync(id);
            // Si la organización existe, la elimina
            if (organizacion != null)
            {
                _context.Organizacions.Remove(organizacion);
            }

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();
            // Redirige a la acción Index
            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para verificar si una organización existe en la base de datos
        private bool OrganizacionExists(int id)
        {
            return _context.Organizacions.Any(e => e.Idorganizacion == id);
        }
    }
}