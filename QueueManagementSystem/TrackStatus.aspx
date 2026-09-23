<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrackStatus.aspx.cs" Inherits="QueueManagementSystem.TrackStatus" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Track Your Token - QueueFlow</title>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    <style>
        /* ===== BLACK & GOLD THEME ===== */
        :root {
            --bg:        #080808;
            --bg-card:   #111111;
            --bg-input:  #1a1a1a;
            --gold:      #D4AF37;
            --gold-lt:   #FFD700;
            --gold-dk:   #A08825;
            --gold-glow: rgba(212, 175, 55, 0.18);
            --gold-brd:  rgba(212, 175, 55, 0.30);
            --text:      #F5F5F5;
            --muted:     #777777;
            --success:   #27ae60;
            --danger:    #c0392b;
            --radius:    14px;
            --radius-sm: 8px;
        }

        *, *::before, *::after { margin:0; padding:0; box-sizing:border-box; }

        body {
            font-family: 'Inter', sans-serif;
            background: var(--bg);
            color: var(--text);
            min-height: 100vh;
            display: flex;
            flex-direction: column;
            align-items: center;
        }

        /* ---- HEADER ---- */
        .track-header {
            width: 100%;
            padding: 18px 20px;
            background: #0d0d0d;
            border-bottom: 1px solid var(--gold-brd);
            text-align: center;
        }
        .track-header h1 {
            font-size: 1.15rem;
            font-weight: 700;
            color: var(--gold);
            letter-spacing: 0.5px;
        }
        .track-header h1 i { margin-right: 8px; }

        /* ---- CONTAINER ---- */
        .track-container {
            width: 100%;
            max-width: 460px;
            padding: 24px 16px 40px;
            display: flex;
            flex-direction: column;
            gap: 18px;
        }

        /* ---- CARD BASE ---- */
        .card {
            background: var(--bg-card);
            border: 1px solid var(--gold-brd);
            border-radius: var(--radius);
            overflow: hidden;
        }

        /* ---- TOKEN CARD ---- */
        .token-card {
            padding: 28px 24px 22px;
            text-align: center;
            background: linear-gradient(160deg, #131106 0%, var(--bg-card) 60%);
        }
        .token-card .token-label {
            font-size: 0.7rem;
            color: var(--muted);
            text-transform: uppercase;
            letter-spacing: 3px;
            margin-bottom: 10px;
        }
        .token-card .token-number {
            font-size: 3.2rem;
            font-weight: 800;
            background: linear-gradient(135deg, var(--gold-lt), var(--gold));
            -webkit-background-clip: text;
            background-clip: text;
            -webkit-text-fill-color: transparent;
            letter-spacing: 3px;
        }
        .token-card .business-badge {
            display: inline-block;
            margin-top: 12px;
            padding: 4px 16px;
            border-radius: 20px;
            font-size: 0.72rem;
            font-weight: 700;
            letter-spacing: 1px;
            text-transform: uppercase;
            background: var(--gold-glow);
            color: var(--gold);
            border: 1px solid var(--gold-brd);
        }

        /* ---- STATUS ROWS ---- */
        .status-section { padding: 6px 0; }
        .status-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 13px 20px;
            border-bottom: 1px solid rgba(255,255,255,0.05);
        }
        .status-row:last-child { border-bottom: none; }
        .status-row .lbl { font-size: 0.82rem; color: var(--muted); }
        .status-row .val { font-size: 0.92rem; font-weight: 600; color: var(--text); }

        /* Status badges */
        .sbadge {
            display: inline-block;
            padding: 4px 13px;
            border-radius: 20px;
            font-size: 0.72rem;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        .st-waiting  { background:rgba(212,175,55,.12); color:#FFD700; border:1px solid rgba(212,175,55,.3); }
        .st-preparing{ background:rgba(155,89,182,.15); color:#c39bd3; border:1px solid rgba(155,89,182,.3); }
        .st-ready    { background:rgba(39,174,96,.15);  color:#2ecc71; border:1px solid rgba(39,174,96,.3); }
        .st-serving  { background:rgba(41,128,185,.15); color:#5dade2; border:1px solid rgba(41,128,185,.3); }
        .st-completed{ background:rgba(39,174,96,.2);   color:#1abc9c; border:1px solid rgba(39,174,96,.4); }

        /* ---- MESSAGE BOX ---- */
        .message-box {
            padding: 22px 20px;
            text-align: center;
        }
        .msg-icon { font-size: 2rem; color: var(--gold); margin-bottom: 10px; display: block; }
        .msg-text { font-size: 0.95rem; color: #aaa; line-height: 1.6; }

        /* ============================
           PAYMENT SECTION
           ============================ */
        .payment-section { overflow: hidden; }

        .pay-header {
            background: linear-gradient(135deg, #1a1500, #111);
            padding: 14px 20px;
            border-bottom: 1px solid var(--gold-brd);
            display: flex;
            align-items: center;
            gap: 10px;
        }
        .pay-header i { color: var(--gold); font-size: 1.1rem; }
        .pay-header span { font-size: 0.95rem; font-weight: 700; color: var(--gold); }

        /* Payment status bar */
        .pay-status-bar {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 12px 20px;
            background: rgba(212,175,55,0.05);
            border-bottom: 1px solid rgba(255,255,255,0.05);
            font-size: 0.85rem;
            color: var(--muted);
        }

        /* QR code area */
        .qr-area {
            padding: 20px;
            text-align: center;
        }
        .qr-frame {
            display: inline-block;
            padding: 10px;
            background: #fff;
            border-radius: 10px;
            border: 3px solid var(--gold);
        }
        .qr-frame img { display: block; width: 160px; height: 160px; }
        .qr-hint {
            margin-top: 10px;
            font-size: 0.78rem;
            color: var(--muted);
        }
        .qr-upi-id {
            margin-top: 4px;
            font-size: 0.8rem;
            color: var(--gold);
            font-weight: 600;
        }

        /* Payment app buttons */
        .pay-apps {
            display: grid;
            grid-template-columns: 1fr 1fr 1fr;
            gap: 10px;
            padding: 0 20px 16px;
        }
        .pay-app-btn {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            gap: 6px;
            padding: 12px 8px;
            border-radius: var(--radius-sm);
            font-size: 0.7rem;
            font-weight: 700;
            text-decoration: none;
            cursor: pointer;
            transition: transform 0.15s, box-shadow 0.15s;
            border: 1px solid rgba(255,255,255,0.08);
        }
        .pay-app-btn i { font-size: 1.4rem; }
        .pay-app-btn:hover { transform: translateY(-2px); box-shadow: 0 6px 20px rgba(0,0,0,0.4); }

        .gpay    { background:#1a1f2b; color:#fff; }
        .gpay i  { color:#4285F4; }
        .phonepe { background:#1a1525; color:#fff; }
        .phonepe i{ color:#7B2FBE; }
        .paytm   { background:#00203F; color:#fff; }
        .paytm i { color:#00BCF2; }

        /* Divider */
        .pay-divider {
            text-align: center;
            padding: 4px 20px 12px;
            font-size: 0.75rem;
            color: var(--muted);
        }

        /* Confirm payment button */
        .btn-confirm {
            display: block;
            width: calc(100% - 40px);
            margin: 0 20px 20px;
            padding: 14px;
            background: linear-gradient(135deg, var(--gold), var(--gold-dk));
            color: #000;
            font-family: 'Inter', sans-serif;
            font-size: 0.95rem;
            font-weight: 800;
            border: none;
            border-radius: var(--radius-sm);
            cursor: pointer;
            letter-spacing: 0.5px;
            transition: opacity 0.2s, transform 0.2s;
        }
        .btn-confirm:hover { opacity: 0.9; transform: translateY(-1px); }

        /* Payment done */
        .pay-done {
            padding: 28px 20px;
            text-align: center;
        }
        .pay-done i { font-size: 2.5rem; color: var(--success); display: block; margin-bottom: 10px; }
        .pay-done .done-title { font-size: 1rem; font-weight: 700; color: var(--success); }
        .pay-done .done-sub   { font-size: 0.85rem; color: var(--muted); margin-top: 4px; }

        /* ---- PAY BADGE ---- */
        .pay-badge {
            display: inline-block;
            padding: 3px 12px;
            border-radius: 20px;
            font-size: 0.72rem;
            font-weight: 700;
            text-transform: uppercase;
        }
        .pay-pending  { background:rgba(212,175,55,.12); color:var(--gold);   border:1px solid var(--gold-brd); }
        .pay-paid     { background:rgba(39,174,96,.15);  color:#2ecc71;       border:1px solid rgba(39,174,96,.3); }

        /* ---- ERROR CARD ---- */
        .error-card {
            margin-top: 40px;
            padding: 48px 24px;
            text-align: center;
            background: var(--bg-card);
            border: 1px solid rgba(192,57,43,0.35);
            border-radius: var(--radius);
        }
        .error-card i      { font-size: 2.8rem; color: #e74c3c; margin-bottom: 14px; display: block; }
        .error-card .etitle{ font-size: 1.1rem; font-weight: 700; color: #e74c3c; margin-bottom: 6px; }
        .error-card .emsg  { font-size: 0.88rem; color: var(--muted); }

        /* ---- FOOTER ---- */
        .track-footer {
            width: 100%;
            text-align: center;
            padding: 16px;
            color: #444;
            font-size: 0.78rem;
            border-top: 1px solid #1a1a1a;
            margin-top: auto;
        }

        @media (min-width: 600px) {
            .track-container { padding-top: 36px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <!-- ===== HEADER ===== -->
        <div class="track-header">
            <h1><i class="fas fa-layer-group"></i> QueueFlow &mdash; Track &amp; Pay</h1>
        </div>

        <!-- ===== VALID TOKEN VIEW ===== -->
        <asp:Panel ID="pnlTokenInfo" runat="server" Visible="false" CssClass="track-container">

            <!-- Token Number Card -->
            <div class="card token-card">
                <div class="token-label">Your Token Number</div>
                <div class="token-number">
                    <asp:Label ID="lblTokenNumber" runat="server" />
                </div>
                <asp:Label ID="lblBusinessType" runat="server" CssClass="business-badge" />
            </div>

            <!-- Status Details -->
            <div class="card status-section">
                <div class="status-row">
                    <span class="lbl">Customer Name</span>
                    <span class="val"><asp:Label ID="lblCustomerName" runat="server" /></span>
                </div>
                <div class="status-row" id="rowReason" runat="server" visible="false">
                    <span class="lbl">Reason for Visiting</span>
                    <span class="val"><asp:Label ID="lblReason" runat="server" /></span>
                </div>
                <div class="status-row">
                    <span class="lbl">Current Status</span>
                    <span class="val"><asp:Label ID="lblStatus" runat="server" CssClass="sbadge" /></span>
                </div>
                <div class="status-row">
                    <span class="lbl">Queue Position</span>
                    <span class="val"><asp:Label ID="lblPosition" runat="server" /></span>
                </div>
                <div class="status-row">
                    <span class="lbl">Estimated Wait</span>
                    <span class="val"><asp:Label ID="lblWaitTime" runat="server" /></span>
                </div>
            </div>

            <!-- Status Message -->
            <div class="card">
                <div class="message-box">
                    <span class="msg-icon"><asp:Label ID="lblMsgIcon" runat="server" /></span>
                    <div class="msg-text"><asp:Label ID="lblMessage" runat="server" /></div>
                </div>
            </div>

            <!-- ===== PAYMENT SECTION ===== -->
            <asp:Panel ID="pnlPaymentSection" runat="server">
                <!-- Pending Payment Panel -->
                <asp:Panel ID="pnlPayment" runat="server" Visible="false" CssClass="card payment-section">

                <div class="pay-header">
                    <i class="fas fa-coins"></i>
                    <span>Complete Your Payment</span>
                </div>

                <div class="pay-status-bar">
                    <span>Payment Status</span>
                    <asp:Label ID="lblPaymentStatus" runat="server" CssClass="pay-badge pay-pending" Text="Pending" />
                </div>

                <!-- QR Code -->
                <div class="qr-area">
                    <div class="qr-frame">
                        <asp:Image ID="imgQR" runat="server" AlternateText="UPI QR Code" />
                    </div>
                    <div class="qr-hint"><i class="fas fa-qrcode"></i> Scan with any UPI app to pay</div>
                    <div class="qr-upi-id">UPI ID: queueflow@paytm</div>
                </div>

                <!-- Payment App Buttons -->
                <div class="pay-apps">
                    <asp:HyperLink ID="lnkGPay" runat="server" CssClass="pay-app-btn gpay" Target="_blank">
                        <i class="fab fa-google-pay"></i>
                        <span>GPay</span>
                    </asp:HyperLink>
                    <asp:HyperLink ID="lnkPhonePe" runat="server" CssClass="pay-app-btn phonepe" Target="_blank">
                        <i class="fas fa-mobile-alt"></i>
                        <span>PhonePe</span>
                    </asp:HyperLink>
                    <asp:HyperLink ID="lnkPaytm" runat="server" CssClass="pay-app-btn paytm" Target="_blank">
                        <i class="fas fa-wallet"></i>
                        <span>Paytm</span>
                    </asp:HyperLink>
                </div>

                <div class="pay-divider">
                    &mdash; After paying, tap the button below &mdash;
                </div>

                <!-- Confirm Button -->
                <asp:Button ID="btnConfirmPayment" runat="server" Text="Confirm Payment"
                    CssClass="btn-confirm" OnClick="btnConfirmPayment_Click" />

            </asp:Panel>

            <!-- Payment Done Panel -->
            <asp:Panel ID="pnlPaymentDone" runat="server" Visible="false" CssClass="card">
                <div class="pay-done">
                    <i class="fas fa-check-circle"></i>
                    <div class="done-title">Payment Confirmed!</div>
                    <div class="done-sub">Thank you. Your payment has been recorded.</div>
                </div>
            </asp:Panel>
            </asp:Panel>

        </asp:Panel>

        <!-- ===== ERROR VIEW ===== -->
        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="track-container">
            <div class="error-card">
                <i class="fas fa-exclamation-triangle"></i>
                <div class="etitle">Invalid or Expired Token</div>
                <div class="emsg">The token you are looking for does not exist or has expired. Please check the link and try again.</div>
            </div>
        </asp:Panel>

    </form>

    <!-- Footer -->
    <div class="track-footer">
        &copy; <%= DateTime.Now.Year %> QueueFlow &mdash; Scalable Queue Management System
    </div>
</body>
</html>
