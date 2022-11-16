<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Browse.aspx.cs" Inherits="GadgetsOnlineWebForms.Views.Store.Browse" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="genre">
    <ul id="album-list">
        <% 
        int categoryId = 0;
        if (!Int32.TryParse(Request.QueryString["categoryId"], out categoryId))
            Server.Transfer("/Views/Home/Index.aspx");

        var inventory = new GadgetsOnline.Services.Inventory();
        var Products = inventory.GetAllProductsInCategory(categoryId);

        if (Products == null)
            Server.Transfer("/Views/Home/Index.aspx");    
        foreach (var product in Products)
        { 
            string productUrl = "/Views/Store/Details.aspx?productId=" + product.ProductId.ToString();  %>
            
            <li>
                <a href="<%= productUrl %>" >
                    <img alt="<%= product.Name %>" src="<%= product.ProductArtUrl %>" />
                    <span><%= product.Name %></span>
                </a>
            </li>
        <% } %>
    </ul>
</div>
</asp:Content>
