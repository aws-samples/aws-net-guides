using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace GadgetsOnlineWebForms.Migrations
{
    public class UpdatedCart : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Carts", "Product_ProductId", "dbo.Products");
            DropIndex("dbo.Carts", new[] { "Product_ProductId" });
            RenameColumn(table: "dbo.Carts", name: "Product_ProductId", newName: "ProductId");
            AlterColumn("dbo.Carts", "ProductId", c => c.Int(nullable: false));
            CreateIndex("dbo.Carts", "ProductId");
            AddForeignKey("dbo.Carts", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
            DropColumn("dbo.Carts", "PizzaId");
        }

        public override void Down()
        {
            AddColumn("dbo.Carts", "PizzaId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Carts", "ProductId", "dbo.Products");
            DropIndex("dbo.Carts", new[] { "ProductId" });
            AlterColumn("dbo.Carts", "ProductId", c => c.Int());
            RenameColumn(table: "dbo.Carts", name: "ProductId", newName: "Product_ProductId");
            CreateIndex("dbo.Carts", "Product_ProductId");
            AddForeignKey("dbo.Carts", "Product_ProductId", "dbo.Products", "ProductId");
        }
    }
}