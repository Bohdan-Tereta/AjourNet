namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuestionSet : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionSets",
                c => new
                    {
                        QuestionSetId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Questions = c.String(),
                    })
                .PrimaryKey(t => t.QuestionSetId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.QuestionSets");
        }
    }
}
