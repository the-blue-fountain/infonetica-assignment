
using WorkflowEngine.Models;
using WorkflowEngine.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<WorkflowRepository>();
builder.Services.AddSingleton<WorkflowService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- Workflow API Endpoints ---

// Create workflow definition
app.MapPost("/workflows", (WorkflowDefinition def, WorkflowService svc) =>
{
    if (!svc.AddDefinition(def, out var error))
        return Results.BadRequest(new { error });
    return Results.Created($"/workflows/{def.Id}", def);
});

// Get workflow definition
app.MapGet("/workflows/{id}", (string id, WorkflowRepository repo) =>
{
    if (!repo.Definitions.TryGetValue(id, out var def))
        return Results.NotFound(new { error = "Workflow definition not found." });
    return Results.Ok(def);
});

// List all workflow definitions
app.MapGet("/workflows", (WorkflowRepository repo) => Results.Ok(repo.Definitions.Values));

// Start a new workflow instance
app.MapPost("/workflows/{id}/instances", (string id, WorkflowService svc) =>
{
    var instance = svc.StartInstance(id, out var error);
    if (instance == null)
        return Results.BadRequest(new { error });
    return Results.Created($"/instances/{instance.Id}", instance);
});

// List all instances
app.MapGet("/instances", (WorkflowRepository repo) => Results.Ok(repo.Instances.Values));

// Get instance state and history
app.MapGet("/instances/{id}", (string id, WorkflowRepository repo) =>
{
    if (!repo.Instances.TryGetValue(id, out var instance))
        return Results.NotFound(new { error = "Instance not found." });
    return Results.Ok(instance);
});

// Execute action on instance
app.MapPost("/instances/{id}/actions/{actionId}", (string id, string actionId, WorkflowService svc) =>
{
    var (success, error) = svc.ExecuteAction(id, actionId);
    if (!success)
        return Results.BadRequest(new { error });
    return Results.Ok();
});

// List states for a workflow
app.MapGet("/workflows/{id}/states", (string id, WorkflowRepository repo) =>
{
    if (!repo.Definitions.TryGetValue(id, out var def))
        return Results.NotFound(new { error = "Workflow definition not found." });
    return Results.Ok(def.States);
});

// List actions for a workflow
app.MapGet("/workflows/{id}/actions", (string id, WorkflowRepository repo) =>
{
    if (!repo.Definitions.TryGetValue(id, out var def))
        return Results.NotFound(new { error = "Workflow definition not found." });
    return Results.Ok(def.Actions);
});

app.Run();
