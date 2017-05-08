using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Types
{
    public enum MessageType
    {
        UnknownType,

        ADMConfirmsPlannedOrRegisteredToBTM,
        ADMConfirmsPlannedOrRegisteredToDIR,
        ADMConfirmsPlannedOrRegisteredToEMP,
        ADMConfirmsPlannedOrRegisteredToACC,

        ADMRegistersPlannedOrPlannedModifiedToBTM,
        ADMRegistersPlannedOrPlannedModifiedToEMP,
        ADMRegistersPlannedOrPlannedModifiedToACC,


        ADMReplansRegisteredOrRegisteredModifiedToBTM,
        ADMReplansRegisteredOrRegisteredModifiedToACC,


        ADMCancelsRegisteredOrRegisteredModifiedToBTM,
        ADMCancelsRegisteredOrRegisteredModifiedToEMP,
        ADMCancelsRegisteredOrRegisteredModifiedToACC,

        ADMCancelsConfirmedOrConfirmedModifiedToBTM,
        ADMCancelsConfirmedOrConfirmedModifiedToDIR,
        ADMCancelsConfirmedOrConfirmedModifiedToEMP,
        ADMCancelsConfirmedOrConfirmedModifiedToACC,

        BTMUpdatesConfirmedOrConfirmedModifiedToEMP,
        BTMUpdatesConfirmedOrConfirmedModifiedToACC,
        BTMUpdateVisaRegistrationDateToEMP,
        BTMCreateVisaRegistrationDateToEMP,
        BTMCreateVisaRegistrationDateToBTM,
        BTMUpdateVisaRegistrationDateToBTM,

        BTMReportsConfirmedOrConfirmedModifiedToACC,
        BTMReportsConfirmedOrConfirmedModifiedToEMP,

        BTMRejectsRegisteredOrRegisteredModifiedToADM,
        BTMRejectsRegisteredOrRegisteredModifiedToACC,

        BTMRejectsConfirmedOrConfirmedModifiedToADM,
        BTMRejectsConfirmedOrConfirmedModifiedToEMP,
        BTMRejectsConfirmedOrConfirmedModifiedToACC,

        ACCCancelsConfirmedReportedToADM,
        ACCCancelsConfirmedReportedToBTM,
        ACCCancelsConfirmedReportedToEMP,

        ACCModifiesConfirmedReportedToADM,
        ACCModifiesConfirmedReportedToBTM,
        ACCModifiesConfirmedReportedToDIR,
        ACCModifiesConfirmedReportedToEMP,

        DIRRejectsConfirmedToADM,
        DIRRejectsConfirmedToEMP,
        DIRRejectsConfirmedToBTM,
        DIRRejectsConfirmedToACC,

        BTMCancelsPermitToADM,

        ADMCancelsPlannedModifiedToBTM,
        ADMCancelsPlannedModifiedToACC,

        ADMConfirmsPlannedOrRegisteredToResponsible,
        ADMCancelsConfirmedOrConfirmedModifiedToResponsible,
        ACCCancelsConfirmedReportedToResponsible,
        BTMRejectsConfirmedOrConfirmedModifiedToResponsible,
        DIRRejectsConfirmedToResponsible,
        BTMUpdatesConfirmedOrConfirmedModifiedToResponsible,
        BTMReportsConfirmedOrConfirmedModifiedToResponsible,
        ACCModifiesConfirmedReportedToResponsible,

        Greeting,

        ResetPassword,

        VisaExpirationWarning,
        PUEditsFInishedBT

    }
}