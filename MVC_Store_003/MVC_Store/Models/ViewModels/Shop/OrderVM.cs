using MVC_Store.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Store.Models.ViewModels.Shop
{
    public class OrderVM
    {
        public OrderVM()
        {

        }

        public OrderVM(OrderDTO dto)
        {
            OrderId = dto.OrderId;
            UserId = dto.UserId;
            CreatedAt = dto.CreatedAt;
        }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}