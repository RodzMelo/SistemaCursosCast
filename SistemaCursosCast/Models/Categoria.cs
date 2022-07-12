using System.ComponentModel.DataAnnotations;

namespace SistemaCursosCast.Models
{
    public class Categoria
    {
        [Key]
        public int CategoriaId { get; set; }

        [StringLength(40), Required]
        public string Nome { get; set; }
        
        public virtual ICollection<Curso> Cursos { get; set; }
    }
}
