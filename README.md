# Playwright Test Framework
Automated browser testing framework for slot game applications using Playwright and C#.

## Project Structure
- **PageObjects/** - Page object models for UI components and pages
  - Components/ - Reusable UI components (buttons, displays, controls)
  - Pages/ - Game page models
- **Tests/** - Test cases organized by game
  - AllAboutTheWilds95/
  - IcyFruits10/
  - IrishWildsTests/
- **Services/** - Browser and application services
- **Helpers/** - Utility functions for waits, retries, screenshots, mocks
- **Configuration/** - Test and service configuration
- **Constants/** - Application constants
- **Data/** - Test data and mock responses

## CI/CD Pipeline
The GitHub Actions workflow pipeline is located at:
- **.github/workflows/dotnet-desktop.yml** - Automated build and test pipeline

This workflow runs on pushes and pull requests to the main branch, performing:
- Build the solution with .NET 8.0
- Publish build artifacts
- Execute automated tests
- Generate test results

## Prerequisites
- .NET 8.0 SDK or later
- Visual Studio Code

## Setup
1. Restore the project
2. Build the project: dotnet build

## Configuration
Edit `appsettings.json` to customize:
- Browser type/device and headless mode
- Navigation and action timeouts
- Screenshot capture settings
- Logging options

## Running Tests
Run all tests: dotnet test
Run specific test file: dotnet test --filter ClassName=IrishWildsTests
Test results and screenshots are saved in the `bin/Debug/net8.0/` directory.
