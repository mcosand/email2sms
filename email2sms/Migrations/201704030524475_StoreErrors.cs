namespace email2sms.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StoreErrors : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.email2sms_ErrorRow",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeUtc = c.DateTime(nullable: false),
                        User = c.String(),
                        Message = c.String(),
                        Stack = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.email2sms_ErrorRow");
        }
    }
}
