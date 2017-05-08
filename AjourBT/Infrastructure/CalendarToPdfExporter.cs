using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using PDFjet.NET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using AjourBT.Domain.Infrastructure;

namespace AjourBT.Infrastructure
{
    public static class CalendarToPdfExporter
    {
        static MemoryStream pdfMemoryStream;
        static Font font;
        static List<List<Cell>> tableData;
        static int headerRowsCount;
        static int fakeEmployeesCount;
        static int daysOfWeekRowNumber;
        static int daysRowNumber;
        static int weeksRowNumber;
        static int monthesRowNumber;
        static double columnWidth;

        public struct PdfColors
        {
            public const int ganttGreen = 0xb9dc70;
            public const int ganttDarkGreen = 0x78a407;
            public const int ganttOrange = 0xff7f00;
            public const int ganttBlue = 0x64c8fa;
            public const int ganttViolet = 0x5a009d;
            public const int ganttRed = 0xff0000;
            public const int ganttMagenta = 0xff00ff;
            public const int ganttYellow = 0xffd800;
            public const int ganttWhite = 0xffffff;

            public const int holidayYellow = 0xfff3b3;
            public const int holidayPink = 0xffe5e5;

            public const int todayGreen = 0xe4f2bf;
            public const int holidayOrange = 0xffd265;

            public const int headerWeekYellow = 0xfff3b3;
            public const int headerMonthBlue = 0xe3ffff;

            public const int pairedBTGreen = 0xa4d146;

        }

        public struct Abbreviations
        {
            public const string SickAbsence = "SA";
            public const string PaidVacation = "PV";
            public const string BT = "BT";
            public const string Journey = "J";
            public const string UnpaidVacation = "UV";
            public const string ReclaimedOvertime = "RO";
            public const string OvertimeForReclaim = "OR";
            public const string PrivateMinus = "P-";
            public const string Unknown = "";
        }

        static CalendarToPdfExporter()
        {
            pdfMemoryStream = new MemoryStream();
            PDF pdf = new PDF(pdfMemoryStream);
            tableData = new List<List<Cell>>();
            font = new Font(pdf, CoreFont.HELVETICA);
            font.SetSize(7f);
            headerRowsCount = 5;
            fakeEmployeesCount = 1;
            daysOfWeekRowNumber = 4;
            daysRowNumber = 3;
            weeksRowNumber = 2;
            monthesRowNumber = 1;
            columnWidth = 14.0;
        }

        public static MemoryStream GeneratePDF(List<CalendarRowViewModel> calendarData, List<Holiday> holidays, DateTime from, DateTime to)
        {
            pdfMemoryStream = new MemoryStream();
            if (from <= to && calendarData != null && calendarData.Count != 0)
            {
                PDF pdf = new PDF(pdfMemoryStream);

                Table calendarTable = new Table();
                tableData = CreateCalendar(calendarData, holidays, from, to);
                calendarTable.SetData(tableData);
                calendarTable.SetPosition(36, 36);
                calendarTable.AutoAdjustColumnWidths();
                AdjustColumnWidths(calendarTable);

                Table legendTable = new Table();
                List<List<Cell>> legendData = CreateLegend();
                legendTable.SetData(legendData);
                legendTable.AutoAdjustColumnWidths();
                legendTable.SetPosition(36, (tableData.Count + 1) * 12.12 + 36);

                Page page = new Page(pdf, new float[] { calendarTable.GetWidth() + 72, (tableData.Count + legendData.Count + 1) * 12.12f + 72 });
                calendarTable.DrawOn(page);
                legendTable.DrawOn(page);
                pdf.Close();

            }
            
             return pdfMemoryStream;
        }

        #region CreateCalendar

        public static List<List<Cell>> CreateCalendar(List<CalendarRowViewModel> calendarData, List<Holiday> holidays, DateTime from, DateTime to)
        {
            if (from <= to && calendarData != null && calendarData.Count != 0)
            {
                List<List<Cell>> calendarHeader = CreateCalendarHeader(from, to);
                List<List<Cell>> calendar = CreateCalendarLeftPanel(calendarData);
                List<List<Cell>> calendarbody = CreateCalendarBody(calendarData, holidays, from, to);

                for (int i = 0; i < headerRowsCount; i++)
                {
                    calendar[i] = calendar[i].Concat(calendarHeader[i]).ToList();
                }

                for (int i = headerRowsCount; i < calendar.Count; i++)
                {
                    calendar[i] = calendar[i].Concat(calendarbody[i - headerRowsCount]).ToList();
                }

                ColorHeader(calendar, holidays, from, to);


                return calendar;
            }
                return new List<List<Cell>  > ( );
        }

        #region CreateCalendarHeader

        public static List<List<Cell>> CreateCalendarHeader(DateTime from, DateTime to)
        {
            List<List<Cell>> calendarTable = new List<List<Cell>>();
            if (from <= to)
            {
                calendarTable.Add(getYearsRow(from, to));
                calendarTable.Add(getMonthsRow(from, to));
                calendarTable.Add(getWeeksRow(from, to));
                calendarTable.Add(getDaysRow(from, to));
                calendarTable.Add(getDaysOfWeekRow(from, to));
                AlignTextInAllCellsByCenter(calendarTable);
            }
            return calendarTable;
        }

        public static List<Cell> getYearsRow(DateTime from, DateTime to)
        {
            List<Cell> yearRow = new List<Cell>();
            Cell yearCell;
            while (from.Date <= to.Date)
            {
                yearCell = new Cell(font, from.Year.ToString());
                int colSpan = getColSpanForYear(from, to);
                yearCell.SetColSpan(colSpan);
                yearRow.Add(yearCell);
                for (int i = 1; i < colSpan; i++)
                {
                    yearRow.Add(new Cell(font));
                }
                from = new DateTime(from.Year + 1, 1, 1);
            }
            return yearRow;
        }

        public static int getColSpanForYear(DateTime from, DateTime to)
        {
            DateTime lastDate = new DateTime(from.Year + 1, 1, 1).AddDays(-1).Date;

            return getLesserOfTimeSpans(from, to, lastDate);
        }

        public static int getLesserOfTimeSpans(DateTime from, DateTime to, DateTime lastDate)
        {
            TimeSpan diff;
            if (to.Date < lastDate.Date)
                diff = to.Date.AddDays(1) - from.Date;
            else
                diff = lastDate.Date.AddDays(1) - from.Date;

            //Zero or less result means wrong input parameters (from is greater than to or lastDate)
            return diff.Days;
        }

        public static List<Cell> getMonthsRow(DateTime from, DateTime to)
        {
            List<Cell> monthRow = new List<Cell>();
            Cell monthCell;
            DateTimeFormatInfo dfi = new DateTimeFormatInfo() { FirstDayOfWeek = DayOfWeek.Monday, CalendarWeekRule = CalendarWeekRule.FirstDay };
            DateTime fromCopy = new DateTime(from.Year, from.Month, from.Day);

            while (fromCopy.Date <= to.Date)
            {
                monthCell = new Cell(font, dfi.GetMonthName(fromCopy.Month));
                int colSpan = getColSpanForMonth(fromCopy, to);
                if (colSpan < 3)
                    monthCell.SetText(monthCell.GetText().Substring(0, 3));
                monthCell.SetColSpan(colSpan);
                monthRow.Add(monthCell);
                for (int i = 1; i < colSpan; i++)
                {
                    monthRow.Add(new Cell(font));
                }
                DateTime lastDate = new DateTime(fromCopy.Year + (int)(fromCopy.Month / 12), fromCopy.Month % 12 + 1, 1);
                fromCopy = lastDate;
            }

            return monthRow;
        }

        public static int getColSpanForMonth(DateTime from, DateTime to)
        {
            DateTime lastDate = (new DateTime(from.Year + (int)(from.Month / 12), from.Month % 12 + 1, 1).AddDays(-1)).Date;

            return getLesserOfTimeSpans(from, to, lastDate);
        }

        public static List<Cell> getWeeksRow(DateTime from, DateTime to)
        {
            List<Cell> weekRow = new List<Cell>();
            Cell weekCell;
            DateTimeFormatInfo dfi = new DateTimeFormatInfo() { FirstDayOfWeek = DayOfWeek.Monday, CalendarWeekRule = CalendarWeekRule.FirstDay };
            Calendar cal = dfi.Calendar;
            int weekNumber;
            int colSpan;

            while (from.Date <= to.Date)
            {
                colSpan = getColSpanForWeek(from, to);
                weekNumber = cal.GetWeekOfYear(from.AddDays(colSpan - 1).Date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                weekCell = new Cell(font, "W" + weekNumber.ToString());
                weekCell.SetColSpan(colSpan);
                weekRow.Add(weekCell);
                for (int i = 1; i < colSpan; i++)
                {
                    weekRow.Add(new Cell(font));
                }

                from = from.AddDays(colSpan);
            }

            return weekRow;
        }

        public static int getColSpanForWeek(DateTime from, DateTime to)
        {
            DateTime lastDate = from.AddDays((7 - (int)from.DayOfWeek) % 7).Date;

            return getLesserOfTimeSpans(from, to, lastDate);
        }

        public static List<Cell> getDaysRow(DateTime from, DateTime to)
        {
            List<Cell> daysRow = new List<Cell>();
            for (DateTime date = from; date <= to; date = date.AddDays(1))
            {
                daysRow.Add(new Cell(font, date.Day.ToString()));
            }
            return daysRow;
        }

        public static List<Cell> getDaysOfWeekRow(DateTime from, DateTime to)
        {
            List<Cell> daysOfWeekRow = new List<Cell>();

            DateTimeFormatInfo dfi = new DateTimeFormatInfo() { FirstDayOfWeek = DayOfWeek.Monday, CalendarWeekRule = CalendarWeekRule.FirstDay };

            for (DateTime date = from; date <= to; date = date.AddDays(1))
            {
                daysOfWeekRow.Add(new Cell(font, dfi.GetShortestDayName(date.DayOfWeek)));
            }
            return daysOfWeekRow;
        }

        public static void AlignTextInAllCellsByCenter(List<List<Cell>> table)
        {
            foreach (List<Cell> row in table)
            {
                foreach (Cell cell in row)
                {
                    cell.SetTextAlignment(Align.CENTER);
                }
            }
        }

        #endregion

        #region CreateCalendarLeftPanel

        public static List<List<Cell>> CreateCalendarLeftPanel(List<CalendarRowViewModel> calendarData)
        {
            List<List<Cell>> calendarTable = new List<List<Cell>>();
            if (calendarData != null && calendarData.Count != 0)
            {
                CreateHeaderSpacer(calendarData, calendarTable);

                CreateEmployeesList(calendarData, calendarTable);
            }
            return calendarTable;

        }

        public static void CreateHeaderSpacer(List<CalendarRowViewModel> calendarData, List<List<Cell>> calendarTable)
        {
            if (calendarTable != null && calendarData != null)
            {
                if (calendarData.Count != 0)
                {
                    for (int i = 0; i < headerRowsCount; i++)
                    {
                        calendarTable.Add(new List<Cell>() { new Cell(font) });
                        if (i != 0)
                            calendarTable[i][0].SetBorder(Border.TOP, false);
                        if (i != headerRowsCount - 1)
                            calendarTable[i][0].SetBorder(Border.BOTTOM, false);
                    }
                }
            }
        }

        public static void CreateEmployeesList(List<CalendarRowViewModel> calendarData, List<List<Cell>> calendarTable)
        {
            if (calendarTable != null && calendarData != null)
            {
                for (int i = 0; i < calendarData.Count - fakeEmployeesCount; i++)
                {
                    calendarTable.Add(new List<Cell>() { new Cell(font, calendarData[i].name) });

                }
            }
        }

        #endregion

        #region CreateCalendarBody

        public static List<List<Cell>> CreateCalendarBody(List<CalendarRowViewModel> calendarData, List<Holiday> holidays, DateTime from, DateTime to)
        {
            List<List<Cell>> calendarTable;
            int colSpan;
            List<Cell> emptyRow = new List<Cell>();
            int colIndex;

            calendarTable = CreateEmptyCalendarBody(calendarData, from, to);
            ApplyWeekends(calendarTable, from, to);
            ApplyHolidays(calendarTable, holidays, from, to);

            for (int i = 0; i < calendarData.Count - fakeEmployeesCount; i++)
            {
                if (calendarData[i].values != null)
                {
                    foreach (CalendarItemViewModel calendarItem in calendarData[i].values)
                    {
                        colSpan = GetColumnSpanForCalendarItem(calendarItem, from, to);
                        colIndex = GetColumnIndexForCalendarItem(calendarItem, from, to);
                        if (calendarItem.customClass == "ganttGreen" && calendarItem.from != calendarItem.to)
                        {
                            if (calendarData[i].values.Where(c => (c.from == calendarItem.to) && c.customClass == "ganttGreen").FirstOrDefault() != null)
                            {
                                colSpan--;
                                calendarTable[i][colIndex + colSpan].SetBorder(Border.RIGHT, false);
                                calendarTable[i][colIndex + colSpan].SetBorder(Border.LEFT, false);
                                calendarTable[i][colIndex].SetBorder(Border.RIGHT, false);
                                calendarTable[i][colIndex + colSpan].SetBgColor(PdfColors.pairedBTGreen);
                            }
                            else if (calendarData[i].values.Where(c => c.to == calendarItem.from && c.customClass == "ganttGreen").FirstOrDefault() != null)
                            {
                                colIndex++;
                                colSpan--;
                                calendarTable[i][colIndex - 1].SetBorder(Border.RIGHT, false);
                                calendarTable[i][colIndex - 1].SetBorder(Border.LEFT, false);
                                calendarTable[i][colIndex].SetBorder(Border.LEFT, false);
                                calendarTable[i][colIndex - 1].SetBgColor(PdfColors.pairedBTGreen);
                            }
                        }
                        calendarTable[i][colIndex].SetColSpan(colSpan);
                        calendarTable[i][colIndex].SetBgColor(GetColorForCalendarItem(calendarItem));
                        calendarTable[i][colIndex].SetBrushColor(Color.white);
                        calendarTable[i][colIndex].SetText(GetAbbreviationForCalendarItem(calendarItem));
                    }
                }
            }

            return calendarTable;
        }

        public static List<List<Cell>> CreateEmptyCalendarBody(List<CalendarRowViewModel> calendarData, DateTime from, DateTime to)
        {
            Cell cell;

            List<List<Cell>> calendarTable = new List<List<Cell>>();
            if (calendarData != null)
                for (int i = 0; i < calendarData.Count - fakeEmployeesCount; i++)
                {
                    calendarTable.Add(new List<Cell>());
                    for (DateTime date = from.Date; date <= to.Date; date = date.AddDays(1))
                    {
                        cell = new Cell(font);
                        cell.SetTextAlignment(Align.CENTER);
                        calendarTable[i].Add(cell);

                    }
                }

            return calendarTable;
        }

        public static void ApplyWeekends(List<List<Cell>> calendar, DateTime from, DateTime to)
        {
            if (calendar != null && calendar.Count != 0 && (to - from).Days + 1 == calendar[0].Count)
                for (DateTime date = from; date <= to; date = date.AddDays(1))
                {
                    if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
                    {
                        foreach (List<Cell> row in calendar)
                        {
                            row[(date - from).Days].SetBgColor(PdfColors.holidayPink);
                        }
                    }
                }
        }

        public static void ApplyHolidays(List<List<Cell>> calendar, List<Holiday> holidays, DateTime from, DateTime to)
        {
            int color;
            if (calendar != null && holidays != null && calendar.Count != 0 && (to - from).Days + 1 == calendar[0].Count)
            {
                foreach (Holiday holiday in holidays)
                {
                    if (holiday.HolidayDate.Date >= from.Date && holiday.HolidayDate.Date <= to.Date)
                    {
                        if (holiday.IsPostponed)
                            color = PdfColors.holidayPink;
                        else
                            color = PdfColors.holidayYellow;
                        foreach (List<Cell> row in calendar)
                        {

                            row[(holiday.HolidayDate.Date - from.Date).Days].SetBgColor(color);
                        }
                    }
                }
            }
        }

        public static int GetColumnSpanForCalendarItem(CalendarItemViewModel calendarItem, DateTime from, DateTime to)
        {
            int colSpan = 0;
            int colIndex = GetColumnIndexForCalendarItem(calendarItem, from, to);

            if (colIndex < 0)
                return -1;

            colSpan = (calendarItem.to.Date - calendarItem.from.Date).Days + 1;
            if (calendarItem.to.Date > to)
                colSpan -= (calendarItem.to.Date - to.Date).Days;
            if (calendarItem.from.Date < from)
                colSpan -= (from.Date - calendarItem.from.Date).Days;

            return colSpan;
        }

        public static int GetColumnIndexForCalendarItem(CalendarItemViewModel calendarItem, DateTime from, DateTime to)
        {
            int colIndex;
            colIndex = (calendarItem.from.Date - from.Date).Days;
            if (colIndex < 0 && calendarItem.to >= from.Date)
                colIndex = 0;
            if (colIndex > (to.Date - from.Date).Days)
                colIndex = -1;
            //colIndex lesser than 0 means wrong input data
            return colIndex;
        }

        public static int GetColorForCalendarItem(CalendarItemViewModel calendaritem)
        {

            switch (calendaritem.customClass)
            {
                case "ganttGreen":
                    return PdfColors.ganttGreen;

                case "ganttDarkGreen":
                    return PdfColors.ganttDarkGreen;

                case "ganttOrange":
                    return PdfColors.ganttOrange;

                case "ganttBlue":
                    return PdfColors.ganttBlue;

                case "ganttViolet":
                    return PdfColors.ganttViolet;

                case "ganttRed":
                    return PdfColors.ganttRed;

                case "ganttMagenta":
                    return PdfColors.ganttMagenta;

                case "ganttYellow":
                    return PdfColors.ganttYellow;

                default:
                    return PdfColors.ganttWhite;
            }

        }

        public static string GetAbbreviationForCalendarItem(CalendarItemViewModel calendaritem)
        {

            switch (calendaritem.customClass)
            {
                case "ganttGreen":
                    return Abbreviations.BT;

                case "ganttDarkGreen":
                    return Abbreviations.Journey;

                case "ganttOrange":
                    return Abbreviations.ReclaimedOvertime;

                case "ganttBlue":
                    return Abbreviations.PaidVacation;

                case "ganttViolet":
                    return Abbreviations.SickAbsence;

                case "ganttRed":
                    return Abbreviations.UnpaidVacation;

                case "ganttMagenta":
                    return Abbreviations.OvertimeForReclaim;

                case "ganttYellow":
                    return Abbreviations.PrivateMinus;

                default:
                    return Abbreviations.Unknown;
            }

        }

        #endregion

        public static void ColorHeader(List<List<Cell>> calendar, List<Holiday> holidays, DateTime from, DateTime to)
        {
            ColorRow(calendar, weeksRowNumber, PdfColors.headerWeekYellow);
            ColorRow(calendar, monthesRowNumber, PdfColors.headerMonthBlue);
            ApplyHolidaysToHeader(calendar, holidays, from, to);
            MarkToday(calendar, from, to);
        }

        public static void ColorRow(List<List<Cell>> calendar, int rowNumber, int color)
        {
            if (calendar != null && rowNumber >= 0 && calendar.Count >= rowNumber + 1)
            {
                for (int i = 1; i < calendar[0].Count(); i++)
                {
                    calendar[rowNumber][i].SetBgColor(color);
                }
            }
        }

        public static void ApplyHolidaysToHeader(List<List<Cell>> calendar, List<Holiday> holidays, DateTime from, DateTime to)
        {
            if (calendar != null && holidays != null && calendar.Count != 0 && (to - from).Days + 2 == calendar[0].Count)
            {
                foreach (Holiday holiday in holidays)
                {
                    if (holiday.HolidayDate.Date >= from.Date && holiday.HolidayDate.Date <= to.Date)
                    {
                        calendar[daysRowNumber][(holiday.HolidayDate.Date - from.Date).Days + 1].SetBgColor(PdfColors.holidayOrange);
                        calendar[daysOfWeekRowNumber][(holiday.HolidayDate.Date - from.Date).Days + 1].SetBgColor(PdfColors.holidayOrange);
                    }
                }
            }
        }

        public static void MarkToday(List<List<Cell>> calendar, DateTime from, DateTime to)
        {
            DateTime today = DateTime.Now.ToLocalTimeAzure().Date;
            if (today >= from.Date && today <= to.Date)
            {
                calendar[daysRowNumber][(today.Date - from.Date).Days + 1].SetBgColor(PdfColors.todayGreen);
                calendar[daysOfWeekRowNumber][(today.Date - from.Date).Days + 1].SetBgColor(PdfColors.todayGreen);
            }
        }

        #endregion

        public static void AdjustColumnWidths(Table table)
        {
            for (int i = 1; i < table.GetRow(0).Count; i++)
            {
                table.SetColumnWidth(i, columnWidth);
            }
        }

        public static List<List<Cell>> CreateLegend()
        {
            List<List<Cell>> legendTable = new List<List<Cell>>();
            for (int i = 0; i < 9; i++)
            {
                legendTable.Add(new List<Cell>());
            }

            legendTable[0].Add(new Cell(font, "Legend: "));
            legendTable[0].Add(new Cell(font));

            legendTable[0][0].SetColSpan(2);

            legendTable[1].Add(new Cell(font, Abbreviations.BT));
            legendTable[2].Add(new Cell(font, Abbreviations.Journey));
            legendTable[3].Add(new Cell(font, Abbreviations.OvertimeForReclaim));
            legendTable[4].Add(new Cell(font, Abbreviations.PaidVacation));
            legendTable[5].Add(new Cell(font, Abbreviations.PrivateMinus));
            legendTable[6].Add(new Cell(font, Abbreviations.ReclaimedOvertime));
            legendTable[7].Add(new Cell(font, Abbreviations.SickAbsence));
            legendTable[8].Add(new Cell(font, Abbreviations.UnpaidVacation));

            legendTable[1][0].SetBgColor(PdfColors.ganttGreen);
            legendTable[2][0].SetBgColor(PdfColors.ganttDarkGreen);
            legendTable[3][0].SetBgColor(PdfColors.ganttMagenta);
            legendTable[4][0].SetBgColor(PdfColors.ganttBlue);
            legendTable[5][0].SetBgColor(PdfColors.ganttYellow);
            legendTable[6][0].SetBgColor(PdfColors.ganttOrange);
            legendTable[7][0].SetBgColor(PdfColors.ganttViolet);
            legendTable[8][0].SetBgColor(PdfColors.ganttRed);

            for (int i = 1; i < 9; i++)
            {
                legendTable[i][0].SetBrushColor(PdfColors.ganttWhite);
            }

            legendTable[1].Add(new Cell(font, "Business Trip"));
            legendTable[2].Add(new Cell(font, "Journey"));
            legendTable[3].Add(new Cell(font, "Overtime for Reclaim"));
            legendTable[4].Add(new Cell(font, "Paid Vacation"));
            legendTable[5].Add(new Cell(font, "Private Minus"));
            legendTable[6].Add(new Cell(font, "Reclaimed Overtime"));
            legendTable[7].Add(new Cell(font, "Sick Absence"));
            legendTable[8].Add(new Cell(font, "Unpaid Vacation"));
            return legendTable;
        }

    }
}