namespace WeList.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modify_message : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "hidden", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "hidden");
        }
    }
}
