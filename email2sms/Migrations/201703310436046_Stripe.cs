namespace email2sms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Stripe : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.email2sms_Subscription", "User", c => c.Guid(nullable: false));
            AddColumn("dbo.email2sms_Subscription", "StripeCustomer", c => c.String());
            AddColumn("dbo.email2sms_Subscription", "LastInvoiceUtc", c => c.DateTime());
            DropColumn("dbo.email2sms_Subscription", "Email");
            DropColumn("dbo.email2sms_Subscription", "Balance");
        }
        
        public override void Down()
        {
            AddColumn("dbo.email2sms_Subscription", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("dbo.email2sms_Subscription", "Email", c => c.String());
            DropColumn("dbo.email2sms_Subscription", "LastInvoiceUtc");
            DropColumn("dbo.email2sms_Subscription", "StripeCustomer");
            DropColumn("dbo.email2sms_Subscription", "User");
        }
    }
}
