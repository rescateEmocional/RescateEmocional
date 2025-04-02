 using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RescateEmocional.Models;

namespace RescateEmocional.Controllers
{
    public class OrganizacionController : Controller
    {
        private readonly RescateEmocionalContext _context;

        public OrganizacionController(RescateEmocionalContext context)
        {
            _context = context;
        }

        // GET: Organizacion
        public async Task<IActionResult> Index()
        {
            var rescateEmocionalContext = _context.Organizacions.Include(o => o.IdrolNavigation);
            return View(await rescateEmocionalContext.ToListAsync());
        }

        // GET: Organizacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organizacion = await _context.Organizacions
                .Include(o => o.IdrolNavigation)
                .FirstOrDefaultAsync(m => m.Idorganizacion == id);
            if (organizacion == null)
            {
                return NotFound();
            }

            return View(organizacion);
        }

        // GET: Organizacion/Create
        public IActionResult Create()
        {
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Nombre"); // Mostrar el nombre del rol
            return View();
        }

        // POST: Organizacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idorganizacion,Nombre,Descripcion,Horario,Ubicacion,Estado,Idrol,CorreoElectronico,Contrasena")] Organizacion organizacion)
        {
            if (ModelState.IsValid)
            {
                organizacion.Contrasena = CalcularHashMD5(organizacion.Contrasena);
                _context.Add(organizacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Nombre", organizacion.Idrol);
            return View(organizacion);
        }

        // GET: Organizacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organizacion = await _context.Organizacions.FindAsync(id);
            if (organizacion == null)
            {
                return NotFound();
            }
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Nombre", organizacion.Idrol);
            return View(organizacion);
        }

        // POST: Organizacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idorganizacion,Nombre,Descripcion,Horario,Ubicacion,Estado,Idrol,CorreoElectronico,Contrasena")] Organizacion organizacion)
        {
            if (id != organizacion.Idorganizacion)
            {
                return NotFound();
            }

            var organizacionUpdate = await _context.Organizacions.FirstOrDefaultAsync(m => m.Idorganizacion == organizacion.Idorganizacion);
            if (organizacionUpdate == null)
            {
                return NotFound();
            }

            try
            {
                organizacionUpdate.Nombre = organizacion.Nombre;
                organizacionUpdate.Descripcion = organizacion.Descripcion;
                organizacionUpdate.Horario = organizacion.Horario;
                organizacionUpdate.Ubicacion = organizacion.Ubicacion;
                organizacionUpdate.Estado = organizacion.Estado;
                organizacionUpdate.Idrol = organizacion.Idrol;
                organizacionUpdate.CorreoElectronico = organizacion.CorreoElectronico;
                organizacionUpdate.Contrasena = CalcularHashMD5(organizacion.Contrasena);

                _context.Update(organizacionUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizacionExists(organizacion.Idorganizacion))
                {
                    return NotFound();
                }
                else
                {
                    return View(organizacion);
                }
            }
        }

        // GET: Organizacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organizacion = await _context.Organizacions
                .Include(o => o.IdrolNavigation)
                .FirstOrDefaultAsync(m => m.Idorganizacion == id);
            if (organizacion == null)
            {
                return NotFound();
            }

            return View(organizacion);
        }

        // POST: Organizacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var organizacion = await _context.Organizacions.FindAsync(id);
            if (organizacion != null)
            {
                _context.Organizacions.Remove(organizacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrganizacionExists(int id)
        {
            return _context.Organizacions.Any(e => e.Idorganizacion == id);
        }
        private string CalcularHashMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); // "x2" convierte el byte en una cadena hexadecimal de dos caracteres.
                }
                return sb.ToString();
            }
        }
    }
}