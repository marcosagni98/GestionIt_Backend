# Incident Management API - Clean Architecture in .NET 8

This repository contains the API for an incident management application, developed in **.NET 8** following the principles of **Clean Architecture**. The API communicates with a **React frontend** (in a separate repository) and uses **SignalR** to implement real-time chat. The backend, frontend, and database are all configured to run in separate **Docker containers**.

## Table of Contents
1. [Installation and Usage](#installation-and-usage)
2. [Architecture](#architecture)
3. [Implemented Patterns](#implemented-patterns)
   - [Repository Pattern](#repository-pattern)
   - [Dependency Injection](#dependency-injection)
   - [Result Pattern](#result-pattern)
   - [Exception Handler](#exception-handler)
   - [Error Details](#error-details)
   - [Generic Repositories](#generic-repositories)
   - [AutoMapper](#automapper)
   - [Fluent Validation](#fluent-validation)
   - [SignalR for Chat](#signalr-for-chat)
4. [Project Structure](#project-structure)
5. [Contributions](#contributions)
   
### Installation Steps

This project is configured to run with Docker. Each component (API, frontend, and database) runs in a separate container.

1. Clone the repository:

   ```bash
   git@github.com:marcosagni98/GestionIt_Backend.git
   ```
   
2. Navigate to the project directory:
   
   ```bash
   cd GestionIt_Backend
   ```
       
3. Configure the JWTSettings in the `SISINF_Backend\src\API\appsettings.json` file in the API layer.

4. Configure the `SISINF_Backend\.env` file

Example:
   ```bash
   DB_USER=sa
   DB_PASSWORD=password@12345#
   EMAIL=email@gmail.com
   EMAIL_PASSWORD=password
   BASE_URL=https://localhost
   REACT_APP_API_BASE_URL=http://localhost:5000
   REACT_APP_DEBUG=true
   ```

> **Note:** For the email and email password, [follow this tutorial
](https://www.hostpapa.com/knowledgebase/how-to-create-and-use-google-app-passwords/)
5. Make sure Docker is installed and running.

6. Run the following command in the project root:

7. Execute the project:
   
    ```bash
    docker-compose up --build
    ```

## Architecture

This project follows the **Clean Architecture** to maintain modular and easily testable code. Responsibilities are separated into the following layers:

1. **API**: Presentation layer containing controllers and SignalR configuration.
2. **Application**: Contains business logic, services, and validations.
3. **Domain**: Defines the main entities and interfaces for repositories.
4. **Infrastructure**: Implements data access details, repositories, and other external services.

## Implemented Patterns

### Repository Pattern

The **Repository Pattern** abstracts database access, allowing business logic to be independent of infrastructure. Repositories are implemented in the **Infrastructure** layer and their interfaces are defined in the **Domain**.

### Dependency Injection

**Dependency Injection (DI)** is managed through the .NET 8 dependency container. All dependencies are configured in the `Program.cs` file of the **API** layer.

### Result Pattern

The **Result Pattern** facilitates handling operation responses, allowing both the result of an operation and its status (success, error, etc.) to be returned using the `Result<T>` type.

### Exception Handler

A global **Exception Handler** has been implemented to capture exceptions and return uniform responses from the API. This ensures centralized error handling.

### Error Details

The `ErrorDetails` object is used to return details of errors that occur in the API, following a standard format in error responses.

### Generic Repositories

**Generic Repositories** are used to implement common CRUD operations for domain entities. This reduces code duplication and centralizes data access logic.

### AutoMapper

**AutoMapper** is used to map between domain entities and DTOs. The AutoMapper configuration is found in `Program.cs` of the **API** layer.

### Fluent Validation

**Fluent Validation** is used to validate data models. Validation rules are separated from controllers, following the single responsibility principle.

### SignalR for Chat
/src /API - Controllers, SignalR configuration, and API entry point. /Application - Business logic, validations, and services. /Domain - Main entities and repository contracts. /Infrastructure - Implementation of repositories, database access, and other services.

**SignalR** is implemented to enable real-time chat between application users. SignalR is configured in the **API** layer and communicates with the React frontend.

## Project Structure
![image](https://github.com/user-attachments/assets/a924da72-e3b0-4661-ab7f-bd18092b1ae0)

## Installation and Usage

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) as the database.
- [Node.js](https://nodejs.org/) and [npm](https://www.npmjs.com/) for the frontend (if running locally).

## Testing

To be implemented

## Contributions

If you want to contribute to this project, follow these steps:

1. Fork the repository.
2. Create a new branch (git checkout -b feature/new-functionality).
3. Make your changes and commit them (git commit -m 'Add new functionality').
4. Push to the branch (git push origin feature/new-functionality).
5. Submit a pull request.

Your contributions are welcome!
