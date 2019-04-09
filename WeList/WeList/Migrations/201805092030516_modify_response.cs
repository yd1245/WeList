namespace WeList.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modify_response : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "PostId", c => c.Int(nullable: false));
            AlterColumn("dbo.Messages", "Body", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Messages", "Body", c => c.String());
            DropColumn("dbo.Messages", "PostId");
        }
    }
}
