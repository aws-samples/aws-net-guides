using GadgetsOnline.Models;
using GadgetsOnline.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GadgetsOnlineWebForms.Views.Checkout
{
    public partial class AddressAndPayment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_click(object sender, EventArgs e)
        {
            var order = new Order();
            TryUpdateModel(order);
            try
            {
                order.Username = "Anonymous";
                order.OrderDate = DateTime.Now;
                var cart = GetCart(this.Context);
                bool result = GetOrderProcess().ProcessOrder(order, cart);
                Response.Redirect("/Views/Checkout/Complete.aspx?orderId=" + order.OrderId, true);                
            }
            catch
            {
                //Invalid - redisplay with errors
                
            }
        }

        OrderProcessing orderProcessing;
        private OrderProcessing GetOrderProcess()
        {
            if (this.orderProcessing == null)
            {
                this.orderProcessing = new OrderProcessing();
            }

            return this.orderProcessing;
        }

        private GadgetsOnline.Services.ShoppingCart GetCart(HttpContext context)
        {
            var cart = new GadgetsOnline.Services.ShoppingCart();
            cart.ShoppingCartId = GetCartId(context);
            return cart;
        }

        private const string CartSessionKey = "CartId";

        private string GetCartId(HttpContext context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] = context.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();

                    // Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }

            return context.Session[CartSessionKey].ToString();
        }
    }
}