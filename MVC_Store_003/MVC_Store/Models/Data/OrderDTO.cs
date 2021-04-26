using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_Store.Models.Data
{
    [Table("tblOrders")]
    public class OrderDTO
    {
        [Key]

        ///Может быть ошибка в этом поле Id (OrderId)
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDTO USers { get; set; }
    }
}