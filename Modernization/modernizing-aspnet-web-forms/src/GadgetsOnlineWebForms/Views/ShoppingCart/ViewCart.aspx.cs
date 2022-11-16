using GadgetsOnline.Services;
using GadgetsOnlineWebForms.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GadgetsOnlineWebForms.Views.ShoppingCart
{
    public partial class ViewCart : System.Web.UI.Page
    {
        protected ShoppingCartViewModel Model;
        protected void Page_Load(object sender, EventArgs e)
        {
            int productId = 0;
            if (Int32.TryParse(Request.QueryString["addToCart"], out productId))
                AddToCart(productId);
            else if (Int32.TryParse(Request.QueryString["removeFromCart"], out productId))
                RemoveFromCart(productId);

            var cart = GetCart(this.Context);
            // Set up our ViewModel
            Model = new ShoppingCartViewModel { CartItems = cart.GetCartItems(), CartTotal = cart.GetTotal() };
        }

        public void AddToCart(int id)
        {
            var cart = GetCart(this.Context);
            cart.AddToCart(id);
            // Go back to the main store page for more shopping
            //return RedirectToAction("Index");
        }

        public void RemoveFromCart(int id)
        {
            var cart = GetCart(this.Context);
            int itemCount = cart.RemoveFromCart(id);
            var inventory = new Inventory();
            var productName = inventory.GetProductNameById(id);
            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel { Message = Server.HtmlEncode(productName) + " has been removed from your shopping cart.", CartTotal = cart.GetTotal(), CartCount = cart.GetCount(), ItemCount = itemCount, DeleteId = id };
            //return RedirectToAction("Index");
        }


        private const string CartSessionKey = "CartId";

        private GadgetsOnline.Services.ShoppingCart GetCart(HttpContext context)
        {
            var cart = new GadgetsOnline.Services.ShoppingCart();
            cart.ShoppingCartId = GetCartId(context);
            return cart;
        }

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