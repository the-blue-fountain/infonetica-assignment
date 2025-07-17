using WorkflowEngine.Models;

namespace WorkflowEngine.Services
{
    public class WorkflowService
    {
        private readonly WorkflowRepository _repo;
        public WorkflowService(WorkflowRepository repo)
        {
            _repo = repo;
        }

        // Validation for workflow definition
        public (bool isValid, string? error) ValidateDefinition(WorkflowDefinition def)
        {
            if (string.IsNullOrWhiteSpace(def.Id))
                return (false, "WorkflowDefinition must have an Id.");
            if (_repo.Definitions.ContainsKey(def.Id))
                return (false, "Duplicate workflow definition Id.");
            if (def.States.GroupBy(s => s.Id).Any(g => g.Count() > 1))
                return (false, "Duplicate state Ids in definition.");
            if (def.Actions.GroupBy(a => a.Id).Any(g => g.Count() > 1))
                return (false, "Duplicate action Ids in definition.");
            if (def.States.Count(s => s.IsInitial) != 1)
                return (false, "Definition must have exactly one initial state.");
            var stateIds = def.States.Select(s => s.Id).ToHashSet();
            foreach (var action in def.Actions)
            {
                if (!stateIds.Contains(action.ToState))
                    return (false, $"Action {action.Id} points to unknown toState {action.ToState}.");
                foreach (var from in action.FromStates)
                    if (!stateIds.Contains(from))
                        return (false, $"Action {action.Id} has unknown fromState {from}.");
            }
            return (true, null);
        }

        // Add workflow definition
        public bool AddDefinition(WorkflowDefinition def, out string? error)
        {
            var (isValid, err) = ValidateDefinition(def);
            if (!isValid)
            {
                error = err;
                return false;
            }
            _repo.Definitions[def.Id] = def;
            error = null;
            return true;
        }

        // Start a new instance
        public WorkflowInstance? StartInstance(string defId, out string? error)
        {
            if (!_repo.Definitions.TryGetValue(defId, out var def))
            {
                error = "Workflow definition not found.";
                return null;
            }
            var initial = def.States.FirstOrDefault(s => s.IsInitial && s.Enabled);
            if (initial == null)
            {
                error = "No enabled initial state found.";
                return null;
            }
            var instance = new WorkflowInstance
            {
                Id = Guid.NewGuid().ToString(),
                DefinitionId = defId,
                CurrentStateId = initial.Id,
                History = new List<InstanceHistoryEntry>()
            };
            _repo.Instances[instance.Id] = instance;
            error = null;
            return instance;
        }

        // Execute action on instance
        public (bool success, string? error) ExecuteAction(string instanceId, string actionId)
        {
            if (!_repo.Instances.TryGetValue(instanceId, out var instance))
                return (false, "Instance not found.");
            if (!_repo.Definitions.TryGetValue(instance.DefinitionId, out var def))
                return (false, "Workflow definition not found.");
            var currentState = def.States.FirstOrDefault(s => s.Id == instance.CurrentStateId);
            if (currentState == null)
                return (false, "Current state not found in definition.");
            if (currentState.IsFinal)
                return (false, "Cannot execute actions from a final state.");
            var action = def.Actions.FirstOrDefault(a => a.Id == actionId);
            if (action == null)
                return (false, "Action not found in definition.");
            if (!action.Enabled)
                return (false, "Action is disabled.");
            if (!action.FromStates.Contains(currentState.Id))
                return (false, "Current state is not a valid source for this action.");
            var toState = def.States.FirstOrDefault(s => s.Id == action.ToState && s.Enabled);
            if (toState == null)
                return (false, "Target state is not enabled or does not exist.");
            // Perform transition
            instance.History.Add(new InstanceHistoryEntry
            {
                ActionId = action.Id,
                FromStateId = currentState.Id,
                ToStateId = toState.Id,
                Timestamp = DateTime.UtcNow
            });
            instance.CurrentStateId = toState.Id;
            return (true, null);
        }
    }
}
