using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
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
