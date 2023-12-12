﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoMonitor.Model
{
    [Index(nameof(name), IsUnique = true)]
    public class Company
    {
        [Key]
        public int id { get; set; }
        [Required]
        [MaxLength(45)]
        public string name { get; set; }
        public string description { get; set; }

        [ForeignKey("Region")]
        public int? region_id { get; set; }

        public Region Region { get; set; }
        public List<News> news { get; set; }
    }
}
