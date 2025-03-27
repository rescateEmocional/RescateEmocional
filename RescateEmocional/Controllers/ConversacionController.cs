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
    public class ConversacionController : Controller
    {
        private readonly RescateEmocionalContext _context;

        public ConversacionController(RescateEmocionalContext context)
        {
            _context = context;
        }

        // GET: Conversacion
        public async Task<IActionResult> Index()
        {
            var rescateEmocionalContext = _context.Conversacions.Include(c => c.IdorganizacionNavigation).Include(c => c.IdusuarioNavigation);
            return View(await rescateEmocionalContext.ToListAsync());
        }

        // GET: Conversacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversacion = await _context.Conversacions
                .Include(c => c.IdorganizacionNavigation)
                .Include(c => c.IdusuarioNavigation)
                .FirstOrDefaultAsync(m => m.Idconversacion == id);
            if (conversacion == null)
            {
                return NotFound();
            }

            return View(conversacion);
        }

        // GET: Conversacion/Create
        public IActionResult Create()
        {
            ViewData["Idorganizacion"] = new SelectList(_context.Organizacions, "Idorganizacion", "Idorganizacion");
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Idusuario");
            return View();
        }

        // POST: Conversacion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idconversacion,Idusuario,Idorganizacion,FechaInicio,Mensaje")] Conversacion conversacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(conversacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idorganizacion"] = new SelectList(_context.Organizacions, "Idorganizacion", "Idorganizacion", conversacion.Idorganizacion);
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Idusuario", conversacion.Idusuario);
            return View(conversacion);
        }

        // GET: Conversacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversacion = await _context.Conversacions.FindAsync(id);
            if (conversacion == null)
            {
                return NotFound();
            }
            ViewData["Idorganizacion"] = new SelectList(_context.Organizacions, "Idorganizacion", "Idorganizacion", conversacion.Idorganizacion);
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Idusuario", conversacion.Idusuario);
            return View(conversacion);
        }

        // POST: Conversacion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idconversacion,Idusuario,Idorganizacion,FechaInicio,Mensaje")] Conversacion conversacion)
        {
            if (id != conversacion.Idconversacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(conversacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConversacionExists(conversacion.Idconversacion))
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
            ViewData["Idorganizacion"] = new SelectList(_context.Organizacions, "Idorganizacion", "Idorganizacion", conversacion.Idorganizacion);
            ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Idusuario", conversacion.Idusuario);
            return View(conversacion);
        }

        // GET: Conversacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversacion = await _context.Conversacions
                .Include(c => c.IdorganizacionNavigation)
                .Include(c => c.IdusuarioNavigation)
                .FirstOrDefaultAsync(m => m.Idconversacion == id);
            if (conversacion == null)
            {
                return NotFound();
            }

            return View(conversacion);
        }

        // POST: Conversacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var conversacion = await _context.Conversacions.FindAsync(id);
            if (conversacion != null)
            {
                _context.Conversacions.Remove(conversacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConversacionExists(int id)
        {
            return _context.Conversacions.Any(e => e.Idconversacion == id);
        }
    }
}
