# TeamShift Lite Warehouse System

A lightweight warehouse management sample that pairs an ASP.NET Core API with a React single-page app. The backend wraps the ReqRes API (https://reqres.in) for authentication, employees, and resources; the frontend consumes those endpoints for login, workforce views, resource browsing, and in-memory assignments.

## Repository layout
- `ReqresIntegratedApplication.WebAPI/` — ASP.NET Core Web API that proxies ReqRes and keeps in-memory assignments.
- `warehouse-frontend/` — Vite + React UI that talks to the API via `/api` routes (proxied in dev).

## Running the full stack locally
1. **Start the API** (from the repository root):
   ```bash
   dotnet run --project ReqresIntegratedApplication.WebAPI/ReqresIntegratedApplication.WebAPI/ReqresIntegratedApplication.WebAPI.csproj
   ```
   The default launch profile listens on `http://localhost:5135` with Swagger enabled in Development. CORS is opened to `http://localhost:5173` (Vite) and `http://localhost:4173` (Vite preview) so the SPA can call the API directly.

2. **Install frontend dependencies** (from `warehouse-frontend/`):
   ```bash
   npm install
   ```

3. **Run the React dev server**:
   ```bash
   npm run dev
   ```
   Vite now proxies `/api` to the Visual Studio HTTPS profile (`https://localhost:7216`) and disables certificate verification so the self-signed dev cert is accepted. If you run the API on a different port or scheme, set `VITE_API_BASE_URL` in a `.env` file (e.g., `VITE_API_BASE_URL=http://localhost:5135/api`).

## One-click run from Visual Studio
You can launch the API directly with the green **Run** button in Visual Studio:

1. Open `ReqresIntegratedApplication.WebAPI.sln`.
2. Right-click the **ReqresIntegratedApplication.WebAPI** project and select **Set as Startup Project** (this ensures Visual Studio uses the Web API when you press Run).
3. In the debug target dropdown (next to the green arrow), pick the **https** profile. This maps to the `https` entry in `Properties/launchSettings.json` and will start the API on `https://localhost:7216` with Swagger.
4. Press **F5** (or click the green arrow) to launch. Visual Studio will open the Swagger UI automatically.
5. Start the React frontend in a terminal (`npm run dev` from `warehouse-frontend/`) and browse to `http://localhost:5173` to exercise the API.

## How the system works
- **Authentication**: `POST /api/auth/login` now performs a local credential check to avoid upstream 401/403 errors from proxies blocking ReqRes. It accepts the demo credentials `eve.holt@reqres.in` / `cityslicka`, caches a local token in `AuthService`, and returns it to the SPA (which stores it in `localStorage`). `POST /api/auth/logout` clears server and client tokens.
- **Employees (Users)**: `GET /api/employees` fetches paged ReqRes users; `GET /api/employees/{id}` reads details. Creates/updates flow through ReqRes via `POST /api/users`, `PUT /api/users/{id}`, and `PATCH /api/users/{id}` with local in-memory updates to reflect changes immediately in the UI. `DELETE /api/employees/{id}` removes the employee from the in-memory cache and attempts the ReqRes delete for parity.
- **Items (Resources)**: `GET /api/items` and `GET /api/items/{id}` surface ReqRes `/unknown` resources, treated as warehouse items.
- **Assignments**: Warehouse associates are promoted from employees through `POST /api/warehouse/assignments/promote/{userId}`. Item assignments are stored in memory and managed with `POST /api/warehouse/assignments/{userId}/items` plus read via `GET /api/warehouse/assignments`.
- **Dashboard views**: `GET /api/warehouse/workforce-summary` and `GET /api/warehouse/employees` provide the SPA with workforce and roster snapshots for the landing dashboard.

## Production build
To produce a static bundle, run `npm run build` inside `warehouse-frontend/`; the output in `dist/` can be hosted behind any web server configured to reach the API base URL. The API itself remains stateless aside from its in-memory assignment cache—no database is required.

## Notes
- The API honors the ReqRes demo login credentials locally: `eve.holt@reqres.in` / `cityslicka` (no external call is made for login).
- Other endpoints still communicate with ReqRes; if your network blocks `reqres.in`, the API now falls back to demo employees/resources so Swagger and the SPA stay functional even when the upstream returns 401/403.
- Because assignment data is in-memory, restarting the API clears promotions/assignments; user/resource data is always refreshed from ReqRes.
