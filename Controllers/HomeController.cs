using Microsoft.AspNetCore.Mvc;
using MotoPack_project.Models;
using MotoPack_project.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace MotoPack_project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalogo()
        {
            var produtos = _context.Produtos.ToList();
            return View(produtos);
        }

        public IActionResult Categorias()
        {
            return View();
        }

        public IActionResult Suporte()
        {
            return View();
        }

    }
}
