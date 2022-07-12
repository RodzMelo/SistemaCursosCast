using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCursosCast.Models;

namespace SistemaCursosCast.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoriasController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoria()
        {
            try
            {
                return await _context.Categorias.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Aconteceu um erro inesperado.", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);

                if (categoria == null) 
                {
                    return NotFound($"Categoria com o id especificado não foi encontrado ({id}).");
                }

                return categoria;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Aconteceu um erro inesperado.", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, Categoria categoria)
        {
            try
            {
                if (!CategoriaExists(id))
                {
                    return NotFound($"A Categoria de ID ({id}) não existe");
                }
                
                if (id != categoria.CategoriaId)
                {
                    return BadRequest("ID da Categoria diferente do ID do Parâmetro");
                }

                if (id <= 0)
                {
                    return BadRequest($"ID informado inválido, deve ser um valor positivo.");
                }

                _context.Entry(categoria).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                
                return Ok($"A atualização da categoria de ID '{id}' foi realizada com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Aconteceu um erro inesperado.", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }


        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
        {
            try
            {
                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Categoria cadastrada com sucesso.", id = categoria.CategoriaId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Aconteceu um erro inesperado.", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria == null)
                {
                    return NotFound($"Categoria com ID ({id}) não encontrada.");
                }

                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Categoria excluída com sucesso.", id = categoria.CategoriaId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Aconteceu um erro inesperado.", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        private bool CategoriaExists(int id)
        {
            return (_context.Categorias?.Any(e => e.CategoriaId == id)).GetValueOrDefault();
        }
    }
}
