using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Infrastructure
{
    public interface IXLSExporter
    {
        byte[] ExportAbsenceToExcel(IList<AbsenceViewModel> absences);
        byte[] ExportEmployeesToExcelADM(IList<EmployeeViewModel> employees);
        byte[] ExportEmployeesToExcelVU(IList<EmployeeViewModel> employees);
        byte[] ExportBusinessTripsToExcelVU(IList<BusinessTripViewModel> businessTrips);
        byte[] ExportVisasAndPermitsVU(IList<Employee> employee);
        byte[] ExportWTR(DateTime fromDate, DateTime toDate, IList<WTRViewModel> wtrDataList); 

    }
}
