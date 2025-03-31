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

        // Resto del controlador (Details, Create, Edit, Delete) sin cambios
        // ... (código omitido por brevedad, pero sigue igual al proporcionado anteriormente)
    }
}