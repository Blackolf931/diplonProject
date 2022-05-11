using MVC_Store.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace MVC_Store.Models.ViewModels.Shop
{
    public class WayBillVM
    {
        public WayBillVM()
        {
        }

        public WayBillVM(WayBillDTO row)
        {
            IdWayBill = row.IdWayBill;
            ProviderName = row.ProviderName;
            RecipientName = row.RecipientName;
            Payer = row.Payer;
            Base = row.Base;
            DocumentNumber = row.DocumentNumber;
            TheCargoWasRealesed = row.TheCargoWasRealesed;
            CreatedAt = row.CreatedAt;
            IdProductInWayBill = row.IdProductInWayBill;
        }

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
        public IEnumerable<SelectListItem> ProductsInWayBill { get; set; }
    }
}