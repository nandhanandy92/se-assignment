using MediatR;
using RL.Backend.Models;

namespace RL.Backend.Commands;

/// <summary>
/// Command to remove all user assignments from a plan procedure
/// </summary>
public class ClearUsersFromPlanProcedureCommand : IRequest<ApiResponse<Unit>>
{
    /// <summary>The ID of the plan</summary>
    /// <example>1</example>
    public int PlanId { get; set; }

    /// <summary>The ID of the procedure</summary>
    /// <example>2</example>
    public int ProcedureId { get; set; }
}
