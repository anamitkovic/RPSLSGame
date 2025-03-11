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
   docker-compose up --build
   ```

   This will start:
   - The API service (`rpslsgame-api`) on port 8081
   - The PostgreSQL database (`rpslsgame-db`) on port 5433
   - The database will be automatically updated via the entrypoint.

3. Apply Database Migrations
   Since the database is empty, you need to apply migrations manually inside the API container:
   ```sh
   docker exec -it rpslsgame-api dotnet ef database update --project src/RPSLSGame.Infrastructure --startup-project src/RPSLSGame.Api
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
    "email": "user@example.com",
    "player": 2
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


