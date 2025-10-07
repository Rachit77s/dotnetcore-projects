# 📝 Notes Application

A modular, maintainable Blazor Server web application for creating and managing notes with clean architecture principles.

## ✨ Features

- ✅ Create, Read, Update, and Delete notes
- ✅ Inline editing (edit notes in the same form)
- ✅ Priority-based color coding (High, Medium, Low)
- ✅ Reverse chronological order display
- ✅ Responsive card layout
- ✅ Timestamp tracking (Created & Updated)
- ✅ Form validation
- ✅ Clean Architecture with separation of concerns

## 🎨 Priority Color Scheme

- 🔴 **High Priority**: Red border and badge
- 🟡 **Medium Priority**: Yellow/Orange border and badge
- 🔵 **Low Priority**: Blue/Cyan border and badge

## 🛠️ Technology Stack

- **.NET 8**
- **Blazor Server** (Interactive Server Render Mode)
- **Entity Framework Core 9.0.9**
- **SQL Server LocalDB**
- **Bootstrap 5**

## 📋 Prerequisites

Before running this project, ensure you have the following installed:

1. **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** or later
2. **SQL Server LocalDB** (included with Visual Studio or install separately)
   - **Visual Studio users**: Already included with Visual Studio installation
   - **VS Code/CLI users**: [Download SQL Server Express LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
3. **Git** (for cloning the repository)

## 🚀 Getting Started

### Step 1: Clone the Repository

```bash
git clone <your-repository-url>
cd NotesApp
```

### Step 2: Restore NuGet Packages

The project will automatically restore packages when you build. To manually restore:

```bash
dotnet restore
```

**Packages included:**
- `Microsoft.EntityFrameworkCore.SqlServer` (9.0.9)
- `Microsoft.EntityFrameworkCore.Tools` (9.0.9)

### Step 3: Configure Database Connection (Optional)

The default connection string in `appsettings.json` uses SQL Server LocalDB:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=NotesAppDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

**To use a different SQL Server instance**, update the connection string accordingly.

### Step 4: Create the Database

Apply Entity Framework migrations to create the database:

**Option A: Using .NET CLI (Recommended)**
```bash
dotnet ef database update
```

**Option B: Using Package Manager Console (Visual Studio)**
```powershell
Update-Database
```

**Note:** If `dotnet ef` command is not found, install the EF Core tools globally:
```bash
dotnet tool install --global dotnet-ef
```

### Step 5: Run the Application

**Option A: Using .NET CLI**
```bash
dotnet run
```

**Option B: Using Visual Studio**
- Press **F5** to run with debugging
- Press **Ctrl+F5** to run without debugging

**Option C: Using VS Code**
- Press **F5** or use the Run menu

### Step 6: Access the Application

Once running, the application will be available at:
- **HTTPS**: `https://localhost:7xxx` (exact port shown in console)
- **HTTP**: `http://localhost:5xxx` (exact port shown in console)

**Navigate to the Notes page:**
- Click **"Notes"** in the navigation menu, OR
- Go directly to `/notes` in your browser

## 📁 Project Structure

```
NotesApp/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor          # Main layout wrapper
│   │   └── NavMenu.razor             # Navigation menu
│   ├── Pages/
│   │   ├── Counter.razor             # Sample counter page
│   │   ├── Error.razor               # Error page
│   │   ├── Home.razor                # Home page
│   │   ├── Notes.razor               # ⭐ Main notes page
│   │   └── Weather.razor             # Sample weather page
│   ├── App.razor                     # Root component
│   ├── Routes.razor                  # Routing configuration
│   └── _Imports.razor                # Global using statements
├── Data/
│   └── ApplicationDbContext.cs       # EF Core DbContext
├── Migrations/                       # EF Core migrations
│   ├── xxxxx_InitialCreate.cs        # Initial database migration
│   └── ApplicationDbContextModelSnapshot.cs
├── Models/
│   └── Note.cs                       # Note entity & Priority enum
├── Services/
│   ├── INoteService.cs               # Service interface
│   └── NoteService.cs                # Service implementation
├── wwwroot/                          # Static files
│   └── css/
│       └── notes.css                 # Custom styling for notes
├── appsettings.json                  # Configuration & connection string
├── Program.cs                        # Application entry point & DI config
├── NotesApp.csproj                   # Project file
└── README.md                         # This file
```

## 🏗️ Architecture Overview

### Clean Architecture Layers

1. **📦 Models Layer** (`Models/`)
   - Domain entities (Note, Priority enum)
   - Data annotations for validation
   - Business rules

2. **💾 Data Layer** (`Data/`)
   - Entity Framework Core DbContext
   - Database configuration
   - Entity relationships

3. **⚙️ Service Layer** (`Services/`)
   - Business logic abstraction
   - CRUD operations
   - Data access logic

4. **🎨 Presentation Layer** (`Components/`)
   - Blazor components
   - UI/UX implementation
   - User interactions

### Key Design Patterns

- **Dependency Injection**: Services registered in `Program.cs`
- **Repository Pattern**: Via service layer abstraction
- **Code-First Approach**: Database created from C# models
- **Async/Await**: All database operations are asynchronous

## 💾 Database Schema

### Notes Table

| Column     | Type           | Constraints              | Description                    |
|------------|----------------|--------------------------|--------------------------------|
| Id         | int            | PRIMARY KEY, IDENTITY    | Auto-incrementing primary key  |
| Title      | nvarchar(100)  | NOT NULL                 | Note title (required)          |
| Content    | nvarchar(2000) | NOT NULL                 | Note content (required)        |
| CreatedAt  | datetime2      | NOT NULL                 | Creation timestamp (UTC)       |
| UpdatedAt  | datetime2      | NULL                     | Last update timestamp (UTC)    |
| Priority   | int            | NOT NULL                 | 0=Low, 1=Medium, 2=High        |

**Indexes:**
- Clustered index on `Id` (Primary Key)
- Non-clustered index on `CreatedAt` (for sorting performance)

## 🧪 Usage Guide

### Creating a Note

1. Navigate to the **Notes** page
2. Fill in the form:
   - **Title**: Enter a descriptive title (max 100 characters)
   - **Content**: Enter your note content (max 2000 characters)
   - **Priority**: Select Low, Medium, or High
3. Click **"Add Note"** button

### Editing a Note

1. Find the note you want to edit in the list
2. Click the **"Edit"** button
3. The form will populate with the note's current data
4. Make your changes
5. Click **"Update Note"** to save or **"Cancel"** to discard

### Deleting a Note

1. Find the note you want to delete
2. Click the **"Delete"** button
3. The note will be removed immediately

### Sample Notes for Testing

Try these examples to see the color-coding in action:

**High Priority Note:**
- Title: `Project Deadline - Q4 Report`
- Content: `Complete the quarterly financial report by Friday. Need to review budget allocations...`
- Priority: High

**Medium Priority Note:**
- Title: `Weekly Team Sync`
- Content: `Discuss sprint progress, address blockers, and plan for next week's deliverables...`
- Priority: Medium

**Low Priority Note:**
- Title: `Books to Read`
- Content: `Check out "Clean Architecture" by Robert C. Martin and "Domain-Driven Design"...`
- Priority: Low

## 🔧 Troubleshooting

### Issue: Database connection fails

**Error:** `Cannot open database "NotesAppDb"...`

**Solution:**
1. Verify SQL Server LocalDB is installed: `sqllocaldb info`
2. Check connection string in `appsettings.json`
3. Try starting LocalDB: `sqllocaldb start mssqllocaldb`

### Issue: Migration not found or not applied

**Error:** `No migrations found...` or `Database does not exist...`

**Solution:**
```bash
# Check if migrations exist
dotnet ef migrations list

# If no migrations, create one
dotnet ef migrations add InitialCreate

# Apply migrations
dotnet ef database update --verbose
```

### Issue: Port already in use

**Error:** `Failed to bind to address...`

**Solution:**
- Stop other applications using the same port
- Modify ports in `Properties/launchSettings.json`

### Issue: NuGet packages not restored

**Error:** `The type or namespace name '...' could not be found`

**Solution:**
```bash
dotnet clean
dotnet restore
dotnet build
```

### Issue: EF Core tools not found

**Error:** `dotnet ef: command not found`

**Solution:**
```bash
# Install globally
dotnet tool install --global dotnet-ef

# Or update if already installed
dotnet tool update --global dotnet-ef
```

## 🔄 Database Management

### View Applied Migrations
```bash
dotnet ef migrations list
```

### Add a New Migration
After modifying models:
```bash
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

### Remove Last Migration
```bash
dotnet ef migrations remove
```

### Reset Database
⚠️ **Warning:** This will delete all data!
```bash
dotnet ef database drop
dotnet ef database update
```

### Generate SQL Script
To see what SQL will be executed:
```bash
dotnet ef migrations script
```

## ✨ Best Practices Implemented

- ✅ **Async/Await**: All database operations are asynchronous
- ✅ **Dependency Injection**: Proper DI container usage
- ✅ **Separation of Concerns**: Clear layer boundaries
- ✅ **Data Validation**: Model-level validation with attributes
- ✅ **Error Handling**: Proper null checks and validation
- ✅ **Clean Code**: Meaningful names and single responsibility
- ✅ **Nullable Reference Types**: Enabled for better null safety
- ✅ **UTC Timestamps**: Consistent timezone handling
- ✅ **Indexed Queries**: Database indexes for performance
- ✅ **Code-First Migrations**: Version-controlled database schema

