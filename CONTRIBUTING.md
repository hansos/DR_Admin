# Contributing to DR_Admin

Thank you for your interest in contributing to DR_Admin! This document provides guidelines and instructions for contributing to this project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [How to Contribute](#how-to-contribute)
- [Pull Request Process](#pull-request-process)
- [Coding Standards](#coding-standards)
- [Reporting Issues](#reporting-issues)

## Code of Conduct

By participating in this project, you agree to maintain a respectful and inclusive environment. Please:

- Be respectful and considerate in all communications
- Accept constructive criticism gracefully
- Focus on what is best for the project and community
- Show empathy towards other contributors

## Getting Started

1. Fork the repository on GitHub
2. Clone your fork locally:
   ```bash
   git clone https://github.com/YOUR_USERNAME/DR_Admin.git
   cd DR_Admin
   ```
3. Add the upstream repository as a remote:
   ```bash
   git remote add upstream https://github.com/hansos/DR_Admin.git
   ```
4. Create a new branch for your work:
   ```bash
   git checkout -b feature/your-feature-name
   ```

## Development Setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A code editor (Visual Studio 2022+, VS Code, or JetBrains Rider)
- Git

### Building the Solution

```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Run tests
dotnet test
```

### Running Locally

1. Configure your local `appsettings.Development.json` in the `DR_Admin` project
2. Run the API:
   ```bash
   dotnet run --project DR_Admin
   ```
3. Run the Web UI (optional):
   ```bash
   dotnet run --project DR_Admin_Web
   ```

### Database Setup

The project supports SQLite (default), SQL Server, and PostgreSQL. For development, SQLite is recommended:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DR_Admin.db"
  },
  "DbSettings": {
    "DatabaseType": "SQLITE"
  }
}
```

## How to Contribute

### Types of Contributions

- **Bug Fixes** - Fix issues reported in the issue tracker
- **Features** - Implement new functionality (discuss first in an issue)
- **Documentation** - Improve README, code comments, or create tutorials
- **Tests** - Add or improve test coverage
- **Refactoring** - Improve code quality without changing functionality

### Before You Start

1. Check the [issue tracker](https://github.com/hansos/DR_Admin/issues) for existing issues
2. For new features, create an issue first to discuss the proposal
3. Ensure your changes align with the project's architecture and goals

## Pull Request Process

1. **Update your fork** with the latest upstream changes:
   ```bash
   git fetch upstream
   git rebase upstream/master
   ```

2. **Make your changes** in a feature branch:
   - Write clear, concise commit messages
   - Keep commits focused and atomic
   - Include tests for new functionality

3. **Ensure quality**:
   ```bash
   # Build the solution
   dotnet build
   
   # Run all tests
   dotnet test
   
   # Check for any compiler warnings
   dotnet build --warnaserror
   ```

4. **Push your branch** and create a Pull Request:
   ```bash
   git push origin feature/your-feature-name
   ```

5. **Fill out the PR template** with:
   - Description of changes
   - Related issue numbers
   - Screenshots (if applicable)
   - Testing steps

6. **Respond to feedback** from reviewers promptly

### PR Requirements

- All tests must pass
- Code must build without errors
- Follow the coding standards (see below)
- Include appropriate documentation updates
- Keep PRs focused - one feature or fix per PR

## Coding Standards

### General Guidelines

- Follow existing code patterns and conventions in the codebase
- Use meaningful variable and method names
- Keep methods small and focused (single responsibility)
- Write self-documenting code; add comments only when necessary

### C# Conventions

- Use C# 12+ features where appropriate
- Enable nullable reference types (`<Nullable>enable</Nullable>`)
- Use `var` when the type is obvious from the right side
- Prefer async/await for asynchronous operations
- Use records for DTOs when appropriate

### Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Classes | PascalCase | `CustomerService` |
| Interfaces | IPascalCase | `ICustomerService` |
| Methods | PascalCase | `GetCustomerById` |
| Properties | PascalCase | `FirstName` |
| Private fields | _camelCase | `_customerRepository` |
| Local variables | camelCase | `customerCount` |
| Constants | PascalCase | `MaxRetryCount` |

### Project Structure

- **Controllers** - API endpoints, thin with minimal logic
- **Services** - Business logic layer
- **Data/Entities** - Entity Framework entities
- **DTOs** - Data Transfer Objects for API contracts
- **Domain** - Domain events, state machines, and workflows

### Library Contributions

When adding new integrations to the library projects:

- Inherit from the appropriate base class (e.g., `BaseRegistrar`, `BasePaymentGateway`)
- Implement all interface methods
- Add settings class in `Infrastructure/Settings`
- Update the factory class to support the new provider
- Include README updates for the library

## Reporting Issues

### Bug Reports

When reporting bugs, please include:

1. **Summary** - Brief description of the issue
2. **Environment** - OS, .NET version, database type
3. **Steps to Reproduce** - Detailed steps to reproduce the issue
4. **Expected Behavior** - What you expected to happen
5. **Actual Behavior** - What actually happened
6. **Logs/Screenshots** - Any relevant error messages or screenshots

### Feature Requests

For feature requests, please include:

1. **Summary** - Brief description of the feature
2. **Motivation** - Why this feature would be useful
3. **Proposed Solution** - How you envision the feature working
4. **Alternatives** - Any alternative solutions you've considered

## Questions?

If you have questions about contributing, feel free to:

- Open a discussion on GitHub
- Create an issue with the "question" label

Thank you for contributing to DR_Admin!
