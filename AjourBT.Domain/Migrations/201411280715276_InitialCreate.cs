namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.Employees",
            //    c => new
            //        {
            //            EmployeeID = c.Int(nullable: false, identity: true),
            //            FirstName = c.String(nullable: false),
            //            LastName = c.String(nullable: false),
            //            EID = c.String(nullable: false),
            //            DepartmentID = c.Int(),
            //            DateEmployed = c.DateTime(),
            //            PositionID = c.Int(),
            //            BirthDay = c.DateTime(),
            //            Comment = c.String(),
            //            FullNameUk = c.String(),
            //            DateDismissed = c.DateTime(),
            //            IsManager = c.Boolean(nullable: false),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //            BTRestrictions = c.String(),
            //            IsUserOnly = c.Boolean(nullable: false),
            //            IsGreetingMessageAllow = c.Boolean(nullable: false),
            //            EMail = c.String(),
            //        })
            //    .PrimaryKey(t => t.EmployeeID)
            //    .ForeignKey("dbo.Departments", t => t.DepartmentID)
            //    .ForeignKey("dbo.Positions", t => t.PositionID)
            //    .Index(t => t.DepartmentID)
            //    .Index(t => t.PositionID);
            
            //CreateTable(
            //    "dbo.Departments",
            //    c => new
            //        {
            //            DepartmentID = c.Int(nullable: false, identity: true),
            //            DepartmentName = c.String(nullable: false),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.DepartmentID);
            
            //CreateTable(
            //    "dbo.Positions",
            //    c => new
            //        {
            //            PositionID = c.Int(nullable: false, identity: true),
            //            TitleUk = c.String(nullable: false),
            //            TitleEn = c.String(nullable: false),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.PositionID);
            
            //CreateTable(
            //    "dbo.BusinessTrips",
            //    c => new
            //        {
            //            BusinessTripID = c.Int(nullable: false, identity: true),
            //            StartDate = c.DateTime(nullable: false),
            //            OldStartDate = c.DateTime(),
            //            EndDate = c.DateTime(nullable: false),
            //            OldEndDate = c.DateTime(),
            //            OrderStartDate = c.DateTime(),
            //            OrderEndDate = c.DateTime(),
            //            DaysInBtForOrder = c.Int(),
            //            Status = c.Int(nullable: false),
            //            LocationID = c.Int(nullable: false),
            //            OldLocationID = c.Int(nullable: false),
            //            UnitID = c.Int(nullable: false),
            //            OldLocationTitle = c.String(),
            //            EmployeeID = c.Int(nullable: false),
            //            Purpose = c.String(),
            //            Manager = c.String(),
            //            Responsible = c.String(),
            //            Comment = c.String(),
            //            RejectComment = c.String(),
            //            CancelComment = c.String(),
            //            AccComment = c.String(),
            //            BTMComment = c.String(),
            //            Habitation = c.String(),
            //            HabitationConfirmed = c.Boolean(nullable: false),
            //            Flights = c.String(),
            //            FlightsConfirmed = c.Boolean(nullable: false),
            //            Invitation = c.Boolean(nullable: false),
            //            LastCRUDedBy = c.String(),
            //            LastCRUDTimestamp = c.DateTime(nullable: false),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.BusinessTripID)
            //    .ForeignKey("dbo.Locations", t => t.LocationID)
            //    .ForeignKey("dbo.Units", t => t.UnitID, cascadeDelete: true)
            //    .ForeignKey("dbo.Employees", t => t.EmployeeID)
            //    .Index(t => t.LocationID)
            //    .Index(t => t.UnitID)
            //    .Index(t => t.EmployeeID);
            
            //CreateTable(
            //    "dbo.Locations",
            //    c => new
            //        {
            //            LocationID = c.Int(nullable: false, identity: true),
            //            Title = c.String(nullable: false),
            //            Address = c.String(nullable: false),
            //            CountryID = c.Int(nullable: false),
            //            ResponsibleForLoc = c.String(),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.LocationID)
            //    .ForeignKey("dbo.Countries", t => t.CountryID, cascadeDelete: true)
            //    .Index(t => t.CountryID);
            
            //CreateTable(
            //    "dbo.Countries",
            //    c => new
            //        {
            //            CountryID = c.Int(nullable: false, identity: true),
            //            CountryName = c.String(nullable: false),
            //            Comment = c.String(),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.CountryID);
            
            //CreateTable(
            //    "dbo.Holidays",
            //    c => new
            //        {
            //            HolidayID = c.Int(nullable: false, identity: true),
            //            Title = c.String(nullable: false),
            //            CountryID = c.Int(nullable: false),
            //            HolidayDate = c.DateTime(nullable: false),
            //            IsPostponed = c.Boolean(nullable: false),
            //            HolidayComment = c.String(),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.HolidayID)
            //    .ForeignKey("dbo.Countries", t => t.CountryID, cascadeDelete: true)
            //    .Index(t => t.CountryID);
            
            //CreateTable(
            //    "dbo.Units",
            //    c => new
            //        {
            //            UnitID = c.Int(nullable: false, identity: true),
            //            Title = c.String(nullable: false),
            //            ShortTitle = c.String(nullable: false),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.UnitID);
            
            //CreateTable(
            //    "dbo.Journeys",
            //    c => new
            //        {
            //            JourneyID = c.Int(nullable: false, identity: true),
            //            BusinessTripID = c.Int(nullable: false),
            //            Date = c.DateTime(nullable: false),
            //            ReclaimDate = c.DateTime(),
            //            DayOff = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.JourneyID)
            //    .ForeignKey("dbo.BusinessTrips", t => t.BusinessTripID, cascadeDelete: true)
            //    .Index(t => t.BusinessTripID);
            
            //CreateTable(
            //    "dbo.Visas",
            //    c => new
            //        {
            //            EmployeeID = c.Int(nullable: false),
            //            VisaType = c.String(nullable: false),
            //            StartDate = c.DateTime(nullable: false),
            //            DueDate = c.DateTime(nullable: false),
            //            Days = c.Int(nullable: false),
            //            DaysUsedInBT = c.Int(),
            //            DaysUsedInPrivateTrips = c.Int(),
            //            Entries = c.Int(nullable: false),
            //            EntriesUsedInBT = c.Int(),
            //            EntriesUsedInPrivateTrips = c.Int(),
            //            CorrectionForVisaDays = c.Int(),
            //            CorrectionForVisaEntries = c.Int(),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.EmployeeID)
            //    .ForeignKey("dbo.Employees", t => t.EmployeeID)
            //    .Index(t => t.EmployeeID);
            
            //CreateTable(
            //    "dbo.PrivateTrips",
            //    c => new
            //        {
            //            PrivateTripID = c.Int(nullable: false, identity: true),
            //            StartDate = c.DateTime(nullable: false),
            //            EndDate = c.DateTime(nullable: false),
            //            EmployeeID = c.Int(nullable: false),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.PrivateTripID)
            //    .ForeignKey("dbo.Visas", t => t.EmployeeID, cascadeDelete: true)
            //    .Index(t => t.EmployeeID);
            
            //CreateTable(
            //    "dbo.VisaRegistrationDates",
            //    c => new
            //        {
            //            EmployeeID = c.Int(nullable: false),
            //            VisaType = c.String(nullable: false),
            //            RegistrationDate = c.DateTime(),
            //            RegistrationTime = c.String(),
            //            City = c.String(),
            //            RegistrationNumber = c.String(),
            //            PaymentDate = c.DateTime(),
            //            PaymentTime = c.String(),
            //            PaymentPIN = c.String(),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.EmployeeID)
            //    .ForeignKey("dbo.Employees", t => t.EmployeeID)
            //    .Index(t => t.EmployeeID);
            
            //CreateTable(
            //    "dbo.Permits",
            //    c => new
            //        {
            //            EmployeeID = c.Int(nullable: false),
            //            IsKartaPolaka = c.Boolean(nullable: false),
            //            Number = c.String(),
            //            StartDate = c.DateTime(),
            //            EndDate = c.DateTime(),
            //            CancelRequestDate = c.DateTime(),
            //            ProlongRequestDate = c.DateTime(),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.EmployeeID)
            //    .ForeignKey("dbo.Employees", t => t.EmployeeID)
            //    .Index(t => t.EmployeeID);
            
            //CreateTable(
            //    "dbo.Insurances",
            //    c => new
            //        {
            //            EmployeeID = c.Int(nullable: false),
            //            StartDate = c.DateTime(nullable: false),
            //            EndDate = c.DateTime(nullable: false),
            //            Days = c.Int(nullable: false),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.EmployeeID)
            //    .ForeignKey("dbo.Employees", t => t.EmployeeID)
            //    .Index(t => t.EmployeeID);
            
            //CreateTable(
            //    "dbo.Passports",
            //    c => new
            //        {
            //            EmployeeID = c.Int(nullable: false),
            //            EndDate = c.DateTime(),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.EmployeeID)
            //    .ForeignKey("dbo.Employees", t => t.EmployeeID)
            //    .Index(t => t.EmployeeID);
            
            //CreateTable(
            //    "dbo.CalendarItems",
            //    c => new
            //        {
            //            CalendarItemID = c.Int(nullable: false, identity: true),
            //            From = c.DateTime(nullable: false),
            //            To = c.DateTime(nullable: false),
            //            EmployeeID = c.Int(nullable: false),
            //            Location = c.String(),
            //            Type = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.CalendarItemID)
            //    .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
            //    .Index(t => t.EmployeeID);
            
            //CreateTable(
            //    "dbo.Overtimes",
            //    c => new
            //        {
            //            OvertimeID = c.Int(nullable: false, identity: true),
            //            EmployeeID = c.Int(nullable: false),
            //            Date = c.DateTime(nullable: false),
            //            ReclaimDate = c.DateTime(),
            //            DayOff = c.Boolean(nullable: false),
            //            Type = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.OvertimeID)
            //    .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
            //    .Index(t => t.EmployeeID);
            
            //CreateTable(
            //    "dbo.Vacations",
            //    c => new
            //        {
            //            VacationID = c.Int(nullable: false, identity: true),
            //            EmployeeID = c.Int(nullable: false),
            //            From = c.DateTime(nullable: false),
            //            To = c.DateTime(nullable: false),
            //            Type = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.VacationID)
            //    .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
            //    .Index(t => t.EmployeeID);
            
            //CreateTable(
            //    "dbo.Sicknesses",
            //    c => new
            //        {
            //            SickID = c.Int(nullable: false, identity: true),
            //            EmployeeID = c.Int(nullable: false),
            //            From = c.DateTime(nullable: false),
            //            To = c.DateTime(nullable: false),
            //            SicknessType = c.String(),
            //            Workdays = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.SickID)
            //    .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
            //    .Index(t => t.EmployeeID);
            
            //CreateTable(
            //    "dbo.UserProfile",
            //    c => new
            //        {
            //            UserId = c.Int(nullable: false, identity: true),
            //            UserName = c.String(),
            //        })
            //    .PrimaryKey(t => t.UserId);
            
            //CreateTable(
            //    "dbo.Messages",
            //    c => new
            //        {
            //            MessageID = c.Int(nullable: false, identity: true),
            //            Role = c.String(),
            //            Subject = c.String(),
            //            Body = c.String(),
            //            Link = c.String(),
            //            TimeStamp = c.DateTime(nullable: false),
            //            ReplyTo = c.String(),
            //        })
            //    .PrimaryKey(t => t.MessageID);
            
            //CreateTable(
            //    "dbo.Greetings",
            //    c => new
            //        {
            //            GreetingId = c.Int(nullable: false, identity: true),
            //            GreetingHeader = c.String(nullable: false),
            //            GreetingBody = c.String(nullable: false),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.GreetingId);
            
        }
        
        public override void Down()
        {
            //DropIndex("dbo.Sicknesses", new[] { "EmployeeID" });
            //DropIndex("dbo.Vacations", new[] { "EmployeeID" });
            //DropIndex("dbo.Overtimes", new[] { "EmployeeID" });
            //DropIndex("dbo.CalendarItems", new[] { "EmployeeID" });
            //DropIndex("dbo.Passports", new[] { "EmployeeID" });
            //DropIndex("dbo.Insurances", new[] { "EmployeeID" });
            //DropIndex("dbo.Permits", new[] { "EmployeeID" });
            //DropIndex("dbo.VisaRegistrationDates", new[] { "EmployeeID" });
            //DropIndex("dbo.PrivateTrips", new[] { "EmployeeID" });
            //DropIndex("dbo.Visas", new[] { "EmployeeID" });
            //DropIndex("dbo.Journeys", new[] { "BusinessTripID" });
            //DropIndex("dbo.Holidays", new[] { "CountryID" });
            //DropIndex("dbo.Locations", new[] { "CountryID" });
            //DropIndex("dbo.BusinessTrips", new[] { "EmployeeID" });
            //DropIndex("dbo.BusinessTrips", new[] { "UnitID" });
            //DropIndex("dbo.BusinessTrips", new[] { "LocationID" });
            //DropIndex("dbo.Employees", new[] { "PositionID" });
            //DropIndex("dbo.Employees", new[] { "DepartmentID" });
            //DropForeignKey("dbo.Sicknesses", "EmployeeID", "dbo.Employees");
            //DropForeignKey("dbo.Vacations", "EmployeeID", "dbo.Employees");
            //DropForeignKey("dbo.Overtimes", "EmployeeID", "dbo.Employees");
            //DropForeignKey("dbo.CalendarItems", "EmployeeID", "dbo.Employees");
            //DropForeignKey("dbo.Passports", "EmployeeID", "dbo.Employees");
            //DropForeignKey("dbo.Insurances", "EmployeeID", "dbo.Employees");
            //DropForeignKey("dbo.Permits", "EmployeeID", "dbo.Employees");
            //DropForeignKey("dbo.VisaRegistrationDates", "EmployeeID", "dbo.Employees");
            //DropForeignKey("dbo.PrivateTrips", "EmployeeID", "dbo.Visas");
            //DropForeignKey("dbo.Visas", "EmployeeID", "dbo.Employees");
            //DropForeignKey("dbo.Journeys", "BusinessTripID", "dbo.BusinessTrips");
            //DropForeignKey("dbo.Holidays", "CountryID", "dbo.Countries");
            //DropForeignKey("dbo.Locations", "CountryID", "dbo.Countries");
            //DropForeignKey("dbo.BusinessTrips", "EmployeeID", "dbo.Employees");
            //DropForeignKey("dbo.BusinessTrips", "UnitID", "dbo.Units");
            //DropForeignKey("dbo.BusinessTrips", "LocationID", "dbo.Locations");
            //DropForeignKey("dbo.Employees", "PositionID", "dbo.Positions");
            //DropForeignKey("dbo.Employees", "DepartmentID", "dbo.Departments");
            //DropTable("dbo.Greetings");
            //DropTable("dbo.Messages");
            //DropTable("dbo.UserProfile");
            //DropTable("dbo.Sicknesses");
            //DropTable("dbo.Vacations");
            //DropTable("dbo.Overtimes");
            //DropTable("dbo.CalendarItems");
            //DropTable("dbo.Passports");
            //DropTable("dbo.Insurances");
            //DropTable("dbo.Permits");
            //DropTable("dbo.VisaRegistrationDates");
            //DropTable("dbo.PrivateTrips");
            //DropTable("dbo.Visas");
            //DropTable("dbo.Journeys");
            //DropTable("dbo.Units");
            //DropTable("dbo.Holidays");
            //DropTable("dbo.Countries");
            //DropTable("dbo.Locations");
            //DropTable("dbo.BusinessTrips");
            //DropTable("dbo.Positions");
            //DropTable("dbo.Departments");
            //DropTable("dbo.Employees");
        }
    }
}
