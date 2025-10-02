# Web Meeting Scheduler API
*RESTful Web API* built on *Clean Architecture* and *SOLID* principles for scheduling meetings as early as possible without conflicts. This project implements *JWT*-based authentication and role-based authorization for *Keycloak* support, database operations with *EF Core*, *CQRS* pattern using the *MediatR* library, and comprehensive logging of operation types and outcomes.

## Prerequisites
- .NET 9.0 SDK
- MySQL
- Keycloak (optional)
- xUnit

## Getting Started
1. Configure appsettings.json with MySQL connection string.
2. Database setup:
```bash
# installing EF Core tools
dotnet tool install --global dotnet-ef

# creating migrations
dotnet ef migrations add InitCreate --project WebMeetingScheduler.Infrastructure --startup-project WebMeetingScheduler.Web

# updating migrations
dotnet ef database update --project WebMeetingScheduler.Infrastructure --startup-project WebMeetingScheduler.Web
```
3. Build and run solution.

## API Endpoints
**Authentication Endpoints**
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Authentication/login` | Login with password |
| POST | `/api/Authentication/refresh-token` | Refresh token |

**Participants Endpoints**
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Participants` | Get all participants |
| GET | `/api/Participants/{id}` | Get participant by ID |
| GET | `/api/Participants/{id}/meetings` | Get participant's meetings |
| POST | `/api/Participants` | Create new participant |
| PUT | `/api/Participants/{id}` | Update participant |
| DELETE | `/api/Participants/{id}` | Delete participant |

**Meetings Endpoints**
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Meetings` | Get all meetings |
| GET | `/api/Meetings/{id}` | Get meeting by ID |
| PUT | `/api/Meetings/{id}` | Update meeting |
| DELETE | `/api/Meetings/{id}` | Delete meeting |
| POST | `/api/Meetings/{meetingId}/participants/{participantId}` | Add participant |
| DELETE | `/api/Meetings/{meetingId}/participants/{participantId}` | Remove participant |

## Examples of Requests
**Authentication**

POST `https://localhost:5001/api/Authentication/login`
```json
{
  "username": "admin",
  "password": "admin123"
}
```
**Create Participant**

POST `https://localhost:5001/api/Participants`
```json
{
  "fullName": "Carl Cori",
  "role": "Software Engineer",
  "email": "cori.carl@company.com"
}
```
**Create Meeting**

POST `https://localhost:5001/api/Meetings`
```json
{
  "title": "Sprint Retrospective",
  "description": "Review last sprint",
  "duration": 60,
  "start": "2025-10-02T09:30:00Z",
  "end": "2025-10-02T15:00:00Z"
}
```
**Update Participant**

PUT `https://localhost:5001/api/Participants/{id}`
```json
{
  "fullName": "Corwin Silver",
  "role": "Senior Software Engineer",
  "email": "corwin.silver@company.com"
}
```
**Update Meeting**

PUT `https://localhost:5001/api/Meetings/{id}`
```json
{
  "title": "Sprint Retrospective - Updated",
  "description": "Updated description"
}
```