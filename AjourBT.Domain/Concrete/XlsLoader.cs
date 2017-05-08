using AjourBT.Domain.Entities;
using ExcelLibrary.SpreadSheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AjourBT.Domain.Infrastructure;
using System.Globalization;

namespace AjourBT.Domain.Concrete
{
    public enum RolesEnum { ACC, ADM, BTM, DIR, EMP, PU, VU };

    public class XlsLoader
    {
        private HttpPostedFileBase file;
        private string fileName;
        private Workbook workBook;
        public static List<Department> departments = new List<Department>();
        public static List<Position> positions = new List<Position>();
        public static List<Employee> employees = new List<Employee>();
        public static List<Visa> visas = new List<Visa>();
        public static List<Permit> permits = new List<Permit>();
        public static List<Location> locations = new List<Location>();
        public static List<BusinessTrip> businessTrips = new List<BusinessTrip>();
        public static Dictionary<string, string[]> rolesDict = new Dictionary<string, string[]>();
        public static Dictionary<string, string> employeeIDs = new Dictionary<string, string>();

        public XlsLoader(HttpPostedFileBase file)
        {
            this.file = file;
        }

        public XlsLoader(string fileName)
        {
            this.fileName = fileName;
        }

        public bool LoadPUWorkBook()
        {
            if (file.ContentLength > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(file.FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    workBook = Workbook.Load(file.InputStream);
                    LoadDepartments();
                    LoadPositions();
                    LoadEmployees();
                    return true;
                }
                return false;
            }
            else
                return false;
        }

        public bool LoadPUWorkBookFromFile()
        {
            //try
            //{
                workBook = Workbook.Load(fileName);
                LoadDepartments();
                LoadPositions();
                LoadEmployees();
                return true;
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
        }

        public bool LoadBTMWorkBook()
        {
            if (file.ContentLength > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(file.FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    workBook = Workbook.Load(file.InputStream);
                    LoadLocations();
                    LoadBusinessTrips();
                    LoadVisas();
                    LoadPermits();
                    return true;
                }
                return false;
            }
            else
                return false;
        }
        public bool LoadBTMWorkBookFromFile()
        {
            //try
            //{
                workBook = Workbook.Load(fileName);
                LoadLocations();
                LoadBusinessTrips();
                LoadVisas();
                LoadPermits();
                return true;
            //}
            //catch (NullReferenceException)
            //{
            //    return false;
            //}
        }
        public void LoadPositions()
        {

            Worksheet sheet = workBook.Worksheets[0];
            int rowIndex = 1;
            int colIndex = 10;
            positions.Clear();

            while (sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue != "")
            {
                Position pos = new Position();
                pos.TitleEn = sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue.Trim();
                pos.TitleUk = sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue.Trim();

                if (((from p in positions where p.TitleUk == pos.TitleUk select p).FirstOrDefault() == null)
                    && ((from p in positions where p.TitleEn == pos.TitleEn select p).FirstOrDefault() == null))
                {
                    positions.Add(pos);
                }
                rowIndex++;
            }
            //file.InputStream.Close();

        }

        public void LoadDepartments()
        {

            Worksheet sheet = workBook.Worksheets[0];
            int rowIndex = 1;
            int colIndex = 2;
            departments.Clear();

            while (sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue != "")
            {
                Department dep = new Department();
                dep.DepartmentName = sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue.Trim();

                if ((from d in departments where d.DepartmentName == dep.DepartmentName select d).FirstOrDefault() == null)
                {
                    departments.Add(dep);
                }
                rowIndex++;
            }
            //file.InputStream.Close();

        }

        public void LoadEmployees()
        {
            Worksheet sheet = workBook.Worksheets[0];
            int rowIndex = 1;
            int colIndex = 3;
            employees.Clear();
            List<string> Roles = new List<string>();
            rolesDict.Clear();
            while (sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue != "")
            {
                Employee emp = new Employee();
                emp.EID = sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue.Trim();
                emp.FirstName = new CultureInfo("en-US").TextInfo.ToTitleCase(sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 2).StringValue.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1].Trim().ToLower());
                emp.LastName = new CultureInfo("en-US").TextInfo.ToTitleCase(sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 2).StringValue.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0].Trim().ToLower());
                emp.FullNameUk = new CultureInfo("en-US").TextInfo.ToTitleCase(sheet.Cells.GetRow(rowIndex).GetCell(colIndex - 2).StringValue.Trim());
                string dateOfEmployment = sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 6).StringValue.Trim(new char[] { '\n' });
                if (dateOfEmployment != "")
                    emp.DateEmployed = DateTime.FromOADate(Int32.Parse(dateOfEmployment));
                string dateOfBirth = sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 3).StringValue;
                if (dateOfBirth != "")
                    emp.BirthDay = DateTime.FromOADate(Int32.Parse(dateOfBirth));
                string dateOfDismissal = sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 9).StringValue;
                if (dateOfDismissal != "")
                    emp.DateDismissed = DateTime.FromOADate(Int32.Parse(dateOfDismissal));
                emp.Department = new Department { DepartmentName = (from d in departments where d.DepartmentName == sheet.Cells.GetRow(rowIndex).GetCell(colIndex - 1).StringValue.Trim() select d.DepartmentName).FirstOrDefault() };
                emp.Position = new Position { TitleUk = (from p in positions where p.TitleUk == sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 7).StringValue.Trim() select p.TitleUk).FirstOrDefault() };
                emp.IsManager = sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 17).StringValue.Trim() != "" ? true : false;
                employees.Add(emp);
                for (int i = 10; i <= 16; i++)
                {

                    string role = sheet.Cells.GetRow(rowIndex).GetCell(colIndex + i).StringValue.Trim();
                    if (role != "")
                    {
                        RolesEnum r = (RolesEnum)(i - 10);
                        Roles.Add(r.ToString());
                    }
                }
                rolesDict.Add(emp.EID, Roles.ToArray<string>());
                Roles.Clear();



                rowIndex++;
            }
        }

        public void LoadLocations()
        {

            Worksheet sheet = workBook.Worksheets[0];
            int rowIndex = 19;
            int colIndex = 7;
            locations.Clear();

            while (sheet.Cells.GetRow(rowIndex).GetCell(1).StringValue != "")
            {
                Location loc = new Location();
                loc.Title = sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue.Trim();

                if (loc.Title != String.Empty && (from l in locations where l.Title == loc.Title select l).FirstOrDefault() == null)
                {
                    loc.Address = "Address need to be added";
                    locations.Add(loc);
                }
                rowIndex++;
            }
            //file.InputStream.Close();

        }

        public void LoadBusinessTrips()
        {

            Worksheet sheet = workBook.Worksheets[0];
            int rowIndex = 19;
            int colIndex = 4;
            businessTrips.Clear();


            while (sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue != "")
            {
                BusinessTrip bTrip = new BusinessTrip();
                bTrip.BTof = new Employee { EID = sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue.Trim() };
                bTrip.Location = new Location { Title = sheet.Cells.GetRow(rowIndex).GetCell(7).StringValue.Trim() };
                bTrip.Manager = sheet.Cells.GetRow(rowIndex).GetCell(6).StringValue.Trim().ToLower();
                bTrip.Responsible = sheet.Cells.GetRow(rowIndex).GetCell(9).StringValue.Trim().ToLower();
                bTrip.LastCRUDedBy = bTrip.Manager;
                bTrip.Comment = sheet.Cells.GetRow(rowIndex).GetCell(10).StringValue.Trim();
                bTrip.StartDate = DateTime.FromOADate(Int32.Parse(sheet.Cells.GetRow(rowIndex).GetCell(23).StringValue));
                bTrip.EndDate = DateTime.FromOADate(Int32.Parse(sheet.Cells.GetRow(rowIndex).GetCell(24).StringValue));
                if (bTrip.StartDate.Date <= DateTime.Now.ToLocalTimeAzure().Date)
                    bTrip.Status = BTStatus.Confirmed | BTStatus.Reported;
                else
                    bTrip.Status = BTStatus.Confirmed;
                businessTrips.Add(bTrip);
                rowIndex++;
            }
            //file.InputStream.Close();

        }

        public void LoadVisas()
        {
            Worksheet sheet = workBook.Worksheets[1];
            int rowIndex = 19;
            int colIndex = 1;
            visas.Clear();
            while (sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue != "")
            {
                if (sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 25).StringValue.Trim() != "" && sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 26).StringValue != "" && sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 27).StringValue != "")
                {
                    Visa visa = new Visa();
                    visa.VisaType = sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 25).StringValue.Trim();
                    visa.StartDate = DateTime.FromOADate(Int32.Parse(sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 26).StringValue));
                    visa.DueDate = DateTime.FromOADate(Int32.Parse(sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 27).StringValue));
                    if (sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 28).StringValue.Trim() != "")
                        visa.Days = Int32.Parse(sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 28).StringValue.Trim());
                    visa.Entries = 0;
                    visa.VisaOf = new Employee { EID = sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 1).StringValue.Trim(), LastName = sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 2).StringValue.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0].Trim(), FirstName = sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 2).StringValue.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1].Trim() };
                    visas.Add(visa);
                }
                rowIndex++;

            }

        }

        public void LoadPermits()
        {
            Worksheet sheet = workBook.Worksheets[1];
            int rowIndex = 19;
            int colIndex = 2;
            permits.Clear();

            while (sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue != "")
            {

                if (sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 28).StringValue.Trim() != "" && sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 29).StringValue != "" && sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 30).StringValue != "")
                {
                    Permit permit = new Permit();
                    permit.Number = sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 28).StringValue.Trim();
                    permit.StartDate = DateTime.FromOADate(Int32.Parse(sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 29).StringValue));
                    permit.EndDate = DateTime.FromOADate(Int32.Parse(sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 30).StringValue));
                    permit.PermitOf = new Employee { EID = sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue.Trim(), LastName = sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 1).StringValue.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0].Trim(), FirstName = sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 1).StringValue.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1].Trim() };
                    permits.Add(permit);
                }
                rowIndex++;
            }
        }

        public bool LoadEIDWorkBook()
        {
            if (file.ContentLength > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(file.FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    workBook = Workbook.Load(file.InputStream);
                    Worksheet sheet = workBook.Worksheets[0];
                    int rowIndex = 0;
                    int colIndex = 0;
                    employeeIDs.Clear();

                    while (sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue != "")
                    {
                        if (sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue.Trim() != HttpContext.Current.User.Identity.Name)
                            employeeIDs.Add(
                            sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue.Trim(),
                            sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 1).StringValue.Trim());
                        rowIndex++;
                    }
                    return true;
                }
                return false;
            }
            else
                return false;
        }

        public bool LoadEIDWorkBookFromFile()
        {
            //try
            //{
                workBook = Workbook.Load(fileName);
                Worksheet sheet = workBook.Worksheets[0];
                int rowIndex = 0;
                int colIndex = 0;
                employeeIDs.Clear();

                while (sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue != "")
                {
                        employeeIDs.Add(
                        sheet.Cells.GetRow(rowIndex).GetCell(colIndex).StringValue.Trim(),
                        sheet.Cells.GetRow(rowIndex).GetCell(colIndex + 1).StringValue.Trim());
                    rowIndex++;
                }
                return true;
            //}
            //catch (Exception)
            //{

            //    return false;
            //}
        }
    }
}



