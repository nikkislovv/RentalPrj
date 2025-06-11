﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Car
    {
        public Guid Id { get; set; }
        public string VehicleNumber { get; set; }
        public string Title { get; set; }
        public string Model { get; set; }
        public DateTime ReleaseYear { get; set; }
        public string Color { get; set; }
        public decimal RentPrice { get; set; }
        public bool IsAvailable { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
