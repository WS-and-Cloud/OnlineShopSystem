﻿namespace OnlineShop.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CreateAdBindingModel 
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        public string Description { get; set; }

        public int TypeId { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public IEnumerable<int> Categories { get; set; } 
    }
}