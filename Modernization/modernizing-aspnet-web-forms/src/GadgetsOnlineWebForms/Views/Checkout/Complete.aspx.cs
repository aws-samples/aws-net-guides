using GadgetsOnline.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GadgetsOnlineWebForms.Views.Checkout
{
    public partial class Complete : System.Web.UI.Page
    {
        protected int Model;
        protected void Page_Load(object sender, EventArgs e)
        {
            int orderId = 0;
            if (!Int32.TryParse(Request.QueryString["orderId"], out orderId))
                Server.Transfer("/Views/Home/Index.aspx");

            Model = orderId;

            this.Page.Title = "Checkout Complete";
        }
    }
}