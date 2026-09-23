<%@ Page Title="Queue Display - Bank" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QueueDisplay.aspx.cs" Inherits="QueueManagementSystem.Bank.QueueDisplay" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h1><i class="fas fa-tv"></i> Bank Queue Display</h1>
        <p>Counter-wise token management</p>
    </div>
    <asp:Label ID="lblMessage" runat="server" Visible="false" />
    <asp:Repeater ID="rptCounters" runat="server" OnItemCommand="rptCounters_ItemCommand">
        <HeaderTemplate><div class="counter-grid"></HeaderTemplate>
        <ItemTemplate>
            <div class="counter-card">
                <div class="counter-card-header"><h3><i class="fas fa-desktop"></i> <%# Eval("CounterName") %></h3></div>
                <div class="counter-card-body">
                    <div class="counter-current-token">
                        <p style="color: var(--text-sec); font-size: 0.85rem; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 4px;">Now Serving</p>
                        <span class="counter-token-num"><%# GetCurrentToken(Eval("CounterId")) %></span>
                        <p style="color: var(--text); font-weight: 500; font-size: 0.95rem; margin-top: 4px;"><%# GetCurrentCustomer(Eval("CounterId")) %></p>
                    </div>
                    <div style="margin-bottom: 16px; padding: 0 4px;">
                        <p style="color: var(--text-muted); font-size: 0.85rem; font-weight: 600;">Waiting Next: <span style="color: var(--gold);"><%# GetWaitingCount(Eval("CounterId")) %></span></p>
                    </div>
                    <div class="token-item-actions">
                        <asp:Button ID="btnCallNext" runat="server" Text="Call Next" CssClass="btn btn-primary btn-action" CommandName="CallNext" CommandArgument='<%# Eval("CounterId") %>' style="flex: 1;" />
                        <asp:Button ID="btnComplete" runat="server" Text="Complete" CssClass="btn btn-success btn-action" CommandName="Complete" CommandArgument='<%# Eval("CounterId") %>' style="flex: 1;" />
                    </div>
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>
    <div class="section-header" style="margin-top: 32px;"><h2>Waiting Queue</h2></div>
    <asp:Repeater ID="rptWaitingQueue" runat="server">
        <HeaderTemplate><div class="waiting-list"></HeaderTemplate>
        <ItemTemplate>
            <div class="waiting-item">
                <span class="waiting-token" style="background: rgba(212,175,55,0.1); padding: 4px 10px; border-radius: 6px; border: 1px solid var(--gold-brd);"><%# Eval("TokenNumber") %></span>
                <span class="waiting-name" style="margin-left: 8px;"><%# Eval("CustomerName") %></span>
                <span style="color: var(--text-sec); font-size: 0.85rem;"><%# Eval("ServiceType") %></span>
                <span class="status-badge" style="background: rgba(33,150,243,0.1); color: var(--blue); border: 1px solid rgba(33,150,243,0.25);"><%# Eval("CounterName") %></span>
            </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>
    <asp:Panel ID="pnlNoWaiting" runat="server" Visible="false" CssClass="empty-state">
        <i class="fas fa-check-circle"></i>
        <p>No customers waiting in queue.</p>
    </asp:Panel>
</asp:Content>
