using GadgetsOnline.Models;
using GadgetsOnline.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GadgetsOnlineWebForms.Views.Home
{
    public partial class Index : System.Web.UI.Page
    {
        protected List<Product> Model;
        protected void Page_Load(object sender, EventArgs e)
        {
            var inventory = new Inventory();
            Model = inventory.GetBestSellers(6);
        }
    }
}