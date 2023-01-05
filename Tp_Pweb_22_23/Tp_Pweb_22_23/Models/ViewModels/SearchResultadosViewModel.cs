using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Tp_Pweb_22_23.Models.ViewModels
{
    public class SearchResultadosViewModel
    {
        public List<Empresa> EmpresasVeiculos{ get; set; }
        public List<Veiculo> VeiculosDisponiveis { get; set; }
        [Display(Name = "Data de recolha")]
        public DateTime? DataRecolha { get; set; }
        [Display(Name = "Data de entrega")]
        public DateTime? DataEntrega { get; set; }
        public decimal? Total { get; set; }
    }
}
