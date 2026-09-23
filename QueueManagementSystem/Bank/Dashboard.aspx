<%@ Page Title="Bank Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="QueueManagementSystem.Bank.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h1><i class="fas fa-university"></i> Bank Dashboard</h1>
        <p>Service counter and queue overview</p>
    </div>
    <div class="stats-grid">
        <div class="stat-card blue"><span class="stat-icon"><i class="fas fa-ticket-alt"></i></span><span class="stat-value"><asp:Label ID="lblTotalTokens" runat="server" Text="0" /></span><span class="stat-label">Total Tokens Today</span></div>
        <div class="stat-card orange"><span class="stat-icon"><i class="fas fa-hourglass-half"></i></span><span class="stat-value"><asp:Label ID="lblWaiting" runat="server" Text="0" /></span><span class="stat-label">Waiting</span></div>
        <div class="stat-card green"><span class="stat-icon"><i class="fas fa-check-circle"></i></span><span class="stat-value"><asp:Label ID="lblCompleted" runat="server" Text="0" /></span><span class="stat-label">Completed</span></div>
        <div class="stat-card purple"><span class="stat-icon"><i class="fas fa-desktop"></i></span><span class="stat-value"><asp:Label ID="lblActiveCounters" runat="server" Text="0" /></span><span class="stat-label">Active Counters</span></div>
    </div>
    <div class="quick-actions">
        <a href="NewToken.aspx" class="quick-action-btn"><i class="fas fa-plus-circle"></i> Generate New Token</a>
        <a href="QueueDisplay.aspx" class="quick-action-btn"><i class="fas fa-tv"></i> Queue Display Board</a>
        <a href="../Reports.aspx" class="quick-action-btn"><i class="fas fa-chart-bar"></i> View Reports</a>
    </div>
    <div class="section-header"><h2>Counter Status</h2></div>
    <asp:Repeater ID="rptCounters" runat="server">
        <HeaderTemplate><div class="counter-grid"></HeaderTemplate>
        <ItemTemplate>
            <div class="counter-card">
                <div class="counter-card-header">
                    <h3><i class="fas fa-desktop"></i> <%# Eval("CounterName") %></h3>
                    <span class="status-badge <%# Convert.ToBoolean(Eval("IsActive")) ? "status-active" : "status-waiting" %>"><%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %></span>
                </div>
                <div class="counter-card-body">
                    <div class="counter-current-token">
                        <p style="color: var(--text-sec); font-size: 0.85rem; letter-spacing: 1px; text-transform: uppercase; margin-bottom: 4px;">Currently Serving</p>
                        <span class="counter-token-num"><%# GetCurrentTokenForCounter(Eval("CounterId")) %></span>
                    </div>
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>
</asp:Content>
