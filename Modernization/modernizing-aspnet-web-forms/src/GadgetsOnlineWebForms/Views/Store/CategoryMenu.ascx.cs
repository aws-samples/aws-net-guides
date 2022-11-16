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
    public partial class CategoryMenu : System.Web.UI.UserControl
    {
        protected List<Category> Model;
        protected void Page_Load(object sender, EventArgs e)
        {
            var inventory = new Inventory();
            Model = inventory.GetAllCategories();
        }
    }
}