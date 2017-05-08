using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Domain.ViewModels
{
    public class JourneysByEmployeeViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "Field First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Field Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Field EID is required")]
        public string EID { get; set; }

        public string Department { get; set; }
     
        [Display(Name = "Date Dismissed")]
        public string DateDismissed { get; set; }

        public List<Journey> Journeys { get; set; }


    }
}