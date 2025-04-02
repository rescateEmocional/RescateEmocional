using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RescateEmocional.Models;
using System.Security.Claims;


namespace RescateEmocional.Controllers
{
    public class DiarioController : Controller
    {
        private readonly RescateEmocionalContext _context;

        public DiarioController(RescateEmocionalContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(Diario diario, int topRegistro = 10)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var rescateEmocionalContext = _context.Diarios
                .Where(d => d.Idusuario == userId) // Filtra por el Idusuario del usuario autenticado
                .Include(d => d.IdusuarioNavigation);

            var query = _context.Diarios.AsQueryable();
            if (!string.IsNullOrWhiteSpace(diario.Titulo))
                query = query.Where(d => d.Titulo.Contains(diario.Titulo));
            if (diario.FechaCreacion != default(DateTime))
                query = query.Where(d => d.FechaCreacion.Date == diario.FechaCreacion.Date);

            query = query.OrderByDescending(d => d.Iddiario);

            if (topRegistro > 0)
                query = query.Take(topRegistro);

            return View(await query.ToListAsync());
        }

        // GET: Diario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diario = await _context.Diarios
                .Include(d => d.IdusuarioNavigation)
                .FirstOrDefaultAsync(m => m.Iddiario == id);
            if (diario == null)
            {
                return NotFound();
            }

            return View(diario);
        }

        // GET: Diario/Create
        public IActionResult Create()
        {
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Idusuario");
            return View();
        }

        // POST: Diario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Iddiario,Idusuario,Titulo,Contenido,FechaCreacion")] Diario diario)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el usuario está autenticado
                if (User.Identity.IsAuthenticated)
                {
                    // Obtener el Id del usuario autenticado desde los claims
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                    // Asignar el Idusuario al diario
                    diario.Idusuario = userId;
                }
                else
                {
                    // Si no está autenticado, redirigir al login
                    return RedirectToAction("Login", "Account");
                }

                // Agregar el diario a la base de datos
                _context.Add(diario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(diario);
        }

        // GET: Diario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diario = await _context.Diarios.FindAsync(id);
            if (diario == null)
            {
                return NotFound();
            }
            return View(diario);
        }

        // POST: Diario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Iddiario,Titulo,Contenido,FechaCreacion")] Diario diario)
        {
            if (id != diario.Iddiario)
            {
                return NotFound();
            }

            var diarioUpdate = await _context.Diarios.FirstOrDefaultAsync(m => m.Iddiario == diario.Iddiario);
            if (diarioUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    diarioUpdate.Titulo = diario.Titulo;
                    diarioUpdate.Contenido = diario.Contenido;
                    diarioUpdate.FechaCreacion = diario.FechaCreacion;

                    _context.Update(diarioUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiarioExists(diario.Iddiario))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(diarioUpdate);
        }


        // GET: Diario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diario = await _context.Diarios
                .Include(d => d.IdusuarioNavigation)
                .FirstOrDefaultAsync(m => m.Iddiario == id);
            if (diario == null)
            {
                return NotFound();
            }

            return View(diario);
        }

        // POST: Diario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diario = await _context.Diarios.FindAsync(id);
            if (diario != null)
            {
                _context.Diarios.Remove(diario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiarioExists(int id)
        {
            return _context.Diarios.Any(e => e.Iddiario == id);
        }
    }
}
