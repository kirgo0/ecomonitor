﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcoMonitor.Model.DTO
{
    public class EnvFactorUpdateDTO
    {
        [Required]
        [Range(0,int.MaxValue)]
        public int id { get; set; }
        [Required]
        [MaxLength(50)]
        public string factor_Name { get; set; }
        [Required]
        [Range(0,double.MaxValue)]
        public double factor_value { get; set; }
        [Required]
        public int passport_id { get; set; }

    }
}
