# TeamShift Lite Warehouse Frontend

A lightweight React single-page app that sits on top of the existing ASP.NET Core warehouse API. It exercises ReqRes-backed login plus GET/POST/PUT/PATCH flows for employees, resources, and warehouse assignments without any database.

## Getting started

1. **Run the API** (from the solution root):
   ```bash
   dotnet run --project ReqresIntegratedApplication.WebAPI/ReqresIntegratedApplication.WebAPI.csproj
   ```
   The launch profile uses `http://localhost:5135`.

2. **Install UI dependencies** (from this folder):
   ```bash
   npm install
   ```

3. **Start the React dev server**:
   ```bash
   npm run dev
   ```
   Vite proxies `/api/*` to `http://localhost:5135` so calls avoid CORS during development.

4. **Build for production** (optional):
   ```bash
   npm run build
   ```

## Key screens
- **Login**: Calls `POST /api/auth/login` with the ReqRes demo credentials and caches the token in memory/localStorage.
- **Dashboard**: Shows the workforce snapshot from `/api/warehouse/workforce-summary` and `/api/warehouse/employees`.
- **Employees**: Lists, creates (POST), updates (PUT), and patches (PATCH) users via `/api/employees` endpoints, reflecting changes locally.
- **Resources**: Lists ReqRes `unknown` records via `/api/items` and lets you inspect each item.
- **Assignments**: Promotes employees to associates and assigns item IDs using `/api/warehouse/assignments/*` endpoints; assignments are kept server-side in memory.

## Configuration
Set `VITE_API_BASE_URL` in a `.env` file if your API runs on a different host/port; otherwise the app defaults to `http://localhost:5135/api`.
