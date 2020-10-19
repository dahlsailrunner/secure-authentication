<%@ Page Title="Company Members" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Members.aspx.cs" Inherits="Globomantics.Framework.Members" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <h3><% = Company.Name %></h3>
    <ul class="list-group">
    <% foreach (var member in Company.Members) { %>
            <li class="list-group-item"><%= member.MemberEmail %></li>
    <% } %>
    </ul>
</asp:Content>
