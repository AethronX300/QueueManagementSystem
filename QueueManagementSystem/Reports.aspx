<%@ Page Title="Reports" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="QueueManagementSystem.Reports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h1><i class="fas fa-chart-bar"></i> Token History &amp; Reports</h1>
        <p>Search, filter and export token service history</p>
    </div>

    <!-- ===== FILTER BAR ===== -->
    <div style="background: var(--bg-card); border: 1px solid var(--border); border-radius: 14px; padding: 20px 24px; margin-bottom: 24px;">
        <div style="display: grid; grid-template-columns: 1fr 1fr 1fr 1fr; gap: 16px; margin-bottom: 16px;">
            <div>
                <label style="font-size:0.78rem; color:var(--text-muted); text-transform:uppercase; letter-spacing:1px;">From Date</label>
                <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date" style="margin-top:6px;" />
            </div>
            <div>
                <label style="font-size:0.78rem; color:var(--text-muted); text-transform:uppercase; letter-spacing:1px;">To Date</label>
                <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date" style="margin-top:6px;" />
            </div>
            <div>
                <label style="font-size:0.78rem; color:var(--text-muted); text-transform:uppercase; letter-spacing:1px;">Name / Token #</label>
                <asp:TextBox ID="txtNameSearch" runat="server" CssClass="form-control" placeholder="Search by name or token..." style="margin-top:6px;" />
            </div>
            <div>
                <label style="font-size:0.78rem; color:var(--text-muted); text-transform:uppercase; letter-spacing:1px;">Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" style="margin-top:6px;">
                    <asp:ListItem Value="All" Text="-- All Statuses --" />
                </asp:DropDownList>
            </div>
        </div>
        <div style="display:grid; grid-template-columns: 1fr auto auto; gap:12px; align-items:flex-end;">
            <div>
                <label style="font-size:0.78rem; color:var(--text-muted); text-transform:uppercase; letter-spacing:1px;">Service Type</label>
                <asp:TextBox ID="txtServiceType" runat="server" CssClass="form-control" placeholder="e.g. Deposit, Dine-In, General Consultation..." style="margin-top:6px;" />
            </div>
            <asp:Button ID="btnFilter" runat="server" Text="Apply Filters" CssClass="btn btn-primary" OnClick="btnFilter_Click" style="margin-top:auto; height:42px;" />
            <asp:Button ID="btnShowAll" runat="server" Text="Clear &amp; Show All" CssClass="btn" OnClick="btnShowAll_Click" style="margin-top:auto; height:42px; background:transparent; border:1px solid var(--border); color:var(--text-sec);" />
        </div>
    </div>

    <asp:Label ID="lblRecordCount" runat="server" CssClass="msg-success" Visible="false" style="display:block; margin-bottom:12px;" />

    <asp:GridView ID="gvReports" runat="server" CssClass="data-table" AutoGenerateColumns="false"
        EmptyDataText="No records found." AllowPaging="true" PageSize="15"
        OnPageIndexChanging="gvReports_PageIndexChanging"
        HeaderStyle-CssClass="grid-header" RowStyle-CssClass="grid-row" AlternatingRowStyle-CssClass="grid-alt-row">
        <Columns>
            <asp:BoundField DataField="TokenNumber" HeaderText="Token #" />
            <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
            <asp:BoundField DataField="ContactNumber" HeaderText="Contact" />
            <asp:BoundField DataField="ServiceType" HeaderText="Service Type" />
            <asp:BoundField DataField="TableNumber" HeaderText="Table / Ref" />
            <asp:BoundField DataField="CurrentStatus" HeaderText="Status" />
            <asp:BoundField DataField="PaymentStatus" HeaderText="Payment" />
            <asp:BoundField DataField="CreatedDate" HeaderText="Created" DataFormatString="{0:dd-MMM-yyyy hh:mm tt}" />
            <asp:BoundField DataField="CompletedDate" HeaderText="Completed" DataFormatString="{0:dd-MMM-yyyy hh:mm tt}" />
        </Columns>
        <PagerStyle CssClass="grid-pager" HorizontalAlign="Center" />
    </asp:GridView>

    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty-state">
        <i class="fas fa-inbox"></i>
        <p>No records match your filters.</p>
    </asp:Panel>
</asp:Content>
