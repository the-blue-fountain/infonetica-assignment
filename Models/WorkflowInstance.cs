namespace WorkflowEngine.Models
{
    public class WorkflowInstance
    {
        public string Id { get; set; } = string.Empty;
        public string DefinitionId { get; set; } = string.Empty;
        public string CurrentStateId { get; set; } = string.Empty;
        public List<InstanceHistoryEntry> History { get; set; } = new();
    }

    public class InstanceHistoryEntry
    {
        public string ActionId { get; set; } = string.Empty;
        public string FromStateId { get; set; } = string.Empty;
        public string ToStateId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
