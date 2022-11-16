<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GadgetsOnlineWebForms._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div id="promotion">

</div>

<h3><em>Our best selling </em>Gadgets Online.</h3>

<class ="row">


   <ul id="album-list">
        <% foreach (var product in Model)
            {
                var productUrl = "/Views/Store/Details.aspx?productId=" + product.ProductId.ToString();
                %>
            <li>
                <a href="<%= productUrl %>" >

                    <img alt="<%= product.Name %>" src="<%= product.ProductArtUrl %>" />
                    <span><%= @product.Name %></span>
                </a>
            </li>
        <% } %>
    </ul>



</asp:Content>
