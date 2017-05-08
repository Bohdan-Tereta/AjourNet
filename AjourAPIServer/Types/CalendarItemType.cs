using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Types
{
    public enum CalendarItemType
    {
        SickAbsence,
        PaidVacation,
        BT,
        Journey,
        UnpaidVacation,
        ReclaimedOvertime,
        OvertimeForReclaim,
        PrivateMinus
    };
}