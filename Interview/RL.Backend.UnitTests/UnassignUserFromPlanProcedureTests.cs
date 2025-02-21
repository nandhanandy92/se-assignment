using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers;
using RL.Backend.Exceptions;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.UnitTests;

[TestClass]
public class UnassignUserFromPlanProcedureTests
{
    [TestMethod]
    [DataRow(-1, 1, 1)]
    [DataRow(0, 1, 1)]
    [DataRow(1, -1, 1)]
    [DataRow(1, 0, 1)]
    [DataRow(1, 1, -1)]
    [DataRow(1, 1, 0)]
    public async Task UnassignUser_InvalidIds_ReturnsBadRequest(int planId, int procedureId, int userId)
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new UnassignUserFromPlanProcedureCommandHandler(context);
        var request = new UnassignUserFromPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        // When
        var result = await sut.Handle(request, new CancellationToken());

        // Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    public async Task UnassignUser_AssignmentNotFound_ReturnsNotFound(int planId, int procedureId, int userId)
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new UnassignUserFromPlanProcedureCommandHandler(context);
        var request = new UnassignUserFromPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        // When
        var result = await sut.Handle(request, new CancellationToken());

        // Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    public async Task UnassignUser_ValidAssignment_RemovesAssignment(int planId, int procedureId, int userId)
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new UnassignUserFromPlanProcedureCommandHandler(context);
        var request = new UnassignUserFromPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        context.Plans.Add(new Plan { PlanId = planId });
        context.Procedures.Add(new Procedure { ProcedureId = procedureId });
        context.Users.Add(new User { UserId = userId });
        context.PlanProcedures.Add(new PlanProcedure { PlanId = planId, ProcedureId = procedureId });
        context.PlanProcedureUsers.Add(new PlanProcedureUser { PlanId = planId, ProcedureId = procedureId, UserId = userId });
        await context.SaveChangesAsync();

        // When
        var result = await sut.Handle(request, new CancellationToken());

        // Then
        result.Exception.Should().BeNull();
        result.Succeeded.Should().BeTrue();
        var assignment = await context.PlanProcedureUsers.FirstOrDefaultAsync(ppu =>
            ppu.PlanId == planId &&
            ppu.ProcedureId == procedureId &&
            ppu.UserId == userId);
        assignment.Should().BeNull();
    }
}
