using Microsoft.AspNetCore.Mvc;
using Habitforger.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Habitforger.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Habitforger.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.NombreUsuario == model.NombreUsuario && u.Contrasena == model.Contrasena);

                if (usuario != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                        new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                        new Claim(ClaimTypes.Email, usuario.Email)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties
                        {
                            IsPersistent = model.Recordarme
                        });

                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
                }
            }

            return View(model);
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el usuario ya existe
                if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == model.NombreUsuario))
                {
                    ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya está en uso");
                    return View(model);
                }

                if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "El email ya está registrado");
                    return View(model);
                }

                // Crear nuevo usuario
                var usuario = new Usuario
                {
                    NombreUsuario = model.NombreUsuario,
                    Email = model.Email,
                    Contrasena = model.Contrasena // ¡En producción deberías hashear la contraseña!
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync(); // Guardamos primero para obtener el IdUsuario

                // Crear estadísticas iniciales para el usuario
                var estadisticas = new Estadisticas
                {
                    IdUsuario = usuario.IdUsuario,
                    NumHabitTotal = 0,
                    NumHabitCompletados = 0,
                    NumHabitActivos = 0,
                    RachaMaxTotNum = 0,
                    RachaMaxTotTitulo = "",
                    RachaMaxActNum = 0,
                    RachaMaxActTitulo = "",
                    PorcentajeMayorAvance = 0,
                    HabitoMayorAvance = ""
                };

                _context.Estadisticas.Add(estadisticas);

                // Crear logros iniciales para el usuario
                await CrearLogrosIniciales(usuario.IdUsuario);

                await _context.SaveChangesAsync(); // Guardamos todo

                // Autenticar al usuario después del registro
                await LoginUserAsync(usuario);

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        private async Task CrearLogrosIniciales(int usuarioId)
        {
            var logrosIniciales = new List<Logro>
            {
                new Logro
                {
                    NombreLogro = "Principiante",
                    Descripcion = "Consigue una racha de 3 en un hábito",
                    Completado = false,
                    IdUsuario = usuarioId
                },
                new Logro
                {
                    NombreLogro = "Primera victoria",
                    Descripcion = "Completa un hábito por primera vez",
                    Completado = false,
                    IdUsuario = usuarioId
                },
                new Logro
                {
                    NombreLogro = "Semana seria",
                    Descripcion = "Consigue una racha de 7 en cualquier hábito",
                    Completado = false,
                    IdUsuario = usuarioId
                },
                new Logro
                {
                    NombreLogro = "Consistente",
                    Descripcion = "Completa una racha de 30",
                    Completado = false,
                    IdUsuario = usuarioId
                },
                new Logro
                {
                    NombreLogro = "Maestro de la racha",
                    Descripcion = "Completa una racha de 100",
                    Completado = false,
                    IdUsuario = usuarioId
                },
                new Logro
                {
                    NombreLogro = "Inquebrantable",
                    Descripcion = "Completa un hábito sin fallos",
                    Completado = false,
                    IdUsuario = usuarioId
                },
                new Logro
                {
                    NombreLogro = "Leyenda",
                    Descripcion = "Has completado todos los logros",
                    Completado = false,
                    IdUsuario = usuarioId
                }
            };

            _context.Logros.AddRange(logrosIniciales);
        }

        private async Task LoginUserAsync(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> Estadisticas()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var estadisticas = await _context.Estadisticas
                .FirstOrDefaultAsync(e => e.IdUsuario == userId);

            if (estadisticas == null)
            {
                return RedirectToAction("Index", "Habitos");
            }

            return View(estadisticas);
        }
        [Authorize]
        public async Task<IActionResult> Logros()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var logros = await _context.Logros
                .Where(l => l.IdUsuario == userId)
                .OrderBy(l => l.Completado) // Opcional: ordenar para mostrar los no completados primero
                .ToListAsync();

            return View(logros);
        }
    }
}