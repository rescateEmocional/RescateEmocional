using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RescateEmocional.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RescateEmocional.Controllers
{
    [Authorize(Roles = "3,2")]
    public class ConversacionController : Controller
    {
        private readonly RescateEmocionalContext _context;

        public ConversacionController(RescateEmocionalContext context)
        {
            _context = context;
        }

       

        // GET: Conversacion/Index
        public async Task<IActionResult> Index()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            System.Diagnostics.Debug.WriteLine("Claims: " + string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));

            string userName = User.FindFirstValue(ClaimTypes.Name);
            int authenticatedUserId = 0;
            int authenticatedOrgId = 0;
            string userRole = "Desconocido";

            if (!string.IsNullOrEmpty(userName))
            {
                if (User.IsInRole("3"))
                {
                    userRole = "Usuario";
                    var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == userName);
                    if (usuario != null)
                    {
                        authenticatedUserId = usuario.Idusuario;
                    }
                    else return RedirectToAction("Index", "Home");
                }
                else if (User.IsInRole("2"))
                {
                    userRole = "Organizacion";
                    var organizacion = await _context.Organizacions.FirstOrDefaultAsync(o => o.Nombre == userName);
                    if (organizacion != null)
                    {
                        authenticatedOrgId = organizacion.Idorganizacion;
                    }
                    else return RedirectToAction("Index", "Home");
                }
                else return Unauthorized();
            }
            else return RedirectToAction("Login", "Account");

            ViewData["AuthenticatedUserId"] = authenticatedUserId;
            ViewData["AuthenticatedOrgId"] = authenticatedOrgId;
            ViewData["UserRole"] = userRole;
            ViewData["UserName"] = userName;

            IQueryable<Conversacion> query = _context.Conversacions
                .Include(c => c.IdorganizacionNavigation)
                .Include(c => c.IdusuarioNavigation);

            if (userRole == "Usuario" && authenticatedUserId > 0)
            {
                query = query.Where(c => c.Idusuario == authenticatedUserId);
            }
            else if (userRole == "Organizacion" && authenticatedOrgId > 0)
            {
                query = query.Where(c => c.Idorganizacion == authenticatedOrgId);
            }
            else
            {
                query = query.Where(c => false);
            }

            var conversaciones = await query
                .OrderByDescending(c => c.FechaInicio)
                .ToListAsync();

            return View(conversaciones);
        }

        private bool ConversacionExists(int id)
        {
            return _context.Conversacions.Any(e => e.Idconversacion == id);
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
        [Authorize]
        public IActionResult Create()
        {
            if (User.IsInRole("3")) // Usuario
            {
                ViewData["Idorganizacion"] = new SelectList(_context.Organizacions, "Idorganizacion", "Nombre");
            }
            else if (User.IsInRole("2")) // Organización
            {
                ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Nombre");
            }
            return View();
        }

        // POST: Conversacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Idorganizacion,Mensaje")] Conversacion conversacion)
        {
            string userName = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("", "No se pudo obtener el usuario autenticado.");
                return View(conversacion);
            }

            // Obtener el usuario autenticado
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == userName);
            if (usuario == null)
            {
                ModelState.AddModelError("", "No se encontró el usuario autenticado.");
                return View(conversacion);
            }

            conversacion.Idusuario = usuario.Idusuario;

            if (conversacion.Idorganizacion <= 0)
            {
                ModelState.AddModelError("Idorganizacion", "Debe seleccionar una organización.");
                return View(conversacion);
            }

            conversacion.Emisor = "Usuario"; // Siempre el usuario inicia la conversación
            conversacion.FechaInicio = DateTime.Now;

            _context.Add(conversacion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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

        
    }
}
