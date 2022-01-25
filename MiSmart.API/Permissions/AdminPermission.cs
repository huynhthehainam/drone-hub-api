using MiSmart.Infrastructure.Permissions;
using MiSmart.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace MiSmart.API.Permissions
{
    public class AdminPermission : IPermission
    {
        public Boolean HasPermission(ActionExecutingContext context)
        {
            var currentUser = UserCacheViewModel.GetUserCache(context.HttpContext.User);
            Console.WriteLine(currentUser);
            if (currentUser is Object)
            {
                return currentUser.IsAdmin;
            }
            else
            {
                return false;
            }
        }
    }
}