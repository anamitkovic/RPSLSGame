# Rock Paper Scissors Lizard Spock API

This project provides a REST API for playing Rock Paper Scissors Lizard Spock. The API allows users to play the game, retrieve choices, and fetch their game history.

## Prerequisites

- Docker & Docker Compose
- .NET 8 SDK (if running locally)
- PostgreSQL client (optional, for direct DB access)

## Getting Started

### Running with Docker

1. Clone the repository:

   ```sh
   git clone https://github.com/anamitkovic/RPSLSGame.git
   cd RPSLSGame
   ```

2. Build and run the containers:

   ```sh
   docker-compose up --build -d
   ```

   This will start:

    - The API service (`rpslsgame-api`) on port 8081
    - The PostgreSQL database (`rpslsgame-db`) on port 5433

3. Apply Database Migrations

   Since the database is empty, you need to apply migrations manually inside the API container:

   ```sh
   dotnet ef database update --project src/RPSLSGame.Infrastructure --startup-project src/RPSLSGame.Api
   ```

4. Verify that the API is running: Open your browser or use a tool like Postman to check:

   ```sh
   http://localhost:8081/swagger/index.html
   ```

## API Endpoints

### Get Choices

```http
GET /choices
```

### Get Random Choice

```http
GET /choice
```

### Play a Game

```http
POST /play
Content-Type: application/json

{
    "player": 2,
    "email": "user@example.com" // Optional
}
```

### Get Game History

```http
POST /history/search
Content-Type: application/json

{
    "email": "user@example.com"
}
```

## Connecting to the Database

To manually connect to the PostgreSQL database inside the running container:

```sh
docker exec -it rpslsgame-db psql -U postgres
```

Once inside the PostgreSQL shell, you can check if the tables exist:

```sql
SELECT * FROM "GameResults";
```

## Running Locally Without Docker

> **Note: PostgreSQL is only required if you want to use optional features like saving email for tracking and the /history/search endpoint. For basic game functionality, the database is not needed. If you do not require these features, skip steps 1 and 2 and proceed directly to step 3.**

If you want to run the project locally, make sure you have .NET 8 installed. Then:

1. Update the `appsettings.Development.json` file to point to your local PostgreSQL instance.
2. Apply migrations manually:
   ```sh
   dotnet ef database update --project src/RPSLSGame.Infrastructure --startup-project src/RPSLSGame.Api
   ```
3. Start the API:
   ```sh
   dotnet run --project src/RPSLSGame.Api
   ```

Now, your API should be accessible at `http://localhost:5000/swagger/index.html`.

## Running Tests

To ensure that everything is working correctly, you can run the integration and unit tests.

### Running Tests Locally with In-Memory Database, WireMock

Integration tests use **In-Memory Database** (for simulating database operations) and **WireMock** (to simulate external service responses).

1. **Run the tests** using the .NET CLI:
   ```sh
   dotnet test tests/RPSLSGame.Tests
   ```

### Understanding WireMock in Tests

**WireMock** is used to mock the external random number service. The test factory class (`CustomWebApplicationFactory`) starts WireMock on port `9090` and configures it to respond with test data.

- To set up a **valid response**:
  ```csharp
  factory.SetupValidResponse();
  ```
- To simulate a **service unavailable** response:
  ```csharp
  factory.SetupServiceUnavailable();
  ```

Before each test, WireMock mappings are reset to ensure test consistency.

## Polly Retry Configuration

Polly retry policies are applied globally to improve resilience when communicating with external services. The retry mechanism is specifically configured for the **Random Number API**, using:

- **Retry Policy**: Retries failed HTTP requests up to **3 times**, with an **exponential backoff strategy** (`2s, 4s, 8s`).
- **Timeout Policy**: Cancels requests that take longer than **5 seconds**.
- **Circuit Breaker Policy**: If too many requests fail within **30 seconds**, the circuit will open for **15 seconds** to prevent overloading the external service.

Polly is integrated into the `HttpClient` used by the application, making communication with the Random Number API more resilient. If an external service returns an error (like **503 Service Unavailable**), Polly will attempt retries based on an exponential backoff strategy before failing permanently.

### Debugging Failing Tests

If the tests fail unexpectedly, try the following:

- Ensure that the WireMock server is running by checking its logs.
- Manually test the mock endpoint to verify it responds correctly:
  ```sh
  curl http://localhost:9090/random
  ```

