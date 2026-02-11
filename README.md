## E-Commerce Project – ASP.NET Core 9

# 📌 Overview

This project is a modern **ASP.NET Core 9 E-Commerce application**.  
It includes role-based authorization, admin management panel, product & category management, cookie-based shopping cart, and online payment integration.

 A Seed Database is used to automatically create demo admin users, products, and categories.

 

#  🚀 Features

- 🧾 Product & category management  
- 🔐 Admin panel (admin-only access)  
- 👤 User registration, login, and role management  
- 🛒 Cookie-based temporary shopping cart  
- 💳 Iyzipay (Iyzico) payment integration


#  🧰 Technologies Used

- **ASP.NET Core 9**
- **Entity Framework Core**
  - Seed Database
- **ASP.NET Core Identity**
  - AppUser / AppRole
- **Iyzipay (Iyzico) Payment Gateway**
- Cookie-based cart management


# 🗄️ Database & Seed Logic
On first run:
A default admin user is created (if not exists)
Demo products and categories are inserted
If the database is deleted and recreated, seed data will be added again
Use the seeded admin account to access the admin panel.


# 👤 User Features
Register and login
Add products to cart without logging in (stored in cookies)
When logged in:
Cookie cart is cleared
Products are transferred to the user account
Update profile information:
First name
Last name
Email
Password
Search products using the search feature

# Admin Features

➕ Add new products
✏️ Update existing products
❌ Delete products
📂 Manage categories
👥 Manage users
💰 Monitor payments and orders


# ⚙️ Installation / Login

1️⃣ Implement Database Migrations

    dotnet ef database update

2️⃣💳Iyzipay (Iyzico) Configuration

To enable payments:
Create an account at iyzico.com
Get your API Key and Secret Key
Add the following section to appsettings.json:

"Iyzipay": {
  "ApiKey": "YOUR_API_KEY",
  "SecretKey": "YOUR_SECRET_KEY",
  "BaseUrl": "https://sandbox-api.iyzipay.com"
}

3️⃣run the project
	
	dotnet run

https://localhost:5001
You can now access the website and register.
