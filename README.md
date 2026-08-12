# ToughGuyAuto

  

## Project Overview

  

ToughGuyAuto is a Car Maintenance Tracker built with ASP.NET Core MVC,

Entity Framework Core and SQL Server.

  

The theme of the project is vehicle ownership and maintenance management.

The application allows vehicle information, maintenance history and service

types to be stored and managed in one system.

  

The three main entities are:

  

- `Vehicle`

- `MaintenanceRecord`

- `ServiceType`

  

A vehicle can have multiple maintenance records. Each maintenance record

belongs to one vehicle and can contain multiple service types, such as an

oil change, brake service or tire rotation.

  

The project also uses ASP.NET Core Identity and role-based authorization.

Different features are available to administrators, regular users and

anonymous visitors.

  

## Technologies Used

  

- ASP.NET Core MVC

- C#

- Entity Framework Core

- SQL Server

- ASP.NET Core Identity

- Razor Views

- Bootstrap

- HTML and CSS

- Dependency Injection

- Repository Pattern

- Service Layer

- Code-First Migrations

  

## Test Accounts

  

The following accounts are created by `DbInitializer` when the application

starts.

  

### Administrator Account

  

- Email: `admin@toughguyauto.com`

- Password: `Admin123!`

- Role: `Admin`

  

### Regular User Account

  

- Email: `user@toughguyauto.com`

- Password: `User123!`

- Role: `User`

  

These credentials are included for project demonstration and testing only.



### Permission Summary

  

| Feature | Anonymous Visitor | Regular User | Administrator |

|---|:---:|:---:|:---:|

| View home page | ✅ | ✅ | ✅ |

| Register and sign in | ✅ | ✅ | ✅ |

| View own vehicles | ❌ | ✅ | ✅ |

| View all vehicles | ❌ | ❌ | ✅ |

| Create a vehicle | ❌ | ✅ | ✅ |

| Edit own vehicle | ❌ | ✅ | ✅ |

| Delete own vehicle | ❌ | ✅ | ✅ |

| Edit or delete another user's vehicle | ❌ | ❌ | ✅ |

| View own maintenance records | ❌ | ✅ | ✅ |

| View all maintenance records | ❌ | ❌ | ✅ |

| Create maintenance records | ❌ | ❌ | ✅ |

| Edit maintenance records | ❌ | ❌ | ✅ |

| Delete maintenance records | ❌ | ❌ | ✅ |

| Manage service types | ❌ | ❌ | ✅ |



## ToughGuyAuto Table Schema

  

### Vehicles

  

| Column | Type | Key | Description |

|---|---|---|---|

| `VehicleId` | `INT` | PK | Unique vehicle identifier |

| `UserId` | `NVARCHAR(450)` | FK | Identity ID of the vehicle owner |

| `Make` | `NVARCHAR(50)` |  | Vehicle manufacturer |

| `Model` | `NVARCHAR(50)` |  | Vehicle model |

| `Year` | `INT` |  | Vehicle year |

| `LicensePlate` | `NVARCHAR(20)` |  | Licence plate number |

| `VIN` | `NVARCHAR(17)` |  | Seventeen-character VIN |

| `Mileage` | `INT` |  | Current vehicle mileage |

  

### MaintenanceRecords

  

| Column | Type | Key | Description |

|---|---|---|---|

| `MaintenanceRecordId` | `INT` | PK | Unique maintenance record identifier |

| `VehicleId` | `INT` | FK | Vehicle connected to the record |

| `ServiceDate` | `DATETIME2` |  | Date of maintenance |

| `Mileage` | `INT` |  | Mileage at the time of service |

| `Cost` | `DECIMAL(10,2)` |  | Total maintenance cost |

| `Description` | `NVARCHAR(500)` |  | Description of the maintenance work |

| `Notes` | `NVARCHAR(1000)` |  | Optional additional information |

  

### ServiceTypes

  

| Column | Type | Key | Description |

|---|---|---|---|

| `ServiceTypeId` | `INT` | PK | Unique service type identifier |

| `Name` | `NVARCHAR(100)` | UNIQUE | Name of the service |

| `Description` | `NVARCHAR(500)` |  | Description of the service |


## Interfaces, Services and Repositories

This project uses interfaces, services and repositories to separate
different responsibilities in the application.

The main request flow is:

1. The View sends form data to a Controller.
2. The Controller calls a Service through a service interface.
3. The Service checks the business rules.
4. The Service calls a Repository through a repository interface.
5. The Repository uses EF Core and DbContext to access SQL Server.
6. The result returns through the same layers to the Controller.
7. The Controller returns a View or redirects to another action.

For example, when a vehicle is created, the request follows this path:

`Create View → VehiclesController → IVehicleService → VehicleService → IVehicleRepository → VehicleRepository → DbContext → SQL Server`

### Interfaces

An interface is a contract that defines which methods a class must provide,
but it does not contain the main implementation of those methods.

For example:


public interface IVehicleRepository
{
    Task<List<Vehicle>> GetAllAsync();

    Task<Vehicle?> GetByIdAsync(int id);

    Task AddAsync(Vehicle vehicle);

    Task UpdateAsync(Vehicle vehicle);

    Task DeleteAsync(int id);
}