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
    public class LogsController : ControllerBase
    {
        private readonly DataContext _context;

        public LogsController(DataContext context)
        {
            _context = context;
        }

        
        [HttpGet("CursoId")]
        public async Task<ActionResult<IEnumerable<Log>>> GetLog(int CursoId)
        {
            try
            {
                List<Log> logs = await _context.Logs.Where(log => log.CursoId == CursoId).ToListAsync();

                if (logs == null || logs.Count == 0)
                {
                    return NotFound($"Não foram encontrados Logs para este curso: ({CursoId}).");
                }

                return logs;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Aconteceu um erro inesperado.", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }


        
        [HttpPost]
        public async Task<ActionResult<Log>> PostLog(Log log)
        {
            try
            {
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"O cadastro do Log foi um sucesso.", id = log.LogId });
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
                var log = await _context.Logs.FindAsync(id);
                if (log == null)
                {
                    return NotFound($"Log não encontrada com o ID informado ({id}).");
                }

                _context.Logs.Remove(log);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"A exclusão do Log foi um sucesso.", id = log.LogId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Aconteceu um erro inesperado.", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        private bool LogExists(int id)
        {
            return (_context.Logs?.Any(e => e.LogId == id)).GetValueOrDefault();
        }
    }
}
