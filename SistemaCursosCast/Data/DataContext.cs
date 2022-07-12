using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaCursosCast.Models;

    public class DataContext : DbContext
    {
        public DataContext (DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Curso> Cursos { get; set; }

        public DbSet<Log> Logs { get; set; }
    }
