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
    [Authorize(Roles = "3")]
    [Authorize(Roles = "2")]
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
            // Depurar los claims para encontrar el identificador correcto
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            System.Diagnostics.Debug.WriteLine("Claims: " + string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));

            // Obtener el nombre del usuario autenticado
            string userName = User.FindFirstValue(ClaimTypes.Name);
            int authenticatedUserId = 0;
            int authenticatedOrgId = 0;
            string userRole = "Desconocido";

            if (!string.IsNullOrEmpty(userName))
            {
                if (User.IsInRole("3")) // Usuario
                {
                    userRole = "Usuario";
                    var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == userName);
                    if (usuario != null)
                    {
                        authenticatedUserId = usuario.Idusuario;
                        System.Diagnostics.Debug.WriteLine($"Usuario autenticado - Idusuario: {authenticatedUserId}, Nombre: {userName}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Usuario no encontrado con Nombre: {userName}");
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (User.IsInRole("2")) // Organización
                {
                    userRole = "Organizacion";
                    var organizacion = await _context.Organizacions.FirstOrDefaultAsync(o => o.Nombre == userName);
                    if (organizacion != null)
                    {
                        authenticatedOrgId = organizacion.Idorganizacion;
                        System.Diagnostics.Debug.WriteLine($"Organización autenticada - Idorganizacion: {authenticatedOrgId}, Nombre: {userName}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Organización no encontrada con Nombre: {userName}");
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Usuario no tiene rol 3 ni 2.");
                    return Unauthorized();
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No se pudo obtener el ClaimTypes.Name.");
                return RedirectToAction("Login", "Account");
            }

            // Pasar los IDs y el rol a la vista
            ViewData["AuthenticatedUserId"] = authenticatedUserId;
            ViewData["AuthenticatedOrgId"] = authenticatedOrgId;
            ViewData["UserRole"] = userRole;
            ViewData["UserName"] = userName;

            // Filtrar las conversaciones para incluir mensajes enviados y recibidos
            IQueryable<Conversacion> rescateEmocionalContext = _context.Conversacions
                .Include(c => c.IdorganizacionNavigation)
                .Include(c => c.IdusuarioNavigation);

            if (userRole == "Usuario" && authenticatedUserId > 0)
            {
                rescateEmocionalContext = rescateEmocionalContext
                    .Where(c => c.Idusuario == authenticatedUserId || c.Idorganizacion == authenticatedOrgId);
            }
            else if (userRole == "Organizacion" && authenticatedOrgId > 0)
            {
                rescateEmocionalContext = rescateEmocionalContext
                    .Where(c => c.Idorganizacion == authenticatedOrgId || c.Idusuario == authenticatedUserId);
            }
            else
            {
                rescateEmocionalContext = rescateEmocionalContext.Where(c => false);
            }

            var conversaciones = await rescateEmocionalContext.ToListAsync();
            return View(conversaciones);
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
        public async Task<IActionResult> Create([Bind("Idorganizacion,Idusuario,FechaInicio,Mensaje")] Conversacion conversacion)
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            System.Diagnostics.Debug.WriteLine("Claims: " + string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));
            System.Diagnostics.Debug.WriteLine($"Valores del formulario - Idusuario: {conversacion.Idusuario}, Idorganizacion: {conversacion.Idorganizacion}, FechaInicio: {conversacion.FechaInicio}, Mensaje: {conversacion.Mensaje}");

            if (ModelState.IsValid)
            {
                string userName = User.FindFirstValue(ClaimTypes.Name);
                if (string.IsNullOrEmpty(userName))
                {
                    ModelState.AddModelError("", "No se pudo obtener el identificador del usuario autenticado.");
                    if (User.IsInRole("3"))
                    {
                        ViewData["Idorganizacion"] = new SelectList(_context.Organizacions, "Idorganizacion", "Nombre", conversacion.Idorganizacion);
                    }
                    else if (User.IsInRole("2"))
                    {
                        ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Nombre", conversacion.Idusuario);
                    }
                    return View(conversacion);
                }

                if (User.IsInRole("3")) // Usuario enviando mensaje
                {
                    var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == userName);
                    if (usuario == null)
                    {
                        ModelState.AddModelError("", "No se encontró el usuario autenticado en la base de datos.");
                        ViewData["Idorganizacion"] = new SelectList(_context.Organizacions, "Idorganizacion", "Nombre", conversacion.Idorganizacion);
                        return View(conversacion);
                    }
                    conversacion.Idusuario = usuario.Idusuario;
                    if (conversacion.Idorganizacion <= 0)
                    {
                        ModelState.AddModelError("Idorganizacion", "Debe seleccionar una organización.");
                        ViewData["Idorganizacion"] = new SelectList(_context.Organizacions, "Idorganizacion", "Nombre", conversacion.Idorganizacion);
                        return View(conversacion);
                    }
                }
                else if (User.IsInRole("2")) // Organización respondiendo
                {
                    var organizacion = await _context.Organizacions.FirstOrDefaultAsync(o => o.Nombre == userName);
                    if (organizacion == null)
                    {
                        ModelState.AddModelError("", "No se encontró la organización autenticada en la base de datos.");
                        ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Nombre", conversacion.Idusuario);
                        return View(conversacion);
                    }
                    conversacion.Idorganizacion = organizacion.Idorganizacion;
                    if (conversacion.Idusuario <= 0)
                    {
                        ModelState.AddModelError("Idusuario", "Debe seleccionar un usuario.");
                        ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Nombre", conversacion.Idusuario);
                        return View(conversacion);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Valores finales - Idusuario: {conversacion.Idusuario}, Idorganizacion: {conversacion.Idorganizacion}, FechaInicio: {conversacion.FechaInicio}, Mensaje: {conversacion.Mensaje}");

                _context.Add(conversacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (User.IsInRole("3"))
            {
                ViewData["Idorganizacion"] = new SelectList(_context.Organizacions, "Idorganizacion", "Nombre", conversacion.Idorganizacion);
            }
            else if (User.IsInRole("2"))
            {
                ViewData["Idusuario"] = new SelectList(_context.Usuarios, "Idusuario", "Nombre", conversacion.Idusuario);
            }
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