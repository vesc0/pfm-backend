# Personal Finance Management Backend

A .NET 10 application for managing personal finances with transaction tracking, categorization, and analytics.

## Contents

- [Features](#features) 
- [Tech Stack](#tech-stack)  
- [Getting Started](#getting-started)
- [Docker Support](#docker-support)   

## Features

- **Transaction Management**: List, filter, categorize, auto-categorize, and split financial transactions.
- **Clean Architecture & CQRS**: Separation of concerns by following Clean Architecture and CQRS design patterns.

## Tech Stack

- **Framework**: .NET 10
- **Database**: PostgreSQL (via EF Core)

## Getting Started

### Prerequisites:
- **.NET 8 SDK**
- **PostgreSQL** (if running locally without Docker)
- **Docker & Docker Compose** (optional, for containerized setup)

Clone the repo:
```bash
git clone https://github.com/vesc0/pfm-backend.git
cd pfm-backend
```

Build the project:
```bash
dotnet build
```

Start the server:
```bash
dotnet run --project PFM.API
```

Swagger support:
- Open [http://localhost:5000/swagger](http://localhost:5000/swagger) in your browser to explore the API.

## Docker Support

```bash
# Build and run with Docker Compose
docker-compose up --build
```
- This will start both the API and PostgreSQL database in containers.
