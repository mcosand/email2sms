namespace email2sms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTextColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.email2sms_MessageLog", "Text", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.email2sms_MessageLog", "Text");
        }
    }
}
