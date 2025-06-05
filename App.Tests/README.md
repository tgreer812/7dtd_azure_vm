# App.Tests

This project contains comprehensive unit tests for the 7DTD frontend application.

## Running Tests

### Prerequisites
- .NET 8.0 SDK
- Dependencies will be restored automatically

### Commands

#### Linux/macOS (Bash)
```bash
# Run all tests
dotnet test

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests in verbose mode
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter "FullyQualifiedName~ServerApiServiceTests"

# Run specific test method
dotnet test --filter "FullyQualifiedName~GetVmStatusAsync_ReturnsVmStatus_WhenApiCallSucceeds"
```

#### Windows (PowerShell)
```powershell
# Run all tests
dotnet test

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests in verbose mode
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter "FullyQualifiedName~ServerApiServiceTests"

# Run specific test method
dotnet test --filter "FullyQualifiedName~GetVmStatusAsync_ReturnsVmStatus_WhenApiCallSucceeds"
```

#### Windows (Command Prompt)
```cmd
REM Run all tests
dotnet test

REM Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"

REM Run tests in verbose mode
dotnet test --verbosity normal

REM Run specific test class
dotnet test --filter "FullyQualifiedName~ServerApiServiceTests"

REM Run specific test method
dotnet test --filter "FullyQualifiedName~GetVmStatusAsync_ReturnsVmStatus_WhenApiCallSucceeds"
```

## Test Structure

### Services Tests (`Services/`)
- **ServerApiServiceTests.cs**: Comprehensive tests for the API service layer
  - Tests all API endpoints (VM management and game server)
  - Tests error handling scenarios
  - Tests HTTP status code handling
  - Tests JSON parsing errors
  - Uses mocked HttpClient for isolated testing

### Models Tests (`Models/`)
- **ApiModelsTests.cs**: Tests for data models
  - Tests default values
  - Tests property setting
  - Tests enum values validation

## Test Coverage

The tests provide comprehensive coverage of:
- All API service methods
- Error handling paths
- Data model validation
- Configuration handling
- HTTP status code scenarios

## Implementation Notes

- Uses xUnit as the testing framework
- Uses Moq for mocking dependencies
- HttpClient is mocked to avoid network dependencies
- Tests are isolated and can run in parallel
- All tests follow AAA pattern (Arrange, Act, Assert)