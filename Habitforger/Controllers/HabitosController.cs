using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Habitforger.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Habitforger.Data;
using System;
using System.Linq;

namespace Habitforger.Controllers
{
    [Authorize]
    public class HabitosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HabitosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Habitos
        public async Task<IActionResult> Index(string searchString)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var habitosQuery = _context.Habitos
                .Where(h => h.IdUsuario == userId);

            if (!string.IsNullOrEmpty(searchString))
            {
                habitosQuery = habitosQuery.Where(h => h.TituloHabito.Contains(searchString));
            }

            var habitos = await habitosQuery.ToListAsync();

            foreach (var habito in habitos)
            {
                await VerificarEstadoDiario(habito);
            }

            ViewData["CurrentFilter"] = searchString;
            await ActualizarEstadisticasUsuario(userId);

            return View(habitos);
        }

        private async Task VerificarEstadoDiario(Habito habito)
        {
            if (habito.UltimaRespuestaFecha.Date == DateTime.Today)
                return;

            habito.HoyRespondido = false;
            habito.UltimaActualizacion = DateTime.Now;

            _context.Update(habito);
            await _context.SaveChangesAsync();
        }

        // GET: Habitos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Habitos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TituloHabito,Objetivo,Frecuencia,HacerPrivado")] Habito habito)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
            {
                ModelState.AddModelError("", "El usuario no existe");
                return View(habito);
            }

            habito.IdUsuario = userId;
            habito.FechaCreacion = DateTime.Now;
            habito.UltimaActualizacion = DateTime.Now;
            habito.UltimaRespuestaFecha = DateTime.MinValue;

            ModelState.Remove("Usuario");
            ModelState.Remove("FechaCreacion");
            ModelState.Remove("UltimaActualizacion");
            ModelState.Remove("UltimaRespuestaFecha");

            if (ModelState.IsValid)
            {
                _context.Add(habito);
                await _context.SaveChangesAsync();
                await ActualizarEstadisticasUsuario(userId);
                return RedirectToAction(nameof(Index));
            }
            return View(habito);
        }

        // GET: Habitos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var habito = await _context.Habitos
                .FirstOrDefaultAsync(m => m.IdHabito == id && m.IdUsuario == userId);

            if (habito == null) return NotFound();

            return View(habito);
        }
        // POST: Habitos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var habito = await _context.Habitos
                .FirstOrDefaultAsync(h => h.IdHabito == id && h.IdUsuario == userId);

            if (habito == null)
            {
                return NotFound();
            }

            _context.Habitos.Remove(habito);
            await _context.SaveChangesAsync();
            await ActualizarEstadisticasUsuario(userId);

            return RedirectToAction(nameof(Index));
        }

        // GET: Habitos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var habito = await _context.Habitos
                .FirstOrDefaultAsync(m => m.IdHabito == id && m.IdUsuario == userId);

            if (habito == null)
            {
                return NotFound();
            }

            return View(habito);
        }

        // POST: Habitos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdHabito,TituloHabito,Objetivo,Frecuencia,HacerPrivado")] Habito habito)
        {
            if (id != habito.IdHabito)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Verificar que el hábito pertenece al usuario
            var existingHabito = await _context.Habitos
                .FirstOrDefaultAsync(h => h.IdHabito == id && h.IdUsuario == userId);

            if (existingHabito == null)
            {
                return NotFound();
            }

            // Guardar el objetivo anterior para comparar
            var objetivoAnterior = existingHabito.Objetivo;

            // Actualizar solo los campos editables
            existingHabito.TituloHabito = habito.TituloHabito;
            existingHabito.Objetivo = habito.Objetivo;
            existingHabito.Frecuencia = habito.Frecuencia;
            existingHabito.HacerPrivado = habito.HacerPrivado;
            existingHabito.UltimaActualizacion = DateTime.Now;

            // Recalcular progreso siempre, no solo cuando cambia el objetivo
            existingHabito.Progreso = (float)existingHabito.DiasCompletados / existingHabito.Objetivo * 100;
            existingHabito.Progreso = Math.Min(existingHabito.Progreso, 100);
            existingHabito.ObjetivoCumplido = existingHabito.DiasCompletados >= existingHabito.Objetivo;

            ModelState.Remove("Usuario");
            ModelState.Remove("FechaCreacion");
            ModelState.Remove("UltimaActualizacion");
            ModelState.Remove("UltimaRespuestaFecha");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(existingHabito);
                    await _context.SaveChangesAsync();
                    await ActualizarEstadisticasUsuario(userId);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HabitoExists(habito.IdHabito))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(habito);
        }


        // POST: Habitos/RegistrarRespuesta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarRespuesta(int id, bool cumplido)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var habito = await _context.Habitos
                .FirstOrDefaultAsync(h => h.IdHabito == id && h.IdUsuario == userId);

            if (habito == null) return NotFound();

            if (habito.UltimaRespuestaFecha.Date == DateTime.Today)
            {
                return BadRequest("Ya has respondido hoy para este hábito");
            }

            habito.UltimaRespuestaFecha = DateTime.Now;
            habito.HoyRespondido = true;
            habito.UltimaActualizacion = DateTime.Now;

            List<Logro> logrosCumplidos = new List<Logro>();

            if (cumplido)
            {
                // Botón verde: Sumar 1 al RecuentoObjetivo
                habito.RecuentoObjetivo += 1;
                habito.RachaActual++;
                habito.DiasCompletados++;

                if (habito.RachaActual > habito.MaximaRacha)
                {
                    habito.MaximaRacha = habito.RachaActual;
                }

                var logrosRacha = await VerificarLogrosRacha(userId, habito.RachaActual);
                logrosCumplidos.AddRange(logrosRacha);
            }
            else
            {
                // Botón rojo: Restar 3 al RecuentoObjetivo (sin bajar de 0)
                habito.RecuentoObjetivo = Math.Max(habito.RecuentoObjetivo - 3, 0);
                habito.RachaActual = 0;
                habito.DiasFallados++;
            }

            // Cálculo del Progreso basado en RecuentoObjetivo/Objetivo
            if (habito.RecuentoObjetivo <= habito.Objetivo)
            {
                habito.Progreso = (float)habito.RecuentoObjetivo / habito.Objetivo * 100;
                habito.Progreso = Math.Min(habito.Progreso, 100);
            }

            bool objetivoRecienCumplido = !habito.ObjetivoCumplido && habito.RecuentoObjetivo >= habito.Objetivo;
            habito.ObjetivoCumplido = habito.RecuentoObjetivo >= habito.Objetivo;

            _context.Update(habito);
            await _context.SaveChangesAsync();

            if (objetivoRecienCumplido)
            {
                var logrosCompletado = await VerificarLogrosCompletado(userId, habito);
                logrosCumplidos.AddRange(logrosCompletado);
            }

            // Almacenar los nombres de los logros cumplidos para mostrar en la vista
            if (logrosCumplidos.Any())
            {
                TempData["LogrosCumplidos"] = logrosCumplidos.Select(l => l.NombreLogro).ToArray();
            }

            await ActualizarEstadisticasUsuario(userId);

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<Logro>> VerificarLogrosRacha(int userId, int rachaActual)
        {
            var logrosCumplidos = new List<Logro>();
            var logros = await _context.Logros
                .Where(l => l.IdUsuario == userId)
                .ToListAsync();

            if (rachaActual >= 3)
            {
                var principiante = logros.FirstOrDefault(l => l.NombreLogro == "Principiante" && !l.Completado);
                if (principiante != null)
                {
                    principiante.Completado = true;
                    _context.Update(principiante);
                    logrosCumplidos.Add(principiante);
                }
            }

            if (rachaActual >= 7)
            {
                var semanaSerio = logros.FirstOrDefault(l => l.NombreLogro == "Semana seria" && !l.Completado);
                if (semanaSerio != null)
                {
                    semanaSerio.Completado = true;
                    _context.Update(semanaSerio);
                    logrosCumplidos.Add(semanaSerio);
                }
            }

            if (rachaActual >= 30)
            {
                var consistente = logros.FirstOrDefault(l => l.NombreLogro == "Consistente" && !l.Completado);
                if (consistente != null)
                {
                    consistente.Completado = true;
                    _context.Update(consistente);
                    logrosCumplidos.Add(consistente);
                }
            }

            if (rachaActual >= 100)
            {
                var maestro = logros.FirstOrDefault(l => l.NombreLogro == "Maestro de la racha" && !l.Completado);
                if (maestro != null)
                {
                    maestro.Completado = true;
                    _context.Update(maestro);
                    logrosCumplidos.Add(maestro);
                }
            }

            await _context.SaveChangesAsync();
            await VerificarLogroLeyenda(userId);

            return logrosCumplidos;
        }

        private async Task<List<Logro>> VerificarLogrosCompletado(int userId, Habito habito)
        {
            var logrosCumplidos = new List<Logro>();
            var logros = await _context.Logros
                .Where(l => l.IdUsuario == userId)
                .ToListAsync();

            var primeraVictoria = logros.FirstOrDefault(l => l.NombreLogro == "Primera victoria" && !l.Completado);
            if (primeraVictoria != null)
            {
                primeraVictoria.Completado = true;
                _context.Update(primeraVictoria);
                logrosCumplidos.Add(primeraVictoria);
            }

            if (habito.DiasFallados == 0)
            {
                var inquebrantable = logros.FirstOrDefault(l => l.NombreLogro == "Inquebrantable" && !l.Completado);
                if (inquebrantable != null)
                {
                    inquebrantable.Completado = true;
                    _context.Update(inquebrantable);
                    logrosCumplidos.Add(inquebrantable);
                }
            }

            await _context.SaveChangesAsync();
            await VerificarLogroLeyenda(userId);

            return logrosCumplidos;
        }

        private async Task<List<Logro>> VerificarLogroLeyenda(int userId)
        {
            var logrosCumplidos = new List<Logro>();
            var logros = await _context.Logros
                .Where(l => l.IdUsuario == userId && l.NombreLogro != "Leyenda")
                .ToListAsync();

            bool todosCompletados = logros.All(l => l.Completado);

            if (todosCompletados)
            {
                var leyenda = await _context.Logros
                    .FirstOrDefaultAsync(l => l.IdUsuario == userId && l.NombreLogro == "Leyenda" && !l.Completado);

                if (leyenda != null)
                {
                    leyenda.Completado = true;
                    _context.Update(leyenda);
                    await _context.SaveChangesAsync();
                    logrosCumplidos.Add(leyenda);
                }
            }

            return logrosCumplidos;
        }

        private async Task ActualizarEstadisticasUsuario(int userId)
        {
            var estadisticas = await _context.Estadisticas
                .FirstOrDefaultAsync(e => e.IdUsuario == userId);

            if (estadisticas == null) return;

            var habitosUsuario = await _context.Habitos
                .Where(h => h.IdUsuario == userId)
                .ToListAsync();

            estadisticas.NumHabitTotal = habitosUsuario.Count;
            estadisticas.NumHabitActivos = habitosUsuario.Count(h => !h.ObjetivoCumplido);
            estadisticas.NumHabitCompletados = habitosUsuario.Count(h => h.ObjetivoCumplido);

            var habitoMaxRacha = habitosUsuario.OrderByDescending(h => h.MaximaRacha).FirstOrDefault();
            estadisticas.RachaMaxTotNum = habitoMaxRacha?.MaximaRacha ?? 0;
            estadisticas.RachaMaxTotTitulo = habitoMaxRacha?.TituloHabito ?? "";

            var habitoActivoMaxRacha = habitosUsuario
                .Where(h => !h.ObjetivoCumplido)
                .OrderByDescending(h => h.RachaActual)
                .FirstOrDefault();
            estadisticas.RachaMaxActNum = habitoActivoMaxRacha?.RachaActual ?? 0;
            estadisticas.RachaMaxActTitulo = habitoActivoMaxRacha?.TituloHabito ?? "";

            var habitoMayorProgreso = habitosUsuario.OrderByDescending(h => h.Progreso).FirstOrDefault();
            estadisticas.PorcentajeMayorAvance = habitoMayorProgreso?.Progreso ?? 0;
            estadisticas.HabitoMayorAvance = habitoMayorProgreso?.TituloHabito ?? "";

            _context.Update(estadisticas);
            await _context.SaveChangesAsync();
        }
        private bool HabitoExists(int id)
        {
            return _context.Habitos.Any(e => e.IdHabito == id);
        }
    }
}