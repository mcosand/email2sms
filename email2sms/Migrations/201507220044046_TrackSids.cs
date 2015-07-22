namespace email2sms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TrackSids : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.email2sms_InvoiceLog", "Sid", c => c.String());
            AlterColumn("dbo.email2sms_InvoiceLog", "Price", c => c.Decimal(precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.email2sms_InvoiceLog", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            DropColumn("dbo.email2sms_InvoiceLog", "Sid");
        }
    }
}
