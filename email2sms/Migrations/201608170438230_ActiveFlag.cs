namespace email2sms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActiveFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.email2sms_Phone", "Active", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.email2sms_Phone", "Active");
        }
    }
}
