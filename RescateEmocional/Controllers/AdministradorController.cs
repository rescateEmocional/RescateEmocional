﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RescateEmocional.Models;

namespace RescateEmocional.Controllers
{

    public class AdministradorController : Controller
    {
        private readonly RescateEmocionalContext _context;

        public AdministradorController(RescateEmocionalContext context)
        {
            _context = context;
        }

        // GET: Administrador
        public async Task<IActionResult> Index(Administrador administrador, int topRegistro = 10)
        {
            var query = _context.Administradors.AsQueryable();
            if (!string.IsNullOrWhiteSpace(administrador.Nombre))
                query = query.Where(a => a.Nombre.Contains(administrador.Nombre));
            if (!string.IsNullOrWhiteSpace(administrador.CorreoElectronico))
                query = query.Where(a => a.CorreoElectronico.Contains(administrador.CorreoElectronico));

            query = query.OrderByDescending(a => a.Idadmin);

            if (topRegistro > 0)
                query = query.Take(topRegistro);

            query = query.Include(a => a.IdrolNavigation);


            return View(await query.ToListAsync());
        }


        // GET: Administrador/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administradors
                .Include(a => a.IdrolNavigation)
                .FirstOrDefaultAsync(m => m.Idadmin == id);
            if (administrador == null)
            {
                return NotFound();
            }

            return View(administrador);
        }

        // GET: Administrador/Create
        public IActionResult Create()
        {
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Nombre");
            return View();
        }

        // POST: Administrador/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idadmin,Nombre,CorreoElectronico,Contrasena,Idrol")] Administrador administrador)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el correo electrónico ya existe en la base de datos
                    var correoExistente = await _context.Administradors
                        .FirstOrDefaultAsync(a => a.CorreoElectronico == administrador.CorreoElectronico);

                    if (correoExistente != null)
                    {
                        // Si el correo ya existe, agregar un error al ModelState
                        ModelState.AddModelError("CorreoElectronico", "Este correo electrónico ya está registrado.");
                    }
                    else
                    {
                        // Si el correo no existe, proceder con la creación del administrador
                        administrador.Contrasena = CalcularHashMD5(administrador.Contrasena);
                        _context.Add(administrador);
                        await _context.SaveChangesAsync();

                        // Redirigir si todo salió bien
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    // Si ocurre algún otro error, agregar un error general al ModelState
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar el administrador.");
                }
            }

            // Recargar datos necesarios para la vista
            return View(administrador);
        }


        // GET: Administrador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administradors.FindAsync(id);
            if (administrador == null)
            {
                return NotFound();
            }
            ViewData["Idrol"] = new SelectList(_context.Rols, "Idrol", "Nombre", administrador.Idrol);
            return View(administrador);
        }

        // POST: Administrador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idadmin,Nombre,CorreoElectronico,Idrol")] Administrador administrador)
{
    if (id != administrador.Idadmin)
    {
        return NotFound();
    }

    var adminUpdate = await _context.Administradors.FirstOrDefaultAsync(m => m.Idadmin == administrador.Idadmin);
    if (adminUpdate == null)
    {
        return NotFound();
    }

    try
    {
        adminUpdate.Nombre = administrador.Nombre;
        adminUpdate.CorreoElectronico = administrador.CorreoElectronico;
        adminUpdate.Idrol = administrador.Idrol;

        _context.Update(adminUpdate);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!AdministradorExists(administrador.Idadmin))
        {
            return NotFound();
        }
        else
        {
            return View(administrador);
        }
    }
}

        // GET: Administrador/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administradors
                .Include(a => a.IdrolNavigation)
                .FirstOrDefaultAsync(m => m.Idadmin == id);
            if (administrador == null)
            {
                return NotFound();
            }

            return View(administrador);
        }

        // POST: Administrador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var administrador = await _context.Administradors.FindAsync(id);
            if (administrador != null)
            {
                _context.Administradors.Remove(administrador);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdministradorExists(int id)
        {
            return _context.Administradors.Any(e => e.Idadmin == id);
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