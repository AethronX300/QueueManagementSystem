<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="QueueManagementSystem.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="auth-container">
        <div class="auth-card">

            <h2>Welcome Back</h2>
            <p class="auth-subtitle">
                <asp:Label ID="lblBusinessType" runat="server" Text="Sign in to manage your queue" />
            </p>

            <asp:Label ID="lblMessage" runat="server" Visible="false" />

            <div class="form-group">
                <label>Username</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter your username" />
                <asp:RequiredFieldValidator ID="rfvUser" runat="server" ControlToValidate="txtUsername"
                    ErrorMessage="Username is required" ForeColor="#F44336" Display="Dynamic" />
            </div>

            <div class="form-group">
                <label>Password</label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter your password" />
                <asp:RequiredFieldValidator ID="rfvPass" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage="Password is required" ForeColor="#F44336" Display="Dynamic" />
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Sign In" CssClass="btn btn-primary" OnClick="btnLogin_Click" />

            <br /><br />
            <p style="text-align:center; color: var(--text-sec); font-size: 0.95rem; font-weight: 500;">
                Don't have an account? <a href="Register.aspx" style="color: var(--gold); border-bottom: 1px dashed var(--gold-brd);">Register your business</a>
            </p>
            <p style="text-align:center; margin-top: 16px;">
                <a href="Default.aspx" style="color: var(--text-muted); font-size: 0.85rem; text-transform: uppercase; letter-spacing: 1px;">← Back to business selection</a>
            </p>

        </div>
    </div>

</asp:Content>
