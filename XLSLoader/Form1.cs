using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AjourBT.Domain.Concrete;
using System.IO;
using AjourBT.Domain.Entities;
using System.Web.Security;
using WebMatrix.WebData;
using System.Web.Configuration;

using AjourBT.Domain.Infrastructure;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml;
using System.Threading;

namespace XLSLoader
{
    public partial class Form1 : Form
    {
        AjourDbRepository repository;
        string connectionString;
        string defaultPassword;
        List<CalendarItem> calendarItems = new List<CalendarItem>();
        List<Journey> journeysToAdd = new List<Journey>();

        [Flags]
        enum UploadStatus
        {
            NoFilesUploaded,
            FirstFileUploaded,
            SecondFileUploaded
        }
        UploadStatus uploadStatus;

        public Form1()
        {
            InitializeComponent();
            System.Web.Configuration.WebConfigurationManager.AppSettings["DBInitType"] = "InitDbClear";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Excel 97-2003 Files|*.xls";
            openFileDialog1.Title = "Select an employees and departments file";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                LoadPUData(textBox1.Text);
            }

        }

        private void LoadPUData(string fileName)
        {
            if (textBox1.Text != "")
            {
                XlsLoader xlsLoader = new XlsLoader(fileName);
                xlsLoader.LoadPUWorkBookFromFile();

                List<DepartmentForLoader> departments = new List<DepartmentForLoader>();
                foreach (Department dep in XlsLoader.departments)
                {
                    departments.Add(new DepartmentForLoader { DepartmentName = dep.DepartmentName });
                }
                var bindingList = new BindingList<DepartmentForLoader>(departments);
                var source = new BindingSource(bindingList, null);
                dataGridView1.DataSource = source;

                List<EmployeeForLoader> employees = new List<EmployeeForLoader>();
                foreach (Employee emp in XlsLoader.employees)
                {
                    employees.Add(new EmployeeForLoader
                    {
                        BirthDay = emp.BirthDay,
                        DepartmentName = emp.Department.DepartmentName,
                        DateDismissed = emp.DateDismissed,
                        DateEmployed = emp.DateEmployed,
                        EID = emp.EID,
                        FirstName = emp.FirstName,
                        LastName = emp.LastName,
                        FullNameUk = emp.FullNameUk,
                        IsManager = emp.IsManager,
                        roles = String.Join(", ", XlsLoader.rolesDict[emp.EID])
                    });
                }
                var bindingList2 = new BindingList<EmployeeForLoader>(employees);
                var source2 = new BindingSource(bindingList2, null);
                dataGridView2.DataSource = source2;

                List<PositionForLoader> positions = new List<PositionForLoader>();
                foreach (Position pos in XlsLoader.positions)
                {
                    positions.Add(new PositionForLoader
                    {
                        TitleUk = pos.TitleUk
                    });
                }
                var bindingList3 = new BindingList<PositionForLoader>(positions);
                var source3 = new BindingSource(bindingList3, null);
                dataGridView3.DataSource = source3;

                uploadStatus = uploadStatus | UploadStatus.FirstFileUploaded;
                if (uploadStatus == (UploadStatus.FirstFileUploaded | UploadStatus.SecondFileUploaded))
                    button7.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                XlsLoader xlsLoader = new XlsLoader(textBox1.Text);
                xlsLoader.LoadPUWorkBookFromFile();

                List<DepartmentForLoader> departments = new List<DepartmentForLoader>();
                foreach (Department dep in XlsLoader.departments)
                {
                    departments.Add(new DepartmentForLoader { DepartmentName = dep.DepartmentName });
                }
                var bindingList = new BindingList<DepartmentForLoader>(departments);
                var source = new BindingSource(bindingList, null);
                dataGridView1.DataSource = source;

                List<EmployeeForLoader> employees = new List<EmployeeForLoader>();
                foreach (Employee emp in XlsLoader.employees)
                {
                    employees.Add(new EmployeeForLoader
                    {
                        BirthDay = emp.BirthDay,
                        DepartmentName = emp.Department.DepartmentName,
                        DateDismissed = emp.DateDismissed,
                        DateEmployed = emp.DateEmployed,
                        EID = emp.EID,
                        FirstName = emp.FirstName,
                        LastName = emp.LastName,
                        FullNameUk = emp.FullNameUk,
                        IsManager = emp.IsManager,
                        roles = String.Join(", ", XlsLoader.rolesDict[emp.EID])
                    });
                }
                var bindingList2 = new BindingList<EmployeeForLoader>(employees);
                var source2 = new BindingSource(bindingList2, null);
                dataGridView2.DataSource = source2;

                List<PositionForLoader> positions = new List<PositionForLoader>();
                foreach (Position pos in XlsLoader.positions)
                {
                    positions.Add(new PositionForLoader
                    {
                        TitleUk = pos.TitleUk
                    });
                }
                var bindingList3 = new BindingList<PositionForLoader>(positions);
                var source3 = new BindingSource(bindingList3, null);
                dataGridView3.DataSource = source3;

                uploadStatus = uploadStatus | UploadStatus.FirstFileUploaded;
                if (uploadStatus == (UploadStatus.FirstFileUploaded | UploadStatus.SecondFileUploaded))
                    button7.Enabled = true;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            label6.Text = "Updating configuration";
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            this.Refresh();
            ConfigurationManager.RefreshSection("connectionStrings");
            Thread.Sleep(1000);
            AjourDbContext context = new AjourDbContext(connectionString);
            label6.Text = "Connecting to database";
            progressBar1.Value = 10;
            this.Refresh();
            context.Database.Initialize(true);
            WebSecurity.InitializeDatabaseConnection("AjourBTConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            if (!System.Web.Security.Roles.RoleExists("EMP"))
            {
                System.Web.Security.Roles.CreateRole("EMP");
            }

            if (!System.Web.Security.Roles.RoleExists("PU"))
            {
                System.Web.Security.Roles.CreateRole("PU");
            }

            if (!System.Web.Security.Roles.RoleExists("BTM"))
            {
                System.Web.Security.Roles.CreateRole("BTM");
            }

            if (!System.Web.Security.Roles.RoleExists("ADM"))
            {
                System.Web.Security.Roles.CreateRole("ADM");
            }

            if (!System.Web.Security.Roles.RoleExists("ACC"))
            {
                System.Web.Security.Roles.CreateRole("ACC");
            }

            if (!System.Web.Security.Roles.RoleExists("DIR"))
            {
                System.Web.Security.Roles.CreateRole("DIR");
            }

            if (!System.Web.Security.Roles.RoleExists("VU"))
            {
                System.Web.Security.Roles.CreateRole("VU");
            }

            Dictionary<string, string> employeesPasswords = new Dictionary<string, string>();
            label6.Text = "Loading departments";
            progressBar1.Value = 20;
            this.Refresh();
            foreach (var dep in XlsLoader.departments)
            {
                repository.SaveDepartment(dep);
            }
            label6.Text = "Loading positions";
            progressBar1.Value = 30;
            this.Refresh();
            foreach (var pos in XlsLoader.positions)
            {
                repository.SavePosition(pos);
            }
            label6.Text = "Loading employees";
            progressBar1.Value = 40;
            this.Refresh();
            foreach (var emp in XlsLoader.employees)
            {
                emp.DepartmentID = repository.Departments.Where(d => d.DepartmentName == emp.Department.DepartmentName).Select(d => d.DepartmentID).FirstOrDefault();
                emp.Department = null;
                emp.PositionID = repository.Positions.Where(p => p.TitleUk == emp.Position.TitleUk).Select(p => p.PositionID).FirstOrDefault();
                emp.Position = null;
                repository.SaveEmployee(emp);
                if (!WebSecurity.UserExists(emp.EID))
                {
                    //if (generate)
                    //{
                    //    currentEmployeePassword = Membership.GeneratePassword(8, 1);
                    //    employeesPasswords.Add(emp.EID, currentEmployeePassword);
                    //    WebSecurity.CreateUserAndAccount(emp.EID, currentEmployeePassword);
                    //}
                    //else
                    WebSecurity.CreateUserAndAccount(emp.EID, WebConfigurationManager.AppSettings["DefaultPassword"].ToString());
                }
                foreach (var role in XlsLoader.rolesDict[emp.EID])
                {
                    if (!System.Web.Security.Roles.IsUserInRole(emp.EID.ToString(), role))
                    {
                        System.Web.Security.Roles.AddUserToRole(emp.EID, role);
                    }
                }

            }
            label6.Text = "Loading locations";
            progressBar1.Value = 50;
            this.Refresh();
            foreach (var loc in XlsLoader.locations)
            {
                repository.SaveLocation(loc);
            }
            label6.Text = "Loading visas";
            progressBar1.Value = 60;
            this.Refresh();
            foreach (var visa in XlsLoader.visas)
            {
                int EmpID = repository.Employees.Where(emp => emp.EID == visa.VisaOf.EID).Select(emp => emp.EmployeeID).FirstOrDefault();
                visa.EmployeeID = EmpID;
                visa.VisaOf = null;
                repository.SaveVisa(visa, EmpID);
            }
            label6.Text = "Loading permits";
            progressBar1.Value = 70;
            this.Refresh();
            foreach (var permit in XlsLoader.permits)
            {
                int EmpID = repository.Employees.Where(emp => emp.EID == permit.PermitOf.EID).Select(emp => emp.EmployeeID).FirstOrDefault();
                permit.EmployeeID = EmpID;
                permit.PermitOf = null;
                repository.SavePermit(permit, EmpID);
            }
            label6.Text = "Loading businessTrips";
            progressBar1.Value = 80;
            this.Refresh();
            foreach (var bTrip in XlsLoader.businessTrips.Where(b => repository.Employees.Where(
                emp => (emp.EID == b.BTof.EID))
                .FirstOrDefault() != null)
                )
            {
                bTrip.EmployeeID = repository.Employees.Where(emp => emp.EID == bTrip.BTof.EID).Select(emp => emp.EmployeeID).FirstOrDefault();
                bTrip.BTof = null;
                bTrip.LocationID = repository.Locations.Where(l => l.Title == bTrip.Location.Title).Select(l => l.LocationID).FirstOrDefault();
                bTrip.Location = null;
                bTrip.LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure();
                repository.SaveBusinessTrip(bTrip);
            }
            string[] roles = new string[] { "" };
            //bool x = Membership.Provider.EnablePasswordRetrieval;
            AjourDbContext dbContext = new AjourDbContext(connectionString);
            foreach (KeyValuePair<string, string> empNewOldEID in XlsLoader.employeeIDs)
            {
                Employee empToChangeEID = repository.Employees.Where(emp => emp.EID == empNewOldEID.Key).FirstOrDefault();
                empToChangeEID.EID = empNewOldEID.Value;
                repository.SaveEmployee(empToChangeEID);
                if (empNewOldEID.Key != empNewOldEID.Value)
                {
                    if (dbContext.UserProfiles.Where(u => u.UserName == empNewOldEID.Key).FirstOrDefault() != null)
                    {
                        dbContext.UserProfiles.Where(u => u.UserName == empNewOldEID.Key).FirstOrDefault().UserName = empNewOldEID.Value;
                    }
                    //if (Membership.GetUser(empNewOldEID.Key) != null)
                    //{
                    //    password = Membership.Provider.GetUser(empNewOldEID.Key, false).GetPassword();
                    //    if (Roles.GetRolesForUser(empNewOldEID.Key).Count() > 0)
                    //    {
                    //        roles = Roles.GetRolesForUser(empNewOldEID.Key);
                    //        Roles.RemoveUserFromRoles(empNewOldEID.Key, Roles.GetRolesForUser(empNewOldEID.Key));
                    //    }
                    //    ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(empNewOldEID.Key); // deletes record from webpages_Membership table
                    //    ((SimpleMembershipProvider)Membership.Provider).DeleteUser(empNewOldEID.Key, true); // deletes record from UserProfile table
                    //    Membership.CreateUser(empNewOldEID.Value, password);
                    //    Roles.AddUserToRoles(empNewOldEID.Value, roles);

                    //}
                }


            }
            dbContext.SaveChanges();
            XlsLoader.businessTrips.Clear();
            XlsLoader.departments.Clear();
            XlsLoader.employeeIDs.Clear();
            XlsLoader.employees.Clear();
            XlsLoader.locations.Clear();
            XlsLoader.rolesDict.Clear();
            XlsLoader.permits.Clear();
            XlsLoader.visas.Clear();
            XlsLoader.positions.Clear();

            //string messagesEIDPasswords = GenerateMessagesMemoryStream(employeesPasswords);
            //if (messagesEIDPasswords.Length != 0)
            //{
            //    return File(System.Text.Encoding.Unicode.GetBytes(messagesEIDPasswords), "text/plain", "Employees.txt");
            //}
            label6.Text = "Generating text file";
            progressBar1.Value = 90;
            this.Refresh();
            StringBuilder sb = new StringBuilder();
            foreach (Employee emp in context.Employees)
            {
                sb.Append("We notify You, that your login and password for AjourBT system were created");
                sb.Append(System.Environment.NewLine);
                sb.Append(String.Format("Login: {0}, password: {1}", emp.EID, WebConfigurationManager.AppSettings["DefaultPassword"].ToString()));
                sb.Append(System.Environment.NewLine);
                sb.Append(System.Environment.NewLine);
            }
            label6.Text = "All tasks are completed";
            progressBar1.Value = 100;
            this.Refresh();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("Employees.txt"))
            {

                file.WriteLine(sb);
            }
            MessageBox.Show("Save completedEmployees logins and initial passwords were saved to Employees.txt");

            uploadStatus = UploadStatus.NoFilesUploaded;
            //context.Database.Connection.Close();
            //context.Dispose();
            //dbContext.Database.Connection.Close();
            //dbContext.Dispose();
            //SqlConnection.ClearAllPools();
            button7.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Filter = "Excel 97-2003 Files|*.xls";
            openFileDialog2.Title = "Select an employees and departments file";

            if (openFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = openFileDialog2.FileName;
                LoadBTMData(textBox2.Text);
            }

        }

        private void LoadBTMData(string fileName)
        {
            if (textBox2.Text != "")
            {
                XlsLoader xlsLoader = new XlsLoader(fileName);
                xlsLoader.LoadBTMWorkBookFromFile();

                List<LocationForLoader> locations = new List<LocationForLoader>();
                foreach (Location loc in XlsLoader.locations)
                {
                    locations.Add(new LocationForLoader { Title = loc.Title });
                }
                var bindingList4 = new BindingList<LocationForLoader>(locations);
                var source4 = new BindingSource(bindingList4, null);
                dataGridView4.DataSource = source4;

                List<VisaForLoader> visas = new List<VisaForLoader>();
                foreach (Visa visa in XlsLoader.visas)
                {
                    visas.Add(new VisaForLoader
                    {
                        Days = visa.Days,
                        DueDate = visa.DueDate,
                        EID = visa.VisaOf.EID,
                        Entries = visa.Entries,
                        StartDate = visa.StartDate,
                        VisaType = visa.VisaType
                    });
                }
                var bindingList5 = new BindingList<VisaForLoader>(visas);
                var source5 = new BindingSource(bindingList5, null);
                dataGridView5.DataSource = source5;

                List<PermitForLoader> permits = new List<PermitForLoader>();
                foreach (Permit permit in XlsLoader.permits)
                {
                    permits.Add(new PermitForLoader
                    {
                        EID = permit.PermitOf.EID,
                        EndDate = permit.EndDate,
                        Number = permit.Number,
                        StartDate = permit.StartDate
                    });
                }
                var bindingList6 = new BindingList<PermitForLoader>(permits);
                var source6 = new BindingSource(bindingList6, null);
                dataGridView6.DataSource = source6;

                List<BusinessTripForLoader> businessTrips = new List<BusinessTripForLoader>();
                foreach (BusinessTrip businessTrip in XlsLoader.businessTrips)
                {
                    businessTrips.Add(new BusinessTripForLoader
                    {
                        Comment = businessTrip.Comment,
                        EID = businessTrip.BTof.EID,
                        EndDate = businessTrip.EndDate,
                        LastCRUDedBy = businessTrip.LastCRUDedBy,
                        LocationTitle = businessTrip.Location.Title,
                        Manager = businessTrip.Manager,
                        Responsible = businessTrip.Responsible,
                        StartDate = businessTrip.StartDate,
                        status = businessTrip.Status
                    });
                }
                var bindingList7 = new BindingList<BusinessTripForLoader>(businessTrips);
                var source7 = new BindingSource(bindingList7, null);
                dataGridView7.DataSource = source7;

                uploadStatus = uploadStatus | UploadStatus.SecondFileUploaded;
                if (uploadStatus == (UploadStatus.FirstFileUploaded | UploadStatus.SecondFileUploaded))
                    button7.Enabled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog3 = new OpenFileDialog();
            openFileDialog3.Filter = "Excel 97-2003 Files|*.xls";
            openFileDialog3.Title = "Select an employees and departments file";

            if (openFileDialog3.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox3.Text = openFileDialog3.FileName;
                LoadEIDData(textBox3.Text);
            }
        }

        private void LoadEIDData(string FileName)
        {
            XlsLoader xlsLoader = new XlsLoader(textBox3.Text);
            xlsLoader.LoadEIDWorkBookFromFile();

            List<EIDsForLoader> eids = new List<EIDsForLoader>();
            foreach (KeyValuePair<string, string> empNewOldEID in XlsLoader.employeeIDs)
            {
                eids.Add(new EIDsForLoader
                {
                    oldEID = empNewOldEID.Key,
                    newEID = empNewOldEID.Value
                });
            }
            var bindingList8 = new BindingList<EIDsForLoader>(eids);
            var source8 = new BindingSource(bindingList8, null);
            dataGridView8.DataSource = source8;
            button9.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                XlsLoader xlsLoader = new XlsLoader(textBox2.Text);
                xlsLoader.LoadBTMWorkBookFromFile();

                List<LocationForLoader> locations = new List<LocationForLoader>();
                foreach (Location loc in XlsLoader.locations)
                {
                    locations.Add(new LocationForLoader { Title = loc.Title });
                }
                var bindingList4 = new BindingList<LocationForLoader>(locations);
                var source4 = new BindingSource(bindingList4, null);
                dataGridView4.DataSource = source4;

                List<VisaForLoader> visas = new List<VisaForLoader>();
                foreach (Visa visa in XlsLoader.visas)
                {
                    visas.Add(new VisaForLoader
                    {
                        Days = visa.Days,
                        DueDate = visa.DueDate,
                        EID = visa.VisaOf.EID,
                        Entries = visa.Entries,
                        StartDate = visa.StartDate,
                        VisaType = visa.VisaType
                    });
                }
                var bindingList5 = new BindingList<VisaForLoader>(visas);
                var source5 = new BindingSource(bindingList5, null);
                dataGridView5.DataSource = source5;

                List<PermitForLoader> permits = new List<PermitForLoader>();
                foreach (Permit permit in XlsLoader.permits)
                {
                    permits.Add(new PermitForLoader
                    {
                        EID = permit.PermitOf.EID,
                        EndDate = permit.EndDate,
                        Number = permit.Number,
                        StartDate = permit.StartDate
                    });
                }
                var bindingList6 = new BindingList<PermitForLoader>(permits);
                var source6 = new BindingSource(bindingList6, null);
                dataGridView6.DataSource = source6;

                List<BusinessTripForLoader> businessTrips = new List<BusinessTripForLoader>();
                foreach (BusinessTrip businessTrip in XlsLoader.businessTrips)
                {
                    businessTrips.Add(new BusinessTripForLoader
                    {
                        Comment = businessTrip.Comment,
                        EID = businessTrip.BTof.EID,
                        EndDate = businessTrip.EndDate,
                        LastCRUDedBy = businessTrip.LastCRUDedBy,
                        LocationTitle = businessTrip.Location.Title,
                        Manager = businessTrip.Manager,
                        Responsible = businessTrip.Responsible,
                        StartDate = businessTrip.StartDate,
                        status = businessTrip.Status
                    });
                }
                var bindingList7 = new BindingList<BusinessTripForLoader>(businessTrips);
                var source7 = new BindingSource(bindingList7, null);
                dataGridView7.DataSource = source7;

                uploadStatus = uploadStatus | UploadStatus.SecondFileUploaded;
                if (uploadStatus == (UploadStatus.FirstFileUploaded | UploadStatus.SecondFileUploaded))
                    button7.Enabled = true;
            }
        }

        private void tabPage8_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            System.Web.Configuration.WebConfigurationManager.AppSettings["DBInitType"] = "InitDbNotChanged";
            AjourDbContext context = new AjourDbContext(connectionString);
            foreach (KeyValuePair<string, string> empNewOldEID in XlsLoader.employeeIDs)
            {
                Employee empToChangeEID = context.Employees.Where(emp => emp.EID == empNewOldEID.Key).FirstOrDefault();
                empToChangeEID.EID = empNewOldEID.Value;
                context.SaveChanges();
                if (empNewOldEID.Key != empNewOldEID.Value)
                {
                    if (context.UserProfiles.Where(
                        u => u.UserName == empNewOldEID.Key).FirstOrDefault() != null
                        )
                    {
                        context.UserProfiles.Where(u => u.UserName == empNewOldEID.Key).FirstOrDefault().UserName = empNewOldEID.Value;
                    }
                }
            }
            context.SaveChanges();
            MessageBox.Show("All EIDs are replaced");
        }

        private void tabPage9_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            XlsLoader xlsLoader = new XlsLoader(textBox3.Text);
            xlsLoader.LoadEIDWorkBookFromFile();

            List<EIDsForLoader> eids = new List<EIDsForLoader>();
            foreach (KeyValuePair<string, string> empNewOldEID in XlsLoader.employeeIDs)
            {
                eids.Add(new EIDsForLoader
                {
                    oldEID = empNewOldEID.Key,
                    newEID = empNewOldEID.Value
                });
            }
            var bindingList8 = new BindingList<EIDsForLoader>(eids);
            var source8 = new BindingSource(bindingList8, null);
            dataGridView8.DataSource = source8;
            button9.Enabled = true;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog4 = new OpenFileDialog();
            openFileDialog4.Filter = "Web.config Files|*.config";
            openFileDialog4.Title = "Select web.config file";

            if (openFileDialog4.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox4.Text = openFileDialog4.FileName;
                LoadWebConfig(textBox4.Text);
            }
        }

        private void LoadWebConfig(string fileName)
        {
            using (XmlTextReader xmlReader = new XmlTextReader(fileName))
            {
                xmlReader.ReadToFollowing("connectionStrings");
                xmlReader.ReadToFollowing("add");
                connectionString = xmlReader.GetAttribute("connectionString");
                xmlReader.ReadToFollowing("connectionStrings");
                xmlReader.ReadToFollowing("add");
            }

            defaultPassword = GetSomeSetting("DefaultPassword", fileName);
            repository = new AjourDbRepository(connectionString);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("XLSLoader.exe.config");
            XmlNodeList userNodes = xmlDoc.SelectNodes("//connectionStrings/add/@connectionString");
            foreach (XmlNode node in userNodes)
            {
                node.Value = connectionString;
            }

            xmlDoc.Save("XLSLoader.exe.config");

            ConfigurationManager.RefreshSection("connectionStrings");

            label5.Text = connectionString;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            using (XmlTextReader xmlReader = new XmlTextReader(textBox4.Text))
            {
                xmlReader.ReadToFollowing("connectionStrings");
                xmlReader.ReadToFollowing("add");
                connectionString = xmlReader.GetAttribute("connectionString");
                xmlReader.ReadToFollowing("connectionStrings");
                xmlReader.ReadToFollowing("add");
            }

            defaultPassword = GetSomeSetting("DefaultPassword", textBox4.Text);
            repository = new AjourDbRepository(connectionString);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("XLSLoader.exe.config");
            XmlNodeList userNodes = xmlDoc.SelectNodes("//connectionStrings/add/@connectionString");
            foreach (XmlNode node in userNodes)
            {
                node.Value = connectionString;
            }

            xmlDoc.Save("XLSLoader.exe.config");

            ConfigurationManager.RefreshSection("connectionStrings");

            label5.Text = connectionString;

        }

        public static string GetSomeSetting(string settingName, string fileName)
        {
            var valueToGet = string.Empty;
            var reader = XmlReader.Create(fileName);
            reader.MoveToContent();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "add")
                {
                    if (reader.HasAttributes)
                    {
                        valueToGet = reader.GetAttribute("key");
                        if (!string.IsNullOrEmpty(valueToGet) && valueToGet == settingName)
                        {
                            valueToGet = reader.GetAttribute("value");
                            return valueToGet;
                        }
                    }
                }
            }
            return valueToGet;
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            System.Web.Configuration.WebConfigurationManager.AppSettings["DBInitType"] = "InitDbNotChanged";
            LoadWebConfig(textBox4.Text);
            AjourDbContext context = new AjourDbContext(connectionString);
            List<BusinessTrip> bts = repository.BusinessTrips.Where(b => b.Status == (BTStatus.Confirmed | BTStatus.Reported)).ToList();
            List<Journey> journeysToremove = new List<Journey>();
            CalendarItemsCreator ciCreator = new CalendarItemsCreator(repository, calendarItems, journeysToAdd, journeysToremove);

            foreach (BusinessTrip bt in bts)
            {
                ciCreator.GenerateItemsFromDataBase(bt);
            }

            var bindingListJourneys = new BindingList<JourneyForLoader>();
            foreach (Journey journey in journeysToAdd)
            {
                bindingListJourneys.Add(new JourneyForLoader(journey));
            }
            var bindingSourceJourneys = new BindingSource(bindingListJourneys, null);
            dataGridViewJourneys.DataSource = bindingSourceJourneys;

            var bindingListCalendarItems = new BindingList<CalendarItemForLoader>();
            foreach (CalendarItem calendarItem in calendarItems)
            {
                bindingListCalendarItems.Add(new CalendarItemForLoader(calendarItem));
            }
            var bindingSourceCalendarItems = new BindingSource(bindingListCalendarItems, null);
            dataGridViewCalendarItems.DataSource = bindingSourceCalendarItems;

            progressBar1.Value = 100;

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            List<Journey> jrns = repository.Journeys.ToList();
            foreach (Journey item in jrns)
            {
                repository.DeleteJourney(item.JourneyID);
            }

            List<CalendarItem> citems = repository.CalendarItems.ToList();
            foreach (CalendarItem item in citems)
            {
                repository.DeleteCalendarItem(item.CalendarItemID);
            }

            foreach (Journey item in journeysToAdd)
            {
                repository.SaveJourney(item);
            }
            foreach (CalendarItem item in calendarItems)
            {
                repository.SaveCalendarItem(item);
            }

            progressBar1.Value = 100;

        }



    }
}

////Example of password generation
//foreach (var emp in XlsLoader.employees)
//            {
//                emp.DepartmentID = repository.Departments.Where(d => d.DepartmentName == emp.Department.DepartmentName).Select(d => d.DepartmentID).FirstOrDefault();
//                emp.Department = null;
//                emp.PositionID = repository.Positions.Where(p => p.TitleUk == emp.Position.TitleUk).Select(p => p.PositionID).FirstOrDefault();
//                emp.Position = null;
//                repository.SaveEmployee(emp);
//                if (!WebSecurity.UserExists(emp.EID))
//                {
//                    if (generate)
//                    {
//                        currentEmployeePassword = Membership.GeneratePassword(8, 1);
//                        employeesPasswords.Add(emp.EID, currentEmployeePassword);
//                        WebSecurity.CreateUserAndAccount(emp.EID, currentEmployeePassword);
//                    }
//                    else
//                    WebSecurity.CreateUserAndAccount(emp.EID, WebConfigurationManager.AppSettings["DefaultPassword"].ToString());
//                }
//                foreach (var role in XlsLoader.rolesDict[emp.EID])
//                {
//                    if (!System.Web.Security.Roles.IsUserInRole(emp.EID.ToString(), role))
//                    {
//                        System.Web.Security.Roles.AddUserToRole(emp.EID, role);
//                    }
//                }

//            }