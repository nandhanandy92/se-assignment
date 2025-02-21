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
public class ClearUsersFromPlanProcedureTests
{
    [TestMethod]
    [DataRow(-1, 1)]
    [DataRow(0, 1)]
    [DataRow(1, -1)]
    [DataRow(1, 0)]
    public async Task ClearUsers_InvalidIds_ReturnsBadRequest(int planId, int procedureId)
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new ClearUsersFromPlanProcedureCommandHandler(context);
        var request = new ClearUsersFromPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        // When
        var result = await sut.Handle(request, new CancellationToken());

        // Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1)]
    public async Task ClearUsers_PlanProcedureNotFound_ReturnsNotFound(int planId, int procedureId)
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new ClearUsersFromPlanProcedureCommandHandler(context);
        var request = new ClearUsersFromPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        // When
        var result = await sut.Handle(request, new CancellationToken());

        // Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1)]
    public async Task ClearUsers_NoAssignments_ReturnsSuccess(int planId, int procedureId)
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new ClearUsersFromPlanProcedureCommandHandler(context);
        var request = new ClearUsersFromPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        context.Plans.Add(new Plan { PlanId = planId });
        context.Procedures.Add(new Procedure { ProcedureId = procedureId });
        context.PlanProcedures.Add(new PlanProcedure { PlanId = planId, ProcedureId = procedureId });
        await context.SaveChangesAsync();

        // When
        var result = await sut.Handle(request, new CancellationToken());

        // Then
        result.Exception.Should().BeNull();
        result.Succeeded.Should().BeTrue();
        var assignments = await context.PlanProcedureUsers.CountAsync(ppu =>
            ppu.PlanId == planId &&
            ppu.ProcedureId == procedureId);
        assignments.Should().Be(0);
    }

    [TestMethod]
    [DataRow(1, 1)]
    public async Task ClearUsers_WithAssignments_RemovesAllAssignments(int planId, int procedureId)
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new ClearUsersFromPlanProcedureCommandHandler(context);
        var request = new ClearUsersFromPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        context.Plans.Add(new Plan { PlanId = planId });
        context.Procedures.Add(new Procedure { ProcedureId = procedureId });
        context.Users.Add(new User { UserId = 1 });
        context.Users.Add(new User { UserId = 2 });
        context.PlanProcedures.Add(new PlanProcedure { PlanId = planId, ProcedureId = procedureId });
        context.PlanProcedureUsers.Add(new PlanProcedureUser { PlanId = planId, ProcedureId = procedureId, UserId = 1 });
        context.PlanProcedureUsers.Add(new PlanProcedureUser { PlanId = planId, ProcedureId = procedureId, UserId = 2 });
        await context.SaveChangesAsync();

        // When
        var result = await sut.Handle(request, new CancellationToken());

        // Then
        result.Exception.Should().BeNull();
        result.Succeeded.Should().BeTrue();
        var assignments = await context.PlanProcedureUsers.CountAsync(ppu =>
            ppu.PlanId == planId &&
            ppu.ProcedureId == procedureId);
        assignments.Should().Be(0);
    }
}
