<%@ Page Title="Clinic Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="QueueManagementSystem.Clinic.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h1><i class="fas fa-stethoscope"></i> Clinic Dashboard</h1>
        <p>Patient queue overview and status</p>
    </div>
    <div class="now-serving">
        <h2>Now Serving</h2>
        <div class="token-display"><asp:Label ID="lblNowServing" runat="server" Text="---" /></div>
        <div class="customer-name"><asp:Label ID="lblNowServingName" runat="server" Text="No patient in consultation" /></div>
    </div>
    <div class="stats-grid">
        <div class="stat-card blue"><span class="stat-icon"><i class="fas fa-ticket-alt"></i></span><span class="stat-value"><asp:Label ID="lblTotalTokens" runat="server" Text="0" /></span><span class="stat-label">Total Patients Today</span></div>
        <div class="stat-card orange"><span class="stat-icon"><i class="fas fa-hourglass-half"></i></span><span class="stat-value"><asp:Label ID="lblWaiting" runat="server" Text="0" /></span><span class="stat-label">Waiting</span></div>
        <div class="stat-card green"><span class="stat-icon"><i class="fas fa-check-circle"></i></span><span class="stat-value"><asp:Label ID="lblCompleted" runat="server" Text="0" /></span><span class="stat-label">Completed</span></div>
        <div class="stat-card purple"><span class="stat-icon"><i class="fas fa-stethoscope"></i></span><span class="stat-value"><asp:Label ID="lblInConsultation" runat="server" Text="0" /></span><span class="stat-label">In Consultation</span></div>
    </div>
    <div class="quick-actions">
        <a href="NewToken.aspx" class="quick-action-btn"><i class="fas fa-user-plus"></i> Register New Patient</a>
        <a href="QueueDisplay.aspx" class="quick-action-btn"><i class="fas fa-tv"></i> Queue Display Board</a>
        <a href="../Reports.aspx" class="quick-action-btn"><i class="fas fa-chart-bar"></i> View Reports</a>
    </div>
    <div class="section-header"><h2>Waiting List</h2></div>
    <div class="waiting-list">
        <asp:Repeater ID="rptWaiting" runat="server">
            <ItemTemplate>
                <div class="waiting-item">
                    <span class="waiting-token"><%# Eval("TokenNumber") %></span>
                    <span class="waiting-name"><%# Eval("CustomerName") %></span>
                    <span style="color: var(--text-sec); font-size: 0.85rem;"><%# Convert.ToDateTime(Eval("CreatedDate")).ToString("hh:mm tt") %></span>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Panel ID="pnlNoWaiting" runat="server" Visible="false">
            <p style="text-align:center; color: var(--text-sec); padding: 20px;">No patients waiting</p>
        </asp:Panel>
    </div>
</asp:Content>
