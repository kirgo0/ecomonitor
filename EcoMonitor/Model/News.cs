﻿using EcoMonitor.Migrations;
using EcoMonitor.Model.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EcoMonitor.Model
{
    [Index(nameof(title), IsUnique = true)]
    public class News : IEntityWithId
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        [MinLength(250)]
        public string body { get; set; }
        public DateTime? post_date { get; set; }
        public DateTime? update_date { get; set; }

        public string? source_url { get; set; }
        public virtual List<User> authors { get; set; }
        public virtual List<User> followers { get; set; }

        public virtual List<Company> companies { get; }

        public virtual List<Region> regions { get; }

    }
}
