﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_HospitalDatabase.Data.Models
{
   public class Visitation
    {
        [Key]
        public int VisitationId { get; set; }

        public DateTime Date { get; set; }

        [Required , MaxLength(250)]
        public string Comments { get; set; }

        [ForeignKey(nameof(Doctor))]
        public int DoctorId { get; set; }

        public Doctor Doctor { get; set; }

        public Patient Patient { get; set; }

        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }

      
    }
}
