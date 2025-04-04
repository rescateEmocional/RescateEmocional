using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RescateEmocional.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
        string contrasenaEncriptada = ConvertirMD5(contrasena);

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.CorreoElectronico == correoElectronico && u.Contrasena == contrasenaEncriptada);

        var administrador = await _context.Administradors
            .FirstOrDefaultAsync(a => a.CorreoElectronico == correoElectronico && a.Contrasena == contrasenaEncriptada);

        var organizacion = await _context.Organizacions
            .FirstOrDefaultAsync(o => o.CorreoElectronico == correoElectronico && o.Contrasena == contrasenaEncriptada);

        if (usuario != null)
        {
            return await Autenticar(usuario.Nombre, usuario.CorreoElectronico, usuario.Idrol, usuario.Idusuario);
        }
        else if (administrador != null)
        {
            return await Autenticar(administrador.Nombre, administrador.CorreoElectronico, administrador.Idrol, administrador.Idadmin);
        }
        else if (organizacion != null)
        {
            return await Autenticar(organizacion.Nombre, organizacion.CorreoElectronico, organizacion.Idrol, organizacion.Idorganizacion);
        }

        ModelState.AddModelError("", "Correo o contraseña incorrectos");
        return View();
    }

    public IActionResult Register()
    {
        ViewData["Roles"] = _context.Rols.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([Bind("Nombre,CorreoElectronico,Telefono,Contrasena,Idrol")] Usuario usuario)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoElectronico == usuario.CorreoElectronico);

            if (existingUser != null)
            {
                ModelState.AddModelError("CorreoElectronico", "El correo electrónico ya está registrado.");
                ViewData["Roles"] = _context.Rols.ToList();
                return View(usuario);
            }

            usuario.Idrol = 3;
            usuario.Contrasena = ConvertirMD5(usuario.Contrasena);

            _context.Add(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "Account");
        }

        ViewData["Roles"] = _context.Rols.ToList();
        return View(usuario);
    }

    private async Task<IActionResult> Autenticar(string nombre, string correo, int idRol, int idUsuario)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, nombre),
            new Claim(ClaimTypes.Email, correo),
            new Claim(ClaimTypes.Role, idRol.ToString()),
            new Claim(ClaimTypes.NameIdentifier, idUsuario.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties { IsPersistent = true };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    private string ConvertirMD5(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
    public IActionResult AccesoDenegado()
    {
        ViewBag.Mensaje = "No tienes acceso a esta pagina :v";
        return View();
    }

}