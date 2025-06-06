using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MotoPack_project.Models;

namespace MotoPack_project.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Catalogo()
    {
        return View();
    }

    public IActionResult Categorias()
    {
        return View();
    }

    public IActionResult Produto()
    {
        return View();
    }

    public IActionResult Suporte()
    {
        return View();
    }

    public IActionResult Perfil()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}