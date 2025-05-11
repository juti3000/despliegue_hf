using Habitforger.Data;
using Habitforger.Models;
using Microsoft.AspNetCore.Mvc;

public class UsuariosController : Controller
{
    private readonly ApplicationDbContext _context;

    public UsuariosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Usuarios
    public IActionResult Index()
    {
        return View(_context.Usuarios.ToList());
    }

    // GET: Usuarios/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Usuarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Usuario usuario)
    {
        if (ModelState.IsValid)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(usuario);
    }
}