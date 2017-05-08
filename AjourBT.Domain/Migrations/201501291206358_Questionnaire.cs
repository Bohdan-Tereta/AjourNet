namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Questionnaire : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Questionnaires",
                c => new
                    {
                        QuestionnaireId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        QuestionSetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QuestionnaireId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Questionnaires");
        }
    }
}
