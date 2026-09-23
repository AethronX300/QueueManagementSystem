<%@ Page Title="Order Status - Restaurant" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrderStatus.aspx.cs" Inherits="QueueManagementSystem.Restaurant.OrderStatus" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h1><i class="fas fa-clipboard-list"></i> Order Status Board</h1>
        <p>Track and update the status of all active orders</p>
    </div>
    <asp:Repeater ID="rptOrders" runat="server" OnItemCommand="rptOrders_ItemCommand">
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
                    <div class="token-detail"><span class="label">Order Time</span><span class="value"><%# Convert.ToDateTime(Eval("CreatedDate")).ToString("hh:mm tt") %></span></div>
                    <div class="token-detail"><span class="label">Payment</span><span class="value"><%# Eval("PaymentStatus") %></span></div>
                </div>
                <div class="token-item-actions">
                    <asp:Button ID="btnPreparing" runat="server" Text="Start Preparing" CssClass="btn btn-warning btn-action" CommandName="UpdateStatus" CommandArgument='<%# Eval("TokenId") + "|Preparing" %>' Visible='<%# Eval("CurrentStatus").ToString() == "Order Placed" %>' />
                    <asp:Button ID="btnReady" runat="server" Text="Ready to Serve" CssClass="btn btn-success btn-action" CommandName="UpdateStatus" CommandArgument='<%# Eval("TokenId") + "|Ready to Serve" %>' Visible='<%# Eval("CurrentStatus").ToString() == "Preparing" %>' />
                    <asp:Button ID="btnServed" runat="server" Text="Served" CssClass="btn btn-info btn-action" CommandName="UpdateStatus" CommandArgument='<%# Eval("TokenId") + "|Served" %>' Visible='<%# Eval("CurrentStatus").ToString() == "Ready to Serve" %>' />
                    <asp:Button ID="btnPayment" runat="server" Text="Payment Done" CssClass="btn btn-primary btn-action" CommandName="UpdateStatus" CommandArgument='<%# Eval("TokenId") + "|Payment Completed" %>' Visible='<%# Eval("CurrentStatus").ToString() == "Served" %>' />
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>
    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty-state">
        <i class="fas fa-clipboard-list"></i>
        <p>No active orders at the moment.</p>
    </asp:Panel>
</asp:Content>
