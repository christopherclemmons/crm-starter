# CRM Enterprise Starter

A production-ready Customer Relationship Management (CRM) system built with .NET, designed for enterprise use. This starter kit provides a solid foundation for building comprehensive CRM solutions with sales pipeline management, customer service, and activity tracking.

## 🚀 Quick Start

### Prerequisites
- Docker & Docker Compose
- .NET 8.0 SDK (for local development)

### Run with Docker (Recommended)
```bash
# Clone the repository
git clone <repository-url>
cd software-agents

# Start the application
docker compose up -d --build

# The API will be available at http://localhost:8080
```

### Local Development
```bash
# Start PostgreSQL database
docker compose up postgres -d

# Run the API locally
cd src/Crm.Api
dotnet run
```

## 📋 Features

### Sales Pipeline Management
- **Leads**: Capture and qualify potential customers
- **Opportunities**: Track deals through the sales pipeline
- **Accounts**: Manage customer organizations
- **Contacts**: Store customer and prospect information

### Customer Service & Support
- **Cases**: Handle customer support tickets and issues
- **Activities**: Log all customer interactions and communications

### Task Management
- **Tasks**: Create and track actionable items
- **Activity Tracking**: Monitor all customer engagement

## 🏗️ Architecture

### Technology Stack
- **Backend**: .NET 8.0 Web API
- **Database**: PostgreSQL 16
- **Containerization**: Docker & Docker Compose
- **Architecture**: Clean Architecture with Domain-Driven Design

### Project Structure
```
src/
├── Crm.Api/                    # Main API application
│   ├── Controllers/           # API endpoints
│   ├── Domain/               # Business logic & entities
│   │   ├── Entities/         # Domain models
│   │   └── Enums.cs          # Domain enumerations
│   ├── Infrastructure/       # Data access & external services
│   └── Contracts/            # Request/Response DTOs
└── tests/                    # Unit and integration tests
```

### Domain Entities
- **Account**: Customer organizations
- **Contact**: Individual people (customers, prospects)
- **Lead**: Sales prospects
- **Opportunity**: Sales deals
- **Case**: Support tickets
- **Activity**: Customer interactions
- **Task**: Actionable items

## 🔧 API Endpoints

### Sales Management
- `GET/POST/PUT/DELETE /api/accounts` - Account management
- `GET/POST/PUT/DELETE /api/contacts` - Contact management
- `GET/POST/PUT/DELETE /api/leads` - Lead management
- `GET/POST/PUT/DELETE /api/opportunities` - Opportunity management

### Customer Service
- `GET/POST/PUT/DELETE /api/cases` - Case management
- `GET/POST/PUT/DELETE /api/activities` - Activity logging

### Task Management
- `GET/POST/PUT/DELETE /api/tasks` - Task management

## 🗄️ Database Schema

The system uses PostgreSQL with the following key tables:
- `accounts` - Customer organizations
- `contacts` - Individual contacts
- `leads` - Sales leads
- `opportunities` - Sales opportunities
- `cases` - Support cases
- `activities` - Customer activities
- `tasks` - Actionable tasks

## 🧪 Testing

```bash
# Run unit tests
dotnet test

# Run integration tests
dotnet test --filter Category=Integration
```

## 📚 Development Guidelines

### Code Organization
- **Domain Layer**: Contains business entities and logic
- **Infrastructure Layer**: Handles data access and external services
- **API Layer**: REST endpoints and request/response handling

### Naming Conventions
- Use PascalCase for classes and methods
- Use camelCase for properties and parameters
- Follow .NET naming guidelines

### API Design
- RESTful endpoints with proper HTTP methods
- Consistent response formats
- Input validation on all endpoints
- Proper error handling and status codes

## 🚀 Deployment

### Production Deployment
```bash
# Build for production
docker compose -f docker-compose.yml up -d --build

# Or deploy to cloud platforms (AWS, Azure, GCP)
# Update environment variables for production database
```

### Environment Variables
```env
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__Postgres=Host=your-db-host;Port=5432;Database=crm;Username=your-user;Password=your-password
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

### Development Setup
```bash
# Install dependencies
dotnet restore

# Run database migrations (if applicable)
# dotnet ef database update

# Run the application
dotnet run --project src/Crm.Api
```

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:
- Create an issue in this repository
- Check the documentation in the `docs/` folder
- Review the API documentation at `/swagger` when running locally

## 🎯 Roadmap

- [ ] Advanced reporting and analytics
- [ ] Email integration
- [ ] Mobile app companion
- [ ] Multi-tenant support
- [ ] Advanced workflow automation
- [ ] Integration with popular business tools (Slack, Outlook, etc.)

---

**Built with .NET 8.0 - Enterprise-ready CRM foundation**
