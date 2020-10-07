<%@ Page Language="C#" Title="Enable Authenticator" AutoEventWireup="true" MasterPageFile="~/Site.Master"  CodeBehind="EnableAuthenticator.aspx.cs" Inherits="Globomantics.Framework.Account.EnableAuthenticator" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    
    <h2><%: Title %></h2>
    
    <div>
    <p>To use an authenticator app go through the following steps:</p>
    <ol class="list">
        <li>
            <p>
                Download a two-factor authenticator app like Microsoft Authenticator for
                <a href="https://go.microsoft.com/fwlink/?Linkid=825072">Android</a> and
                <a href="https://go.microsoft.com/fwlink/?Linkid=825073">iOS</a> or
                Google Authenticator for
                <a href="https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2&amp;hl=en">Android</a> and
                <a href="https://itunes.apple.com/us/app/google-authenticator/id388497605?mt=8">iOS</a>.
            </p>
        </li>
        <li>
            <p>Scan the QR Code or enter this key <kbd><%: SharedKey %></kbd> into your two factor authenticator app. Spaces and casing do not matter.</p>
            <div id="qrCode"></div>
            <div id="qrCodeData" data-url="<%= AuthenticatorUri %>"></div>
        </li>
        <li>
            <p>
                Once you have scanned the QR code or input the key above, your two factor authentication app will provide you
                with a unique code. Enter the code in the confirmation box below.
            </p>
            <div class="row">
                <div class="col-md-6">
                    <div class="form">
                        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                            <p class="text-danger">
                                <asp:Literal runat="server" ID="FailureText" />
                            </p>
                        </asp:PlaceHolder>
                        <div class="form-group">
                            <asp:HiddenField runat="server" ID="SharedKeyField"/>
                            <asp:Label runat="server" AssociatedControlID="CodeVerification" CssClass="col-md-2 control-label">Verification Code</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="CodeVerification" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="CodeVerification"
                                                            CssClass="text-danger" ErrorMessage="The Verification Code field is required." />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-md-offset-2 col-md-10">
                                <asp:Button runat="server" OnClick="Verify" Text="Verify" CssClass="btn btn-default" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </li>
    </ol>
</div>
    
    <script type="text/javascript" src="/Scripts/qrcode.min.js"></script>
    <script type="text/javascript">
        new QRCode(document.getElementById("qrCode"),
            {
                text: "<%= AuthenticatorUri %>",
                width: 150,
                height: 150
            });
    </script>

</asp:Content>
