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
using System.Security.Cryptography;
using System.Text;

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

        private string EncriptarMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes);
            }
        }

        public async Task<IActionResult> Organizaciones(string nombre, int topRecords = 10)
        {
            var organizacionesQuery = _context.Organizacions
                .Where(o => o.Estado == 1)
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

        public async Task<IActionResult> Perfil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdrolNavigation)
                .FirstOrDefaultAsync(u => u.Idusuario == int.Parse(userId));

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        public async Task<IActionResult> Index()
        {
            var rescateEmocionalContext = _context.Usuarios.Include(u => u.IdrolNavigation);
            return View(await rescateEmocionalContext.ToListAsync());
        }

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

        public IActionResult Create()
        {
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Idrol");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idusuario,Nombre,CorreoElectronico,Telefono,Contrasena,Estado,Idrol")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Encriptar contraseña antes de guardar
                usuario.Contrasena = EncriptarMD5(usuario.Contrasena);
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Idrol", usuario.Idrol);
            return View(usuario);
        }

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
                    var usuarioExistente = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Idusuario == id);
                    if (usuarioExistente == null)
                    {
                        return NotFound();
                    }

                    usuario.Idrol = usuarioExistente.Idrol;

                    if (!string.IsNullOrWhiteSpace(usuario.Contrasena) &&
                        EncriptarMD5(usuario.Contrasena) != usuarioExistente.Contrasena)
                    {
                        usuario.Contrasena = EncriptarMD5(usuario.Contrasena);
                    }
                    else
                    {
                        usuario.Contrasena = usuarioExistente.Contrasena;
                    }

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();

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
