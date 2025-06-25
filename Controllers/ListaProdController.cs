using Microsoft.AspNetCore.Mvc;
using MotoPack_project.Models;
using MotoPack_project.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MotoPack_project.Controllers
{
    public class ListaProdController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ListaProdController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Catalogo(string categoria, int? precoMax)
        {
            var produtos = _context.Produtos.AsQueryable();

            if (!string.IsNullOrEmpty(categoria))
            {
                produtos = produtos.Where(p => p.Categoria == categoria);
                ViewBag.CategoriaAtual = categoria;
            }

            // Calcular preços mínimo e máximo
            var precoMin = produtos.Any() ? (int)produtos.Min(p => p.Preco) : 0;
            var precoMaximo = produtos.Any() ? (int)produtos.Max(p => p.Preco) : 1000;

            ViewBag.PrecoMin = precoMin;
            ViewBag.PrecoMax = precoMaximo;

            if (precoMax.HasValue)
            {
                produtos = produtos.Where(p => p.Preco <= precoMax.Value);
                ViewBag.PrecoSelecionado = precoMax.Value;
            }

            return View(produtos.ToList());
        }



        public IActionResult Produto(int id)
        {
            var produto = _context.Produtos
                .Include(p => p.Utilizador)
                .FirstOrDefault(p => p.Id == id);

            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        public IActionResult Categorias()
        {
            return View();
        }
    }
}
