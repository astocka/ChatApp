# ChatApp
Web application for publishing messages on one of the available channels.

Functions
--
* create an account, login, log out
* managing channels CRUD
* managing messages CRUD

Getting started
--
* Get copy of the project
* Open MS SQL Server Management Studio and create new database called ChatAppDb
* Open ChatApp.sln in MS Visual Studio and in Package Manager Console write:
```sh
PM> add-migration Initial
PM> update-database
```
Then you can create new account at:
```sh
/account/register
```
and start using the application.

Used technologies
--
* ASP.NET Core MVC
* Entity Framework Core
* ASP.NET Core Identity
* HTML
* CSS
* JavaScript
* jQuery
* Bootstrap
