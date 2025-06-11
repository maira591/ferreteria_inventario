using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Ferreteria.Application.Website.Utils
{
    public class Common
    {
        public static string GetUrlBase(HttpRequest Request)
        {
            string pathBase = string.IsNullOrWhiteSpace(Request.PathBase) ? string.Empty : Request.PathBase + "/";
            Uri location = new Uri($"{Request.Scheme}://{Request.Host}{pathBase}");
            //return location.AbsoluteUri;

            return "https://q86x597w-7042.use.devtunnels.ms/";
        }

        public static List<SelectListItem> LoadList<T>(List<T> list, string text, string value, string selectedValue = "")
        {
            List<SelectListItem> returnList = new List<SelectListItem>();
            string propertyValue = string.Empty;
            string propertyText = string.Empty;
            bool selected = false;

            foreach (object item in list)
            {
                if (item == null) continue;
                foreach (PropertyInfo property in item.GetType().GetProperties())
                {
                    if (property.Name.ToLower() == value.ToLower())
                    {
                        propertyValue = property.GetValue(item, null).ToString();
                        selected = selectedValue.Equals(value);
                    }
                    if (property.Name.ToLower() == text.ToLower())
                    {
                        propertyText = property.GetValue(item, null).ToString();
                    }
                }

                returnList.Add(new SelectListItem { Text = propertyText, Value = propertyValue, Selected = selected });
            }
            return returnList;
        }

        public static string DecimalValue(string decimalValue)
        {
            decimalValue = decimalValue.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            decimalValue = decimalValue.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            return decimalValue;
        }
    }
}
