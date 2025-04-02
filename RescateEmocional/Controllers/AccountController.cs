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
    //LOGICA DE REGISTRO DE USUARIOS
    // GET: Usuario/Register
    public IActionResult Register()
    {
        // Cargar los roles para la vista (aunque no usaremos el rol en el formulario)
        ViewData["Roles"] = _context.Rols.ToList();
        return View();
    }

    // POST: Usuario/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([Bind("Nombre,CorreoElectronico,Telefono,Contrasena,Idrol")] Usuario usuario)
    {
        if (ModelState.IsValid)
        {
            // Verifica si ya existe un usuario con ese correo
            var existingUser = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoElectronico == usuario.CorreoElectronico);

            if (existingUser != null)
            {
                ModelState.AddModelError("CorreoElectronico", "El correo electrónico ya está registrado.");
                ViewData["Roles"] = _context.Rols.ToList();  // Cargar los roles nuevamente
                return View(usuario);
            }

            // Asignar el IDRol predeterminado a 3 para todos los usuarios
            usuario.Idrol = 3;

            // Crear un nuevo usuario
            _context.Add(usuario);
            await _context.SaveChangesAsync();

            // Redirige a la lista de usuarios (o a otro lugar según lo que necesites)
            return RedirectToAction("Login", "account");
        }

        ViewData["Roles"] = _context.Rols.ToList();  // Cargar los roles nuevamente
        return View(usuario);
    }



    // Método para autenticar y asignar rol
    private async Task<IActionResult> Autenticar(string nombre, string correo, int idRol, int idUsuario)
    {
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, nombre),
        new Claim(ClaimTypes.Email, correo),
        new Claim(ClaimTypes.Role, idRol.ToString()), // Guardamos el rol
        new Claim(ClaimTypes.NameIdentifier, idUsuario.ToString()) // Guardamos el id del usuario
    };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        return RedirectToAction("Organizaciones", "Usuario");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
