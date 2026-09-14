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

#### Create / Update Product Upload Flow (Recommended)
`POST /api/products` and `PUT /api/products/{id}` now support `multipart/form-data` so the frontend can upload files directly with drag-and-drop.

Supported fields:
- `Name`, `NameAr`, `Price`, `OriginalPrice`, `UnitType`, `SurfaceType`, `CategoryId`, `CategoryName`, `CategoryNameAr`, `Material`, `MaterialAr`, `Finish`, `FinishAr`, `Color`, `ColorAr`, `ColorHex`, `Thickness`, `OriginCountry`, `OriginCountryAr`, `ShortDescription`, `ShortDescriptionAr`, `Description`, `DescriptionAr`, `Badge`, `BadgeAr`, `IsFeatured`, `IsBestSeller`, `InStock`, `Features`, `FeaturesAr`, `Gallery`
- Optional file uploads:
  - `ImageFile` for the main product image
  - `VideoFile` for the product video
  - `GalleryFiles` for additional images/videos in the gallery
- Optional legacy URL fallback:
  - `ImageUrl`, `VideoUrl`, `Gallery` can still be sent as strings

Upload behavior:
- If a file is provided, the API uploads it to the server and saves the public URL in the database.
- If no file is provided, you can still send a direct remote URL.
- The final stored values are persisted in the `Products` table as `ImageUrl`, `VideoUrl`, and `GalleryJson`.

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
You can send category data as either JSON or multipart form-data.

JSON example:
```json
{
  "name": "Kitchen Islands",
  "nameAr": "جزر المطبخ ووحدات المنتصف",
  "icon": "🏝️",
  "slug": "kitchen-islands",
  "displayOrder": 9
}
```

Multipart upload example (`POST /api/categories` or `PUT /api/categories/{id}`):
- Form fields: `Name`, `NameAr`, `Slug`, `DisplayOrder`
- Optional file field: `IconFile`
- Optional legacy URL fallback: `Icon`

When `IconFile` is sent, the API uploads the image, stores the generated URL in the database, and uses it as the category icon.

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

For products and categories, the preferred flow is now the direct multipart upload on the resource endpoints themselves:
- `POST /api/products`
- `PUT /api/products/{id}`
- `POST /api/categories`
- `PUT /api/categories/{id}`

This gives the frontend a simpler drag-and-drop UX with the same end result: uploaded files are stored on disk and the generated public URL is saved into the database.

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

### 5. Creating or Updating Products with Drag-and-Drop Files

The new upload flow is built for a much easier frontend experience. Instead of manually pasting a long image/video URL, the React app can now send a multipart request and let the backend upload the file for you.

#### How the endpoints work

For products:
- `POST /api/products` creates a product
- `PUT /api/products/{id}` updates a product

For categories:
- `POST /api/categories` creates a category
- `PUT /api/categories/{id}` updates a category

Both resource endpoints now support `multipart/form-data`, which means you can send:
- normal text fields
- optional `ImageFile`, `VideoFile`, `GalleryFiles`, or `IconFile`
- optional URL fallbacks such as `ImageUrl`, `VideoUrl`, or `Icon`

#### Important note

If you send a file, the backend uploads it to the server and stores the generated public URL in the database.

If you do not send a file, the backend still accepts direct URL values, so older frontend code can continue working.

#### Example: product create with files

```javascript
import api from './apiClient';

export async function createProductWithFiles(product, files) {
  const formData = new FormData();

  // Required product fields
  formData.append('Name', product.name);
  formData.append('NameAr', product.nameAr);
  formData.append('Price', String(product.price));
  formData.append('OriginalPrice', String(product.originalPrice || product.price));
  formData.append('UnitType', product.unitType || 'per_sqm');
  formData.append('SurfaceType', product.surfaceType || 'none');
  formData.append('CategoryId', product.categoryId || '');
  formData.append('CategoryName', product.categoryName || '');
  formData.append('CategoryNameAr', product.categoryNameAr || '');
  formData.append('Material', product.material || '');
  formData.append('MaterialAr', product.materialAr || '');
  formData.append('Finish', product.finish || '');
  formData.append('FinishAr', product.finishAr || '');
  formData.append('Color', product.color || '');
  formData.append('ColorAr', product.colorAr || '');
  formData.append('ColorHex', product.colorHex || '');
  formData.append('Thickness', product.thickness || '');
  formData.append('OriginCountry', product.originCountry || '');
  formData.append('OriginCountryAr', product.originCountryAr || '');
  formData.append('ShortDescription', product.shortDescription || '');
  formData.append('ShortDescriptionAr', product.shortDescriptionAr || '');
  formData.append('Description', product.description || '');
  formData.append('DescriptionAr', product.descriptionAr || '');
  formData.append('Badge', product.badge || '');
  formData.append('BadgeAr', product.badgeAr || '');
  formData.append('IsFeatured', String(Boolean(product.isFeatured)));
  formData.append('IsBestSeller', String(Boolean(product.isBestSeller)));
  formData.append('InStock', String(product.inStock !== false));

  // Optional URL fallback if you already have remote URLs
  if (product.imageUrl) formData.append('ImageUrl', product.imageUrl);
  if (product.videoUrl) formData.append('VideoUrl', product.videoUrl);
  if (product.gallery?.length) {
    formData.append('Gallery', JSON.stringify(product.gallery));
  }

  // File uploads for drag-and-drop flows
  if (files?.mainImage) formData.append('ImageFile', files.mainImage);
  if (files?.video) formData.append('VideoFile', files.video);
  if (files?.gallery?.length) {
    files.gallery.forEach((file) => formData.append('GalleryFiles', file));
  }

  const response = await api.post('/api/products', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

  return response.data;
}
```

#### Example: product update with new files

```javascript
import api from './apiClient';

export async function updateProductWithFiles(id, product, files) {
  const formData = new FormData();

  formData.append('Name', product.name);
  formData.append('NameAr', product.nameAr);
  formData.append('Price', String(product.price));
  formData.append('OriginalPrice', String(product.originalPrice || product.price));
  formData.append('UnitType', product.unitType || 'per_sqm');
  formData.append('SurfaceType', product.surfaceType || 'none');
  formData.append('CategoryId', product.categoryId || '');
  formData.append('CategoryName', product.categoryName || '');
  formData.append('CategoryNameAr', product.categoryNameAr || '');
  formData.append('Material', product.material || '');
  formData.append('MaterialAr', product.materialAr || '');
  formData.append('Finish', product.finish || '');
  formData.append('FinishAr', product.finishAr || '');
  formData.append('Color', product.color || '');
  formData.append('ColorAr', product.colorAr || '');
  formData.append('ColorHex', product.colorHex || '');
  formData.append('Thickness', product.thickness || '');
  formData.append('OriginCountry', product.originCountry || '');
  formData.append('OriginCountryAr', product.originCountryAr || '');
  formData.append('ShortDescription', product.shortDescription || '');
  formData.append('ShortDescriptionAr', product.shortDescriptionAr || '');
  formData.append('Description', product.description || '');
  formData.append('DescriptionAr', product.descriptionAr || '');
  formData.append('Badge', product.badge || '');
  formData.append('BadgeAr', product.badgeAr || '');
  formData.append('IsFeatured', String(Boolean(product.isFeatured)));
  formData.append('IsBestSeller', String(Boolean(product.isBestSeller)));
  formData.append('InStock', String(product.inStock !== false));

  if (product.imageUrl) formData.append('ImageUrl', product.imageUrl);
  if (product.videoUrl) formData.append('VideoUrl', product.videoUrl);
  if (product.gallery?.length) {
    formData.append('Gallery', JSON.stringify(product.gallery));
  }

  if (files?.mainImage) formData.append('ImageFile', files.mainImage);
  if (files?.video) formData.append('VideoFile', files.video);
  if (files?.gallery?.length) {
    files.gallery.forEach((file) => formData.append('GalleryFiles', file));
  }

  const response = await api.put(`/api/products/${id}`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

  return response.data;
}
```

#### Example: category create with icon file

```javascript
import api from './apiClient';

export async function createCategoryWithIcon(category, iconFile) {
  const formData = new FormData();

  formData.append('Name', category.name);
  formData.append('NameAr', category.nameAr);
  formData.append('Slug', category.slug || category.name);
  formData.append('DisplayOrder', String(category.displayOrder || 0));

  // Optional legacy icon URL fallback
  if (category.icon) {
    formData.append('Icon', category.icon);
  }

  // Optional file upload for drag-and-drop UX
  if (iconFile) {
    formData.append('IconFile', iconFile);
  }

  const response = await api.post('/api/categories', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

  return response.data;
}
```

#### Example: category update with icon file

```javascript
import api from './apiClient';

export async function updateCategoryWithIcon(id, category, iconFile) {
  const formData = new FormData();

  formData.append('Name', category.name);
  formData.append('NameAr', category.nameAr);
  formData.append('Slug', category.slug || category.name);
  formData.append('DisplayOrder', String(category.displayOrder || 0));

  if (category.icon) {
    formData.append('Icon', category.icon);
  }

  if (iconFile) {
    formData.append('IconFile', iconFile);
  }

  const response = await api.put(`/api/categories/${id}`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

  return response.data;
}
```

#### Full React example with drag-and-drop

```jsx
import { useState } from 'react';
import api from './apiClient';

export default function ProductForm() {
  const [form, setForm] = useState({
    name: '',
    nameAr: '',
    categoryName: 'Countertops',
    categoryNameAr: 'أسطح المطبخ',
    material: 'Quartz',
    materialAr: 'كوارتز',
    price: 2000,
    originalPrice: 2400,
    description: '',
    descriptionAr: '',
    imageUrl: '',
    videoUrl: '',
  });

  const [files, setFiles] = useState({
    mainImage: null,
    video: null,
    gallery: [],
  });

  const handleFileChange = (event) => {
    const { name, files: selectedFiles } = event.target;

    if (name === 'gallery') {
      setFiles((prev) => ({ ...prev, gallery: Array.from(selectedFiles) }));
      return;
    }

    setFiles((prev) => ({ ...prev, [name]: selectedFiles[0] || null }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    const formData = new FormData();

    formData.append('Name', form.name);
    formData.append('NameAr', form.nameAr);
    formData.append('Price', String(form.price));
    formData.append('OriginalPrice', String(form.originalPrice));
    formData.append('UnitType', 'per_sqm');
    formData.append('SurfaceType', 'countertop');
    formData.append('CategoryName', form.categoryName);
    formData.append('CategoryNameAr', form.categoryNameAr);
    formData.append('Material', form.material);
    formData.append('MaterialAr', form.materialAr);
    formData.append('Description', form.description);
    formData.append('DescriptionAr', form.descriptionAr);
    formData.append('IsFeatured', 'true');
    formData.append('IsBestSeller', 'false');
    formData.append('InStock', 'true');

    if (form.imageUrl) formData.append('ImageUrl', form.imageUrl);
    if (form.videoUrl) formData.append('VideoUrl', form.videoUrl);

    if (files.mainImage) formData.append('ImageFile', files.mainImage);
    if (files.video) formData.append('VideoFile', files.video);
    if (files.gallery.length) {
      files.gallery.forEach((file) => formData.append('GalleryFiles', file));
    }

    try {
      const response = await api.post('/api/products', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });

      console.log('Product created:', response.data);
      alert('Product saved successfully');
    } catch (error) {
      console.error(error);
      alert('Something went wrong while saving the product');
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} placeholder="Product name" />
      <input value={form.nameAr} onChange={(e) => setForm({ ...form, nameAr: e.target.value })} placeholder="Product name Arabic" />
      <input type="number" value={form.price} onChange={(e) => setForm({ ...form, price: Number(e.target.value) })} placeholder="Price" />
      <input type="number" value={form.originalPrice} onChange={(e) => setForm({ ...form, originalPrice: Number(e.target.value) })} placeholder="Original price" />
      <input value={form.categoryName} onChange={(e) => setForm({ ...form, categoryName: e.target.value })} placeholder="Category name" />
      <input value={form.categoryNameAr} onChange={(e) => setForm({ ...form, categoryNameAr: e.target.value })} placeholder="Category name Arabic" />

      <input type="file" name="mainImage" onChange={handleFileChange} accept="image/*" />
      <input type="file" name="video" onChange={handleFileChange} accept="video/*" />
      <input type="file" name="gallery" onChange={handleFileChange} accept="image/*" multiple />

      <input value={form.imageUrl} onChange={(e) => setForm({ ...form, imageUrl: e.target.value })} placeholder="Optional image URL" />
      <input value={form.videoUrl} onChange={(e) => setForm({ ...form, videoUrl: e.target.value })} placeholder="Optional video URL" />

      <textarea value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} placeholder="Description" />
      <textarea value={form.descriptionAr} onChange={(e) => setForm({ ...form, descriptionAr: e.target.value })} placeholder="Description Arabic" />

      <button type="submit">Save Product</button>
    </form>
  );
}
```

#### What this does in practice

1. The React form collects product details.
2. The user can either:
   - choose a local image/video file, or
   - paste an image/video URL manually
3. The frontend sends the data using `FormData`.
4. The backend receives the request and saves the uploaded media to `wwwroot/uploads`.
5. The generated public URL is stored in the database (`ImageUrl`, `VideoUrl`, `GalleryJson`).
6. The API returns the created product object with the saved media URLs.

> This is the recommended pattern for admin pages, CMS tools, and product management dashboards.

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
