# API de Gestión de Incidencias - Clean Architecture en .NET 8

Este repositorio contiene la API para una aplicación de gestión de incidencias, desarrollada en **.NET 8** siguiendo los principios de **Clean Architecture**. La API se comunica con un **frontend en React** (en un repositorio separado) y utiliza **SignalR** para implementar un chat en tiempo real. Tanto el backend, frontend como la base de datos están configurados para ejecutarse en **contenedores Docker** separados.

## Tabla de Contenidos

1. [Arquitectura](#arquitectura)
2. [Patrones Implementados](#patrones-implementados)
   - [Repository Pattern](#repository-pattern)
   - [Inyección de Dependencias](#inyección-de-dependencias)
   - [Pattern Result](#pattern-result)
   - [Exception Handler](#exception-handler)
   - [Error Details](#error-details)
   - [Repositorios Genéricos](#repositorios-genéricos)
   - [AutoMapper](#automapper)
   - [Fluent Validation](#fluent-validation)
   - [SignalR para Chat](#signalr-para-chat)
3. [Estructura del Proyecto](#estructura-del-proyecto)
4. [Instalación y Uso](#instalación-y-uso)
5. [Docker](#docker)
6. [Contribuciones](#contribuciones)


## Arquitectura

Este proyecto sigue la **Clean Architecture** para mantener un código modular y fácilmente testeable. Las responsabilidades están separadas en las siguientes capas:

1. **API**: Capa de presentación que contiene los controladores y la configuración de SignalR.
2. **Application**: Contiene la lógica de negocio, los servicios y validaciones.
3. **Domain**: Define las entidades principales y las interfaces para los repositorios.
4. **Infrastructure**: Implementa los detalles de acceso a datos, repositorios y otros servicios externos.

## Patrones Implementados

### Repository Pattern

El **Repository Pattern** abstrae el acceso a la base de datos, permitiendo que la lógica de negocio no dependa de la infraestructura. Los repositorios se implementan en la capa de **Infrastructure** y sus interfaces están definidas en **Domain**.

### Inyección de Dependencias

La **Inyección de Dependencias (DI)** se gestiona mediante el contenedor de dependencias de .NET 8. Se configuran todas las dependencias en el archivo `Program.cs` de la capa **API**.

### Pattern Result

El **Pattern Result** facilita el manejo de respuestas de las operaciones, permitiendo devolver tanto el resultado de una operación como su estado (éxito, error, etc.) mediante el tipo `Result<T>`.

### Exception Handler

Se ha implementado un **Exception Handler** global para capturar excepciones y devolver respuestas uniformes desde la API. Esto asegura un manejo centralizado de errores.

### Error Details

El objeto `ErrorDetails` se utiliza para devolver detalles de los errores ocurridos en la API, siguiendo un formato estándar en las respuestas de error.

### Repositorios Genéricos

Se utilizan **Repositorios Genéricos** para implementar operaciones CRUD comunes para las entidades del dominio. Esto reduce la duplicación de código y centraliza la lógica de acceso a datos.

### AutoMapper

**AutoMapper** se utiliza para mapear entre entidades de dominio y DTOs. La configuración de AutoMapper se encuentra en `Program.cs` de la capa **API**.

### Fluent Validation

**Fluent Validation** se utiliza para validar los modelos de datos. Las reglas de validación están separadas de los controladores, siguiendo el principio de responsabilidad única.

### SignalR para Chat
/src /API - Controladores, configuración de SignalR y punto de entrada de la API. /Application - Lógica de negocio, validaciones y servicios. /Domain - Entidades principales y contratos de repositorios. /Infrastructure - Implementación de los repositorios, acceso a base de datos y otros servicios.

Se implementa **SignalR** para habilitar el chat en tiempo real entre los usuarios de la aplicación. SignalR se configura en la capa **API**, y se comunica con el frontend en React.

## Estructura del Proyecto

## Instalación y Uso

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) como base de datos.
- [Node.js](https://nodejs.org/) y [npm](https://www.npmjs.com/) para el frontend (si lo ejecutas localmente).

### Pasos para la Instalación

1. Clonar el repositorio:

   ```bash
   git clone https://github.com/marcosagni98/SISINF_Backend.git
   
2. Navegar al directorio del proyecto:
   
   ```bash
   cd src/API

3. Restaurar los paquetes NuGet:
   
     ```bash
    dotnet restore
     
4. Configurar la cadena de conexión a la base de datos SQL Server en el archivo `appsettings.json` de la capa API.

5. Ejecutar las migraciones para la base de datos:

    ```bash
   dotnet ef database update --project Infrastructure

6. Ejecutar el proyecto:
   
    ```bash
    dotnet run --project API
## Docker

Este proyecto está configurado para ejecutarse con Docker. Cada componente (API, frontend y base de datos) se ejecuta en un contenedor separado.

### Docker Compose

Para levantar todo el entorno usando Docker Compose:

1. Asegúrate de que Docker esté instalado y corriendo.
2. Ejecuta el siguiente comando en la raíz del proyecto:

    ```bash
    docker-compose up --build

 Esto levantará tres contenedores:

   - API (Backend): La API de .NET 8.
   - Frontend: La aplicación de React.
   - Base de datos: Un contenedor de SQL Server.

### Variables de Entorno

Asegúrate de configurar las variables de entorno necesarias para la conexión a la base de datos y otros servicios en el archivo docker-compose.yml y appsettings.json.

## Pruebas

To be implemented

## Contribuciones

Si deseas contribuir a este proyecto, sigue estos pasos:

1. Haz un fork del repositorio.
2. Crea una nueva rama (git checkout -b feature/nueva-funcionalidad).
3. Realiza tus cambios y haz commit (git commit -m 'Añadir nueva funcionalidad').
4. Haz push a la rama (git push origin feature/nueva-funcionalidad).
5. Envía un pull request.

¡Tus contribuciones son bienvenidas!
