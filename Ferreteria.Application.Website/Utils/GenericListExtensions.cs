using Core.DataAccess.Model;
using Core.DataAccess.Model.Formulator;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Core.Application.Website.Utils
{
    public static class GenericListExtensions
    {
        public static List<SelectListItem> ToListItemsGeneric(this List<ValueCatalog> list)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            foreach (var valueCatalog in list)
            {
                listItems.Add(new SelectListItem()
                {
                    Text = valueCatalog.Name,
                    Value = valueCatalog.Id.ToString()
                });
            }

            return listItems;
        }

        public static List<SelectListItem> ToListItemsGeneric(this List<SimpleItem> list)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            foreach (var item in list)
            {
                listItems.Add(new SelectListItem()
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return listItems;
        }
        public static List<SelectListItem> ToListItemsGenericWithId(this List<DataAccess.Model.Formulator.Type> list)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            foreach (var item in list)
            {
                listItems.Add(new SelectListItem()
                {
                    Text = item.Value,
                    Value = item.Id.ToString()
                });
            }

            return listItems;
        }


        public static List<SelectListItem> ToLoadGenericList<T>(this List<T> list, string text, string value)
        {
            return Common.LoadList(list, text, value);
        }
    }
}