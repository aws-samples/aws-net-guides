using System;
using System.Collections.Generic;
using System.Linq;
using GadgetsOnline.Models;

namespace GadgetsOnline.Services
{
    public class OrderProcessing
    {
        GadgetsOnlineEntities store = new GadgetsOnlineEntities();
        internal bool ProcessOrder(Order order, ShoppingCart cart)
        {
            store.Orders.Add(order);
            store.SaveChanges();

            //Process the order
            cart.CreateOrder(order);

            return true;
        }
    }
}