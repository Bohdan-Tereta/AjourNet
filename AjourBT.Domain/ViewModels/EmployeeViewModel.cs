using AjourBT.Domain.Abstract;
using AjourBT.Domain.CustomAnnotations;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Domain.ViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeID { get; set; }
        [Required(ErrorMessage = "Field First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Field Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Field EID is required")]
        public string EID { get; set; }
        //TODO: move custom attributes and viewModels to domain [tebo]
        [RequiredIf("IsUserOnly", false, ErrorMessage = "Field DepartmentID Is Required")]
        public Nullable<int> DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        //TODO: move custom attributes and viewModels to domain [tebo]
        [RequiredIf("IsUserOnly", false, ErrorMessage = "Field Date Employed Is Required")]  
        [Display(Name = "Date Employed")]
        public string DateEmployed { get; set; }
        [GreaterThan("DateEmployed",ErrorMessage = "Date Dismissed should be greater than Date Employed")]
        [Display(Name = "Date Dismissed")]
        public string DateDismissed { get; set; }
        [Display(Name = "Is Manager?")]
        public bool IsManager { get; set; }
        public bool IsGreetingMessageAllow { get; set; }

        public List<BusinessTrip> BusinessTrips { get; set; }
        //TODO: move custom attributes and viewModels to domain [tebo]
        [RequiredIf("IsUserOnly", false, ErrorMessage = "Field Position Is Required")]
        public Nullable<int> PositionID { get; set; }
        public Position Position { get; set; }

        public string BirthDay { get; set; }
        public string Comment { get; set; }
        public string FullNameUk { get; set; }

        public string TitleUk { get; set; }
        public string TitleEn { get; set; }



        //-------------Permit-----------
        [Display(Name = "From")]
        public string PermitStartDate { get; set; }
        [Display(Name = "To")]
        public string PermitEndDate { get; set; }
        [Display(Name = "Karta Polaka")]
        public bool IsKartaPolaka { get; set; }
        [Display(Name = "Num")]
        public string PermitNumber { get; set; }

        //=-------------Visa
        //TODO: move custom attributes and viewModels to domain [tebo]
        //[RequiredIf("IsUserOnly", false, ErrorMessage = "Field Date Employed Is Required")]  
        [Display(Name = "From")]
        public string VisaStartDate { get; set; }
        //TODO: move custom attributes and viewModels to domain [tebo]
        //[RequiredIf("IsUserOnly", false, ErrorMessage = "Field Date Employed Is Required")]  
        [Display(Name = "To")]
        public string VisaDueDate { get; set; }
        [Display(Name = "Registration")]
        public string RegistrationDate { get; set; }
        [Display(Name = "Payment Date")]
        public string PaymentDate { get; set; }
        [Display(Name = "Type")]
        public string VisaType { get; set; }

        public int Entries { get; set; }
        [Display(Name = "Days")]
        public int VisaDays { get; set; }

        [Display(Name = "Entries Used in BT")]
        public int? EntriesUsedInBT { get; set; }
        [Display(Name = "Private Used Entries")]
        public int? EntriesUsedInPrivateTrips { get; set; }

        [Display(Name = "Days Used in BT")]
        public int? DaysUsedInBT { get; set; }
        [Display(Name = "Private Used Days")]
        public int? DaysUsedInPrivateTrips { get; set; }
        public Passport Passport { get; set; }

        public Department Department { get; set; }
        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public bool IsUserOnly;
        public string EMail { get; set; }
        public string Name { get; private set; }
        public string Role { get; private set; }

        [Display(Name = "Education Acquired")]
        public EducationType EducationAcquiredType { get; set; }
        [Display(Name = "Education Acquired Date")]
        public string EducationAcquiredDate { get; set; }

        [Display(Name = "Education In Progress")]
        public EducationType EducationInProgressType { get; set; }
        [Display(Name = "Education In Progress Date")]
        public string EducationInProgressDate { get; set; }
        public string EducationComment { get; set; }

        public string EducationSummary { 
            get
            {
                StringBuilder stringBuilder = new StringBuilder(); 
                if(EducationAcquiredType!=0)
                {
                    stringBuilder.Append(EducationAcquiredType.Description());
                    if (EducationAcquiredDate != null && EducationAcquiredDate != "")
                    { 
                    stringBuilder.Append(", ");
                    stringBuilder.Append(EducationAcquiredDate);
                    }
                }
                if (EducationInProgressType != 0)
                {
                    if (EducationAcquiredType != 0)
                    {
                        stringBuilder.Append("; ");
                    }
                    stringBuilder.Append(EducationInProgressType.Description());
                    if (EducationInProgressDate != null && EducationInProgressDate != "")
                    { 
                    stringBuilder.Append(", ");
                    stringBuilder.Append(EducationInProgressDate);
                    }
                }
                if (stringBuilder.Length != 0 && EducationComment!=null && EducationComment!="")
                {
                    stringBuilder.Append(" ");
                }
              //  stringBuilder.AppendFormat("<b>{0}</b>", EducationComment);

                return stringBuilder.ToString();
            }
            }

        public string EducationFullSummary {
            get { return string.Format(EducationSummary + EducationComment); }
        }

        public EmployeeViewModel(Employee employee)
        {
            EmployeeID = employee.EmployeeID;
            FirstName = employee.FirstName;
            LastName = employee.LastName;
            EID = employee.EID;
            DateEmployed = string.Format("{0:d}", employee.DateEmployed);
            DateDismissed = string.Format("{0:d}", employee.DateDismissed);
            IsManager = employee.IsManager;
            DepartmentID = employee.DepartmentID;
            Department = employee.Department;
            DepartmentName = employee.Department!=null?employee.Department.DepartmentName:null;
            BusinessTrips = employee.BusinessTrips;
            IsKartaPolaka = employee.Permit == null ? false : employee.Permit.IsKartaPolaka;
            PermitStartDate = employee.Permit == null ? null : string.Format("{0:d}", employee.Permit.StartDate);
            PermitEndDate = employee.Permit == null ? null : string.Format("{0:d}", employee.Permit.EndDate);
            PermitNumber = employee.Permit == null ? null : employee.Permit.Number;
            VisaStartDate = employee.Visa == null ? null : string.Format("{0:d}", employee.Visa.StartDate);
            VisaDueDate = employee.Visa == null ? null : string.Format("{0:d}", employee.Visa.StartDate);
            VisaDays = employee.Visa == null ? default(int) : employee.Visa.Days;
            DaysUsedInBT = employee.Visa == null ? default(int) : employee.Visa.DaysUsedInBT;
            DaysUsedInPrivateTrips = employee.Visa == null ? default(int) : employee.Visa.DaysUsedInPrivateTrips;
            Entries = employee.Visa == null ? default(int) : employee.Visa.Entries;
            EntriesUsedInBT = employee.Visa == null ? default(int) : employee.Visa.EntriesUsedInBT;
            EntriesUsedInPrivateTrips = employee.Visa == null ? default(int) : employee.Visa.EntriesUsedInPrivateTrips;
            RegistrationDate = employee.VisaRegistrationDate == null ? null : string.Format("{0:d}", employee.VisaRegistrationDate.RegistrationDate);
            PaymentDate = employee.VisaRegistrationDate == null ? null : string.Format("{0:d}", employee.VisaRegistrationDate.PaymentDate);

            Passport = employee.Passport;
            BirthDay = string.Format("{0:d}", employee.BirthDay);
            Comment = employee.Comment;
            FullNameUk = employee.FullNameUk;
            PositionID = employee.PositionID;
          //  Position = employee.Position;
            TitleEn = employee.Position == null ? null : employee.Position.TitleEn;
            TitleUk = employee.Position == null ? null : employee.Position.TitleUk;
            RowVersion = employee.RowVersion;
            IsUserOnly = employee.IsUserOnly;
            IsGreetingMessageAllow = employee.IsGreetingMessageAllow;
            EMail = employee.EMail;
            Name = employee.LastName + " " + employee.FirstName;
            if (System.Web.Security.Membership.GetUser(employee.EID) != null)
                        {
                           Role = String.Join(", ", System.Web.Security.Roles.GetRolesForUser(employee.EID));
                        }
            EducationAcquiredType = employee.EducationAcquiredType; 
            EducationAcquiredDate = string.Format("{0:d}", employee.EducationAcquiredDate); 
            EducationInProgressType = employee.EducationInProgressType;
            EducationInProgressDate = string.Format("{0:d}", employee.EducationInProgressDate);
            EducationComment =  employee.EducationComment; 
        }

        public List<String> PrepareToXLSExportADM()
        {
            List<string> stringifiedProperties = new List<string>();
                stringifiedProperties.Add(this.EID ?? String.Empty);
                stringifiedProperties.Add(this.Name ?? String.Empty);
                stringifiedProperties.Add(this.Role ?? String.Empty);
                stringifiedProperties.Add(this.DepartmentName ?? String.Empty);
                stringifiedProperties.Add(this.TitleEn ?? String.Empty);
                stringifiedProperties.Add(this.DateEmployed ?? String.Empty);
                stringifiedProperties.Add(this.FullNameUk ?? String.Empty);
                stringifiedProperties.Add(this.BirthDay ?? String.Empty);
                stringifiedProperties.Add(this.Comment ?? String.Empty);
                stringifiedProperties.Add(this.DateDismissed ?? String.Empty);
                stringifiedProperties.Add(this.EducationFullSummary ?? String.Empty);


            return stringifiedProperties; 
        }

        public List<String> PrepareToXLSExportVU()
        {
            List<string> stringifiedProperties = new List<string>();
            stringifiedProperties.Add(this.EID ?? String.Empty);
            stringifiedProperties.Add(this.Name ?? String.Empty);
            stringifiedProperties.Add(this.FullNameUk ?? String.Empty);
            stringifiedProperties.Add(this.DepartmentName ?? String.Empty);
            stringifiedProperties.Add(this.TitleEn ?? String.Empty);
            stringifiedProperties.Add(this.TitleUk ?? String.Empty);
            stringifiedProperties.Add(this.DateEmployed ?? String.Empty);
            stringifiedProperties.Add(this.DateDismissed ?? String.Empty); 

            return stringifiedProperties;
        }
    }
}