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
    public partial class Details : System.Web.UI.Page
    {
        protected Product Model;
        protected void Page_Load(object sender, EventArgs e)
        {
            int productId = 0;
            if(!Int32.TryParse(Request.QueryString["productId"], out productId))
                Server.Transfer("/Views/Home/Index.aspx");
            
            var inventory = new Inventory();
            Model = inventory.GetProductById(productId);

            if(Model == null)
                Server.Transfer("/Views/Home/Index.aspx");

            Title = "Gadget - " + Model.Name;
        }
    }
}