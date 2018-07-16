namespace Atreemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Ratings", "reviewer_id_user", "dbo.Reviewer");
            DropIndex("dbo.Ratings", new[] { "reviewer_id_user" });
            RenameColumn(table: "dbo.Ratings", name: "reviewer_id_user", newName: "reviewer_id");
            DropPrimaryKey("dbo.Reviewer");
            AlterColumn("dbo.Ratings", "reviewer_id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Reviewer", "id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Reviewer", "id");
            CreateIndex("dbo.Ratings", "reviewer_id");
            AddForeignKey("dbo.Ratings", "reviewer_id", "dbo.Reviewer", "id");
            DropColumn("dbo.Reviewer", "id_user");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reviewer", "id_user", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Ratings", "reviewer_id", "dbo.Reviewer");
            DropIndex("dbo.Ratings", new[] { "reviewer_id" });
            DropPrimaryKey("dbo.Reviewer");
            AlterColumn("dbo.Reviewer", "id", c => c.String());
            AlterColumn("dbo.Ratings", "reviewer_id", c => c.Int());
            AddPrimaryKey("dbo.Reviewer", "id_user");
            RenameColumn(table: "dbo.Ratings", name: "reviewer_id", newName: "reviewer_id_user");
            CreateIndex("dbo.Ratings", "reviewer_id_user");
            AddForeignKey("dbo.Ratings", "reviewer_id_user", "dbo.Reviewer", "id_user");
        }
    }
}
