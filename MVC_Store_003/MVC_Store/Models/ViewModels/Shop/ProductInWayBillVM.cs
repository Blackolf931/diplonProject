using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_Store.Models.ViewModels.Shop
{
    public class ProductInWayBillVM
    {
        public class ProductInWayBillDTO
        {
            public ProductInWayBillDTO()
            {
            }

            public ProductInWayBillDTO(ProductInWayBillDTO row)
            {
                IdProductInWayBill = row.IdProductInWayBill;
                IdProduct = row.IdProduct;
                IdWayBill = row.IdWayBill;
                CountProduct = row.CountProduct;
            }

            [Key]
            public int IdProductInWayBill { get; set; }
            public int IdProduct { get; set; }
            public int IdWayBill { get; set; }
            public int CountProduct { get; set; }
        }
    }
}