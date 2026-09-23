<%@ Page Title="New Order - Restaurant" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewToken.aspx.cs" Inherits="QueueManagementSystem.Restaurant.NewToken" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h1><i class="fas fa-ticket-alt"></i> New Order Token</h1>
        <p>Generate a table token — each table gets its own QR code for self-tracking</p>
    </div>
    <div style="display: flex; gap: 32px; align-items: flex-start; flex-wrap: wrap;">

        <!-- ===== FORM CARD ===== -->
        <div class="auth-container" style="min-height: auto; padding-top: 0; flex: 1; min-width: 300px;">
            <div class="auth-card" style="max-width: 520px; margin: 0;">
                <asp:Label ID="lblMessage" runat="server" Visible="false" />
                <h2 style="font-size:1.2rem; margin-bottom: 24px;"><i class="fas fa-utensils"></i> Order Details</h2>
                <div class="form-group">
                    <label>Customer Name</label>
                    <asp:TextBox ID="txtCustomerName" runat="server" CssClass="form-control" placeholder="e.g. Rahul Sharma" />
                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtCustomerName" ErrorMessage="Customer name is required" ForeColor="#F44336" Display="Dynamic" />
                </div>
                <div class="form-group">
                    <label>Table Number</label>
                    <asp:TextBox ID="txtTableNumber" runat="server" CssClass="form-control" placeholder="e.g. 2, 5, 10" />
                    <asp:RequiredFieldValidator ID="rfvTable" runat="server" ControlToValidate="txtTableNumber" ErrorMessage="Table number is required" ForeColor="#F44336" Display="Dynamic" />
                    <small style="color: var(--text-muted); font-size: 0.8rem; margin-top: 4px; display:block;">
                        <i class="fas fa-info-circle"></i> Token will be: T{Table}-{Count} e.g. <strong style="color:var(--gold);">T2-03</strong>
                    </small>
                </div>
                <div class="form-group">
                    <label>Contact Number <span style="color:var(--text-muted); font-size:0.85em;">(Optional)</span></label>
                    <asp:TextBox ID="txtContact" runat="server" CssClass="form-control" placeholder="Phone number" />
                </div>
                <asp:Button ID="btnGenerateToken" runat="server" Text="Generate Table Token" CssClass="btn btn-primary" OnClick="btnGenerateToken_Click" style="width:100%;" />
                <br /><br />
                <p style="text-align:center;">
                    <a href="OrderStatus.aspx" style="color: var(--text-sec); font-size: 0.9rem; font-weight: 600; text-transform: uppercase;"><i class="fas fa-tasks"></i> View Order Status Board</a>
                </p>
            </div>
        </div>

        <!-- ===== QR PANEL (shown after token creation) ===== -->
        <asp:Panel ID="pnlTokenGenerated" runat="server" Visible="false">
            <div style="background: linear-gradient(160deg, #111 0%, var(--bg-card) 100%); border: 1px solid var(--gold-brd); border-radius: 16px; padding: 28px; min-width: 300px; text-align: center; box-shadow: 0 0 30px rgba(212,175,55,0.1);">
                <p style="color: var(--gold); font-size: 0.75rem; font-weight: 700; letter-spacing: 2px; text-transform: uppercase; margin-bottom: 8px;">
                    <i class="fas fa-check-circle"></i> Token Generated
                </p>
                <!-- Big token display -->
                <div class="token-display" style="font-size: 3rem; margin: 8px 0;">
                    <asp:Label ID="lblTokenNumber" runat="server" />
                </div>
                <div style="color: var(--text); font-weight: 600; margin-bottom: 4px;">
                    <asp:Label ID="lblConfirmName" runat="server" />
                </div>
                <hr style="border-color: var(--border); margin: 20px 0;" />

                <!-- QR CODE -->
                <p style="color: var(--text-muted); font-size: 0.8rem; letter-spacing: 1px; text-transform: uppercase; margin-bottom: 12px;">
                    <i class="fas fa-qrcode"></i> Scan to Track Order
                </p>
                <div style="background: #fff; border-radius: 12px; padding: 12px; display: inline-block; box-shadow: 0 4px 20px rgba(212,175,55,0.2);">
                    <asp:Image ID="imgQR" runat="server" style="width: 180px; height: 180px; display: block;" />
                </div>
                <p style="margin-top: 12px; font-size: 0.78rem; color: var(--text-muted);">
                    Place this on the table. Customer scans to see live order status.
                </p>

                <!-- Shareable link -->
                <div style="background: rgba(212,175,55,0.06); border: 1px dashed var(--gold-brd); border-radius: 8px; padding: 10px 14px; margin-top: 14px; word-break: break-all; font-size: 0.78rem;">
                    <span style="color: var(--text-muted);">Link: </span>
                    <asp:Label ID="lblTrackURL" runat="server" style="color: var(--gold);" />
                </div>

                <asp:Button ID="btnNewToken" runat="server" Text="Generate Another" CssClass="btn" style="margin-top: 18px; background: transparent; border: 1px solid var(--gold-brd); color: var(--gold); width:100%;" OnClick="btnNewToken_Click" />
            </div>
        </asp:Panel>

    </div>

    <!-- HOW IT WORKS -->
    <div style="margin-top: 40px; padding: 24px; background: rgba(255,255,255,0.02); border-radius: 14px; border: 1px solid var(--border);">
        <h3 style="font-size: 1rem; color: var(--gold); margin-bottom: 16px;"><i class="fas fa-lightbulb"></i> How Table QR Works</h3>
        <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 16px;">
            <div style="text-align:center; padding: 12px;">
                <i class="fas fa-ticket-alt" style="color:var(--gold); font-size:1.5rem; margin-bottom: 8px;"></i>
                <p style="font-size:0.85rem; color:var(--text-sec);">Admin generates a table token (e.g., <strong style="color:var(--text);">T2-03</strong> = 3rd customer at Table 2 today)</p>
            </div>
            <div style="text-align:center; padding: 12px;">
                <i class="fas fa-qrcode" style="color:var(--gold); font-size:1.5rem; margin-bottom: 8px;"></i>
                <p style="font-size:0.85rem; color:var(--text-sec);">Print/show this QR on table or give to customer on their bill receipt</p>
            </div>
            <div style="text-align:center; padding: 12px;">
                <i class="fas fa-mobile-alt" style="color:var(--gold); font-size:1.5rem; margin-bottom: 8px;"></i>
                <p style="font-size:0.85rem; color:var(--text-sec);">Customer scans QR → sees live order status without asking staff</p>
            </div>
        </div>
    </div>
</asp:Content>
