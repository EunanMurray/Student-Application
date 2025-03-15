# Scholarship Application Management System

## Overview

This repository contains a comprehensive Scholarship Application Management System built with ASP.NET Core and Entity Framework Core. The system facilitates the entire scholarship application process, from applicant submissions to committee reviews and scholarship awards.

## Technology Stack

- **Framework**: ASP.NET Core (.NET 6)
- **UI**: Razor Pages
- **Database ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Styling**: Bootstrap 5
- **Email Service**: SMTP integration with Gmail

## System Architecture

### Database Structure

The application employs two distinct database contexts:

1. **PrimaryContext**: Manages core application data
   - Applicants and their details
   - Sports and categories
   - Scholarships and offer histories
   - Campus information
   - Budgets and allocations

2. **ApplicationDbContext**: Handles identity and authorization
   - User accounts and profiles
   - Role-based access control
   - Committee member sport assignments

### Key Models

- **Applicant**: Contains personal details, academic history, and sports achievements.
- **Sport**: Defines available sports for scholarship applications.
- **ScholarshipType**: Defines scholarship levels (Gold, Silver, Bronze) and payment amounts.
- **ScholarshipOfferHistory**: Tracks scholarship offers, responses, and statuses.
- **UserSport**: Links committee members to their assigned sports for review.

## User Roles and Access Control

The system implements a role-based access control mechanism with the following predefined roles:

- **Admin**: Full system access, can manage users, roles, and system settings.
- **Committee Member**: Reviews applications for assigned sports, makes scholarship recommendations.
- **Secretary**: Manages budget views and allocation reporting.
- **Applicant**: Submits initial scholarship applications.
- **ReturningApplicant**: Submits renewal applications for existing scholarships.

## Core Features

### Applicant Workflow

- **Registration and Email Verification**: Secure account creation with email verification.
- **Multi-step Application Form**: Guided application process with built-in validation.
- **Application Status Tracking**: Real-time status updates on review progress.
- **Scholarship Acceptance**: Digital acceptance or rejection of scholarship offers.

### Committee Member Workflow

- **Sport-specific Application Review**: Filtered access to applications based on assigned sports.
- **Scholarship Decision Making**: Structured decision process with supporting notes.
- **Application Filtering**: Advanced filtering by student details and application status.

### Administrative Features

- **User Management**: Create, update, and delete system users.
- **Role Assignment**: Dynamic role allocation to system users.
- **Committee Sport Assignment**: Link committee members to specific sports for review.
- **Scholarship Offer Management**: Track and manage all scholarship offers.

### Secretary Features

- **Budget Overview**: Comprehensive view of scholarship budget allocation.
- **Year-based Reporting**: Breakdown of scholarships by college year.
- **Financial Tracking**: Monitor remaining budget and allocation history.

## Setup Instructions

### Prerequisites

- **.NET 6 SDK**
- **SQL Server** (local or hosted)
- **Visual Studio 2022** (recommended) or another compatible IDE

### Database Configuration

1. **Update Connection Strings**:  
   Edit the connection strings in `appsettings.json` to point to your SQL Server instance:

   ```json
   "ConnectionStrings": {
     "Project300Database": "Server=YOUR_SERVER;Initial Catalog=YOUR_DB;Trusted_Connection=True;",
     "DefaultConnection": "Server=YOUR_SERVER;Initial Catalog=YOUR_DB;Trusted_Connection=True;",
     "StudentApplicationPagesContextConnection": "Server=YOUR_SERVER;Initial Catalog=YOUR_DB;Trusted_Connection=True;"
   }
   ```

2. **Configure Email Settings**:  
   Set up your SMTP configuration in `appsettings.json`:

   ```json
   "EmailSettings": {
     "SmtpServer": "smtp.example.com",
     "SmtpPort": 587,
     "Username": "your-email@example.com",
     "Password": "your-app-password",
     "FromEmail": "your-email@example.com",
     "FromName": "Scholarship System",
     "EnableSsl": true
   }
   ```

### Database Migration

Apply database migrations for both contexts:

```bash
# For PrimaryContext
dotnet ef migrations add InitialPrimaryMigration --context PrimaryContext
dotnet ef database update --context PrimaryContext

# For ApplicationDbContext
dotnet ef migrations add InitialIdentityMigration --context ApplicationDbContext
dotnet ef database update --context ApplicationDbContext
```

If you encounter any migration issues:
1. Ensure the database does not already exist with conflicting schema.
2. Clear your migrations folder if necessary.
3. Refer to Stack Overflow or contact the project maintainer (Eunan) for assistance.

### Running the Application

To run the application, use your IDE or execute:

```bash
dotnet run
```

### Default Credentials

Upon initialization, the system creates the following default users:

- **Admin**: admin@example.com / Admin123!
- **Committee Member**: member@example.com / Member123!

## Project Structure

```
StudentApplication/
├── Areas/
│   └── Identity/           # Identity-related pages and functionality
├── Data/
│   ├── ApplicationDbContext.cs  # Identity context
│   ├── DbInitializer.cs    # Database seed data
│   └── PrimaryContext.cs   # Main application context
├── Models/
│   ├── Applicant.cs        # Applicant entity
│   ├── Scholarship.cs      # Scholarship entities
│   └── Sport.cs            # Sport entity
├── Pages/
│   ├── Admin/              # Administrative interfaces
│   ├── Applications/       # Application submission flows
│   ├── Redirections/       # Navigation logic
│   └── Shared/             # Shared components and layouts
├── Services/
│   ├── EmailService.cs     # Email notification service
│   └── EmailSettings.cs    # Email configuration
├── ViewModels/             # Data transfer objects
├── wwwroot/                # Static assets
├── Program.cs              # Application entry point and configuration
└── appsettings.json        # Configuration settings
```

## Development Workflow

1. **Fork and Clone**: Fork this repository and clone it to your local machine.
2. **Create a Branch**: Create a feature branch for your changes.
3. **Implement Features**: Develop your changes following the project's established patterns.
4. **Test**: Thoroughly test your changes.
5. **Submit a Pull Request**: Provide a detailed description of your changes.

## Troubleshooting

### Common Issues

- **Database Connection Errors**: Verify your connection strings and ensure SQL Server is running.
- **Email Sending Failures**: Check your SMTP settings and credentials.
- **Migration Errors**:  
  - Clear the migrations folder if conflicts occur.
  - Ensure the database doesn't exist with a conflicting schema.
  - Run `Update-Database` with the correct context specified.

## Contributors

- Eunan Murray
- Kian Gillespie
- Evan Brady
- Damian Polakov
