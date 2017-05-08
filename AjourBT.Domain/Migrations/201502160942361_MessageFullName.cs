namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MessageFullName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "FullName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "FullName");
        }
    }
}
