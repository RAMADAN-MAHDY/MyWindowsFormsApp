# MyWindowsFormsApp

## Overview
MyWindowsFormsApp is a simple restaurant table reservation management system developed using C# and Windows Forms. This application allows administrators to manage tables and reservations, and customers to make and view their reservations.

## Features
- **Admin Management**: Add and manage tables with password protection.
- **Customer Reservations**: View available tables, make reservations, and check reservation details.
- **Menu Display**: Show the restaurant menu.

## Libraries and Tools Used
The following libraries and tools were used in this project:
- `System`
- `System.Data`
- `System.Windows.Forms`
- `Microsoft.Data.SqlClient`
- `Microsoft.Extensions.Configuration`
- `System.IO`

## Database Schema
### Tables:
1. **tables**:
    ```sql
    CREATE TABLE tables (
        id INTEGER PRIMARY KEY IDENTITY(1,1),
        table_number INTEGER NOT NULL UNIQUE,
        capacity INTEGER NOT NULL,
        is_reserved BIT DEFAULT 0
    );
    ```

2. **reservations**:
    ```sql
    CREATE TABLE reservations (
        id INTEGER PRIMARY KEY IDENTITY(1,1),
        table_id INTEGER NOT NULL,
        customer_name NVARCHAR(100) NOT NULL,
        reservation_time DATETIME NOT NULL,
        duration INTEGER NOT NULL,
        FOREIGN KEY (table_id) REFERENCES tables(id)
    );
    ```

## Getting Started
### Prerequisites
- .NET SDK

### Installation
1. Clone the repository:
    ```bash
    git clone https://github.com/RAMADAN-MAHDY/MyWindowsFormsApp.git
    ```
2. Navigate to the project directory:
    ```bash
    cd MyWindowsFormsApp
    ```
3. Restore the dependencies:
    ```bash
    dotnet restore
    ```

### Running the Application
```bash
dotnet run
