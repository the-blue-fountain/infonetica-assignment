using WorkflowEngine.Models;

namespace WorkflowEngine.Services
{
    public class WorkflowRepository
    {
        public Dictionary<string, WorkflowDefinition> Definitions { get; } = new();
        public Dictionary<string, WorkflowInstance> Instances { get; } = new();
    }
}
