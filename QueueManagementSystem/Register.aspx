<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="QueueManagementSystem.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="auth-container">
        <div class="auth-card">
            <h2>Register Your Business</h2>
            <p class="auth-subtitle">Create an account to start managing your queues</p>
            <asp:Label ID="lblMessage" runat="server" Visible="false" />
            <div class="form-group">
                <label>Business Type</label>
                <asp:DropDownList ID="ddlBusinessType" runat="server" CssClass="form-control">
                    <asp:ListItem Value="" Text="-- Select Business Type --" />
                    <asp:ListItem Value="1" Text="Restaurant" />
                    <asp:ListItem Value="2" Text="Clinic" />
                    <asp:ListItem Value="3" Text="Bank" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvBusinessType" runat="server" ControlToValidate="ddlBusinessType"
                    InitialValue="" ErrorMessage="Please select a business type" ForeColor="#F44336" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Business Name</label>
                <asp:TextBox ID="txtBusinessName" runat="server" CssClass="form-control" placeholder="e.g., Sunrise Restaurant" />
                <asp:RequiredFieldValidator ID="rfvBizName" runat="server" ControlToValidate="txtBusinessName"
                    ErrorMessage="Business name is required" ForeColor="#F44336" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Address</label>
                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Business address" />
            </div>
            <div class="form-group">
                <label>Contact Number</label>
                <asp:TextBox ID="txtContact" runat="server" CssClass="form-control" placeholder="Phone number" />
            </div>
            <div class="form-group">
                <label>Admin Full Name</label>
                <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Your full name" />
                <asp:RequiredFieldValidator ID="rfvFullName" runat="server" ControlToValidate="txtFullName"
                    ErrorMessage="Full name is required" ForeColor="#F44336" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Username</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Choose a username" />
                <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername"
                    ErrorMessage="Username is required" ForeColor="#F44336" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Password</label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Create a password" />
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage="Password is required" ForeColor="#F44336" Display="Dynamic" />
            </div>
            <asp:Button ID="btnRegister" runat="server" Text="Register Business" CssClass="btn btn-primary" OnClick="btnRegister_Click" />
            <br /><br />
            <p style="text-align:center; color: var(--text-sec); font-size: 0.95rem; font-weight: 500;">
                Already registered? <a href="Default.aspx">Go to Login</a>
            </p>
        </div>
    </div>
</asp:Content>
