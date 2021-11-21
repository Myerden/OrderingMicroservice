using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Application.Dto
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        [Required]
        public Guid CustomerId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        [Range(double.Epsilon, double.MaxValue)]
        public double Price { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public AddressDTO Address { get; set; }
        [Required]
        public ProductDTO Product { get; set; }
    }
}
