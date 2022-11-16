<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Complete.aspx.cs" Inherits="GadgetsOnlineWebForms.Views.Checkout.Complete" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Checkout Complete</h2>

    <p>Thanks for your order! Your order number is: 000-176-54- <%= Model %></p>

    <p>
        You know this is a demo application, right? <br />
        Nothing is gonna get shipped. Try the <a href="http://www.amazon.com">everything store</a>
        for real shopping !

    </p>

    <p>
        Go back to  <a href="/Views/Home/Index.aspx">home page</a>!
    </p>
</asp:Content>
