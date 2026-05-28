# Blind-Match PAS - Secure Project Approval System

![Tech Stack](https://img.shields.io/badge/.NET_8-MVC-purple)
![Database](https://img.shields.io/badge/SQL-Server-red)
![Testing](https://img.shields.io/badge/Coverage-100%25-brightgreen)

**Blind-Match PAS** is an enterprise-grade Project Approval System designed to eliminate bias in academic and corporate evaluations. Built on .NET 8 MVC, it enforces strict state-machine logic for the project lifecycle and guarantees reliability through 100% verified code coverage.

---

## 📸 Interface Preview
<img width="1280" height="604" alt="image" src="https://github.com/user-attachments/assets/d1e739d7-9efd-4760-9362-cace6592c001" />


## 🚀 Key Features
* **Unbiased Evaluation Logic:** Custom blind-matching algorithm to strip identifiable data and remove reviewer bias.
* **Strict State-Machine:** Immutable enforcement of project lifecycle states (Draft -> Pending -> Approved/Rejected).
* **Enterprise Security:** Implements robust Role-Based Access Control (RBAC) to separate admin, reviewer, and submitter privileges.
* **Test-Driven Reliability:** 100% verified business logic using xUnit and Moq frameworks.

## 🛠️ Technical Stack
* **Framework:** C# .NET 8 (ASP.NET Core MVC)
* **Database:** Microsoft SQL Server
* **ORM:** Entity Framework Core
* **Testing:** xUnit, Moq
* **Architecture:** SOLID Principles, Dependency Injection, State Pattern

---

## 💻 Local Development Setup

**1. Clone the repository:**
`git clone https://github.com/theekshana-git/PAS-BlindMatch.git`  
`cd PAS-BlindMatch`

**2. Configure the Database:**
Update the `appsettings.json` file with your local SQL Server connection string.

**3. Apply Migrations:**
`dotnet ef database update`

**4. Run the Application:**
`dotnet run`
