using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers;
using RL.Backend.Exceptions;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.UnitTests;

[TestClass]
public class AssignUserToPlanProcedureTests
{
    [TestMethod]
    [DataRow(-1, 1, 1)]
    [DataRow(0, 1, 1)]
    [DataRow(int.MinValue, 1, 1)]
    public async Task AssignUser_InvalidPlanId_ReturnsBadRequest(int planId, int procedureId, int userId)
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new AssignUserToPlanProcedureCommandHandler(context);
        var request = new AssignUserToPlanProcedureCommand
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
    [DataRow(1, -1, 1)]
    [DataRow(1, 0, 1)]
    [DataRow(1, int.MinValue, 1)]
    public async Task AssignUser_InvalidProcedureId_ReturnsBadRequest(int planId, int procedureId, int userId)
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new AssignUserToPlanProcedureCommandHandler(context);
        var request = new AssignUserToPlanProcedureCommand
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
    [DataRow(1, 1, -1)]
    [DataRow(1, 1, 0)]
    [DataRow(1, 1, int.MinValue)]
    public async Task AssignUser_InvalidUserId_ReturnsBadRequest(int planId, int procedureId, int userId)
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new AssignUserToPlanProcedureCommandHandler(context);
        var request = new AssignUserToPlanProcedureCommand
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
    public async Task AssignUser_PlanProcedureNotFound_ReturnsNotFound(int planId, int procedureId, int userId)
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new AssignUserToPlanProcedureCommandHandler(context);
        var request = new AssignUserToPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        context.Users.Add(new User { UserId = userId });
        await context.SaveChangesAsync();

        // When
        var result = await sut.Handle(request, new CancellationToken());

        // Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    public async Task AssignUser_UserNotFound_ReturnsNotFound(int planId, int procedureId, int userId)
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new AssignUserToPlanProcedureCommandHandler(context);
        var request = new AssignUserToPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        context.Plans.Add(new Plan { PlanId = planId });
        context.Procedures.Add(new Procedure { ProcedureId = procedureId });
        context.PlanProcedures.Add(new PlanProcedure { PlanId = planId, ProcedureId = procedureId });
        await context.SaveChangesAsync();

        // When
        var result = await sut.Handle(request, new CancellationToken());

        // Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    public async Task AssignUser_AlreadyAssigned_ReturnsSuccess(int planId, int procedureId, int userId)
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new AssignUserToPlanProcedureCommandHandler(context);
        var request = new AssignUserToPlanProcedureCommand
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
        result.Succeeded.Should().BeTrue();
        var assignments = await context.PlanProcedureUsers.CountAsync(ppu => 
            ppu.PlanId == planId && 
            ppu.ProcedureId == procedureId && 
            ppu.UserId == userId);
        assignments.Should().Be(1);
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    public async Task AssignUser_ValidAssignment_CreatesAssignment(int planId, int procedureId, int userId)
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new AssignUserToPlanProcedureCommandHandler(context);
        var request = new AssignUserToPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        context.Plans.Add(new Plan { PlanId = planId });
        context.Procedures.Add(new Procedure { ProcedureId = procedureId });
        context.Users.Add(new User { UserId = userId });
        context.PlanProcedures.Add(new PlanProcedure { PlanId = planId, ProcedureId = procedureId });
        await context.SaveChangesAsync();

        // When
        var result = await sut.Handle(request, new CancellationToken());

        // Then
        result.Succeeded.Should().BeTrue();
        var assignment = await context.PlanProcedureUsers.FirstOrDefaultAsync(ppu => 
            ppu.PlanId == planId && 
            ppu.ProcedureId == procedureId && 
            ppu.UserId == userId);
        assignment.Should().NotBeNull();
        assignment.CreateDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.UpdateDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
