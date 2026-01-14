# 📝 TodoApp


A full-featured note-taking web application built with ASP.NET Core MVC, SQL Server, and Entity Framework Core.

![Notes App Screenshot](https://via.placeholder.com/800x400.png?text=Notes+App+Screenshot)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-purple)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-blue)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.0-blueviolet)
![License](https://img.shields.io/badge/License-MIT-green)

## ✨ Features

- 🔐 **User Authentication** - Register & Login with secure password hashing
- 📋 **CRUD Operations** - Create, Read, Update, Delete notes
- 📱 **Responsive Design** - Mobile-friendly Bootstrap 5 interface
- 🔍 **Search & Filter** - Find notes quickly
- 📊 **Database Integration** - SQL Server with Entity Framework Core
- 🛡️ **Security** - XSS protection, CSRF tokens, input validation
- 📅 **Timestamps** - Automatic created/modified dates

## 🏗️ Architecture

NotesApp/
├── Controllers/ # MVC Controllers (Account, Notes)
├── Models/ # Data Models & ViewModels
├── Views/ # Razor Views
├── Services/ # Business Logic Layer
├── Data/ # Entity Framework Context
├── wwwroot/ # Static Files (CSS, JS, Images)
└── Migrations/ # Database Migrations


## 🛠️ Technology Stack

**Backend:**
- ASP.NET Core 8.0 MVC
- Entity Framework Core 8.0
- Microsoft SQL Server
- BCrypt.Net for password hashing
- ASP.NET Core Authentication

**Frontend:**
- Bootstrap 5.3
- JavaScript (ES6+)
- Font Awesome Icons
- Responsive Design

**Development:**
- Visual Studio 2026
- Git Version Control
- SQL Server Management Studio

## 📦 Installation

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Microsoft SQL Server +](https://www.microsoft.com/sql-server)
- [Visual Studio 2026](https://visualstudio.microsoft.com/) (Recommended)
