namespace News.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 50),
                        Summary = c.String(nullable: false),
                        Content = c.String(nullable: false),
                        PublishedAt = c.DateTime(nullable: false),
                        Author = c.String(nullable: false),
                        isLead = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImageSmall = c.Binary(),
                        ImageLarge = c.Binary(),
                        ArticleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Articles", t => t.ArticleId, cascadeDelete: true)
                .Index(t => t.ArticleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Images", "ArticleId", "dbo.Articles");
            DropIndex("dbo.Images", new[] { "ArticleId" });
            DropTable("dbo.Images");
            DropTable("dbo.Articles");
        }
    }
}
