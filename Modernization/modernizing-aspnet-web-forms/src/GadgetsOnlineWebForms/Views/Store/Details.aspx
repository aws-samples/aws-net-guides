<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="GadgetsOnlineWebForms.Views.Store.Details" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <h2><%= Model.Name %></h2>

<p>
    <img alt="<%= Model.Name %>" src="<%= Model.ProductArtUrl %>" />
</p>

    <div id="album-details">
        <p>
            <em>Type:</em>
            <%: Model.Category.Name%>
        </p>
        <p>
            <em>Description:</em>
            <%: Model.Category.Description%>
        </p>
        <p>
            <em>Price:</em>$
            <%: String.Format("{0:F}", Model.Price)%>
        </p>
        <p class="button">
            <a href="<%= "/Views/ShoppingCart/ViewCart.aspx?addToCart=" + Model.ProductId %>">Add to cart</a>
        </p>
    </div>
</asp:Content>
