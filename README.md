# E-Commerce API

A comprehensive e-commerce REST API built with .NET 8, implementing Clean Architecture principles with JWT authentication, role-based authorization, and comprehensive testing.

## 🚀 Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or full instance)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation & Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ECommerceApi
   ```

2. **Configure database connection**
   
   Update `appsettings.json` in `src/ECommerceApi.API/`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=ECommerceApiDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
     }
   }
   ```

3. **Setup database**
   ```bash
   cd src/ECommerceApi.API
   dotnet ef migrations add InitialCreate --project ../ECommerceApi.Infrastructure
   dotnet ef database update --project ../ECommerceApi.Infrastructure
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access Swagger UI**
   
   Navigate to: `https://localhost:7120/swagger`

### 🎯 Test Accounts

The database is automatically seeded with test accounts:

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@ecommerce.com` | `Admin123!` |
| Customer | `customer@example.com` | `Customer123!` |

## 📡 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and receive JWT token
- `POST /api/auth/logout` - Logout user
- `GET /api/auth/profile` - Get current user profile *(Protected)*

### Products
- `GET /api/products` - Get all products with search/filter
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product *(Admin only)*
- `PUT /api/products/{id}` - Update product *(Admin only)*
- `DELETE /api/products/{id}` - Delete product *(Admin only)*

### 🔍 Search & Filter Parameters

```http
GET /api/products?search=iphone&category=Electronics&minPrice=100&maxPrice=1000&inStock=true&page=1&pageSize=10&sortBy=Price&sortDescending=false
```

| Parameter | Description | Example |
|-----------|-------------|---------|
| `search` | Search in name/description | `iphone` |
| `category` | Filter by category | `Electronics` |
| `minPrice` | Minimum price filter | `100` |
| `maxPrice` | Maximum price filter | `1000` |
| `inStock` | Filter by stock availability | `true` |
| `page` | Page number (default: 1) | `1` |
| `pageSize` | Items per page (default: 10) | `10` |
| `sortBy` | Sort field (Name, Price, CreatedAt) | `Price` |
| `sortDescending` | Sort direction | `false` |

## 🔐 Authentication Flow

### 1. Register/Login to get JWT token
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@ecommerce.com",
  "password": "Admin123!"
}
```

### 2. Use token in subsequent requests
```http
GET /api/products
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 3. Swagger Authentication
1. Click **"Authorize"** button in Swagger UI
2. Enter: `Bearer <your-jwt-token>`
3. Test protected endpoints

## 🏗️ Architecture

This project implements **Clean Architecture** with four distinct layers:

```mermaid
graph TB
    subgraph "Clean Architecture"
        subgraph "Domain Layer"
            ENT[Entities]
            VO[Value Objects]
            ENUM[Enums]
        end
        
        subgraph "Application Layer"
            SVC[Services]
            DTO[DTOs]
            VAL[Validators]
        end
        
        subgraph "Infrastructure Layer"
            DB[Database]
            JWT[JWT Service]
            BCR[BCrypt]
        end
        
        subgraph "API Layer"
            CTRL[Controllers]
            MW[Middleware]
            SWAG[Swagger]
        end
    end
    
    CTRL --> SVC
    SVC --> ENT
    DB --> ENT
    JWT --> SVC
    
    style Domain fill:#e1f5fe
    style Application fill:#f3e5f5
    style Infrastructure fill:#fff3e0
    style API fill:#e8f5e8
```

### 📂 Project Structure

```
ECommerceApi/
├── src/
│   ├── ECommerceApi.API/              # 🌐 Presentation Layer
│   ├── ECommerceApi.Application/      # 📋 Application Layer
│   ├── ECommerceApi.Infrastructure/   # 🗄️ Infrastructure Layer
│   └── ECommerceApi.Domain/           # 💎 Domain Layer
├── tests/
│   └── ECommerceApi.UnitTests/        # 🧪 Unit Tests
├── docs/
│   └── architecture.md                # 📖 Architecture Documentation
└── README.md                          # 📝 Project Overview
```

For detailed architecture information, see [Architecture Documentation](docs/architecture.md).

## 🛠️ Technology Stack

### Core Technologies
- **.NET 8** - Latest LTS framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **JWT Bearer** - Authentication
- **Swagger/OpenAPI** - API Documentation

### Development Tools
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **BCrypt.Net** - Password hashing
- **xUnit** - Testing framework
- **Moq** - Mocking framework
- **FluentAssertions** - Test assertions

## 🧪 Testing

### Run Tests

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test tests/ECommerceApi.UnitTests/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "ClassName=ProductServiceTests"
```

### Test Coverage
- **Domain Logic** - Entity business rules
- **Application Services** - Business logic
- **Validators** - Input validation
- **Authentication** - JWT flows

## 🗄️ Database Schema

### Users Table
```sql
CREATE TABLE Users (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Email nvarchar(256) UNIQUE NOT NULL,
    PasswordHash nvarchar(500) NOT NULL,
    FirstName nvarchar(100) NOT NULL,
    LastName nvarchar(100) NOT NULL,
    Role int NOT NULL,
    IsActive bit DEFAULT 1,
    CreatedAt datetime2 DEFAULT GETUTCDATE(),
    UpdatedAt datetime2 DEFAULT GETUTCDATE()
);
```

### Products Table
```sql
CREATE TABLE Products (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(200) NOT NULL,
    Description nvarchar(2000) NOT NULL,
    Price decimal(18,2) NOT NULL,
    Stock int DEFAULT 0,
    Category nvarchar(100) NOT NULL,
    Sku nvarchar(50) UNIQUE NOT NULL,
    ImageUrl nvarchar(500) NOT NULL,
    IsActive bit DEFAULT 1,
    CreatedByUserId int FOREIGN KEY REFERENCES Users(Id),
    CreatedAt datetime2 DEFAULT GETUTCDATE(),
    UpdatedAt datetime2 DEFAULT GETUTCDATE()
);
```

## 📝 API Examples

### User Registration
```bash
curl -X POST "https://localhost:7120/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newuser@example.com",
    "password": "SecurePass123!",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

### Create Product (Admin)
```bash
curl -X POST "https://localhost:7120/api/products" \
  -H "Authorization: Bearer <jwt-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "iPhone 15 Pro",
    "description": "Latest iPhone with advanced features",
    "imageUrl": "https://example.com/iphone15.jpg",
    "price": 999.99,
    "stock": 100,
    "category": "Electronics",
    "sku": "IPH15PRO001"
  }'
```

### Search Products
```bash
curl -X GET "https://localhost:7120/api/products?search=iphone&category=Electronics&page=1&pageSize=5"
```

## 🔧 Development

### Adding New Migrations
```bash
cd src/ECommerceApi.API
dotnet ef migrations add YourMigrationName --project ../ECommerceApi.Infrastructure
dotnet ef database update --project ../ECommerceApi.Infrastructure
```

### Code Standards
- Follow **Clean Architecture** principles
- Maintain **95%+ test coverage**
- Use **meaningful commit messages**
- Update **documentation** for new features

### Environment Variables
```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-that-is-at-least-32-characters-long",
    "Issuer": "ECommerceApi",
    "Audience": "ECommerceApi",
    "ExpirationHours": "24"
  }
}
```

## 📊 Performance Features

- **Pagination** - Efficient handling of large datasets
- **Database Indexing** - Optimized queries on Email, SKU
- **Async Operations** - Non-blocking I/O operations
- **Input Validation** - Early validation to prevent invalid data
- **Connection Pooling** - Efficient database connections

## 🔐 Security Features

- **JWT Authentication** - Stateless token-based auth
- **Password Hashing** - BCrypt with salt rounds
- **Role-Based Authorization** - Admin/Customer access control
- **Input Sanitization** - SQL injection prevention
- **HTTPS Enforcement** - Secure communication

## 🚀 Deployment

### Docker Support
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ECommerceApi.API.dll"]
```

### Production Checklist
- [ ] Update connection strings
- [ ] Configure JWT secrets
- [ ] Enable HTTPS
- [ ] Setup logging
- [ ] Configure CORS
- [ ] Database migrations
- [ ] Health checks

## 📈 Future Enhancements

- **Order Management** - Shopping cart, checkout process
- **Payment Integration** - Stripe, PayPal support
- **Email Notifications** - Registration, order confirmations
- **File Upload** - Product image management
- **Caching Layer** - Redis for performance
- **API Versioning** - v1, v2 endpoint support

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

- **Documentation**: [Architecture Guide](docs/architecture.md)
- **API Documentation**: `/swagger` endpoint
- **Issues**: GitHub Issues
- **Email**: phuongnd.tech@gmail.com

---

**Built with ❤️ using Clean Architecture and .NET 8**