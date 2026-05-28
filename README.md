# 🦷 Dental-Hub

A comprehensive, modern dental case management system built with clean architecture principles and microservice patterns. Dental-Hub enables seamless collaboration between dental professionals, students, and patients through intelligent case tracking, AI-powered case creation, and real-time session management.

---

## 📋 Table of Contents

- [Features](#features)
- [Current Stage](#current-stage)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Installation](#installation)
- [Usage](#usage)
- [API Documentation](#api-documentation)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)

---

## ✨ Features

### Core Features

- **👥 Multi-Role User Management**
  - Doctors, Students, Patients, and Admins
  - University affiliation system (Cairo, Ain Shams, Mansoura, Alexandria, Assiut, Benha)
  - Role-based access control with JWT authentication

- **📁 Intelligent Case Management**
  - Create and manage dental cases with detailed patient information
  - Case status tracking (Pending, InProgress, Completed, Cancelled, UnderReview, Rejected)
  - Advanced filtering and search by patient name or case type
  - Pagination support for large datasets

- **🤖 AI-Powered Case Creation** (Currently Integrated with Frontend)
  - Automated case creation through AI endpoints
  - Secure API key authentication
  - Seamless integration with the frontend application
  - **Future Enhancement:** Backend AI integration for intelligent case analysis and recommendations

- **📞 Session & Request Management**
  - Schedule and track consultation sessions
  - Session statuses: Scheduled, Done, Cancelled, Expired
  - Case request workflow with status tracking
  - Request statuses: Pending, UnderReview, Approved, Rejected, Taken, Cancelled

- **📸 Media Management**
  - Support for case-related media uploads
  - Integration with Cloudinary for image storage and CDN delivery
  - Secure media associations with cases and sessions

- **🔐 Authentication & Security**
  - JWT token-based authentication
  - Refresh token mechanism
  - Password hashing with ASP.NET Core Identity
  - Password reset and change functionality
  - Email verification workflows

- **📧 Email Notifications**
  - Background job processing with Hangfire
  - Account activation emails
  - Password reset confirmations
  - Case status notifications
  - Session reminders

---

## 🚀 Current Stage

### Phase 1: Frontend AI Integration ✅ COMPLETED
- AI is currently integrated with the **frontend application**
- Doctors and students can create cases through AI-powered endpoints
- Secure API key authentication for AI endpoints

### Phase 2: Backend AI Integration (Coming Next)
- Enhanced AI integration at the backend layer
- Intelligent case analysis and recommendations
- Automated clinical insights and pattern recognition
- Predictive analytics for case outcomes

### Phase 3: Super Admin Dashboard (Upcoming)
- Comprehensive system administration features
- User and role management
- System-wide analytics and reporting
- Audit logs and compliance tracking
- University and institution management

---

## 🛠️ Tech Stack

### Backend
- **.NET 9.0** - Latest C# framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 9.0** - ORM for database operations
- **MySQL** - Primary database with Pomelo EF provider
- **Redis** - Caching and session management
- **Hangfire** - Background job processing
- **JWT** - Token-based authentication
- **Cloudinary** - Image storage and CDN

### Architecture & Patterns
- **Clean Architecture** - Separation of concerns
- **Domain-Driven Design (DDD)** - Rich domain models
- **CQRS** - Command Query Responsibility Segregation with MediatR
- **Repository Pattern** - Data access abstraction
- **Unit of Work Pattern** - Transaction management
- **Factory Pattern** - Entity creation
- **Specification Pattern** - Reusable query logic

### API Versioning
- **API v1.0** - Initial stable endpoints
- **API v2.0** - Enhanced features with additional capabilities
- Swagger/OpenAPI documentation for all versions

---

## 🏗️ Architecture

### Project Structure

```
DentalHub.Domain/
├── Entities/              # Core business entities (User, PatientCase, Session, etc.)
├── Factories/             # Entity creation with validation
├── Specifications/        # Reusable query specifications
├── DomainExceptions/      # Custom domain exceptions
└── Repositories/          # Repository interfaces

DentalHub.Application/
├── DTOs/                  # Data transfer objects for API responses
├── Commands/              # CQRS commands (mutations)
├── Queries/               # CQRS queries (reads)
├── Handlers/              # Command and query handlers
├── Services/              # Business logic services
├── Validators/            # FluentValidation rules
└── Common/                # Shared utilities and constants

DentalHub.Infrastructure/
├── Persistence/           # EF Core DbContext and configurations
├── Repositories/          # Repository implementations
├── Services/              # Infrastructure services (email, storage, etc.)
├── Seeders/               # Database seeders for initial data
└── Cloudinary/            # Image storage integration

DentalHub.API/
├── Controllers/           # API endpoints (v1 and v2)
├── Middleware/            # Custom middleware
├── Extensions/            # Dependency injection setup
└── Program.cs             # Application startup configuration
```

### Key Entities

- **User** - Base user entity with authentication
- **Doctor** - Doctor profile linked to university
- **Student** - Student profile with university affiliation
- **Patient** - Patient information and history
- **PatientCase** - Dental case with patient and case type
- **CaseRequest** - Request for case assistance between professionals
- **Session** - Consultation sessions with notes
- **SessionNote** - Detailed notes from sessions
- **CaseType** - Predefined dental case types
- **Media** - Images and files associated with cases

---

## 📦 Installation

### Prerequisites

- **.NET 9.0 SDK** or later
- **MySQL 8.0** or later
- **Docker** (optional, for containerized deployment)
- **Redis** (for caching)

### Setup Steps

1. **Clone the Repository**
   ```bash
   git clone https://github.com/omargamal1121/Dental-Hub.git
   cd Dental-Hub
   ```

2. **Configure Database Connection**
   - Update `appsettings.json` with your MySQL connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=DentalHub;User Id=root;Password=yourpassword;"
   }
   ```

3. **Configure Redis Connection**
   ```json
   "Redis": {
     "ConnectionString": "localhost:6379"
   }
   ```

4. **Configure Cloudinary (for media storage)**
   ```json
   "Cloudinary": {
     "CloudName": "your_cloud_name",
     "ApiKey": "your_api_key",
     "ApiSecret": "your_api_secret"
   }
   ```

5. **Configure Email Service**
   ```json
   "EmailConfiguration": {
     "SmtpServer": "smtp.gmail.com",
     "Port": 587,
     "SenderEmail": "your_email@gmail.com",
     "SenderPassword": "your_app_password"
   }
   ```

6. **Configure AI API Key** (for AI endpoints)
   ```json
   "AI_Configuration": {
     "ApiKey": "your_ai_api_key"
   }
   ```

7. **Run Database Migrations**
   ```bash
   cd DentalHub.Infrastructure
   dotnet ef database update
   ```

8. **Restore Dependencies & Run**
   ```bash
   dotnet restore
   dotnet run --project DentalHub.API
   ```

   The API will be available at `https://localhost:7000`

### Docker Deployment

```bash
docker build -t dental-hub:latest .
docker run -d -p 7000:7000 \
  -e "ConnectionStrings:DefaultConnection=Server=mysql;Database=DentalHub;User Id=root;Password=password;" \
  dental-hub:latest
```

---

## 📖 Usage

### Authentication

1. **Register a New User**
   ```bash
   POST /api/v1/auth/register
   Content-Type: application/json
   
   {
     "fullName": "Dr. Ahmed Hassan",
     "email": "ahmed@example.com",
     "password": "SecurePassword123!"
   }
   ```

2. **Login**
   ```bash
   POST /api/v1/auth/login
   Content-Type: application/json
   
   {
     "email": "ahmed@example.com",
     "password": "SecurePassword123!"
   }
   ```

3. **Use JWT Token**
   ```bash
   Authorization: Bearer {your_jwt_token}
   ```

### Creating a Case

**Standard Case Creation:**
```bash
POST /api/v1/cases
Authorization: Bearer {token}
Content-Type: multipart/form-data

{
  "patientName": "John Doe",
  "patientEmail": "john@example.com",
  "caseTypeId": "123e4567-e89b-12d3-a456-426614174000",
  "description": "Root canal treatment case",
  "status": "Pending",
  "image": [file]
}
```

**AI-Powered Case Creation:**
```bash
POST /api/v1/cases/ai/create
X-AI-API-KEY: {your_ai_api_key}
Content-Type: multipart/form-data

{
  "patientName": "Jane Smith",
  "patientEmail": "jane@example.com",
  "caseTypeId": "...",
  "description": "...",
  "image": [file]
}
```

### Fetching Cases with Filters

```bash
GET /api/v1/cases?search=patient_name&status=InProgress&page=1&pageSize=10
Authorization: Bearer {token}
```

---

## 📚 API Documentation

Full API documentation is available via Swagger UI:

- **Local Development**: `https://localhost:7000/swagger/index.html`
- **API Endpoints Documentation**:
  - `/api/v1/cases` - Case management
  - `/api/v1/auth` - Authentication
  - `/api/v2/university-members` - University member registry
  - And more...

### Supported Universities

| University Name        | University ID                        |
|------------------------|--------------------------------------|
| Cairo University       | 11111111-1111-1111-1111-111111111111 |
| Ain Shams University   | 22222222-2222-2222-2222-222222222222 |
| Mansoura University    | 33333333-3333-3333-3333-333333333333 |
| Alexandria University  | 44444444-4444-4444-4444-444444444444 |
| Assiut University      | 55555555-5555-5555-5555-555555555555 |
| Benha University       | 66666666-6666-6666-6666-666666666666 |

---

## 🗺️ Roadmap

### Phase 2 - Backend AI Integration 🔄 (In Progress)
- [ ] Backend AI model integration
- [ ] Intelligent case analysis engine
- [ ] Clinical recommendations system
- [ ] Pattern recognition for common dental issues
- [ ] Predictive case outcome analysis

### Phase 3 - Super Admin Dashboard 👑 (Upcoming)
- [ ] Comprehensive admin dashboard
- [ ] System-wide user and role management
- [ ] Advanced analytics and reporting
- [ ] Audit logging and compliance
- [ ] Institution and university management
- [ ] System health monitoring
- [ ] User activity tracking

### Phase 4 - Enhanced Features 🚀 (Future)
- [ ] Mobile app for iOS and Android
- [ ] Real-time notifications with SignalR
- [ ] Video consultation support
- [ ] Advanced case analytics
- [ ] Integration with popular dental management systems
- [ ] Multi-language support
- [ ] Export case reports to PDF

---

## 🤝 Contributing

We welcome contributions! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

### Coding Standards
- Follow C# naming conventions
- Use meaningful variable and method names
- Add XML documentation for public methods
- Write unit tests for business logic
- Follow the clean architecture principles

---

## 📄 License

This project is currently closed-source. All rights reserved. Contact the repository owner for licensing information.

---

## 📞 Support

For issues, questions, or feature requests, please:

1. Check existing [Issues](https://github.com/omargamal1121/Dental-Hub/issues)
2. Create a new issue with detailed description
3. Contact the development team

---

## 👨‍💻 Development Team

**Lead Developer:** Omar Gamal ([@omargamal1121](https://github.com/omargamal1121))

---

## 🙏 Acknowledgments

- Built with modern .NET technologies
- Inspired by clean architecture and domain-driven design principles
- Leveraging the power of AI for healthcare innovation

---

**Last Updated:** May 28, 2026  
**Current Version:** 1.0.0

