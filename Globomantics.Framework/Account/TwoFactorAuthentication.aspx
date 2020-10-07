<%@ Page Title="Two-Factor Authentication" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TwoFactorAuthentication.aspx.cs" Inherits="Globomantics.Framework.Account.TwoFactorAuthentication" %>


<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<h2><%: Title %>.</h2>
    <% if (!HasAuthenticator)
       { %>
    <asp:HyperLink runat="server" NavigateUrl="/Account/EnableAuthenticator" Text="Enable Authenticator" ></asp:HyperLink>
        <% }
       else
       { %>
        <asp:HyperLink runat="server" NavigateUrl="/Account/DisableAuthenticator" Text="Disable Authenticator" ></asp:HyperLink>
        <% } %>
</asp:Content>
