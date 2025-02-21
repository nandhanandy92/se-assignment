using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RL.Backend.Commands;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public class PlanProcedureUserController : ControllerBase
{
    private readonly ILogger<PlanProcedureUserController> _logger;
    private readonly RLContext _context;
    private readonly IMediator _mediator;

    public PlanProcedureUserController(
        ILogger<PlanProcedureUserController> logger,
        RLContext context,
        IMediator mediator)
    {
        _logger = logger;
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Gets all user assignments with OData query support
    /// </summary>
    /// <returns>List of user assignments</returns>
    [HttpGet]
    [EnableQuery]
    [ProducesResponseType(typeof(IEnumerable<PlanProcedureUser>), StatusCodes.Status200OK)]
    public IEnumerable<PlanProcedureUser> Get()
    {
        return _context.PlanProcedureUsers;
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignUser([FromBody] AssignUserToPlanProcedureCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Exception != null)
            return BadRequest(result.Exception.Message);
            
        return Ok();
    }

    /// <summary>
    /// Removes a specific user assignment from a plan procedure
    /// </summary>
    /// <param name="command">Assignment details containing PlanId, ProcedureId, and UserId</param>
    /// <returns>No content on success</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnassignUser([FromQuery] UnassignUserFromPlanProcedureCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Exception != null)
            return BadRequest(result.Exception.Message);

        return NoContent();
    }

    /// <summary>
    /// Removes all user assignments from a specific plan procedure
    /// </summary>
    /// <param name="command">Plan procedure details containing PlanId and ProcedureId</param>
    /// <returns>No content on success</returns>
    [HttpDelete("clear")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClearUsers([FromQuery] ClearUsersFromPlanProcedureCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Exception != null)
            return BadRequest(result.Exception.Message);

        return NoContent();
    }
}
