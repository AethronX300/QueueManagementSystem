<%@ Page Title="Restaurant Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="QueueManagementSystem.Restaurant.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h1><i class="fas fa-utensils"></i> Restaurant Dashboard</h1>
        <p>Overview of today's orders and service status</p>
    </div>
    <div class="stats-grid">
        <div class="stat-card blue"><span class="stat-icon"><i class="fas fa-ticket-alt"></i></span><span class="stat-value"><asp:Label ID="lblTotalTokens" runat="server" Text="0" /></span><span class="stat-label">Total Orders Today</span></div>
        <div class="stat-card orange"><span class="stat-icon"><i class="fas fa-fire"></i></span><span class="stat-value"><asp:Label ID="lblActiveOrders" runat="server" Text="0" /></span><span class="stat-label">Active Orders</span></div>
        <div class="stat-card green"><span class="stat-icon"><i class="fas fa-check-circle"></i></span><span class="stat-value"><asp:Label ID="lblCompleted" runat="server" Text="0" /></span><span class="stat-label">Completed</span></div>
        <div class="stat-card purple"><span class="stat-icon"><i class="fas fa-rupee-sign"></i></span><span class="stat-value"><asp:Label ID="lblPaymentPending" runat="server" Text="0" /></span><span class="stat-label">Payment Pending</span></div>
    </div>
    <div class="quick-actions">
        <a href="NewToken.aspx" class="quick-action-btn"><i class="fas fa-plus-circle"></i> New Order Token</a>
        <a href="OrderStatus.aspx" class="quick-action-btn"><i class="fas fa-tasks"></i> View Order Status</a>
        <a href="../Reports.aspx" class="quick-action-btn"><i class="fas fa-chart-bar"></i> View Reports</a>
    </div>
    <div class="section-header"><h2>Recent Active Orders</h2></div>
    <asp:Repeater ID="rptActiveOrders" runat="server">
        <HeaderTemplate><div class="token-list"></HeaderTemplate>
        <ItemTemplate>
            <div class="token-item">
                <div class="token-item-header">
                    <span class="token-number"><%# Eval("TokenNumber") %></span>
                    <span class='<%# "status-badge status-" + GetStatusClass(Eval("CurrentStatus").ToString()) %>'><%# Eval("CurrentStatus") %></span>
                </div>
                <div class="token-item-body">
                    <div class="token-detail"><span class="label">Customer</span><span class="value"><%# Eval("CustomerName") %></span></div>
                    <div class="token-detail"><span class="label">Table</span><span class="value"><%# Eval("TableNumber") %></span></div>
                    <div class="token-detail"><span class="label">Time</span><span class="value"><%# Convert.ToDateTime(Eval("CreatedDate")).ToString("hh:mm tt") %></span></div>
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>
    <asp:Panel ID="pnlNoOrders" runat="server" Visible="false" CssClass="empty-state">
        <i class="fas fa-utensils"></i>
        <p>No active orders right now. Create a new order to get started!</p>
    </asp:Panel>
</asp:Content>
