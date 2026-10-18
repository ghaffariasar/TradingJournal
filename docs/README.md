# Technical Documentation

## Architecture
This project follows the **MVC (Model-View-Controller)** pattern using **ASP.NET Core 8.0**.

### 1. Data Layer
- **ApplicationDbContext**: Handles database interactions using EF Core.
- **Models**:
    - `Trade`: Core entity for storing trade data.
    - `ApplicationUser`: Custom user identity model.
    - `SystemSetting`: Key-value store for global settings (e.g., Registration toggle).
    - `AppPermission`: Permission-based access control.

### 2. Security
- **Authentication**: Cookie-based authentication.
- **Authorization**: Role-Based Access Control (RBAC) with permission overrides.
- **Hashing**: BCrypt is used for secure password storage.
- **Validation**: Server-side and client-side validation for all inputs.
- **Anti-Forgery**: CSRF protection enabled on all POST requests.

### 3. Localization
- **Resources**: Uses `.resx` files for English and Persian translations.
- **Middleware**: Culture is detected via URL, Cookie, or Header.
- **Calendar**: Supports both Gregorian and Jalali (Persian) dates.

### 4. Frontend Components
- **Dashboard**: Integrated with `Chart.js` for data visualization.
- **Tables**: `DataTables.net` for advanced data grids.
- **Forms**: `Select2` for searchable dropdowns and `Persian Datepicker` for Jalali dates.
- **Responsiveness**: Custom CSS in `site.css` combined with Bootstrap 5 grid system.

## Database Migrations
To add a new migration:
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Folder Structure
- `Controllers/`: Application logic.
- `Models/`: Database entities and ViewModels.
- `Views/`: Razor templates.
- `wwwroot/`: Static assets (CSS, JS, Libs).
- `Resources/`: Localization files.
