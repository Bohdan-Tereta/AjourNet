using AjourBT.Domain.Entities;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.ViewModels;
using ExcelLibrary.SpreadSheet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace AjourBT.Infrastructure
{
    public  class XlsExporter: IXLSExporter
    {
        public  string[] caption;
        public  ushort[] columnWidths;
        public  Worksheet workSheet;

        public  byte[] ExportAbsenceToExcel(IList<AbsenceViewModel> absences)
        {
            Workbook workBook = new Workbook();
            workSheet = new Worksheet("Absence");
            caption = new string[] { "Department", "Name", "EID", "Journeys", "BusinessTrips", "Overtimes", "Sickness", "Vacations" };
            columnWidths = new ushort[] { 3000, 6000, 3000, 6000, 7500, 6000, 6000, 6000 };
            CreateCaption(caption); 
            WriteAbsenceData(absences);
            SetColumnWidths(columnWidths); 
            workBook.Worksheets.Add(workSheet);
            MemoryStream stream = new MemoryStream();
            workBook.SaveToStream(stream);
            return stream.ToArray();
        }

        public  byte[] ExportEmployeesToExcelADM(IList<EmployeeViewModel> employees)
        {
            Workbook workBook = new Workbook();
            workSheet = new Worksheet("Employees");
            columnWidths = new ushort[] { 2000, 6000, 4000, 3500, 8000, 3500, 8000, 3500, 7500, 3500, 16000 };
            caption = new string[]{"EID", "Name", "Role", "Dept", "Position", "Employed", "Full Name", "Birthday", "Comment", "Dismissed", "Education"};
            CreateCaption(caption); 
            WriteEmployeeDataADM(employees);
            SetColumnWidths(columnWidths); 
            workBook.Worksheets.Add(workSheet);
            MemoryStream stream = new MemoryStream();
            workBook.SaveToStream(stream);
            return stream.ToArray();
        }

        public  byte[] ExportEmployeesToExcelVU(IList<EmployeeViewModel> employees)
        {
            Workbook workBook = new Workbook();
            workSheet = new Worksheet("Employees");
            columnWidths = new ushort[] { 2000, 6000, 8000, 3500, 8000, 10000, 3500, 3500 };
            caption = new string[] {"EID", "Name", "Full Name", "Dept", "Position", "PositionUk", "Employed", "Dismissed"};
            CreateCaption(caption);
            WriteEmployeeDataVU(employees);
            SetColumnWidths(columnWidths); 
            workBook.Worksheets.Add(workSheet);
            MemoryStream stream = new MemoryStream();
            workBook.SaveToStream(stream);
            return stream.ToArray();
        }

        public  byte[] ExportBusinessTripsToExcelVU(IList<BusinessTripViewModel> businessTrips)
        {
            Workbook workBook = new Workbook();
            workSheet = new Worksheet("BusinessTrips");
            columnWidths = new ushort[] { 1067, 1700, 4867, 2067, 2867, 2867, 2434, 4867, 1466, 1631 };
            caption = new string[] { "ID", "EID", "Name", "Loc", "From", "To", "Unit", "Purpose", "Mgr", "Resp" };
            CreateCaption(caption); 
            WriteBusinessTripDataVU(businessTrips);
            SetColumnWidths(columnWidths);
            workBook.Worksheets.Add(workSheet);
            MemoryStream stream = new MemoryStream();
            workBook.SaveToStream(stream);
            return stream.ToArray();
        }

        public  byte[] ExportVisasAndPermitsVU(IList<Employee> employee)
        {
            Workbook workBook = new Workbook();
            workSheet = new Worksheet("VisasAndPermits");
            //TODO: check widths
            columnWidths = new ushort[] { 1700, 4969, 2867, 1968, 2867, 2867, 2034, 2100, 3070, 2668, 5635, 6268, 3134 };
            caption = new string[] { "EID", "Name", "Passport", "Type", "Visa From", "Visa To", "Entries", "Days", "Registration", "Num", "Permit From - To", "Last BT", "Status" };
            CreateCaption(caption);
            WriteVisasAndPermitsDataVU(employee);
            SetColumnWidths(columnWidths);
            workBook.Worksheets.Add(workSheet);
            MemoryStream stream = new MemoryStream();
            workBook.SaveToStream(stream);
            return stream.ToArray();
        }

        public  byte[] ExportWTR(DateTime fromDate, DateTime toDate, IList<WTRViewModel> wtrDataList)
        {
            Workbook workBook = new Workbook();
            workSheet = new Worksheet("WTR");
            //TODO: check widths
            columnWidths = new ushort[] { 6000, 3000, 6000, 6000, 3000 };
            string[] caption = new string[] { "Employee", "Location", "Factor", "Dates", "Hours" };
            CreateCaption(caption);
            WritingOfWTRData(fromDate, toDate, wtrDataList);
            SetColumnWidths(columnWidths);
            workBook.Worksheets.Add(workSheet);
            MemoryStream stream = new MemoryStream();
            workBook.SaveToStream(stream);
            return stream.ToArray();
        }


        public class Interval
        {
            public int weekFrom { get; set; }
            public int weekTo { get; set; }
            public int year { get; set; }
        }

        public  List<Interval> CreateWeekInterval(int YearFrom, int YearTo, int WeekFrom, int WeekTo)
        {
            List<Interval> result = new List<Interval>();

            for (int i = YearFrom; i <= YearTo; i++)
            {
                if (YearFrom == YearTo)
                {
                    result.Add(new Interval { weekFrom = WeekFrom, weekTo = WeekTo, year = YearFrom });
                    break;
                }

                if (i == YearFrom)
                {
                    result.Add(new Interval { weekFrom = WeekFrom, weekTo = 52, year = YearFrom });
                }

                if (i == YearTo)
                {
                    result.Add(new Interval { weekFrom = 1, weekTo = WeekTo, year = YearTo });
                }

                if (i != YearFrom && i != YearTo)
                {
                    result.Add(new Interval { weekFrom = 1, weekTo = 52, year = i });
                }
            }

            return result;

        }

        public  void WritingOfWTRData(DateTime fromDate, DateTime toDate, IList<WTRViewModel> wtrDataList)
        {
            CultureInfo _culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CultureInfo _uiculture = (CultureInfo)CultureInfo.CurrentUICulture.Clone();

            _culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
            _uiculture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;

            System.Threading.Thread.CurrentThread.CurrentCulture = _culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = _uiculture; 

            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            int FromWeek = cal.GetWeekOfYear(fromDate, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            int ToWeek = cal.GetWeekOfYear(toDate, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

            var weekInterval = CreateWeekInterval(fromDate.Year, toDate.Year, FromWeek, ToWeek);

            int count = 1;

            foreach (var weekInt in weekInterval)
            {
                for (int i = weekInt.weekFrom; i <= weekInt.weekTo; i++)
                {
                    var week = SelectEmployeeByWeek(wtrDataList, i, weekInt.year);
                    if (week.ToList().Count == 0)
                    {
                        workSheet.Cells[count, 0] = new Cell("");
                        workSheet.Cells[count + 1, 0] = new Cell(weekInt.year + "- W " + i);
                        workSheet.Cells[count + 2, 0] = new Cell("No absence data");
                        workSheet.Cells[count + 3, 0] = new Cell("");
                        count += 4;
                    }
                    else
                    {
                        workSheet.Cells[count, 0] = new Cell("");
                        workSheet.Cells[count + 1, 0] = new Cell(weekInt.year + "- W " + i);
                        workSheet.Cells[count + 2, 0] = new Cell("");
                        count += 3;
                    }

                    if (week.ToList().Count != 0)
                    {
                        foreach (var emp in week)
                        {

                            workSheet.Cells[count, 0] = new Cell(emp.LastName + ' ' + emp.FirstName + "(" + emp.ID + ")");


                            foreach (var factor in emp.FactorDetails)
                            {
                                workSheet.Cells[count, 1] = new Cell(factor.Location);
                                workSheet.Cells[count, 2] = new Cell(factor.Factor.ToString());
                                workSheet.Cells[count, 3] = new Cell(factor.From.ToString(String.Format("dd.MM.yyyy")) + " - " + factor.To.ToString(String.Format("dd.MM.yyyy")));
                                workSheet.Cells[count, 4] = new Cell(factor.Hours);
                                count++;
                            }
                        }
                    }
                }
            }
        }

        public  IEnumerable<WTRViewModel> SelectEmployeeByWeek(IEnumerable<WTRViewModel> wtrList, int weekNum, int year)
        {
            List<WTRViewModel> result = new List<WTRViewModel>();
            foreach (WTRViewModel wtrPerson in wtrList.Where(w => w.FactorDetails.Count > 0))
            {
                WTRViewModel onePerson = new WTRViewModel(wtrPerson, weekNum, year);
                if (onePerson.FactorDetails.Count > 0)
                {
                    result.Add(onePerson);
                }
            }
            return result;
        }

        public  void WriteVisasAndPermitsDataVU(IList<Employee> employees)
        {
            int row = 1;
            foreach (Employee employee in employees)
            {

                WriteRow(employee.PrepareToXLSExportVisasVU(), row);
               row++;  
            } 
        }

        public  void WriteEmployeeDataADM(IList<EmployeeViewModel> employees)
        {
            int row = 1;
            foreach (EmployeeViewModel employeeViewModel in employees)
            {

                WriteRow(employeeViewModel.PrepareToXLSExportADM(), row);
               row++;  
            }
        }

        public  void WriteEmployeeDataVU(IList<EmployeeViewModel> employees)
        {
            int row = 1;
            foreach (EmployeeViewModel employeeViewModel in employees)
            {

                WriteRow(employeeViewModel.PrepareToXLSExportVU(), row);
                row++;
            }
        }

        public  void WriteBusinessTripDataVU(IList<BusinessTripViewModel> businessTrips)
        { 
            int row = 1;
            foreach (BusinessTripViewModel businessTripViewModel in businessTrips)
            {
                List<string> values = businessTripViewModel.PrepareToXLSExportVU(); 
                WriteRow(values, row); 
                if(values[4].Contains(" To be updated soon") && columnWidths[4]!=7166)
                {
                    columnWidths[4] = 7166; 
                }
                row++;
            } 
        }

        public  void CreateCaption(string[] caption)
        {
            for (int i = 0; i < caption.Length; i++)
            {
                workSheet.Cells[0, i] = new Cell(caption[i]);
            }
        }

        public  void SetColumnWidths(ushort[] columnWidths)
        {
            for (ushort i = 0; i < columnWidths.Length; i++)
            {
                workSheet.Cells.ColumnWidth[i] = columnWidths[i];
            }
        }


        public  void WriteAbsenceData(IList<AbsenceViewModel> absences)
        {
            int row = 1; 
            foreach (AbsenceViewModel absenceViewModel in absences)
            {

               WriteRow(absenceViewModel.PrepareToXLSExportABM(), row);
               row++;  
            }
        }

        public  void WriteRow(List<string> valuesOfProperties, int row) 
        {
            int index=0;
                foreach (string propertyValue in valuesOfProperties)
                {
                    workSheet.Cells[row, index] = new Cell(propertyValue);
                    index++; 
                }
        }
    }
}
