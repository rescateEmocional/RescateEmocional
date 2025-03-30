using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RescateEmocional.Models;

namespace RescateEmocional.Controllers
{
    public class AdministradorController : Controller
    {
        private readonly RescateEmocionalContext _context;

        public AdministradorController(RescateEmocionalContext context)
        {
            _context = context;
        }

        // GET: Administrador
        public async Task<IActionResult> Index(Administrador administrador, int topRegistro = 10)
        {
            var query = _context.Administradors.AsQueryable();
            if (!string.IsNullOrWhiteSpace(administrador.Nombre))
                query = query.Where(a => a.Nombre.Contains(administrador.Nombre));
            if (!string.IsNullOrWhiteSpace(administrador.CorreoElectronico))
                query = query.Where(a => a.CorreoElectronico.Contains(administrador.CorreoElectronico));
            if (administrador.Idrol > 0)
                query = query.Where(a => a.Idrol == administrador.Idrol);

            query = query.OrderByDescending(administrador => administrador.Idadmin);

            if (topRegistro > 0)
                query = query.Take(topRegistro);

            query = query.Include(a => a.IdrolNavigation);

            var roles = _context.Rols.ToList();
            roles.Add(new Rol { Nombre = "SELECCIONAR", Idrol = 0 });
            ViewData["Idrol"] = new SelectList(roles, "Idrol", "Nombre", 0);

            return View(await query.ToListAsync());
        }


        // GET: Administrador/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administradors
                .Include(a => a.IdrolNavigation)
                .FirstOrDefaultAsync(m => m.Idadmin == id);
            if (administrador == null)
            {
                return NotFound();
            }

            return View(administrador);
        }

        // GET: Administrador/Create
        public IActionResult Create()
        {
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Nombre");
            return View();
        }

        // POST: Administrador/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idadmin,Nombre,CorreoElectronico,Contrasena,Idrol")] Administrador administrador)
        {
            if (ModelState.IsValid)
            {
                _context.Add(administrador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Nombre", administrador.Idrol);
            return View(administrador);
        }

        // GET: Administrador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administradors.FindAsync(id);
            if (administrador == null)
            {
                return NotFound();
            }
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Nombre", administrador.Idrol);
            return View(administrador);
        }

        // POST: Administrador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idadmin,Nombre,CorreoElectronico,Contrasena,Idrol")] Administrador administrador)
        {
            if (id != administrador.Idadmin)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(administrador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdministradorExists(administrador.Idadmin))
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
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Nombre", administrador.Idrol);
            return View(administrador);
        }

        // GET: Administrador/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administradors
                .Include(a => a.IdrolNavigation)
                .FirstOrDefaultAsync(m => m.Idadmin == id);
            if (administrador == null)
            {
                return NotFound();
            }

            return View(administrador);
        }

        // POST: Administrador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var administrador = await _context.Administradors.FindAsync(id);
            if (administrador != null)
            {
                _context.Administradors.Remove(administrador);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdministradorExists(int id)
        {
            return _context.Administradors.Any(e => e.Idadmin == id);
        }
    }
}
