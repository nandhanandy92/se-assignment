using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers;

public class ClearUsersFromPlanProcedureCommandHandler : IRequestHandler<ClearUsersFromPlanProcedureCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public ClearUsersFromPlanProcedureCommandHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(ClearUsersFromPlanProcedureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate request
            if (request.PlanId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));

            // Verify PlanProcedure exists
            var planProcedure = await _context.PlanProcedures
                .FirstOrDefaultAsync(pp => pp.PlanId == request.PlanId && pp.ProcedureId == request.ProcedureId, 
                    cancellationToken);

            if (planProcedure == null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"PlanProcedure with PlanId: {request.PlanId} and ProcedureId: {request.ProcedureId} not found"));

            // Get all user assignments for this plan procedure
            var assignments = await _context.PlanProcedureUsers
                .Where(ppu => ppu.PlanId == request.PlanId && ppu.ProcedureId == request.ProcedureId)
                .ToListAsync(cancellationToken);

            if (!assignments.Any())
                return ApiResponse<Unit>.Succeed(new Unit()); // No assignments to remove

            _context.PlanProcedureUsers.RemoveRange(assignments);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }
}
