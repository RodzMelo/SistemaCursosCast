using System.ComponentModel.DataAnnotations;

namespace SistemaCursosCast.Models
{
    public class Curso
    {
        [Key]
        public int CursoId { get; set; }

        
        [StringLength(40), Required]
        public string Titulo { get; set; }

        
        [StringLength(400), Required]
        public string Descricao { get; set; }

        
        [DataType(DataType.Date), Required]
        public DateTime DataInicial { get; set; }

        
        [DataType(DataType.Date), Required]
        public DateTime DataFinal { get; set; }


        public int? QuantidadeEstudantes { get; set; }


        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }


        public virtual ICollection<Log> Logs { get; set; }
    }
}
