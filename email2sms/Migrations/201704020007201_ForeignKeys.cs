namespace email2sms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignKeys : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.email2sms_InvoiceLog", "Message_Id", "dbo.email2sms_MessageLog");
            DropForeignKey("dbo.email2sms_InvoiceLog", "SendTo_Id", "dbo.email2sms_Phone");
            DropForeignKey("dbo.email2sms_Phone", "Subscription_Id", "dbo.email2sms_Subscription");
            DropIndex("dbo.email2sms_InvoiceLog", new[] { "Message_Id" });
            DropIndex("dbo.email2sms_InvoiceLog", new[] { "SendTo_Id" });
            DropIndex("dbo.email2sms_Phone", new[] { "Subscription_Id" });
            RenameColumn(table: "dbo.email2sms_InvoiceLog", name: "SendTo_Id", newName: "Phone_Id");
            AlterColumn("dbo.email2sms_InvoiceLog", "Message_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.email2sms_InvoiceLog", "Phone_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.email2sms_Phone", "Subscription_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.email2sms_InvoiceLog", "Phone_Id");
            CreateIndex("dbo.email2sms_InvoiceLog", "Message_Id");
            CreateIndex("dbo.email2sms_Phone", "Subscription_Id");
            AddForeignKey("dbo.email2sms_InvoiceLog", "Message_Id", "dbo.email2sms_MessageLog", "Id", cascadeDelete: true);
            AddForeignKey("dbo.email2sms_InvoiceLog", "Phone_Id", "dbo.email2sms_Phone", "Id", cascadeDelete: true);
            AddForeignKey("dbo.email2sms_Phone", "Subscription_Id", "dbo.email2sms_Subscription", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.email2sms_Phone", "Subscription_Id", "dbo.email2sms_Subscription");
            DropForeignKey("dbo.email2sms_InvoiceLog", "Phone_Id", "dbo.email2sms_Phone");
            DropForeignKey("dbo.email2sms_InvoiceLog", "Message_Id", "dbo.email2sms_MessageLog");
            DropIndex("dbo.email2sms_Phone", new[] { "Subscription_Id" });
            DropIndex("dbo.email2sms_InvoiceLog", new[] { "Message_Id" });
            DropIndex("dbo.email2sms_InvoiceLog", new[] { "Phone_Id" });
            AlterColumn("dbo.email2sms_Phone", "Subscription_Id", c => c.Int());
            AlterColumn("dbo.email2sms_InvoiceLog", "Phone_Id", c => c.Int());
            AlterColumn("dbo.email2sms_InvoiceLog", "Message_Id", c => c.Int());
            RenameColumn(table: "dbo.email2sms_InvoiceLog", name: "Phone_Id", newName: "SendTo_Id");
            CreateIndex("dbo.email2sms_Phone", "Subscription_Id");
            CreateIndex("dbo.email2sms_InvoiceLog", "SendTo_Id");
            CreateIndex("dbo.email2sms_InvoiceLog", "Message_Id");
            AddForeignKey("dbo.email2sms_Phone", "Subscription_Id", "dbo.email2sms_Subscription", "Id");
            AddForeignKey("dbo.email2sms_InvoiceLog", "SendTo_Id", "dbo.email2sms_Phone", "Id");
            AddForeignKey("dbo.email2sms_InvoiceLog", "Message_Id", "dbo.email2sms_MessageLog", "Id");
        }
    }
}
