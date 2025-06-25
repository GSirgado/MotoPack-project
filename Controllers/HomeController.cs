using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using MotoPack_project.Models;
using MotoPack_project.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
=======
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97

namespace MotoPack_project.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
<<<<<<< HEAD

        [HttpGet]
        [HttpGet]
        public IActionResult Suporte()
        {
            var suporte = new Suporte();

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.Registars.FirstOrDefault(u => u.Id == int.Parse(userId));

                if (user != null)
                {
                    suporte.Nome = user.Nome;
                    suporte.Email = user.Email;
                }
            }

            return View(suporte);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnviarPedido(Suporte model)
        {
            Console.WriteLine(">>> EnviarPedido CHAMADO");

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Erro em {entry.Key}: {error.ErrorMessage}");
                    }
                }

                return View("Suporte", model);
            }

            try
            {
                model.DataHora = DateTime.Now;

                _context.Suportes.Add(model);
                _context.SaveChanges();

                Console.WriteLine(">>> Pedido guardado com sucesso!");

                TempData["SuporteSucesso"] = "Pedido de ajuda enviado com sucesso!";
            }
            catch (Exception ex)
            {
                Console.WriteLine(">>> ERRO ao guardar o pedido de suporte: " + ex.Message);
                TempData["SuporteSucesso"] = "Ocorreu um erro ao enviar o pedido.";
                return View("Suporte", model);
            }

            return RedirectToAction("Suporte");
        }



        [HttpGet]
        [Authorize]
        public IActionResult AdicionarProduto()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarProduto(Produto produto)
        {
            if (!ModelState.IsValid) return View(produto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");

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
            _context.SaveChanges();

            return RedirectToAction("Perfil");
        }
=======
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
    }
}
