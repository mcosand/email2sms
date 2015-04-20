namespace email2sms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DecimalPrecision : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.email2sms_InvoiceLog", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("dbo.email2sms_Subscription", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.email2sms_Subscription", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.email2sms_InvoiceLog", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
