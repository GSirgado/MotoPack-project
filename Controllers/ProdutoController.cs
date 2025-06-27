using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MotoPack_project.Models;
using MotoPack_project.Data;
using System.Security.Claims;

namespace MotoPack_project.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProdutoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Categorias()
        {
            return View();
        }

        public IActionResult Catalogo(string categoria, int? precoMax, string search)
        {
            var produtos = _context.Produtos.AsQueryable();

            if (!string.IsNullOrEmpty(categoria))
            {
                produtos = produtos.Where(p => p.Categoria == categoria);
                ViewBag.CategoriaAtual = categoria;
            }

            if (!string.IsNullOrEmpty(search))
            {
                produtos = produtos.Where(p => p.Nome.Contains(search));
                ViewBag.SearchTerm = search;
            }

            var precoMin = produtos.Any() ? (int)produtos.Min(p => (double)p.Preco) : 0;
            var precoMaximo = produtos.Any() ? (int)produtos.Max(p => (double)p.Preco) : 1000;

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
            var produto = _context.Produtos.Include(p => p.Utilizador).FirstOrDefault(p => p.Id == id);
            return produto == null ? NotFound() : View(produto);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AdicionarProduto()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AdicionarProduto(Produto produto)
        {
            if (!ModelState.IsValid)
                return View(produto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToAction("Login", "Conta");

            produto.UtilizadorId = int.Parse(userId);
            produto.DataCriacao = DateTime.Now;

            if (produto.Imagem != null && produto.Imagem.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(produto.Imagem.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await produto.Imagem.CopyToAsync(stream);
                }

                produto.ImageUrl = "/uploads/" + fileName;
            }

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return RedirectToAction("Perfil", "Conta");
        }
    }
}
