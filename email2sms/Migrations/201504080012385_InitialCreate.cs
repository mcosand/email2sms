namespace email2sms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.email2sms_InvoiceLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SendTime = c.DateTime(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Message_Id = c.Int(),
                        SendTo_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.email2sms_MessageLog", t => t.Message_Id)
                .ForeignKey("dbo.email2sms_Phone", t => t.SendTo_Id)
                .Index(t => t.Message_Id)
                .Index(t => t.SendTo_Id);
            
            CreateTable(
                "dbo.email2sms_MessageLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Json = c.String(),
                        Received = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.email2sms_Phone",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Address = c.String(),
                        Subscription_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.email2sms_Subscription", t => t.Subscription_Id)
                .Index(t => t.Subscription_Id);
            
            CreateTable(
                "dbo.email2sms_Subscription",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.email2sms_InvoiceLog", "SendTo_Id", "dbo.email2sms_Phone");
            DropForeignKey("dbo.email2sms_Phone", "Subscription_Id", "dbo.email2sms_Subscription");
            DropForeignKey("dbo.email2sms_InvoiceLog", "Message_Id", "dbo.email2sms_MessageLog");
            DropIndex("dbo.email2sms_Phone", new[] { "Subscription_Id" });
            DropIndex("dbo.email2sms_InvoiceLog", new[] { "SendTo_Id" });
            DropIndex("dbo.email2sms_InvoiceLog", new[] { "Message_Id" });
            DropTable("dbo.email2sms_Subscription");
            DropTable("dbo.email2sms_Phone");
            DropTable("dbo.email2sms_MessageLog");
            DropTable("dbo.email2sms_InvoiceLog");
        }
    }
}
