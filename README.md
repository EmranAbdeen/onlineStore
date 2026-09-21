# Online Store

An ASP.NET Core MVC e-commerce application with product browsing, a shopping cart, user accounts, and an admin panel for managing categories, products, and users.

## Tech Stack

- **Framework:** ASP.NET Core MVC (.NET 10)
- **ORM:** Entity Framework Core 10 (SQL Server provider)
- **Auth:** Session-based login with `BCrypt.Net-Next` for password hashing
- **Front end:** Razor views, Bootstrap 5

## Features

- **Catalog:** Browse products by category on the home page and product listing page, with product detail pages.
- **Cart:** Add items, increase/decrease quantity, update quantity directly, and remove items — cart state is tied to the logged-in user.
- **Accounts:** Register and log in. New accounts require admin approval before they can log in, and admins can block/unblock or approve/unapprove users.
- **Admin panel:**
  - Manage categories (create, delete — deleting a category cascades to its products and their cart items).
  - Manage products (create with image upload, delete).
  - Manage users (approve, unapprove, block, unblock, delete).
- **Authorization:** A custom `AuthFilter` action filter protects routes, optionally restricting them to a specific role (e.g. `"Admin"`).

## Project Structure

```
onlineStore/
├── Controllers/        # AccountController, AdminController, CartController,
│                        # CategoryController, HomeController, ProductController
├── Models/              # Cart, CartItem, Category, Product, Role, User
├── DTOs/                # LoginDto, RegisterDto
├── Database/            # database.sql (schema creation — run manually on SQL Server)
├── Data/                # OnlineStoreDbContext (EF Core)
├── Filters/             # AuthFilter (session-based role authorization)
├── Services/
│   ├── Interfaces/      # IAccountService
│   └── Implemntations/  # AccountService (login/registration, password hashing)
├── Views/                # Razor views (Account, Admin, Cart, Category, Home, Product, Shared)
└── wwwroot/              # Static assets (css, js, images, third-party libs)
```

## Data Model

- **User** — has a `Role` (e.g. `User`/`Admin`), an `IsApproved` flag, an `IsBlocked` flag, and an optional one-to-one `Cart`.
- **Role** — e.g. `User`, `Admin`.
- **Category** — has many `Product`s; unique `Name`.
- **Product** — belongs to a `Category`, has `Name`, `Description`, `Price`, `ImageUrl`.
- **Cart** — one per `User`, has many `CartItem`s.
- **CartItem** — links a `Cart` to a `Product` with a `Quantity`.

## Getting Started

### Prerequisites

- [.NET 10 SDK]
- SQL Server (local or remote) with a database for the app

### Configure the database

Update the `ConnectionOnlineStore` connection string in `appsettings.json` (and `appsettings.Development.json`) to point at your SQL Server instance:

```json
"ConnectionStrings": {
  "ConnectionOnlineStore": "Data Source=YOUR_SERVER;Initial Catalog=OnlineStoreDB;Integrated Security=True;TrustServerCertificate=True;"
}
```

> The connection string currently checked into `Data/OnlineStoreDbContext.cs` and `appsettings.json` points at a specific developer machine (`EMRAN-ABDEEN`) — replace it before running elsewhere.

The `Roles` table needs at least a `User` row (and typically an `Admin` row) since new registrations are assigned the `User` role by name.

### Run the app

```bash
git clone https://github.com/EmranAbdeen/onlineStore.git
cd onlineStore
dotnet restore
dotnet ef database update   # apply migrations, if any are added
dotnet run
```

Then open the URL shown in the console (see `Properties/launchSettings.json` for configured ports).

## Notes

- Uploaded product images are saved to `wwwroot/images` and referenced by relative URL on the `Product`.
- Session state (`UserId`, `FullName`, `Email`, `Role`, `IsBlocked`) drives both the UI and the `AuthFilter` authorization checks — there's no cookie/JWT-based auth.
- Newly registered users start as unapproved (`IsApproved = false`) and can't log in until an admin approves them.
