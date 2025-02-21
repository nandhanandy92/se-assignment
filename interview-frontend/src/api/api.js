const api_url = "http://localhost:10010";

export const startPlan = async () => {
    const url = `${api_url}/Plan`;
    const response = await fetch(url, {
        method: "POST",
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
        },
        body: JSON.stringify({}),
    });

    if (!response.ok) throw new Error("Failed to create plan");

    return await response.json();
};

export const addProcedureToPlan = async (planId, procedureId) => {
    const url = `${api_url}/Plan/AddProcedureToPlan`;
    var command = { planId: planId, procedureId: procedureId };
    const response = await fetch(url, {
        method: "POST",
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
        },
        body: JSON.stringify(command),
    });

    if (!response.ok) throw new Error("Failed to create plan");

    return true;
};

export const getProcedures = async () => {
    const url = `${api_url}/Procedures`;
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get procedures");

    return await response.json();
};

export const getPlanProcedures = async (planId) => {
    const url = `${api_url}/PlanProcedure?$filter=planId eq ${planId}&$expand=procedure`;
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get plan procedures");

    return await response.json();
};

export const getUsers = async () => {
    const url = `${api_url}/Users`;
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get users");

    return await response.json();
};

export const assignUserToPlanProcedure = async (planId, procedureId, userId) => {
    const url = `${api_url}/PlanProcedureUser/assign`;
    const command = { planId, procedureId, userId };
    const response = await fetch(url, {
        method: "POST",
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
        },
        body: JSON.stringify(command),
    });

    if (!response.ok) throw new Error("Failed to assign user to procedure");

    return true;
};

export const unassignUserFromPlanProcedure = async (planId, procedureId, userId) => {
    const url = `${api_url}/PlanProcedureUser?planId=${planId}&procedureId=${procedureId}&userId=${userId}`;
    const response = await fetch(url, {
        method: "DELETE",
    });

    if (!response.ok) throw new Error("Failed to unassign user from procedure");

    return true;
};

export const clearUsersFromPlanProcedure = async (planId, procedureId) => {
    const url = `${api_url}/PlanProcedureUser/clear?planId=${planId}&procedureId=${procedureId}`;
    const response = await fetch(url, {
        method: "DELETE",
    });

    if (!response.ok) throw new Error("Failed to clear users from procedure");

    return true;
};

export const getPlanProcedureUsers = async (planId, procedureId) => {
    const url = `${api_url}/PlanProcedureUser?$filter=planId eq ${planId} and procedureId eq ${procedureId}&$expand=user`;
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get plan procedure users");

    return await response.json();
};
