using RL.Data.DataModels.Common;

namespace RL.Data.DataModels;

public class PlanProcedureUser : IChangeTrackable
{
    // Composite key will be formed by these three properties
    public int PlanId { get; set; }
    public int ProcedureId { get; set; }
    public int UserId { get; set; }

    // Navigation properties
    public virtual PlanProcedure PlanProcedure { get; set; }
    public virtual User User { get; set; }

    // IChangeTrackable implementation
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}
