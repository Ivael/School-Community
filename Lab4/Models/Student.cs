﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Lab4.Models
{
    public class Student
    {
        public int id { 
            get; 
            set; 
        }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { 
            get; 
            set; 
        }

        [Required]
        [StringLength(50)]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        public string FirstName { 
            get; 
            set; 
        }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]

        public DateTime? Enrollmentdate { 
            get; 
            set; 
        }

        [Display(Name = "Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }
        public List<CommunityMembership> Membership
        {
            get;
            set;
        }

    }
}