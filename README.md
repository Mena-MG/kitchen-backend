# 🍳 Kitchen Backend API (`kitchen-backend`)

> **Comprehensive E-Commerce & Workshop Backend Web API for Aura Kitchen Market.**  
> Built with **ASP.NET Core 8**, **Entity Framework Core**, **SQLite**, and **JWT Authentication**.

---

## 📑 Table of Contents
- [✨ Key Features](#-key-features)
- [🛠️ Tech Stack](#️-tech-stack)
- [📁 Project Structure](#-project-structure)
- [🚀 Quickstart & Local Setup](#-quickstart--local-setup)
- [🔑 Default Credentials & Seeding](#-default-credentials--seeding)
- [📖 Swagger / OpenAPI Documentation](#-swagger--openapi-documentation)
- [📡 API Endpoints Reference](#-api-endpoints-reference)
  - [1. Authentication (`/api/auth`)](#1-authentication-apiauth)
  - [2. Products & Materials (`/api/products`)](#2-products--materials-apiproducts)
  - [3. Categories (`/api/categories`)](#3-categories-apicategories)
  - [4. Orders & Checkout (`/api/orders`)](#4-orders--checkout-apiorders)
  - [5. Promo Codes (`/api/promos`)](#5-promo-codes-apipromos)
  - [6. Reviews & Ratings (`/api/reviews`)](#6-reviews--ratings-apireviews)
  - [7. Showrooms & Workshops (`/api/locations`)](#7-showrooms--workshops-apilocations)
  - [8. Store Settings (`/api/settings`)](#8-store-settings-apisettings)
  - [9. Media & File Uploads (`/api/media`)](#9-media--file-uploads-apimedia)
- [💻 Frontend Integration Guide (React / Vue / Next.js / Mobile)](#-frontend-integration-guide)
  - [Base API Client Setup (Axios)](#1-base-api-client-with-automatic-jwt-refresh-axios)
  - [Authentication Example](#2-authentication-example)
  - [Fetching Products with Filtering](#3-fetching-products-with-filtering--pagination)
  - [Placing an Order & Uploading Instapay Receipt](#4-placing-an-order-with-instapay-receipt-upload)
- [🔒 CORS Configuration](#-cors-configuration)
- [🛡️ Error Handling](#️-error-handling)

---

## ✨ Key Features
- **Authentication & Identity**: ASP.NET Core Identity with role-based access control (`Owner`, `Customer`).
- **JWT Authentication Flow**: Access tokens (30 min expiry) paired with sliding refresh tokens (7 days expiry).
- **Product & Material Catalog**: Multi-attribute filtering (material, surface type, featured, stock), sorting, pagination, and bilingual support (EN & AR).
- **Category System**: Hierarchical kitchen category organization with real-time product count aggregation.
- **Checkout & Order Processing**: Flexible fulfillment (`delivery` / `pickup`), promo discount application, and Instapay payment verification with receipt capture.
- **Dynamic Promo Engine**: Support for percentage discounts, fixed discounts, and minimum order values.
- **Customer Reviews**: Rating calculations (1–5 stars) automatically updating product metrics upon review submission.
- **Physical Locations**: Multi-branch showroom & stone fabrication workshop directory.
- **Media Pipeline**: Static file hosting for images (up to 15MB), video walkthroughs (up to 100MB), and payment receipts.
- **Automated Database Seeding**: Auto-migrates and seeds initial catalog, promo codes, showrooms, and owner admin upon first boot.

---

## 🛠️ Tech Stack
- **Framework**: .NET 8.0 (C# 12)
- **Web API**: ASP.NET Core Web API with Controllers
- **Database**: SQLite (configured via EF Core 8)
- **ORM**: Entity Framework Core 8 (`Microsoft.EntityFrameworkCore.Sqlite`)
- **Security**: `Microsoft.AspNetCore.Authentication.JwtBearer` & `Microsoft.AspNetCore.Identity`
- **Documentation**: Swagger / Swashbuckle OpenAPI with Bearer Auth integration

---

## 📁 Project Structure
```text
kitchen-backend/
├── KitchenApi.sln                     # Visual Studio / Rider solution file
├── .gitignore                         # Git ignore for .NET, SQLite, & temp files
├── README.md                          # Complete API & frontend integration documentation
├── install-sdk.ps1                    # Optional script to bootstrap .NET 8 SDK
└── src/
    └── KitchenApi/
        ├── KitchenApi.csproj          # .NET 8 project file with package dependencies
        ├── Program.cs                 # Pipeline, DI, JWT, CORS, & DB initialization
        ├── appsettings.json           # App settings (Connection strings, JWT, Seed accounts)
        ├── appsettings.Development.json
        ├── Controllers/               # Web API endpoints
        │   ├── AuthController.cs
        │   ├── CategoriesController.cs
        │   ├── LocationsController.cs
        │   ├── MediaController.cs
        │   ├── OrdersController.cs
        │   ├── ProductsController.cs
        │   ├── PromosController.cs
        │   ├── ReviewsController.cs
        │   └── SettingsController.cs
        ├── Data/                      # EF Core DbContext & Seed data
        │   ├── AppDbContext.cs
        │   └── DbInitializer.cs
        ├── DTOs/                      # Request & Response Data Transfer Objects
        │   ├── Auth/
        │   ├── Categories/
        │   ├── Locations/
        │   ├── Orders/
        │   ├── Products/
        │   ├── Promos/
        │   ├── Reviews/
        │   └── Settings/
        ├── Migrations/                # EF Core database migrations
        ├── Models/                    # Entity models (AppUser, Product, Order, etc.)
        ├── Services/                  # Business logic services (Auth, Products, Orders)
        └── wwwroot/                   # Uploaded media assets (images, receipts, videos)
```

---

## 🚀 Quickstart & Local Setup

### 1. Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher.

### 2. Clone the Repository
```bash
git clone https://github.com/Mena-MG/kitchen-backend.git
cd kitchen-backend
```

### 3. Run the Backend
```bash
dotnet run --project src/KitchenApi
```
The API starts on:  
👉 **`http://localhost:5000`**

On startup, EF Core automatically:
1. Creates the SQLite database file (`kitchen.db`).
2. Applies all pending schema migrations.
3. Seeds default user roles (`Owner`, `Customer`), the initial owner account, sample product categories, materials, locations, and promo codes.

---

## 🔑 Default Credentials & Seeding

| Role | Email | Password | Permissions |
| :--- | :--- | :--- | :--- |
| **Owner (Admin)** | `owner@aura.kitchen` | `Owner@123456` | Full CRUD on products, orders, categories, locations, promos, settings |
| **Customer** | *(Register via `/api/auth/register`)* | *(Min 6 chars)* | Place orders, view my orders, leave reviews |

---

## 📖 Swagger / OpenAPI Documentation
When running in `Development` mode, access the interactive Swagger UI at:  
👉 **`http://localhost:5000/swagger`**

To test protected endpoints in Swagger:
1. Call `POST /api/auth/login` with your credentials.
2. Copy the `token` from the response.
3. Click the **Authorize** button at the top right of the Swagger UI.
4. Enter `Bearer <YOUR_TOKEN>` and click **Authorize**.

---

## 📡 API Endpoints Reference

### 1. Authentication (`/api/auth`)

| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/auth/register` | Public | Register a new customer account |
| `POST` | `/api/auth/login` | Public | Authenticate user & obtain JWT + refresh token |
| `POST` | `/api/auth/refresh` | Public | Obtain new JWT using active refresh token |
| `POST` | `/api/auth/revoke` | Authenticated | Revoke refresh token (logout) |
| `GET` | `/api/auth/me` | Authenticated | Fetch current user profile & roles |
| `POST` | `/api/auth/register-owner` | Owner | Create another Owner account |

#### Register Payload (`POST /api/auth/register`):
```json
{
  "firstName": "Karim",
  "lastName": "El-Sayed",
  "email": "karim@example.com",
  "password": "Password123",
  "phoneNumber": "+201012345678"
}
```

#### Login Payload (`POST /api/auth/login`):
```json
{
  "email": "owner@aura.kitchen",
  "password": "Owner@123456"
}
```

#### Login / Refresh Response:
```json
{
  "isAuthenticated": true,
  "token": "eyJhbGciOi...",
  "refreshToken": "4a7f6c8b-...",
  "refreshTokenExpiration": "2026-09-12T10:00:00Z",
  "email": "owner@aura.kitchen",
  "roles": ["Owner"]
}
```

---

### 2. Products & Materials (`/api/products`)

| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/products` | Public | List products with filtering, search, and pagination |
| `GET` | `/api/products/{id}` | Public | Retrieve single product details by ID |
| `POST` | `/api/products` | Owner | Create new product / material |
| `PUT` | `/api/products/{id}` | Owner | Update existing product |
| `DELETE` | `/api/products/{id}` | Owner | Delete product |
| `POST` | `/api/products/seed` | Owner | Re-seed standard catalog materials |

#### Query Parameters for `GET /api/products`:
- `search`: Case-insensitive search on Name, NameAr, Material, or Description
- `category`: Filter by Category ID (e.g. `cat-countertops`, `cat-cabinets`)
- `surfaceType`: Filter by surface (`countertop`, `cabinet`, `backsplash`, `accessory`, `none`)
- `material`: Filter by material keyword (e.g. `Granite`, `Quartz`, `Marble`, `Wood`)
- `featured`: Filter `true` / `false`
- `bestSeller`: Filter `true` / `false`
- `inStock`: Filter `true` / `false`
- `sortBy`: `price_asc`, `price_desc`, `rating`, `newest` (default: `newest`)
- `pageNumber`: Default `1`
- `pageSize`: Default `20`

#### Example Product Object:
```json
{
  "id": "prod-calacatta-gold",
  "name": "Calacatta Gold Quartz Slab",
  "nameAr": "كوارتز كلاكتا جولد إيطالي فاخر",
  "price": 3800,
  "originalPrice": 4200,
  "unitType": "per_sqm",
  "surfaceType": "countertop",
  "categoryId": "cat-countertops",
  "categoryName": "Countertops",
  "categoryNameAr": "أسطح المطبخ (رخام وجرانيت)",
  "material": "Engineered Quartz",
  "materialAr": "كوارتز صناعي فائق الصلابة",
  "finish": "Polished Glossy",
  "color": "Warm White with Gold & Grey Veining",
  "colorHex": "#F7F5F0",
  "thickness": "20mm / 30mm",
  "originCountry": "Italy",
  "imageUrl": "/uploads/images/calacatta-gold.webp",
  "videoUrl": "",
  "badge": "Popular Choice",
  "badgeAr": "الأكثر طلباً",
  "isFeatured": true,
  "isBestSeller": true,
  "inStock": true,
  "rating": 4.9,
  "reviewsCount": 18,
  "shortDescription": "Stunning luxury quartz with warm gold veining, non-porous and stain-resistant.",
  "shortDescriptionAr": "كوارتز فاخر بعروق ذهبية دافئة، غير مسامي ومقاوم للبقع والخدش تماماً.",
  "features": [
    "Resistant to acids, lemons, and coffee stains",
    "Heat resistant up to 150°C",
    "Food-safe certified surface"
  ],
  "gallery": []
}
```

---

### 3. Categories (`/api/categories`)

| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/categories` | Public | List all categories with product count per category |
| `POST` | `/api/categories` | Owner | Create a new category |
| `PUT` | `/api/categories/{id}` | Owner | Update an existing category |
| `DELETE` | `/api/categories/{id}` | Owner | Delete category |

#### Create Category Payload:
```json
{
  "name": "Kitchen Islands",
  "nameAr": "جزر المطبخ ووحدات المنتصف",
  "icon": "🏝️",
  "slug": "kitchen-islands",
  "displayOrder": 9
}
```

---

### 4. Orders & Checkout (`/api/orders`)

| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/orders` | Public / Customer | Create order or submit custom quotation request |
| `GET` | `/api/orders/{id}` | Public / Customer / Owner | Get order details by ID |
| `GET` | `/api/orders/my-orders` | Authenticated | Get logged-in user's order history |
| `GET` | `/api/orders` | Owner | View all store orders (status filter & pagination) |
| `PUT` | `/api/orders/{id}/status` | Owner | Update order status (`Pending`, `Confirmed`, `InProduction`, `Delivered`, `Cancelled`) |

#### Create Order Payload (`POST /api/orders`):
```json
{
  "customerName": "Ahmed Hassan",
  "customerPhone": "+201001234567",
  "deliveryMethod": "delivery",
  "shippingAddress": "Villa 14, Beverly Hills, Zayed",
  "shippingCity": "Giza",
  "pickupLocationId": "",
  "paymentMethod": "instapay",
  "transactionRef": "INSTA-98745231",
  "instapayReceiptBase64": "/uploads/receipts/receipt_sample.jpg",
  "appliedPromoCode": "KITCHEN10",
  "notes": "Please call before delivery for slab measurement.",
  "items": [
    {
      "productId": "prod-calacatta-gold",
      "productName": "Calacatta Gold Quartz Slab",
      "productNameAr": "كوارتز كلاكتا جولد",
      "unitPrice": 3800,
      "quantity": 3.5,
      "unitType": "per_sqm",
      "customNote": "3.5 sqm with beveled edge polish"
    }
  ]
}
```

---

### 5. Promo Codes (`/api/promos`)

| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/promos/validate` | Public | Validate code against cart subtotal |
| `GET` | `/api/promos` | Owner | List all promotional coupon codes |
| `POST` | `/api/promos` | Owner | Create a new promo code |

#### Validate Promo Request (`POST /api/promos/validate`):
```json
{
  "code": "KITCHEN10",
  "subtotal": 13300
}
```

#### Validation Response:
```json
{
  "isValid": true,
  "code": "KITCHEN10",
  "message": "Promo code applied successfully.",
  "discountPercent": 10,
  "fixedDiscount": 0,
  "discountAmount": 1330,
  "newTotal": 11970,
  "label": "10% Off Kitchen Materials",
  "labelAr": "خصم 10% للخامات"
}
```

---

### 6. Reviews & Ratings (`/api/reviews`)

| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/reviews/product/{productId}` | Public | Get all reviews for a product |
| `POST` | `/api/reviews` | Public / Customer | Submit a rating (1–5) and review |

#### Submit Review Payload:
```json
{
  "productId": "prod-calacatta-gold",
  "userName": "Nouran E.",
  "rating": 5,
  "comment": "Outstanding quartz quality and clean edge fabrication. Highly recommended!"
}
```

---

### 7. Showrooms & Workshops (`/api/locations`)

| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/locations` | Public | Get physical showrooms, design hubs & workshops |
| `POST` | `/api/locations` | Owner | Add new showroom or workshop |
| `PUT` | `/api/locations/{id}` | Owner | Update showroom details |
| `DELETE` | `/api/locations/{id}` | Owner | Remove location |

---

### 8. Store Settings (`/api/settings`)

| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/settings` | Public | Key-value store configuration |
| `PUT` | `/api/settings/{key}` | Owner | Update or create store setting |

---

### 9. Media & File Uploads (`/api/media`)

| Method | Endpoint | Access | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/media/upload-image` | Public | Upload product/material image (max 15MB: JPG, PNG, WebP, SVG) |
| `POST` | `/api/media/upload-receipt` | Public | Upload Instapay receipt screenshot (JPG, PNG, PDF) |
| `POST` | `/api/media/upload-video` | Owner | Upload showroom/craftsmanship video (max 100MB: MP4, WebM) |

*Send payload as `multipart/form-data` with form field key `file`.*

#### Upload Response:
```json
{
  "url": "http://localhost:5000/uploads/images/img_1725512345_a1b2c3d4.webp",
  "fileName": "img_1725512345_a1b2c3d4.webp",
  "size": 348120,
  "contentType": "image/webp"
}
```

---

## 💻 Frontend Integration Guide

Here are practical, copy-pasteable examples for integrating this backend into any frontend (React, Vue, Next.js, Svelte, Angular, or React Native).

### 1. Base API Client with Automatic JWT Refresh (Axios)

Create `apiClient.js` (or `.ts`):

```javascript
import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor: Attach JWT
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('access_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor: Auto refresh on 401 Unauthorized
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      const refreshToken = localStorage.getItem('refresh_token');

      if (refreshToken) {
        try {
          const res = await axios.post(`${API_BASE_URL}/api/auth/refresh`, {
            refreshToken,
          });
          const { token, refreshToken: newRefreshToken } = res.data;
          localStorage.setItem('access_token', token);
          localStorage.setItem('refresh_token', newRefreshToken);

          originalRequest.headers.Authorization = `Bearer ${token}`;
          return api(originalRequest);
        } catch (refreshErr) {
          localStorage.removeItem('access_token');
          localStorage.removeItem('refresh_token');
          window.location.href = '/login';
        }
      }
    }
    return Promise.reject(error);
  }
);

export default api;
```

---

### 2. Authentication Example

```javascript
import api from './apiClient';

// Login
export async function loginUser(email, password) {
  const response = await api.post('/api/auth/login', { email, password });
  const { token, refreshToken, roles, email: userEmail } = response.data;

  localStorage.setItem('access_token', token);
  localStorage.setItem('refresh_token', refreshToken);
  return { userEmail, roles };
}

// Check current user
export async function getCurrentUser() {
  const response = await api.get('/api/auth/me');
  return response.data;
}

// Logout
export async function logoutUser() {
  const refreshToken = localStorage.getItem('refresh_token');
  if (refreshToken) {
    await api.post('/api/auth/revoke', { refreshToken }).catch(() => {});
  }
  localStorage.removeItem('access_token');
  localStorage.removeItem('refresh_token');
}
```

---

### 3. Fetching Products with Filtering & Pagination

```javascript
import api from './apiClient';

export async function fetchProducts({ search, category, surfaceType, pageNumber = 1, pageSize = 20 }) {
  const params = new URLSearchParams();
  if (search) params.append('search', search);
  if (category) params.append('category', category);
  if (surfaceType) params.append('surfaceType', surfaceType);
  params.append('pageNumber', pageNumber);
  params.append('pageSize', pageSize);

  const response = await api.get(`/api/products?${params.toString()}`);
  return response.data; // { items: [...], totalCount: 42, pageNumber: 1, totalPages: 3 }
}
```

---

### 4. Placing an Order with Instapay Receipt Upload

```javascript
import api from './apiClient';

// 1. Upload the Instapay Screenshot
export async function uploadReceiptFile(file) {
  const formData = new FormData();
  formData.append('file', file);

  const response = await api.post('/api/media/upload-receipt', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return response.data.url;
}

// 2. Validate Coupon Code
export async function validateCoupon(code, subtotal) {
  const response = await api.post('/api/promos/validate', { code, subtotal });
  return response.data;
}

// 3. Submit Checkout Order
export async function checkoutOrder(cartItems, customerInfo, receiptUrl, promoCode) {
  const payload = {
    customerName: customerInfo.name,
    customerPhone: customerInfo.phone,
    deliveryMethod: customerInfo.deliveryMethod, // 'delivery' or 'pickup'
    shippingAddress: customerInfo.address || '',
    shippingCity: customerInfo.city || '',
    pickupLocationId: customerInfo.pickupLocationId || '',
    paymentMethod: customerInfo.paymentMethod, // 'instapay', 'cod', 'pickup'
    transactionRef: customerInfo.transactionRef || '',
    instapayReceiptBase64: receiptUrl || '',
    appliedPromoCode: promoCode || '',
    notes: customerInfo.notes || '',
    items: cartItems.map((item) => ({
      productId: item.id,
      productName: item.name,
      productNameAr: item.nameAr,
      unitPrice: item.price,
      quantity: item.quantity,
      unitType: item.unitType,
      customNote: item.customNote || '',
    })),
  };

  const response = await api.post('/api/orders', payload);
  return response.data; // Created Order with Order ID
}
```

---

## 🔒 CORS Configuration
CORS is configured out-of-the-box in `Program.cs` under the policy `"AllowFrontend"`, allowing requests and credentials from:
- `http://localhost:5173` (Vite)
- `http://localhost:5174` (Secondary Vite dev)
- `http://localhost:3000` (React / Next.js dev)
- `https://kitchen-mena-mg.vercel.app` (Vercel production deployment)

To add another frontend domain, add it to `builder.Services.AddCors(...)` in `Program.cs`.

---

## 🛡️ Error Handling
Standard HTTP response status codes:
- `200 OK`: Request succeeded.
- `201 Created`: Resource created successfully (with `Location` header).
- `204 NoContent`: Deletion / void action succeeded.
- `400 Bad Request`: Validation failure or missing parameters (returns `{ message: "..." }` or model validation errors).
- `401 Unauthorized`: Missing or invalid JWT Bearer token.
- `403 Forbidden`: Authenticated user lacks the necessary role (e.g. `Owner`).
- `404 Not Found`: Resource ID not found.

---

## 📄 License
This project is licensed under the MIT License.
