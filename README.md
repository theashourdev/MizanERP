# MizanERP

MizanERP is a modern ERP (Enterprise Resource Planning) system designed for manufacturing, inventory, trading, and accounting operations.

The system follows Clean Architecture and Domain-Driven Design (DDD) principles to ensure scalability, maintainability, and separation of concerns.

## Core Features

* Inventory Management

  * Raw materials and finished goods tracking
  * Inventory movements and stock adjustments
  * Weighted average cost calculation
  * Multi-warehouse ready architecture

* Purchasing Module

  * Purchase order workflows
  * Supplier management
  * Goods receiving and inventory updates
  * Automatic accounting entries

* Sales Module

  * Sales order processing
  * Customer management
  * Revenue recognition
  * Cost of Goods Sold (COGS) calculation

* Manufacturing Module

  * Bill of Materials (BOM)
  * Production orders
  * Raw material consumption
  * Finished goods production costing

* Accounting System

  * Double-entry accounting
  * Journal entry generation
  * Accounts receivable/payable
  * Financial transaction tracking

* Architecture & Technical Features

  * Clean Architecture
  * Domain-Driven Design (DDD)
  * Repository & Unit of Work patterns
  * Entity Framework Core
  * SQL Server
  * Dependency Injection
  * Soft delete support
  * Audit-ready transaction tracking

## Technology Stack

* ASP.NET Core
* Entity Framework Core
* SQL Server
* Clean Architecture
* C#
* Razor Pages / MVC (planned)

## Project Status

Backend architecture and business workflows are fully implemented, including:

* Domain Layer
* Application Layer
* Infrastructure Layer

Current focus:

* Integration testing
* Database migrations
* Web/UI layer

MizanERP is designed to support real-world manufacturing and trading workflows with accurate inventory tracking, production costing, and financial accounting.
