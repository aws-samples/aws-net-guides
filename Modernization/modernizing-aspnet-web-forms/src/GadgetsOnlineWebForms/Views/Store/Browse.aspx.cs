using GadgetsOnline.Models;
using GadgetsOnline.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GadgetsOnlineWebForms.Views.Store
{
    public partial class Browse : System.Web.UI.Page
    {
        protected List<Product> Products;
        protected void Page_Load(object sender, EventArgs e)
        {
            int categoryId = 0;
            if (!Int32.TryParse(Request.QueryString["categoryId"], out categoryId))
                Server.Transfer("/Views/Home/Index.aspx");

            //var inventory = new Inventory();
            //var Products = inventory.GetAllProductsInCategory(categoryId);

            //if (Products == null)
            //    Server.Transfer("/Views/Home/Index.aspx");
        }
    }
}