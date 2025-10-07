# 🚀 Quick Start Guide - Notes App

This is a condensed version for quick reference. See `README_NOTES_APP.md` for full documentation.

## ⚡ Prerequisites
- .NET 8 SDK
- SQL Server LocalDB (or SQL Server)

## 🏃 Quick Setup (5 minutes)

```bash
# 1. Clone repository
git clone <your-repo-url>
cd NotesApp

# 2. Restore packages (automatic)
dotnet restore

# 3. Create database
dotnet ef database update

# 4. Run application
dotnet run
```

## 🎨 Color Coding
- 🔴 High Priority → Red
- 🟡 Medium Priority → Orange/Yellow
- 🔵 Low Priority → Blue/Cyan

## 🆘 Quick Fixes

### Database Error
```bash
sqllocaldb start mssqllocaldb
dotnet ef database update
```

### EF Tools Missing
```bash
dotnet tool install --global dotnet-ef
```

## 📂 Key Files
- `Models/Note.cs` - Data model
- `Services/NoteService.cs` - Business logic
- `Components/Pages/Notes.razor` - UI
- `appsettings.json` - Connection string

## 🔧 Commands Cheat Sheet

| Action | Command |
|--------|---------|
| Run app | `dotnet run` |
| Build | `dotnet build` |
| Create migration | `dotnet ef migrations add <Name>` |
| Apply migrations | `dotnet ef database update` |
| List migrations | `dotnet ef migrations list` |
| Reset DB | `dotnet ef database drop` |


---
✨ For complete documentation, see `README_NOTES_APP.md`
