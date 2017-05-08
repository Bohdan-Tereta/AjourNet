namespace AjourBT.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuestionnaireQuestId : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Questionnaires", "QuestionSetId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Questionnaires", "QuestionSetId", c => c.Int(nullable: false));
        }
    }
}
