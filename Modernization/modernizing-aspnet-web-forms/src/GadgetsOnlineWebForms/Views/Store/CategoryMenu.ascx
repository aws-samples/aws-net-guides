<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryMenu.ascx.cs" Inherits="GadgetsOnlineWebForms.Views.Store.CategoryMenu" %>

<ul id="categories">
<% foreach (var category in Model)
    {
        string categoryUrl = "/Views/Store/Browse.aspx?categoryId=" + category.CategoryId;  %>
        <li>
            <a href="<%= categoryUrl %>" ><%= category.Name %></a>
        </li>
<% } %>
</ul>
