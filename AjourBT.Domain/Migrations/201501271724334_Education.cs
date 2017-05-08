namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Education : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "EducationAcquiredType", c => c.Int(nullable: false));
            AddColumn("dbo.Employees", "EducationAcquiredDate", c => c.DateTime());
            AddColumn("dbo.Employees", "EducationInProgressType", c => c.Int(nullable: false));
            AddColumn("dbo.Employees", "EducationInProgressDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "EducationInProgressDate");
            DropColumn("dbo.Employees", "EducationInProgressType");
            DropColumn("dbo.Employees", "EducationAcquiredDate");
            DropColumn("dbo.Employees", "EducationAcquiredType");
        }
    }
}
