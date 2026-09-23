<%@ Page Title="Queue Display - Clinic" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QueueDisplay.aspx.cs" Inherits="QueueManagementSystem.Clinic.QueueDisplay" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h1><i class="fas fa-tv"></i> Clinic Queue Display &amp; Control</h1>
        <p>Manage patient queue - call next, update consultation status</p>
    </div>
    <div class="now-serving">
        <h2>Now Serving</h2>
        <div class="token-display"><asp:Label ID="lblNowServing" runat="server" Text="---" /></div>
        <div class="customer-name"><asp:Label ID="lblNowServingName" runat="server" Text="No patient in consultation" /></div>
    </div>
    <div class="quick-actions" style="justify-content: center; margin-bottom: 32px;">
        <asp:Button ID="btnCallNext" runat="server" Text="Call Next Patient" CssClass="btn btn-success" OnClick="btnCallNext_Click" />
        <asp:Button ID="btnCompleteConsultation" runat="server" Text="Complete Consultation" CssClass="btn btn-info" OnClick="btnCompleteConsultation_Click" />
    </div>
    <asp:Label ID="lblMessage" runat="server" Visible="false" />
    <div class="section-header"><h2>Today's Patients</h2></div>
    <asp:Repeater ID="rptPatients" runat="server">
        <HeaderTemplate><div class="token-list"></HeaderTemplate>
        <ItemTemplate>
            <div class="token-item">
                <div class="token-item-header">
                    <span class="token-number"><%# Eval("TokenNumber") %></span>
                    <span class='<%# "status-badge status-" + GetStatusClass(Eval("CurrentStatus").ToString()) %>'><%# Eval("CurrentStatus") %></span>
                </div>
                <div class="token-item-body">
                    <div class="token-detail"><span class="label">Patient</span><span class="value"><%# Eval("CustomerName") %></span></div>
                    <div class="token-detail"><span class="label">Symptoms</span><span class="value"><%# Eval("ServiceType") %></span></div>
                    <div class="token-detail"><span class="label">Registered</span><span class="value"><%# Convert.ToDateTime(Eval("CreatedDate")).ToString("hh:mm tt") %></span></div>
                    <div class="token-detail"><span class="label">Contact</span><span class="value"><%# Eval("ContactNumber") %></span></div>
                </div>
                <%# GetActionButtons(Convert.ToInt32(Eval("TokenId")), Eval("CurrentStatus").ToString()) %>
            </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>
    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty-state">
        <i class="fas fa-hospital-user"></i>
        <p>No patients registered today.</p>
    </asp:Panel>
</asp:Content>
