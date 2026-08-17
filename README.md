#🚚 LogisticCompany

A full-stack logistics management system built with **Blazor Server**, **ASP.NET Core Web API**, and **Flutter**.
The system covers the entire delivery lifecycle — from order creation to real-time tracking — with a cross-platform mobile app for clients.

## 📦 Project Structure

LogisticCompany/
├── LogisticCompany/        # Blazor Server — web application
├── LogisticCompany.Api/    # ASP.NET Core Web API — mobile backend
└── logistic_company_app/   # Flutter — cross-platform mobile app (Android & iOS)


## ✨ Features

### 🌐 Web Application (Blazor Server)
- **Role-based access** — Main Admin, Branch Manager, Client
- **Order management** — multi-step order creation with price calculation
- **Client management** — individual and company clients
- **Employee management** — staff and branch administration
- **Real-time tracking** — order status updates with location history
- **Smart delivery routing** — ground transport via OpenRouteService API, air transport via Haversine formula
- **Air-only routes** — automatically restricts transport type for Kazakhstan ↔ China and Russia ↔ China routes
- **Payment processing** — manager confirms payment at branch
- **PDF preview** — order summary before submission
- **Security** — BCrypt password hashing, cookie authentication, rate limiting, HSTS

### 📱 Mobile Application (Flutter)
- **JWT authentication** — secure token-based login
- **Order list** — all client orders with pull-to-refresh
- **Order details** — full information: route, recipient, parcel, payment, status
- **Tracking timeline** — visual history of order statuses and locations
- **Profile management** — view profile and change password
- **Secure storage** — JWT token stored in device Keychain / Keystore

### ⚙️ REST API (ASP.NET Core)
- JWT Bearer authentication
- Endpoints: auth, orders, tracking, profile
- Swagger UI documentation
- Clean architecture — controllers use services, no direct DB access

## 🛠 Tech Stack

| Layer | Technology |
|---|---|
| Web Frontend | Blazor Server (.NET 9) |
| Web Backend | ASP.NET Core, EF Core |
| Database | MySQL |
| Mobile | Flutter (Dart) |
| API | ASP.NET Core Web API |
| Auth (Web) | Cookie Authentication |
| Auth (Mobile) | JWT Bearer |
| Maps | OpenRouteService API, OpenStreetMap |
| Password | BCrypt.Net |
| HTTP Client | Dio (Flutter) |
| Secure Storage | flutter_secure_storage |


## 🏗 Architecture

The project follows a **layered clean architecture**:

UI Layer (Blazor / Flutter)
        ↓
Application Layer (Services, Interfaces, DTOs)
        ↓
Domain Layer (Entities)
        ↓
Infrastructure Layer (EF Core, DbContext, External APIs)

- All business logic lives in `Application/Services`
- UI components only call service interfaces — no direct DB access
- API controllers use the same services as the Blazor app
- DTOs separate input models from output models


## 🗺 Distance Calculation

| Route type | Method |
|---|---|
| Ground transport | OpenRouteService API (real road distance) |
| Air transport | Haversine formula (great-circle distance) |
| API unavailable | Haversine × 1.25 (fallback) |
| Kazakhstan ↔ China | Air only |
| Russia ↔ China | Air only |

## 👥 Roles

| Role | Access |
|---|---|
| `MainAdmin` | Full system access |
| `Admin` | Branch management, employees, orders |
| `Manager` | Branch orders, tracking updates |
| `User` | Own orders and profile (web + mobile) |

## 📸 Screenshots web service

Main screen
<img width="750" height="418" alt="image" src="https://github.com/user-attachments/assets/22d8d721-843a-42f0-a30d-df666b189918" />
Login Screen
<img width="760" height="426" alt="image" src="https://github.com/user-attachments/assets/85131886-7a46-4724-b0bb-b1148d814b37" />
Register screen
<img width="798" height="417" alt="image" src="https://github.com/user-attachments/assets/c2d1f09a-6c4d-4a8f-ad3a-2af320a9777f" />
Add client in system
<img width="578" height="289" alt="image" src="https://github.com/user-attachments/assets/f25cd6a0-bd56-41db-8ae5-41d00b8b6ead" />
List client
<img width="707" height="421" alt="image" src="https://github.com/user-attachments/assets/baed1178-e3a9-4f4d-9092-86a16fa09b18" />
List order
<img width="756" height="406" alt="image" src="https://github.com/user-attachments/assets/42341548-27b4-4582-a518-33033a06d113" />
Create order
<img width="671" height="309" alt="image" src="https://github.com/user-attachments/assets/e269a9ba-a03f-4237-8415-632c431d9917" />
Manager account
<img width="947" height="331" alt="image" src="https://github.com/user-attachments/assets/091b8e20-3132-4ce3-adc3-5658d5070328" />
Client account
<img width="943" height="393" alt="image" src="https://github.com/user-attachments/assets/54e5d3e4-0067-4a87-8420-b50374309a2f" />
<img width="934" height="358" alt="image" src="https://github.com/user-attachments/assets/b16608bb-5496-42f5-be24-14372319c548" />
<img width="939" height="386" alt="image" src="https://github.com/user-attachments/assets/ee73aa80-3210-4f7c-b0ef-e4da31650a3e" />
<img width="529" height="418" alt="image" src="https://github.com/user-attachments/assets/85827256-dd19-4793-abd0-9b21a132cde0" />
<img width="511" height="394" alt="image" src="https://github.com/user-attachments/assets/03145773-d503-47e8-9a2b-5f1893496304" />

## 📸 Screenshots mobile
<img width="240" height="533" alt="image" src="https://github.com/user-attachments/assets/6cbd6bb2-65bb-4e2e-977a-7695396b1e1f" />
<img width="240" height="533" alt="image" src="https://github.com/user-attachments/assets/90dce557-bc87-4097-8d4d-7c95d2dd4dfa" />
<img width="240" height="533" alt="image" src="https://github.com/user-attachments/assets/49169aca-1e7f-4176-b1e8-c930eef9ef41" />
<img width="240" height="533" alt="image" src="https://github.com/user-attachments/assets/c92e9b9d-07c1-4895-b138-4b5aa93c5a98" />
<img width="240" height="533" alt="image" src="https://github.com/user-attachments/assets/98606558-a1d1-4643-b88e-b79656b7afe5" />
<img width="240" height="533" alt="image" src="https://github.com/user-attachments/assets/c7cdc7ff-e087-41e0-84fc-afc0a4c8f40a" />
## 👩‍💻 Author

Built with ❤️ as a pet project to explore full-stack development with .NET and Flutter.
