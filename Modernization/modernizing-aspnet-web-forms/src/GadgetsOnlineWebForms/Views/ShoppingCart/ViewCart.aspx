<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewCart.aspx.cs" Inherits="GadgetsOnlineWebForms.Views.ShoppingCart.ViewCart" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

<%--    <script type="text/javascript">$(function () {
        // Document.ready -> link up remove event handler
        $(".RemoveLink").click(function () {
            // Get the id from the link
            var recordToDelete = $(this).attr("data-id");

            if (recordToDelete != '') {

                // Perform the ajax post
                $.post("/ShoppingCart/RemoveFromCart", { "id": recordToDelete },
                    function (data) {
                        // Successful requests get here
                        // Update the page elements
                        if (data.ItemCount == 0) {
                            $('#row-' + data.DeleteId).fadeOut('slow');
                        } else {
                            $('#item-count-' + data.DeleteId).text(data.ItemCount);
                        }

                        $('#cart-total').text(data.CartTotal);
                        $('#update-message').text(data.Message);
                        $('#cart-status').text('Cart (' + data.CartCount + ')');
                    });
            }
        });

    });


    function handleUpdate() {
        // Load and deserialize the returned JSON data
        var json = context.get_data();
        var data = Sys.Serialization.JavaScriptSerializer.deserialize(json);

        // Update the page elements
        if (data.ItemCount == 0) {
            $('#row-' + data.DeleteId).fadeOut('slow');
        } else {
            $('#item-count-' + data.DeleteId).text(data.ItemCount);
        }

        $('#cart-total').text(data.CartTotal);
        $('#update-message').text(data.Message);
        $('#cart-status').text('Cart (' + data.CartCount + ')');
    }</script>--%>
<h3>
    <em>Review</em> your cart:
</h3>
<p class="button">
    <a href="/Views/Checkout/AddressAndPayment.aspx">Checkout >></a>
</p>
<div id="update-message">
</div>
<table>
    <tr>
        <th>
            Product
        </th>
        <th>
            Price (each)
        </th>
        <th>
            Quantity
        </th>
        <th></th>
    </tr>
    <% foreach (var item in Model.CartItems)
        {
            var productUrl = "/Views/Store/Details.aspx?productId=" + item.ProductId;
            var removeFromCartUrl = "/Views/ShoppingCart/ViewCart.aspx?removeFromCart=" + item.ProductId;
            %>
    <tr id="row-<%=item.RecordId%>">
        <td>
            <a href="<%= productUrl %>">"<%= item.Product.Name %>"</a>
        </td>
        <td>
            <%= item.Product.Price %>
        </td>
        <td id="item-count-@item.RecordId">
            <%= item.Count %>
        </td>
        <td>
            <a href="<%= removeFromCartUrl %>" class="RemoveLink">Remove from cart</a>
        </td>
    </tr>
<%  } %>
    <tr>
        <td>
            Total
        </td>
        <td>
        </td>
        <td>
        </td>
        <td id="cart-total">
            <%= Model.CartTotal %>
        </td>
    </tr>
</table>



</asp:Content>
