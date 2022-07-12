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
    public class CursosController : ControllerBase
    {
        private readonly DataContext _context;

        public CursosController(DataContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Curso>>> GetCurso()
        {
            try
            {
                return await _context.Cursos.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Aconteceu um erro inesperado.", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

 
        [HttpGet("{id}")]
        public async Task<ActionResult<Curso>> GetCurso(int id)
        {
            try
            {
                var curso = await _context.Cursos.FindAsync(id);

                if (curso == null)
                {
                    return NotFound($"Curso informado não encontrado com o ID ({id}).");
                }

                return curso;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Aconteceu um erro inesperado.", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCurso(int id, Curso curso)
        {
            try
            {
                int validarResultado = ValidarData(curso.DataInicial, curso.DataFinal, curso.CursoId);

                switch (validarResultado)
                {
                    case 1:
                        return BadRequest(new { message = "A data de início não pode ser anterior à data de hoje.", errorCode = 1 });
                        break;
                    case 2:
                        return BadRequest(new { message = "A data de término não pode ser anterior à data de início.", errorCode = 2 });
                        break;
                    case 3:
                        return BadRequest(new { message = "Existem cursos planejados no mesmo período.", errorCode = 3 });
                        break;
                }

                Curso duplicado = ChecarDuplicados(curso);

                if (duplicado != default && duplicado.CursoId != 0 && duplicado.CursoId != curso.CursoId)
                {
                    return BadRequest(new { message = "Curso já cadastrado no sistema.", errorCode = 4 });
                }



                if (id != curso.CursoId)
                {
                    return BadRequest("ID do Curso diferente do ID do parâmetro.");
                }

                if (id <= 0)
                {
                    return BadRequest($"ID informado inválido ({id}), deve ser um valor positivo.");
                }

                if (!CursoExists(id))
                {
                    return NotFound($"O Curso de ID informado ({id}) não existe");
                }

                _context.Entry(curso).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Curso atualizado com sucesso.", id = curso.CursoId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Aconteceu um erro inesperado.", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

     
        
        [HttpPost]
        public async Task<ActionResult<Curso>> PostCurso(Curso curso)
        {
            try
            {
                int resultadoValidacao = ValidarData(curso.DataInicial, curso.DataFinal, curso.CursoId);

                switch (resultadoValidacao)
                {
                    case 1:
                        return BadRequest(new { message = "A data de início não pode ser anterior à data de hoje.", errorCode = 1 });
                        break;
                    case 2:
                        return BadRequest(new { message = "A data de término não pode ser anterior à data de início.", errorCode = 2 });
                        break;
                    case 3:
                        return BadRequest(new { message = "Existem cursos planejados no mesmo período", errorCode = 3 });
                        break;
                }

                Curso duplicado = ChecarDuplicados(curso);

                if (duplicado != default && duplicado.CursoId != 0)
                {
                    return BadRequest(new { message = "Este curso já está cadastrado.", errorCode = 4 });
                }

                _context.Cursos.Add(curso);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Cadastro do curso realizado com sucesso", id = curso.CursoId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Aconteceu um erro inesperado.", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

            
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurso(int id)
        {
            try
            {
                Curso curso = await _context.Cursos.FindAsync(id);

                if (curso.DataFinal < DateTime.Now.Date)
                {
                    return BadRequest(new { message = "Não é permitido excluir cursos já realizados!.", errorCode = 5 });
                }



                if (curso == null)
                {
                    return NotFound($"Curso com o ID ({id}) não encontrado.");
                }

                List<Log> Logs = await _context.Logs.Where(log => log.CursoId == id).ToListAsync();

                foreach (Log log in Logs)
                {
                    _context.Logs.Remove(log);
                }

                _context.Cursos.Remove(curso);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Realizado a exclusão do curso com sucesso.", id = curso.CursoId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Aconteceu um erro inesperado.", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        private bool CursoExists(int id)
        {
            return (_context.Cursos?.Any(e => e.CursoId == id)).GetValueOrDefault();
        }

        
        List<Curso> Checar(DateTime dataInicial, DateTime dataFinal, int id)
        {
            List<Curso> encontrarCursos = _context.Cursos.Where(c =>
                 c.DataInicial <= dataFinal && c.DataFinal >= dataInicial && c.CategoriaId != id).ToList();

            return encontrarCursos;
        }

        Curso ChecarDuplicados(Curso curso)
        {
            Curso cursoDuplicado = _context.Cursos.Where(c =>
              (c.Descricao.ToLower() == curso.Descricao.ToLower() || c.Titulo.ToLower() == curso.Titulo.ToLower()) && c.CategoriaId != curso.CategoriaId).FirstOrDefault();

            return curso;
        }

        int ValidarData(DateTime inicio, DateTime fim, int id)
        {
            DateTime dia = DateTime.Now.Date;
            
            if (inicio < dia)
            {
                //Inicio anterior a hoje;
                return 1;
            }

            if (fim < inicio)
            {
                //Término anterior ao inicio
                return 2;
            }

            List<Curso> sobreposicaoCursos = Checar(inicio, fim, id);

            if (sobreposicaoCursos != null && sobreposicaoCursos.Count > 0)
            {
                //Agenda ocupada
                return 3;
            }

            return 0;
        }
    }
}
