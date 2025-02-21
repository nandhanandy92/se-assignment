using MediatR;
using RL.Backend.Models;

namespace RL.Backend.Commands;

/// <summary>
/// Command to remove a user assignment from a plan procedure
/// </summary>
public class UnassignUserFromPlanProcedureCommand : IRequest<ApiResponse<Unit>>
{
    /// <summary>The ID of the plan</summary>
    /// <example>1</example>
    public int PlanId { get; set; }

    /// <summary>The ID of the procedure</summary>
    /// <example>2</example>
    public int ProcedureId { get; set; }

    /// <summary>The ID of the user to unassign</summary>
    /// <example>3</example>
    public int UserId { get; set; }
}
