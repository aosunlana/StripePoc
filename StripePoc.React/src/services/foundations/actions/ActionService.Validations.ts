// ---------------------------------------------------------------
// Copyright (c) Emerald Kilonova. All rights reserved.
// ---------------------------------------------------------------

import { InvalidActionException } from '../../../models/foundations/actions/exceptions/InvalidActionException';
import { ServiceSelectionView } from '../../../models/views/ServiceSelectionView';

export class ActionServiceValidations {
    protected validateServiceSelection(selection: ServiceSelectionView): void {
        const invalidActionException = new InvalidActionException("Invalid action, fix errors and try again.");

        if (!selection) {
            invalidActionException.upsertDataList("selection", "Selection is required");
        } else {
            if (!selection.serviceName) {
                invalidActionException.upsertDataList("serviceName", "Service name is required");
            }
            if (selection.amount <= 0) {
                invalidActionException.upsertDataList("amount", "Amount must be greater than zero");
            }
        }

        invalidActionException.throwIfContainsErrors();
    }

    protected validateId(id: string, name: string): void {
        if (!id) {
            const invalidActionException = new InvalidActionException("Invalid action, fix errors and try again.");
            invalidActionException.upsertDataList(name, `${name} is required`);
            invalidActionException.throwIfContainsErrors();
        }
    }
}
