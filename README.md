
# Infonetica – Configurable Workflow Engine (State-Machine API)

## Overview
This project implements a minimal backend service for defining and running configurable workflows as state machines. It is built with C# 12 and .NET 9, using ASP.NET Core Minimal API style. The service allows clients to:

- Define workflow state machines (states and actions)
- Start workflow instances
- Execute actions to move instances between states, with validation
- Inspect/list states, actions, definitions, and running instances

All data is stored in memory. No external dependencies or databases are used.

---

## Quick Start

1. **Install .NET 9 SDK and ASP.NET Core Runtime**
    - On Manjaro: `sudo pacman -Sy dotnet-sdk aspnet-runtime`
2. **Build and run the API:**
    ```sh
    dotnet run
    ```
    The API will start (default: `http://localhost:5000` or as shown in the output).
3. **Open Swagger UI:**
    - Visit `http://localhost:5000/swagger` in your browser for interactive API documentation and testing.

---

## Project Structure

- `Models/` — Data models: State, ActionTransition, WorkflowDefinition, WorkflowInstance
- `Services/` — Business logic: validation, state transitions, in-memory storage
- `Program.cs` — Main entry point and API endpoint definitions

---

## API Endpoints

| Method | Route                                         | Description                                 |
|--------|-----------------------------------------------|---------------------------------------------|
| POST   | `/workflows`                                  | Create a new workflow definition            |
| GET    | `/workflows/{id}`                             | Retrieve a workflow definition              |
| GET    | `/workflows`                                  | List all workflow definitions               |
| POST   | `/workflows/{id}/instances`                   | Start a new workflow instance               |
| GET    | `/instances`                                  | List all workflow instances                 |
| GET    | `/instances/{id}`                             | Get current state and history of an instance|
| POST   | `/instances/{id}/actions/{actionId}`          | Execute an action on an instance            |
| GET    | `/workflows/{id}/states`                      | List all states for a workflow              |
| GET    | `/workflows/{id}/actions`                     | List all actions for a workflow             |

All endpoints return JSON. See Swagger UI for schemas and example payloads.

---

## Example Workflow Definition (JSON)

```
{
   "id": "leave-approval",
   "name": "Leave Approval Workflow",
   "states": [
      { "id": "draft", "name": "Draft", "isInitial": true, "isFinal": false, "enabled": true },
      { "id": "pending", "name": "Pending Approval", "isInitial": false, "isFinal": false, "enabled": true },
      { "id": "approved", "name": "Approved", "isInitial": false, "isFinal": true, "enabled": true },
      { "id": "rejected", "name": "Rejected", "isInitial": false, "isFinal": true, "enabled": true }
   ],
   "actions": [
      { "id": "submit", "name": "Submit", "enabled": true, "fromStates": ["draft"], "toState": "pending" },
      { "id": "approve", "name": "Approve", "enabled": true, "fromStates": ["pending"], "toState": "approved" },
      { "id": "reject", "name": "Reject", "enabled": true, "fromStates": ["pending"], "toState": "rejected" }
   ]
}
```

---

## Assumptions & Notes

- All data is stored in memory; restarting the app clears all workflows and instances.
- No authentication or authorization is implemented.
- Validation rules are enforced for workflow definitions and action execution (see assignment spec).
- No concurrency handling; not suitable for production use.
- No custom frontend is provided; the API can be explored and tested interactively using the built-in Swagger UI at `/swagger`.
- Minimal comments and no unit tests (time-boxed as per instructions).
- If you need to re-create the project, run:
   ```sh
   dotnet new webapi --no-https --framework net9.0 -o .
   ```

---

## Known Limitations

- In-memory only; no persistence across restarts
- No user authentication or access control
- No concurrency or multi-user safety
- No advanced error handling or logging
- No custom front-end; interactive API documentation and testing is available via Swagger UI


