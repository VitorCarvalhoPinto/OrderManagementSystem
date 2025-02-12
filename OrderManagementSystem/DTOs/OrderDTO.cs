using Microsoft.AspNetCore.Mvc;

namespace OrderManagementSystem.Models
{
    public class OrderDTO
    {
        public Guid OrderId { get; set; }
        public String Cliente { get; set; }
        public String Produto { get; set; }
        public decimal Valor { get; set; }
        public String Status { get; set; } // Pendente, Processando, Finalizado
        public DateTime DataCriacao { get; set; }
    }
}
