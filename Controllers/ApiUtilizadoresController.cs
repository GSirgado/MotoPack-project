using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoPack_project.Data;
using MotoPack_project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MotoPack_project.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[action]")]
    [ApiController]
    public class ApiUtilizadoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiUtilizadoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/GetUtilizadores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Registar>>> GetUtilizadores()
        {
            return await _context.Registars.ToListAsync();
        }

        // GET: api/GetUtilizadorById/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Registar>> GetUtilizadorById(int id)
        {
            var user = await _context.Registars.FindAsync(id);

            if (user == null)
                return NotFound();

            return user;
        }

        // POST: api/CreateUtilizador
        [HttpPost]
        public async Task<ActionResult<Registar>> CreateUtilizador(Registar registar)
        {
            // Hash da senha antes de salvar no banco de dados
            var passwordHasher = new PasswordHasher<Registar>();
            registar.Pass = passwordHasher.HashPassword(registar, registar.Pass);
            registar.ConfPass = registar.Pass;


            _context.Registars.Add(registar);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUtilizadorById), new { id = registar.Id }, registar);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUtilizador(int id, UpdateRegistarModel registar)
        {
            if (id != registar.Id)
                return BadRequest("O ID da URL não coincide com o ID do objeto.");

            var utilizadorExistente = await _context.Registars.FindAsync(id);
            if (utilizadorExistente == null)
                return NotFound("Utilizador não encontrado.");

            utilizadorExistente.Nome = registar.Nome;
            utilizadorExistente.Email = registar.Email;
            utilizadorExistente.IsAdmin = registar.IsAdmin;
            utilizadorExistente.FotoPerfil = registar.FotoPerfil;

            if (!string.IsNullOrWhiteSpace(registar.NovaPass))
            {
                var hasher = new PasswordHasher<Registar>();
                var novaHash = hasher.HashPassword(utilizadorExistente, registar.NovaPass);
                utilizadorExistente.Pass = novaHash;
                utilizadorExistente.ConfPass = novaHash;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }





        // DELETE: api/DeleteUtilizador/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUtilizador(int id)
        {
            var user = await _context.Registars.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Registars.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UtilizadorExists(int id)
        {
            return _context.Registars.Any(e => e.Id == id);
        }
    }
}