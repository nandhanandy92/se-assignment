using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers;

public class UnassignUserFromPlanProcedureCommandHandler : IRequestHandler<UnassignUserFromPlanProcedureCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public UnassignUserFromPlanProcedureCommandHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(UnassignUserFromPlanProcedureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate request
            if (request.PlanId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));
            if (request.UserId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid UserId"));

            var assignment = await _context.PlanProcedureUsers
                .FirstOrDefaultAsync(ppu => 
                    ppu.PlanId == request.PlanId && 
                    ppu.ProcedureId == request.ProcedureId && 
                    ppu.UserId == request.UserId,
                    cancellationToken);

            if (assignment == null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"Assignment not found for PlanId: {request.PlanId}, ProcedureId: {request.ProcedureId}, UserId: {request.UserId}"));

            _context.PlanProcedureUsers.Remove(assignment);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }
}
