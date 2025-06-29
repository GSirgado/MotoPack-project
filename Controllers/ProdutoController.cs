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

        // Injeta o contexto da base de dados
        public ProdutoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Página de categorias (ex: lista de categorias disponíveis)
        [HttpGet]
        public IActionResult Categorias()
        {
            return View();
        }

        // Página de catálogo - filtra por categoria, nome e preço
        public IActionResult Catalogo(string categoria, int? precoMax, string search)
        {
            var produtos = _context.Produtos.AsQueryable(); // Começa com todos os produtos

            // Filtro por categoria, se fornecida
            if (!string.IsNullOrEmpty(categoria))
            {
                produtos = produtos.Where(p => p.Categoria == categoria);
                ViewBag.CategoriaAtual = categoria;
            }

            // Filtro por nome (pesquisa), se fornecido
            if (!string.IsNullOrEmpty(search))
            {
                produtos = produtos.Where(p => p.Nome.Contains(search));
                ViewBag.SearchTerm = search;
            }

            // Determina o preço mínimo e máximo dos produtos filtrados
            var precoMin = produtos.Any() ? (int)produtos.Min(p => (double)p.Preco) : 0;
            var precoMaximo = produtos.Any() ? (int)produtos.Max(p => (double)p.Preco) : 1000;

            ViewBag.PrecoMin = precoMin;
            ViewBag.PrecoMax = precoMaximo;

            // Filtro por preço máximo, se especificado
            if (precoMax.HasValue)
            {
                produtos = produtos.Where(p => p.Preco <= precoMax.Value);
                ViewBag.PrecoSelecionado = precoMax.Value;
            }

            return View(produtos.ToList()); // Envia os produtos finais para a view
        }

        // Página de detalhes de um produto específico
        public IActionResult Produto(int id)
        {
            var produto = _context.Produtos
                .Include(p => p.Utilizador) // Inclui info do utilizador associado ao produto
                .FirstOrDefault(p => p.Id == id);

            return produto == null ? NotFound() : View(produto); // Retorna 404 se não existir
        }

        // GET - Formulário para adicionar novo produto (apenas autenticados)
        [Authorize]
        [HttpGet]
        public IActionResult AdicionarProduto()
        {
            return View();
        }

        // POST - Submissão do formulário para adicionar produto
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AdicionarProduto(Produto produto)
        {
            if (!ModelState.IsValid)
                return View(produto); // Se o modelo for inválido, retorna à view com erros

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToAction("Login", "Conta"); // Garante que o user está autenticado

            // Define o utilizador e a data de criação
            produto.UtilizadorId = int.Parse(userId);
            produto.DataCriacao = DateTime.Now;

            // Processa a imagem enviada, se houver
            if (produto.Imagem != null && produto.Imagem.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploads); // Garante que a pasta existe

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(produto.Imagem.FileName);
                var filePath = Path.Combine(uploads, fileName);

                // Guarda a imagem no servidor
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await produto.Imagem.CopyToAsync(stream);
                }

                produto.ImageUrl = "/uploads/" + fileName; // Define a URL da imagem
            }

            _context.Produtos.Add(produto); // Adiciona o produto à base de dados
            await _context.SaveChangesAsync(); // Salva alterações

            return RedirectToAction("Perfil", "Conta"); // Redireciona para o perfil do utilizador
        }
    }
}
