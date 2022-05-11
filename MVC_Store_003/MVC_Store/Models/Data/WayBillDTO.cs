using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Models.Data
{
    [Table("tblWayBill")]
    public class WayBillDTO
    {
        [Key]
        public int IdWayBill { get; set; }
        public string ProviderName { get; set; }
        public string RecipientName { get; set; }
        public string Payer { get; set; }
        public string Base { get; set; }
        public int DocumentNumber { get; set; }
        public string TheCargoWasRealesed { get; set; }
        public DateTime CreatedAt { get; set; }
        public int IdProductInWayBill { get; set; }

        [ForeignKey("IdProductInWayBill")]
        public virtual ProductDTO ProductsInWayBill { get; set; }
    }
}