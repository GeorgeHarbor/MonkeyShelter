# 🐒 Monkey Shelter Management System

## 🔗 Overview
This project is a **full-stack Monkey Shelter Management System** built with:

- **Backend**: ASP.NET Core 9, MassTransit, RabbitMQ, PostgreSQL, Redis
- **Frontend**: Angular 19, Angular Material UI

### Core Features:
- 📦 **Monkey Arrival/Departure Management**
- 🏋️ **Shelter Assignment**
- 📊 **Reporting and Audit Logs**
- 📅 **Veterinarian Check Scheduling**
- 🔒 **JWT-based Authentication & Authorization**

---

## 🔧 Architecture Overview

### High-Level Design:

The system follows a **microservices architecture** with the following components:

- **Authentication Service (`MonkeyShelter.Auth`)**: Manages user registration, login, and JWT token issuance.
- **API Gateway (`MonkeyShelter.Api`)**: Handles monkey-related CRUD operations, communicates with the database, and coordinates services.
- **Reporting Service (`MonkeyShelter.Reports`)**: Listens for events via RabbitMQ and logs audit data for reporting purposes.
- **Worker Services (`MonkeyShelter.Workers`)**: Background services, such as the Vet Scheduler, that operate independently of user requests.
- **RabbitMQ**: Message broker used for decoupled communication between services.
- **PostgreSQL**: Central database used by the API and reporting service.
- **Redis**: A cache layer implemented with smart invalidation

### Communication Flow:
1. **Frontend** interacts with the **API Gateway** for monkey management.
2. **API Gateway** publishes events (e.g., MonkeyArrived) to **RabbitMQ**.
3. **Reporting Service** listens on RabbitMQ for these events and logs them to the **audit database**.
4. **Worker Services** (e.g., Vet Scheduler) consume RabbitMQ messages to trigger background tasks like scheduling vet checks.

### Security Layer:
- **JWT Tokens** are used for secure communication between the frontend and backend.
- **Authorization** middleware verifies tokens on every API request.

### Layered Architecture within Services:
Each backend microservice follows a **Clean Architecture (Onion Architecture)** pattern with these layers:

- **Domain Layer**:
  - Contains core business entities, aggregates, value objects, and interfaces (e.g., `Monkey`, `Shelter`, `IVetCheckScheduler`).
  - Independent of external dependencies.

- **Application Layer**:
  - Contains business logic, service interfaces, use cases (e.g., `MonkeyArrivalHandler`, `VetCheckScheduler`).
  - Depends on the **Domain Layer**.

- **Infrastructure Layer**:
  - Contains implementations for persistence, messaging (e.g., EF Core repositories, MassTransit configuration).
  - Depends on **Application** and **Domain** layers but not vice versa.

- **Presentation Layer**:
  - In **API** projects only, defines the HTTP endpoints.
  - Coordinates requests through **Application Layer**.

This ensures:
- **Separation of concerns**.
- **Testability**.
- **Scalability** (each service independently maintainable).

---

## 🛠️ Backend Setup

### Tech Stack:
- **.NET 9** (Minimal APIs, Dependency Injection)
- **MassTransit + RabbitMQ** (Message-based communication)
- **PostgreSQL** (Database)
- **Redis** (Cache)
- **Entity Framework Core** (Data access)

### Projects:
- `MonkeyShelter.Api` (Main API)
- `MonkeyShelter.Auth` (Authentication microservice)
- `MonkeyShelter.Reports` (Reporting microservice)
- `MonkeyShelter.Workers` (Background services like Vet Scheduler)

### Key Concepts:
- **Monkeys** have:
  - `Id`, `Name`, `SpeciesId`, `Weight`, `ShelterId`, `ArrivalDate`
- **Shelters** hold monkeys, limited by species-specific constraints.
- **Vet Checks** scheduled every 60 days.

### Endpoints:
| Endpoint                         | Method | Description                           |
|----------------------------------|--------|---------------------------------------|
| `/monkeys`                       | GET    | List all monkeys                      |
| `/monkeys`                       | POST   | Add a new monkey                      |
| `/monkeys/`      | DELETE | Depart a monkey (with body payload)   |
| `/monkeys/`         | PUT    | Update a monkey's weight              |
| `/species`                       | GET    | List species                          |
| `/shelters`                      | GET    | List shelters                         |

---

## 🛍️ Frontend Setup (Angular)

### Tech Stack:
- **Angular 19** (Standalone components)
- **Angular Material** (UI components)

### Components:
- `LoginComponent` / `RegisterComponent` (Auth forms)
- `DashboardComponent` (Main user area)
- `MonkeyListComponent` (CRUD interface for monkeys)
- `ReportsComponent` (Displays reports -> all and in date rage)
- `AuditComponent` (Displays all audit logs)

### Features:
- 🔐 **JWT Authentication**
- 🌐 **Responsive Material UI**
- 💡 **Dialogs** for creating/updating monkeys

### Frontend Routes:
| Route        | Component            | Auth Protected |
|--------------|-----------------------|----------------|
| `/auth`      | Login/Register Tabs   | No             |
| `/dashboard` | Dashboard + Monkey UI | Yes            |

---

## 🔑 Authentication & Security
- **JWT Tokens** issued via `/auth/login` and `/auth/register`.
- Stored in **localStorage** as `auth_token`.
- All **API requests** attach `Authorization: Bearer <token>`.

---

## 📢 RabbitMQ & MassTransit
- **MassTransit** is configured in each microservice.
- **IPublishEndpoint** is used for sending messages (e.g., audit logs).
- **Audit Microservice** listens on `audit-queue`.

---

## 📆 Scheduling (Vet Checks)
- **Background service** schedules vet checks every **60 days**.
- Uses **MassTransit messages** to trigger vet actions.

---

## 📊 Reports & Audits
- **Audit microservice** listens to events like **MonkeyArrived**.
- Logs:
  - `EventType`
  - `Payload`
  - `ReceivedAt`
- **Reports** generated via queries (e.g., monkeys by species, arrivals by date).

---

## 🛠️ Development Commands

### Backend:
```bash
cd MonkeyShelter

docker compose up --build -d

```

### Frontend:
```bash
# Run Angular client
cd client
npm install
ng serve
```

---

## 🔧 TODO / Improvements
- [ ] Add **pagination** and **sorting** to monkey list
- [ ] Enhance **error handling** on frontend forms
- [ ] Implement **role-based access control** for managers and shelters
- [ ] Add **integration tests** 

