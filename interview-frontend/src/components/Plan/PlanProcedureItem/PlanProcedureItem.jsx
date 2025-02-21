import React, { useState, useEffect, useCallback } from "react";
import ReactSelect from "react-select";
import { assignUserToPlanProcedure, unassignUserFromPlanProcedure, getPlanProcedureUsers } from "../../../api/api";

const PlanProcedureItem = ({ procedure, users, planId }) => {
    const [selectedUsers, setSelectedUsers] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const loadAssignedUsers = useCallback(async () => {
        try {
            const assignedUsers = await getPlanProcedureUsers(planId, procedure.procedureId);
            const selectedOptions = assignedUsers.map(u => ({
                value: u.user.userId,
                label: u.user.name
            }));
            setSelectedUsers(selectedOptions);
        } catch (err) {
            setError("Failed to load assigned users");
            console.error(err);
        }
    }, [planId, procedure.procedureId]);

    useEffect(() => {
        loadAssignedUsers();
    }, [loadAssignedUsers]);

    const handleAssignUserToProcedure = async (selected) => {
        setLoading(true);
        setError(null);
        try {
            // Get currently selected user IDs
            const currentUserIds = new Set(selectedUsers.map(u => u.value));
            // Get newly selected user IDs
            const newUserIds = new Set(selected.map(u => u.value));

            // Find users to assign (in new but not in current)
            for (const userId of newUserIds) {
                if (!currentUserIds.has(userId)) {
                    await assignUserToPlanProcedure(planId, procedure.procedureId, userId);
                }
            }

            // Find users to unassign (in current but not in new)
            for (const userId of currentUserIds) {
                if (!newUserIds.has(userId)) {
                    await unassignUserFromPlanProcedure(planId, procedure.procedureId, userId);
                }
            }

            setSelectedUsers(selected);
        } catch (err) {
            setError("Failed to update user assignments");
            console.error(err);
            // Reload current assignments to ensure UI is in sync
            await loadAssignedUsers();
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="py-2">
            <div className="d-flex justify-content-between align-items-center">
                <div>{procedure.procedureTitle}</div>
                {error && <div className="text-danger small">{error}</div>}
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select Users to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={handleAssignUserToProcedure}
                isDisabled={loading}
                isLoading={loading}
            />
        </div>
    );
};

export default PlanProcedureItem;
