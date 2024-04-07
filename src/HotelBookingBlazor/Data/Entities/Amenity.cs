﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingBlazor.Data.Entities
{
    public class Amenity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(25), Unicode(false)]
        public string Name { get; set; }

        [Required, MaxLength(25), Unicode(false)]
        public string Icon { get; set; }
    }
}
