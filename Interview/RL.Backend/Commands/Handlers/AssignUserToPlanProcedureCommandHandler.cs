using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers;

public class AssignUserToPlanProcedureCommandHandler : IRequestHandler<AssignUserToPlanProcedureCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public AssignUserToPlanProcedureCommandHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(AssignUserToPlanProcedureCommand request, CancellationToken cancellationToken)
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

            // Check if PlanProcedure exists
            var planProcedure = await _context.PlanProcedures
                .FirstOrDefaultAsync(pp => pp.PlanId == request.PlanId && pp.ProcedureId == request.ProcedureId);
            if (planProcedure == null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"PlanProcedure with PlanId: {request.PlanId} and ProcedureId: {request.ProcedureId} not found"));

            // Check if User exists
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            if (user == null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"UserId: {request.UserId} not found"));

            // Check if assignment already exists
            var existingAssignment = await _context.PlanProcedureUsers
                .FirstOrDefaultAsync(ppu => 
                    ppu.PlanId == request.PlanId && 
                    ppu.ProcedureId == request.ProcedureId && 
                    ppu.UserId == request.UserId);

            if (existingAssignment != null)
                return ApiResponse<Unit>.Succeed(new Unit()); // Already assigned, return success

            // Create new assignment
            var assignment = new PlanProcedureUser
            {
                PlanId = request.PlanId,
                ProcedureId = request.ProcedureId,
                UserId = request.UserId
            };

            _context.PlanProcedureUsers.Add(assignment);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }
}
