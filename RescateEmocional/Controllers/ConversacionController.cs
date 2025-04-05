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

        // GET: Conversacion/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var rol = User.FindFirst(ClaimTypes.Role).Value;

            ViewBag.EsOrganizacion = rol == "Organizacion" || rol == "2";

            if (rol == "Organizacion" || rol == "2")
            {
                ViewBag.Idusuario = new SelectList(await _context.Usuarios.ToListAsync(), "Idusuario", "Nombre");
            }
            else
            {
                ViewBag.Idorganizacion = new SelectList(await _context.Organizacions.ToListAsync(), "Idorganizacion", "Nombre");
            }

            return View();
        }

        // POST: Conversacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Conversacion conversacion)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var rol = User.FindFirst(ClaimTypes.Role).Value;

            if (rol == "Organizacion" || rol == "2")
            {
                conversacion.Idorganizacion = usuarioId;
            }
            else
            {
                conversacion.Idusuario = usuarioId;
            }

            conversacion.FechaInicio = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(conversacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.EsOrganizacion = rol == "Organizacion" || rol == "2";

            if (rol == "Organizacion" || rol == "2")
            {
                ViewBag.Idusuario = new SelectList(await _context.Usuarios.ToListAsync(), "Idusuario", "Nombre", conversacion.Idusuario);
            }
            else
            {
                ViewBag.Idorganizacion = new SelectList(await _context.Organizacions.ToListAsync(), "Idorganizacion", "Nombre", conversacion.Idorganizacion);
            }

            return View(conversacion);
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
    }
}
