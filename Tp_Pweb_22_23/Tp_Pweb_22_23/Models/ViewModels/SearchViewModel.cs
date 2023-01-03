using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Tp_Pweb_22_23.Models.ViewModels
{
    public class SearchViewModel
    {
        public string Localizacao { get; set; }
        [Display(Name = "Data de recolha")]
        public DateTime? DataRecolha { get; set; }
        [Display(Name = "Data de entrega")]
        public DateTime? DataEntrega { get; set; }
        public string Categoria { get; set; }
    }
}
