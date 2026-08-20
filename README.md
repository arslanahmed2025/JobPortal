# JobPortal 💼

A full-stack **job portal web application** built with ASP.NET Core MVC, featuring role-based authentication, employer job management, job seeker applications, and a complete admin panel.

🔗 **Live Repo:** [github.com/arslanahmed2025/JobPortal](https://github.com/arslanahmed2025/JobPortal)

---

## 📋 Overview

JobPortal connects **Employers** who want to post jobs with **Job Seekers** looking for opportunities, all managed under a secure, role-based system with **Admin** oversight. Employers can build a company profile, post and manage job listings, and review applications. Job Seekers can browse, search, and apply to jobs with resume uploads. Admins moderate the platform — managing users and job postings.

## ✨ Features

### 🔐 Authentication & Roles
- Secure registration/login with **ASP.NET Core Identity**
- Three roles: **Admin**, **Employer**, **Job Seeker**
- Role-based dashboard redirects after login
- Dynamic navbar reflecting login state and role

### 🏢 Employer Features
- Create and edit company profile (with logo upload)
- Post, edit, and delete job listings
- View and manage applications per job
- Update application status (Pending → Shortlisted → Accepted, etc.)

### 🔍 Job Seeker Features
- Browse and search active job postings (by title, category, or location)
- View detailed job descriptions
- Apply to jobs with resume upload (PDF) and cover letter
- Track application status in "My Applications"

### 🛠️ Admin Panel
- Platform-wide statistics dashboard (users, employers, job seekers, jobs, applications)
- Manage users — block/unblock accounts
- Manage job postings — moderate/delete listings

### 🎨 UI/UX
- Clean, consistent design system with a custom color palette
- Fully responsive Bootstrap-based layout
- Data-driven landing page: latest job notifications, upcoming deadlines, monthly hiring stats, and a featured "Employer of the Month"

---

## 🧰 Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Authentication | ASP.NET Core Identity |
| Frontend | Razor Views, Bootstrap 5 |
| File Storage | Local file system (`wwwroot/uploads`) |

---

## 🗂️ Project Structure

```
JobPortal/
├── Controllers/          # MVC Controllers (Account, Employer, JobSeeker, Admin, Home)
├── Models/                # Entity models + ViewModels
├── Views/                 # Razor views organized by controller
├── Data/                  # DbContext, Role/Admin seeders
├── Migrations/            # EF Core migrations
└── wwwroot/                # Static files, uploaded logos & resumes
```

## 🔗 Entity Relationships

```
ApplicationUser (Identity)
    └── 1-to-1 → EmployerProfile
                    └── 1-to-many → JobPosting
                                        └── 1-to-many → JobApplication
                                                             └── many-to-1 → ApplicationUser (Job Seeker)
```

---

## ⚙️ Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (Express or full)
- Visual Studio 2022 (recommended)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/arslanahmed2025/JobPortal.git
   cd JobPortal
   ```

2. **Configure the connection string**

   Open `appsettings.json` and update `DefaultConnection` with your local SQL Server instance:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER\\SQLEXPRESS;Database=jobportal;Trusted_Connection=True;TrustServerCertificate=True"
   }
   ```

3. **Apply migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

   On first run, the app automatically seeds:
   - Roles: `Admin`, `Employer`, `JobSeeker`
   - A default admin account:
     - **Email:** `admin@jobportal.com`
     - **Password:** `Admin@123`

   > ⚠️ Change the default admin password before any real deployment.

5. Open your browser at `https://localhost:{port}` and start exploring!

---

## 📸 Screenshots

*(Add screenshots here — Home page, Employer dashboard, Job listing, Admin panel)*

---

## 🚀 Future Improvements

- Cloud-based file storage (Azure Blob / AWS S3) instead of local disk
- Unit & integration tests
- Email notifications for application status updates
- Multi-role support for a single user account
- Pagination for job listings and admin tables

---

## 👤 Author

**Arslan Ahmed**
Full-Stack Developer | ASP.NET Core, Angular, EF Core
📍 Karachi, Pakistan

---

*Built as a portfolio project to demonstrate full-stack development with ASP.NET Core MVC, EF Core, and role-based application architecture.*
