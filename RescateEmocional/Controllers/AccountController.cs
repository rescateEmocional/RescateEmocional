using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RescateEmocional.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly RescateEmocionalContext _context;

    public AccountController(RescateEmocionalContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string correoElectronico, string contrasena)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.CorreoElectronico == correoElectronico && u.Contrasena == contrasena);

        var administrador = await _context.Administradors
            .FirstOrDefaultAsync(a => a.CorreoElectronico == correoElectronico && a.Contrasena == contrasena);

        var organizacion = await _context.Organizacions
            .FirstOrDefaultAsync(o => o.CorreoElectronico == correoElectronico && o.Contrasena == contrasena);

        if (usuario != null)   
        {
            return await Autenticar(usuario.Nombre, usuario.CorreoElectronico, usuario.Idrol);
        }
        else if (administrador != null)
        {
            return await Autenticar(administrador.Nombre, administrador.CorreoElectronico, administrador.Idrol);
        }
        else if (organizacion != null)
        {
            return await Autenticar(organizacion.Nombre, organizacion.CorreoElectronico, organizacion.Idrol);
        }

        ModelState.AddModelError("", "Correo o contraseña incorrectos");
        return View();
    }

    // Método para autenticar y asignar rol
    private async Task<IActionResult> Autenticar(string nombre, string correo, int idRol)
    {
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, nombre),
        new Claim(ClaimTypes.Email, correo),
        new Claim(ClaimTypes.Role, idRol.ToString()) // Guardamos el rol
    };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        return RedirectToAction("Index", "Home");
    }


    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
