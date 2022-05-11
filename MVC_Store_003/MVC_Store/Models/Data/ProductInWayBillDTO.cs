using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Store.Models.Data
{
    [Table("tblProductInWayBill")]
    public class ProductInWayBillDTO
    {
        [Key]
        public int IdProductInWayBill { get; set; }
        public int IdProduct { get; set; }
        public int IdWayBill { get; set; }
        public int CountProduct { get; set; }
    }
}