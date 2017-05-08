using AjourBT.Domain.CustomAnnotations;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourBT.Domain.ViewModels
{
    public class RegistrationDateViewModel
    {

        public int EmployeeID { get; set; }
        [Required]
        [Display(Name = "Type")]
        public string VisaType { get; set; }

        [RequiredIf("PaymentDate", "", ErrorMessage = "Either Registration Date or Payment Date must be filled in")]
        [Display(Name = "Registration Date")]
        public string RegistrationDate { get; set; }

        [Display(Name = "Registration Time")]
        public string RegistrationTime { get; set; }
        
        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }

        [RequiredIf("RegistrationDate", "", ErrorMessage = "Either Registration Date or Payment Date must be filled in")]
        [Display(Name = "Payment Date")]
        public string PaymentDate { get; set; }

        [Display(Name = "Payment Time")]
        public string PaymentTime { get; set; }

        [Display(Name = "Payment PIN")]
        public string PaymentPIN { get; set; }

        public RegistrationDateViewModel(VisaRegistrationDate visaRegistrationDate)
        {
            EmployeeID = visaRegistrationDate.EmployeeID;
            VisaType = visaRegistrationDate.VisaType;
            RegistrationDate = string.Format("{0:d}", visaRegistrationDate.RegistrationDate);
            RegistrationTime = visaRegistrationDate.RegistrationTime;
            City = visaRegistrationDate.City;
            RegistrationNumber = visaRegistrationDate.RegistrationNumber;
            PaymentDate = string.Format("{0:d}", visaRegistrationDate.PaymentDate);
            PaymentTime = visaRegistrationDate.PaymentTime;
            PaymentPIN = visaRegistrationDate.PaymentPIN;
            RowVersion = visaRegistrationDate.RowVersion;
        }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}