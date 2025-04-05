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
    [Authorize(Roles = "3,1")]
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

        // El resto del código sigue igual...
        // Details, Create, Edit, Delete, etc.

        private bool ConversacionExists(int id)
        {
            return _context.Conversacions.Any(e => e.Idconversacion == id);
        }
    }
}
