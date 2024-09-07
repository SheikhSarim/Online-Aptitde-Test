# Online-Aptitude Test

## Table of Contents
- [Online-Aptitude Test](#online-aptitude-test)
  - [Table of Contents](#table-of-contents)
  - [Introduction](#introduction)
  - [Features](#features)
  - [Project Structure](#project-structure)
  - [Technologies Used](#technologies-used)
  - [Packages](#packages)
  - [Setup and Installation](#setup-and-installation)
  - [Usage](#usage)
  - [Contributing](#contributing)

## Introduction
The Online-Aptitude Test application is designed for the Webster organization, a leading Oil-Gas company, to facilitate online testing for job seekers. The application allows candidates to take a series of aptitude tests in a structured manner, and based on their performance, they may advance to further interview rounds.

## Features
- **Aptitude Test**: Divided into three parts: General Knowledge, Mathematics, and Computer Technology.
- **Admin Role**: 
  - View the number of candidates, questions, and tests.
  - Add (register) or delete a manager.
  - Manage the overall system and ensure smooth operation.
- **Manager Role**: 
  - Manage candidate details, test questions, and view test results.
  - Can log in to access their functionalities.
- **Candidate Role**: 
  - Log in, take the test, and view results.
  - Must complete each test within a specified time limit.


## Project Structure
- **Models**: Admin, Manager, Candidate, Test, Question, Result, etc.
- **Controllers**: Managing different user roles and test functionality.
- **Views**: For displaying test interfaces and user management screens.

## Technologies Used
- **Frontend**: HTML, CSS, JavaScript
- **Backend**: ASP.NET Core MVC
- **Database**: SQL Server
- **Tools**: Visual Studio .NET, IIS Server, .NET Framework

## Packages
The following packages are used in this project:
- `BCrypt.Net-Next` (4.0.3)
- `Microsoft.EntityFrameworkCore` (7.0.20)
- `Microsoft.EntityFrameworkCore.Design` (7.0.20)
- `Microsoft.EntityFrameworkCore.SqlServer` (7.0.20)
- `Microsoft.EntityFrameworkCore.Tools` (7.0.20)
- `Microsoft.VisualStudio.Web.CodeGeneration.Design` (7.0.12)

## Setup and Installation
1. Clone the repository:
    ```bash
    git clone https://github.com/SheikhSarim/Online-Aptitude-Test.git
    ```
2. Open the project in Visual Studio.
3. Update the database connection string in `appsettings.json`.
4. Run the application using IIS Express or your preferred method.

## Usage
- **Admin**: 
  - Log in to view the number of candidates, questions, and tests.
  - Add or delete managers.
- **Manager**: 
  - Log in to manage candidate data, edit test questions, and view reports.
- **Candidate**: 
  - Log in, complete each test within the specified time limit, and view results.

## Contributing
Contributions are welcome! Please fork this repository, create a new branch for your feature or bugfix, and submit a pull request.
