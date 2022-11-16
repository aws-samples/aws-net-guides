<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddressAndPayment.aspx.cs" Inherits="GadgetsOnlineWebForms.Views.Checkout.AddressAndPayment" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

<form>  
    <h2>Address And Payment</h2>
    <fieldset>
        <legend>Shipping Information</legend>

            <div class="editor-label"><label for="FirstName">First Name</label></div>
            <div class="editor-field"><input class="text-box single-line" data-val="true" data-val-length="The field First Name must be a string with a maximum length of 160." data-val-length-max="160" data-val-required="First Name is required" id="FirstName" name="FirstName" type="text" value=""> <span class="field-validation-valid" data-valmsg-for="FirstName" data-valmsg-replace="true"></span></div>
            <div class="editor-label"><label for="LastName">Last Name</label></div>
            <div class="editor-field"><input class="text-box single-line" data-val="true" data-val-length="The field Last Name must be a string with a maximum length of 160." data-val-length-max="160" data-val-required="Last Name is required" id="LastName" name="LastName" type="text" value=""> <span class="field-validation-valid" data-valmsg-for="LastName" data-valmsg-replace="true"></span></div>
            <div class="editor-label"><label for="Address">Address</label></div>
            <div class="editor-field"><input class="text-box single-line" data-val="true" data-val-length="The field Address must be a string with a maximum length of 70." data-val-length-max="70" data-val-required="Address is required" id="Address" name="Address" type="text" value=""> <span class="field-validation-valid" data-valmsg-for="Address" data-valmsg-replace="true"></span></div>
            <div class="editor-label"><label for="City">City</label></div>
            <div class="editor-field"><input class="text-box single-line" data-val="true" data-val-length="The field City must be a string with a maximum length of 40." data-val-length-max="40" data-val-required="City is required" id="City" name="City" type="text" value=""> <span class="field-validation-valid" data-valmsg-for="City" data-valmsg-replace="true"></span></div>
            <div class="editor-label"><label for="State">State</label></div>
            <div class="editor-field"><input class="text-box single-line" data-val="true" data-val-length="The field State must be a string with a maximum length of 40." data-val-length-max="40" data-val-required="State is required" id="State" name="State" type="text" value=""> <span class="field-validation-valid" data-valmsg-for="State" data-valmsg-replace="true"></span></div>
            <div class="editor-label"><label for="PostalCode">Postal Code</label></div>
            <div class="editor-field"><input class="text-box single-line" data-val="true" data-val-length="The field Postal Code must be a string with a maximum length of 10." data-val-length-max="10" data-val-required="Postal Code is required" id="PostalCode" name="PostalCode" type="text" value=""> <span class="field-validation-valid" data-valmsg-for="PostalCode" data-valmsg-replace="true"></span></div>
            <div class="editor-label"><label for="Country">Country</label></div>
            <div class="editor-field"><input class="text-box single-line" data-val="true" data-val-length="The field Country must be a string with a maximum length of 40." data-val-length-max="40" data-val-required="Country is required" id="Country" name="Country" type="text" value=""> <span class="field-validation-valid" data-valmsg-for="Country" data-valmsg-replace="true"></span></div>
            <div class="editor-label"><label for="Phone">Phone</label></div>
            <div class="editor-field"><input class="text-box single-line" data-val="true" data-val-length="The field Phone must be a string with a maximum length of 24." data-val-length-max="24" data-val-required="Phone is required" id="Phone" name="Phone" type="text" value=""> <span class="field-validation-valid" data-valmsg-for="Phone" data-valmsg-replace="true"></span></div>
            <div class="editor-label"><label for="Email">Email Address</label></div>
            <div class="editor-field"><input class="text-box single-line" data-val="true" data-val-regex="Email is is not valid." data-val-regex-pattern="[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}" data-val-required="Email Address is required" id="Email" name="Email" type="email" value=""> <span class="field-validation-valid" data-valmsg-for="Email" data-valmsg-replace="true"></span></div>

    </fieldset>
    <fieldset>
        <legend>Payment</legend>
        <p>We're running a promotion: all music is free with the promo code: "FREE"</p>

        <div class="editor-label"><label for="PromoCode">Promo Code</label></div>
        <div class="editor-field"><input class="text-box single-line" id="PromoCode" name="PromoCode" type="text" value="" /></div>
    </fieldset>
    <asp:Button runat="server" ID="btnSubmit" Text="Submit Order" OnClick="btnSubmit_click" />
</form>

</asp:Content>
