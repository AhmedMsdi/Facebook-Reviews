namespace Atreemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        access_token = c.String(),
                        category = c.String(),
                        name = c.String(),
                        IsCheck = c.Boolean(nullable: false),
                        fbuser_id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Users", t => t.fbuser_id)
                .Index(t => t.fbuser_id);
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        created_time = c.DateTime(nullable: false),
                        rating = c.Int(nullable: false),
                        review_text = c.String(),
                        reviewer_id_user = c.Int(),
                        page_id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Reviewer", t => t.reviewer_id_user)
                .ForeignKey("dbo.Pages", t => t.page_id)
                .Index(t => t.reviewer_id_user)
                .Index(t => t.page_id);
            
            CreateTable(
                "dbo.Reviewer",
                c => new
                    {
                        id_user = c.Int(nullable: false, identity: true),
                        id = c.String(),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.id_user);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pages", "fbuser_id", "dbo.Users");
            DropForeignKey("dbo.Ratings", "page_id", "dbo.Pages");
            DropForeignKey("dbo.Ratings", "reviewer_id_user", "dbo.Reviewer");
            DropIndex("dbo.Ratings", new[] { "page_id" });
            DropIndex("dbo.Ratings", new[] { "reviewer_id_user" });
            DropIndex("dbo.Pages", new[] { "fbuser_id" });
            DropTable("dbo.Users");
            DropTable("dbo.Reviewer");
            DropTable("dbo.Ratings");
            DropTable("dbo.Pages");
        }
    }
}
