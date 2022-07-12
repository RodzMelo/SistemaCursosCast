using System.ComponentModel.DataAnnotations;

namespace SistemaCursosCast.Models
{
    public class Log
    {
        [Key]
        public int LogId { get; set; }


        [StringLength(40), Required]
        public string Usuario { get; set; }


        [StringLength(50), Required]
        public string Acao { get; set; }


        public int CursoId { get; set; }
        public Curso Curso { get; set; }


        [DataType(DataType.Date)]
        public DateTime DataInclusao { get; set; }


        [DataType(DataType.Date)]
        public DateTime DataAtualizacao { get; set; }
    }
}
