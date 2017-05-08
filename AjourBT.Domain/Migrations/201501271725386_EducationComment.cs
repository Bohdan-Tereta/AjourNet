namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EducationComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "EducationComment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "EducationComment");
        }
    }
}
