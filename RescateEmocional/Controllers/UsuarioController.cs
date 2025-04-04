using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RescateEmocional.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace RescateEmocional.Controllers
{
    [Authorize(Roles = "3")]
    public class UsuarioController : Controller
    {
        private readonly RescateEmocionalContext _context;

        public UsuarioController(RescateEmocionalContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Organizaciones(string nombre, int topRecords = 10)
        {
            var organizacionesQuery = _context.Organizacions
                .Where(o => o.Estado == 1) // Solo organizaciones verificadas
                .AsQueryable();

            if (!string.IsNullOrEmpty(nombre))
            {
                organizacionesQuery = organizacionesQuery.Where(o => o.Nombre.Contains(nombre));
            }

            if (topRecords > 0)
            {
                organizacionesQuery = organizacionesQuery.Take(topRecords);
            }

            var organizaciones = await organizacionesQuery.ToListAsync();
            return View(organizaciones);
        }




        //LOGICA DEL PERFIL
        public async Task<IActionResult> Perfil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtener ID del usuario autenticado
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdrolNavigation) // Para obtener el nombre del rol
                .FirstOrDefaultAsync(u => u.Idusuario == int.Parse(userId));

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }







        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            var rescateEmocionalContext = _context.Usuarios.Include(u => u.IdrolNavigation);
            return View(await rescateEmocionalContext.ToListAsync());
        }

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdrolNavigation)
                .FirstOrDefaultAsync(m => m.Idusuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Idrol");
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idusuario,Nombre,CorreoElectronico,Telefono,Contrasena,Estado,Idrol")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Idrol", usuario.Idrol);
            return View(usuario);
        }

        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Idrol", usuario.Idrol);
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idusuario,Nombre,CorreoElectronico,Telefono,Contrasena,Estado")] Usuario usuario)
        {
            if (id != usuario.Idusuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el usuario actual de la base de datos
                    var usuarioExistente = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Idusuario == id);
                    if (usuarioExistente == null)
                    {
                        return NotFound();
                    }

                    // Mantener el IDRol actual
                    usuario.Idrol = usuarioExistente.Idrol;

                    // Actualizar usuario
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();

                    // Redirigir al perfil
                    return RedirectToAction("Perfil");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Idusuario))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Idrol", usuario.Idrol);
            return View(usuario);
        }

        

        // GET: Usuario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdrolNavigation)
                .FirstOrDefaultAsync(m => m.Idusuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Idusuario == id);
        }
    }
}
