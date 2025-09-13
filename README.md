# 🏪 MiniMart - E-Commerce Mini System

## 📖 Overview
MiniMart is a simple **ASP.NET Core MVC** application designed to manage products, customers, and orders for a small e-commerce company. This project demonstrates CRUD operations, transactions, middleware logging, global exception handling, pagination, and reporting using both **Entity Framework Core** and **Dapper**.

The system includes:
- Product management
- Customer management
- Order processing (with multiple products per order)
- Reporting via Dapper
- Performance optimization and error handling

---

## 🎯 Features

### **1. Product Management**
- Add new products.
- Update product details.
- Delete products.
- View all products.

### **2. Customer Management**
- Add new customers.
- Update customer information.
- Delete customers.
- View all customers.

### **3. Orders & Transactions**
- Create orders with multiple products.
- Transaction support to ensure **atomic order creation**.
- Eager loading to fetch orders with their order items.
- Pagination in order listing (5 orders per page).
- Optimized queries using `.AsNoTracking()`.

### **4. Validation & Error Handling**
- **Data Annotations** for input validation:
  - Name → Required
  - Email → Required + Email format
  - Price → Positive numbers only
- **Custom Middleware** for request logging.
- **Custom Exception Filter** for global error handling.

### **5. Reporting with Dapper**
- Generate a **Top Selling Products Report**.
- Show:
  - Product Name
  - Total Quantity Sold
  - Total Sales Amount
- Performance comparison between EF Core and Dapper using Stopwatch.

---

## 🗄 Database Schema
| Table      | Columns |
|------------|------------------------------------------------------------|
| **Product** | Id (PK), Name, Price, Stock |
| **Customer** | Id (PK), Name, Email, Phone |
| **Order** | Id (PK), CustomerId (FK), OrderDate, TotalAmount |
| **OrderItem** | Id (PK), OrderId (FK), ProductId (FK), Quantity, Price |

---

## 🛠 Technology Stack
- **Backend:** ASP.NET Core MVC, C#
- **Database:** SQL Server
- **ORM:** Entity Framework Core + Dapper
- **UI:** Razor Pages, Bootstrap
- **Logging:** Custom Middleware (text file)
- **Error Handling:** Custom Exception Filter

---

## 📂 Project Structure
```
MiniMart/
│
├── Controllers/
│   ├── HomeController.cs
│   ├── ProductController.cs
│   ├── CustomerController.cs
│   ├── OrderController.cs
│   └── ReportsController.cs
│
├── Models/
│   ├── Product.cs
│   ├── Customer.cs
│   ├── Order.cs
│   └── OrderItem.cs
│
├── Views/
│   ├── Shared/ (Layout & partial views)
│   ├── Product/
│   ├── Customer/
│   ├── Order/
│   └── Reports/
│
├── Middleware/
│   └── RequestLoggingMiddleware.cs
│
├── Filters/
│   └── GlobalExceptionFilter.cs
│
├── Data/
│   └── MiniMartDbContext.cs
│
├── logs/
│   └── requests.txt
│
└── Program.cs / Startup.cs
```

---

## 🚀 Setup Instructions

### 1️⃣ Clone the Repository
```bash
git clone https://github.com/yourusername/MiniMart.git
cd MiniMart
```

### 2️⃣ Configure Database
- Update the **connection string** in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=MiniMartDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 3️⃣ Run Migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4️⃣ Run the Application
```bash
dotnet run
```
Then open: `https://localhost:5001`

---

## 📊 Dapper Reporting Query Example
```sql
SELECT 
    p.Name AS ProductName,
    SUM(oi.Quantity) AS TotalQuantitySold,
    SUM(oi.Quantity * oi.Price) AS TotalSalesAmount
FROM OrderItems oi
INNER JOIN Products p ON oi.ProductId = p.Id
GROUP BY p.Name
ORDER BY TotalQuantitySold DESC;
```

---

## 📝 Marking Guidelines
| Criteria                     | Marks |
|------------------------------|-------|
| Project Setup & Folder Structure | 5 |
| EF Core Models & CRUD | 10 |
| Validation, Middleware & Filter | 10 |
| Order & Transaction Handling | 10 |
| Pagination & Query Optimization | 10 |
| Razor Layout, Partial Views | 10 |
| Dapper Reporting | 10 |
| Code Quality & Documentation | 10 |
| Code Understanding | 15 |
| **Total** | **90 + 10 (Bonus)** |

---

## 📌 Final Deliverables
- ✅ Working MiniMart application with all features.
- ✅ Logs stored in `logs/requests.txt`.
- ✅ Custom global error handling.
- ✅ CRUD for Product and Customer.
- ✅ Order creation with transaction.
- ✅ Pagination in order listing.
- ✅ Dapper-based reporting.

---

## 👨‍💻 Author
**MiniMart Development Team**

Happy Coding! 🎉
