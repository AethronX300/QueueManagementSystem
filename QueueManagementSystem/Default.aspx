<%@ Page Title="Welcome" Language="C#" MasterPageFile="~/Site.Master"
AutoEventWireup="true" CodeBehind="Default.aspx.cs"
Inherits="QueueManagementSystem.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <div class="landing-container">
    <div class="landing-title">
      <h1>Queue Management System</h1>
      <p>A scalable platform for managing queues and tokens across multiple business types. Select your business to get started.</p>
    </div>
    <p class="landing-subtitle">Choose your business type below</p>

    <!-- ===== ALL BUSINESS CARDS — 7 per row ===== -->
    <div class="biz-grid">

      <!-- ACTIVE -->
      <asp:LinkButton ID="btnRestaurant" runat="server" CssClass="biz-card biz-active" OnClick="btnRestaurant_Click">
        <span class="biz-icon"><i class="fas fa-utensils"></i></span>
        <span class="biz-name">Restaurant</span>
      </asp:LinkButton>

      <asp:LinkButton ID="btnClinic" runat="server" CssClass="biz-card biz-active" OnClick="btnClinic_Click">
        <span class="biz-icon"><i class="fas fa-stethoscope"></i></span>
        <span class="biz-name">Clinic</span>
      </asp:LinkButton>

      <asp:LinkButton ID="btnBank" runat="server" CssClass="biz-card biz-active" OnClick="btnBank_Click">
        <span class="biz-icon"><i class="fas fa-university"></i></span>
        <span class="biz-name">Bank</span>
      </asp:LinkButton>

      <!-- COMING SOON -->
      <div class="biz-card biz-soon">
        <span class="soon-badge">Soon</span>
        <span class="biz-icon"><i class="fas fa-plane-departure"></i></span>
        <span class="biz-name">Airport</span>
      </div>

      <div class="biz-card biz-soon">
        <span class="soon-badge">Soon</span>
        <span class="biz-icon"><i class="fas fa-hospital-alt"></i></span>
        <span class="biz-name">Hospital</span>
      </div>

      <div class="biz-card biz-soon">
        <span class="soon-badge">Soon</span>
        <span class="biz-icon"><i class="fas fa-passport"></i></span>
        <span class="biz-name">Govt. Office</span>
      </div>

      <div class="biz-card biz-soon">
        <span class="soon-badge">Soon</span>
        <span class="biz-icon"><i class="fas fa-shopping-cart"></i></span>
        <span class="biz-name">Supermarket</span>
      </div>

      <div class="biz-card biz-soon">
        <span class="soon-badge">Soon</span>
        <span class="biz-icon"><i class="fas fa-graduation-cap"></i></span>
        <span class="biz-name">University</span>
      </div>

      <div class="biz-card biz-soon">
        <span class="soon-badge">Soon</span>
        <span class="biz-icon"><i class="fas fa-car"></i></span>
        <span class="biz-name">DMV / RTO</span>
      </div>

      <div class="biz-card biz-soon">
        <span class="soon-badge">Soon</span>
        <span class="biz-icon"><i class="fas fa-cut"></i></span>
        <span class="biz-name">Salon</span>
      </div>

      <div class="biz-card biz-soon">
        <span class="soon-badge">Soon</span>
        <span class="biz-icon"><i class="fas fa-film"></i></span>
        <span class="biz-name">Cinema</span>
      </div>

      <div class="biz-card biz-soon">
        <span class="soon-badge">Soon</span>
        <span class="biz-icon"><i class="fas fa-tools"></i></span>
        <span class="biz-name">Service Center</span>
      </div>

      <div class="biz-card biz-soon">
        <span class="soon-badge">Soon</span>
        <span class="biz-icon"><i class="fas fa-bus"></i></span>
        <span class="biz-name">Bus Station</span>
      </div>

      <div class="biz-card biz-soon">
        <span class="soon-badge">Soon</span>
        <span class="biz-icon"><i class="fas fa-dumbbell"></i></span>
        <span class="biz-name">Gym</span>
      </div>

    </div>

    <p class="landing-register-link">
      New business? <a href="Register.aspx">Register here</a> to create your account.
    </p>
  </div>

  <style>
    /* ── Compact 7-column business card grid ─────────────────────── */
    .biz-grid {
      display: grid;
      grid-template-columns: repeat(7, 1fr);
      gap: 14px;
      max-width: 1260px;
      margin: 0 auto 32px;
      padding: 0 12px;
    }

    .biz-card {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 10px;
      padding: 20px 8px 18px;
      background: var(--bg-card);
      border: 1px solid var(--border);
      border-radius: 14px;
      text-decoration: none;
      cursor: pointer;
      position: relative;
      transition: transform 0.18s ease, border-color 0.18s ease, box-shadow 0.18s ease;
      min-height: 130px;
    }

    /* Active cards */

    .biz-icon {
      width: 52px;
      height: 52px;
      border-radius: 50%;
      background: rgba(212,175,55,0.08);
      border: 1px solid rgba(212,175,55,0.2);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.3rem;
      color: var(--gold);
      transition: background 0.18s ease;
      flex-shrink: 0;
    }


    .biz-name {
      font-size: 0.82rem;
      font-weight: 700;
      color: var(--gold);
      text-align: center;
      letter-spacing: 0.3px;
      line-height: 1.2;
    }

    /* Coming soon overrides */
    .biz-soon {
      opacity: 0.55;
      cursor: not-allowed;
    }
    .biz-soon .biz-icon {
      background: rgba(255,255,255,0.03);
      border-color: rgba(255,255,255,0.08);
      color: var(--text-muted);
    }
    .biz-soon .biz-name {
      color: var(--text-sec);
    }

    /* "Soon" ribbon badge */
    .soon-badge {
      position: absolute;
      top: 10px;
      right: 10px;
      background: linear-gradient(135deg, var(--gold-dk, #b8860b), var(--gold));
      color: #000;
      font-size: 0.6rem;
      font-weight: 800;
      padding: 2px 7px;
      border-radius: 20px;
      letter-spacing: 0.8px;
      text-transform: uppercase;
    }

    /* Responsive: collapse to 4, then 3 on smaller screens */
    @media (max-width: 1100px) {
      .biz-grid { grid-template-columns: repeat(5, 1fr); }
    }
    @media (max-width: 700px) {
      .biz-grid { grid-template-columns: repeat(3, 1fr); }
    }
  </style>
</asp:Content>
