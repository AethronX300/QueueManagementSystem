<%@ Page Title="Payment Simulation" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PaymentSim.aspx.cs" Inherits="QueueManagementSystem.PaymentSim" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <meta charset="utf-8" />
    <div class="page-header">
        <h1>&#128179; Payment Simulation</h1>
        <p>Simulate QR/UPI payment status for pending tokens</p>
    </div>
    <div class="payment-container">
        <div class="auth-card" style="max-width: 500px; margin: 0 auto;">
            <asp:Label ID="lblMessage" runat="server" Visible="false" />
            <div class="form-group">
                <label>Select Token with Pending Payment</label>
                <asp:DropDownList ID="ddlPendingTokens" runat="server" CssClass="form-control" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlPendingTokens_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
            <asp:Panel ID="pnlPayment" runat="server" Visible="false">
                <div style="text-align:center; padding: 20px;">
                    <div style="padding: 20px; text-align: center; border: 1px dashed rgba(212,175,55,0.4); border-radius: 12px; margin-bottom: 20px;">
                        <div>
                            <p style="font-weight:700; font-size:1rem; margin-bottom:8px;">&#128241; QR Code</p>
                            <p style="font-size:0.8rem; margin-bottom:12px;">Scan with your phone to track token status</p>
                            <asp:Image ID="imgQrCode" runat="server"
                                AlternateText="QR Code"
                                style="width:160px;height:160px;border-radius:8px;background:#fff;padding:4px;" />
                            <p style="font-size:0.75rem; margin-top:10px; color:var(--text-muted);">
                                Direct link: <asp:HyperLink ID="lnkTrackUrl" runat="server"
                                    Target="_blank"
                                    style="color:#D4AF37; word-break:break-all;" />
                            </p>
                        </div>
                    </div>
                    <p style="color: var(--text-muted); font-size: 0.9rem; margin-top: 12px;">
                        Token: <strong><asp:Label ID="lblPayToken" runat="server" /></strong> |
                        Customer: <strong><asp:Label ID="lblPayCustomer" runat="server" /></strong>
                    </p>
                    <div style="margin: 20px 0;">
                        <asp:Label ID="lblPaymentStatus" runat="server" CssClass="payment-status-display payment-pending" Text="Pending" />
                    </div>
                    <asp:Button ID="btnSimulatePayment" runat="server" Text="Simulate Payment (Mark as Paid)"
                        CssClass="btn btn-success" OnClick="btnSimulatePayment_Click" />
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlNoPending" runat="server" Visible="false" CssClass="empty-state">
                <i class="fas fa-check-circle"></i>
                <p>No pending payments found.</p>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
