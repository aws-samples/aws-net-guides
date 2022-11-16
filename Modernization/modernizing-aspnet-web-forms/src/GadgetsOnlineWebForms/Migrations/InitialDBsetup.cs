using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace GadgetsOnlineWebForms.Migrations
{
    public class InitialDBsetup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Carts",
                c => new
                {
                    RecordId = c.Int(nullable: false, identity: true),
                    CartId = c.String(),
                    PizzaId = c.Int(nullable: false),
                    Count = c.Int(nullable: false),
                    DateCreated = c.DateTime(nullable: false),
                    Product_ProductId = c.Int(),
                })
                .PrimaryKey(t => t.RecordId)
                .ForeignKey("dbo.Products", t => t.Product_ProductId)
                .Index(t => t.Product_ProductId);

            CreateTable(
                "dbo.Products",
                c => new
                {
                    ProductId = c.Int(nullable: false, identity: true),
                    CategoryId = c.Int(nullable: false),
                    Name = c.String(nullable: false, maxLength: 255),
                    Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    ProductArtUrl = c.String(maxLength: 1024),
                })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);

            CreateTable(
                "dbo.Categories",
                c => new
                {
                    CategoryId = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    Description = c.String(),
                })
                .PrimaryKey(t => t.CategoryId);

            CreateTable(
                "dbo.OrderDetails",
                c => new
                {
                    OrderDetailId = c.Int(nullable: false, identity: true),
                    OrderId = c.Int(nullable: false),
                    ProductId = c.Int(nullable: false),
                    Quantity = c.Int(nullable: false),
                    UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                })
                .PrimaryKey(t => t.OrderDetailId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId);

            CreateTable(
                "dbo.Orders",
                c => new
                {
                    OrderId = c.Int(nullable: false, identity: true),
                    OrderDate = c.DateTime(nullable: false),
                    Username = c.String(),
                    FirstName = c.String(nullable: false, maxLength: 160),
                    LastName = c.String(nullable: false, maxLength: 160),
                    Address = c.String(nullable: false, maxLength: 70),
                    City = c.String(nullable: false, maxLength: 40),
                    State = c.String(nullable: false, maxLength: 40),
                    PostalCode = c.String(nullable: false, maxLength: 10),
                    Country = c.String(nullable: false, maxLength: 40),
                    Phone = c.String(nullable: false, maxLength: 24),
                    Email = c.String(nullable: false),
                    Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                })
                .PrimaryKey(t => t.OrderId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Carts", "Product_ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropIndex("dbo.OrderDetails", new[] { "ProductId" });
            DropIndex("dbo.OrderDetails", new[] { "OrderId" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropIndex("dbo.Carts", new[] { "Product_ProductId" });
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Categories");
            DropTable("dbo.Products");
            DropTable("dbo.Carts");
        }
    }
}