﻿using Ferreteria.Application.Website.Models;
using Ferreteria.Application.Website.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ferreteria.Application.Website.Providers
{
    public enum PrivilegesEnum
    {
        AdminUser,
        AdminProduct,
        AdminCategory,
        AdminSupplier,
        MakeSales
    }

    public class AuthorizeAction : AuthorizeAttribute, IAuthorizationFilter
    {
        public AuthorizeAction(params object[] privileges)
        {
            if (privileges.Any(r => r.GetType().BaseType != typeof(Enum)))
                throw new ArgumentException("The privileges parameter may only contain enums", nameof(privileges));

            var temp = privileges.Select(r => Enum.GetName(r.GetType(), r)).ToList();
            Roles = string.Join(",", temp);
        }
        public AuthorizeAction()
        {

        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var privilegesAllowed = Roles.Split(',');
            var user = context.HttpContext.Session.GetObject<UserBasicModel>("Identity");

            if (user == null)
            {
                context.Result = new RedirectResult("~/Login/Index");
                return;
            }

            var userPrivileges = new List<string>();
            if (privilegesAllowed.Any())
            {
                user.Roles.ForEach((rol) =>
                {
                    rol.Privileges.ForEach((privilege) =>
                    {
                        if (privilegesAllowed.Any(x => x == privilege.Code))
                        {
                            if (!userPrivileges.Contains(privilege.Code))
                                userPrivileges.Add(privilege.Code);
                        }
                    });
                });
            }

            if (!userPrivileges.Any())
            {
                //Page of access denied
                context.Result = new RedirectResult("~/Home/AccessDenied");
            }
        }
    }
}
