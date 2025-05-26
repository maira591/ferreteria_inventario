using Core.Domain.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Application.Website.Helpers
{
    public static class SelectListItemExtensions
    {
        public static List<SelectListItem> ToSelectListItem(this List<string> list, string dateSelected = null)
        {
            var listItems = new List<SelectListItem>();

            foreach (var item in list)
                listItems.Add(new SelectListItem() { Value = item, Text = item });

            return listItems;
        }

        public static List<SelectListItem> ToListCuts(this List<SelectListItem> list, string periodicity, int numberMonths, int maxMonthToCalculateCuts, string dateSelected = null)
        {
            return ToListCuts(list, periodicity.ToLower(), numberMonths, false, maxMonthToCalculateCuts, dateSelected);
        }

        public static List<SelectListItem> ToListCuts(this List<SelectListItem> list, string periodicity, int numberMonths, bool calculate, int maxMonthToCalculateCuts, string dateSelected = null)
        {
            var listEspecialCut = new List<string>() { PeriodicityType.Semanal, PeriodicityType.Quincenal };
            var currentDate = listEspecialCut.Contains(periodicity) ? DateTime.Now :
                (
                    numberMonths == ((int)MonthsEnum.Bimensual) || numberMonths == ((int)MonthsEnum.Trimestral) ||
                    numberMonths == ((int)MonthsEnum.Semestral) || numberMonths == ((int)MonthsEnum.Anual)
                ) ? new DateTime(DateTime.Now.Year, LastDayPair(numberMonths), 1).AddMonths(1).AddDays(-1) :
                (calculate || DateTime.Now.Day < 20) ? DateTime.Now.AddMonths(-1)
                : DateTime.Now;

            numberMonths = periodicity.Equals(PeriodicityType.Semanal) || periodicity.Equals(PeriodicityType.Esporadica) ? 1 : numberMonths;

            if (!periodicity.Equals(PeriodicityType.Quincenal))
            {
                for (var i = 0; i <= maxMonthToCalculateCuts; i = i + numberMonths)
                {
                    var year = currentDate.AddMonths(-i).Year;
                    var month = currentDate.AddMonths(-i).Month;
                    if (!listEspecialCut.Contains(periodicity))
                    {
                        var day = DateTime.DaysInMonth(year, month);
                        var date = new DateTime(year, month, day);
                        addToList(list, dateSelected, date);
                    }
                    //Si la periodicidad es semanal ingresa y obtiene los domingos por mes para listar las fechas de corte semanal
                    else
                    {
                        var listDays = GetWeeklyLastDay(year, month);
                        listDays.Reverse(0, listDays.Count());
                        foreach (var item in listDays)
                        {
                            var day = int.Parse(item.Value);
                            var date = new DateTime(year, month, day);
                            addToList(list, dateSelected, date);
                        }
                    }
                }
            }
            else
            {
                var listDays = GetBiweeklyDates(currentDate);
                foreach (var date in listDays)
                {
                    addToList(list, dateSelected, date);
                }
            }

            return list;
        }

        private static void addToList(List<SelectListItem> list, string dateSelected, DateTime date)
        {
            var dateValue = $"{date:dd/MM/yyyy}";

            if (date < DateTime.Now)
                list.Add(new SelectListItem
                {
                    Text = dateValue,
                    Value = dateValue,
                    Selected = !string.IsNullOrEmpty(dateSelected) && dateValue.Equals(dateSelected)
                });
        }

        #region Private Methods
        private static List<SelectListItem> GetWeeklyLastDay(int year, int month)
        {
            var list = new List<SelectListItem>();
            var diasMes = new DateTime(year, month, day: 1).AddMonths(1).AddDays(-1);

            for (var i = 1; i <= diasMes.Day; i++)
            {
                var fecha = new DateTime(year, month, day: i);

                if ((byte)fecha.DayOfWeek == 0)
                    list.Add(new SelectListItem { Value = i.ToString() });
            }

            return list;
        }

        private static List<DateTime> GetBiweeklyDates(DateTime currentDate)
        {
            var dates = new List<DateTime>();
            //Se calculan las fechas de un año
            for (int i = 0; i < 12; i++)
            {
                var newDate = currentDate.AddMonths(-i);
                dates.Add(new DateTime(newDate.Year, newDate.Month, DateTime.DaysInMonth(newDate.Year, newDate.Month)));
                dates.Add(new DateTime(newDate.Year, newDate.Month, day: 15));
            }
            return dates;
        }

        private static int LastDayPair(int numberMonths)
        {
            var mes = 0;

            for (var i = DateTime.Now.Month; i >= 1; i--)
            {
                if (i % numberMonths == 0)
                {
                    mes = i;
                    break;
                }
            }
            mes = mes == 0 ? numberMonths : mes;

            return mes;
        }

        #endregion
    }
}
