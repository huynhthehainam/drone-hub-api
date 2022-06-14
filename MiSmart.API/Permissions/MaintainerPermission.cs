using MiSmart.Infrastructure.Permissions;
using MiSmart.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;
namespace MiSmart.API.Permissions
{
    public class MaintainerPermission : IPermission
    {
        public bool HasPermission(ActionExecutingContext context)
        {
            var currentUser = UserCacheViewModel.GetUserCache(context.HttpContext.User);
            if (currentUser is not null)
            {
                return currentUser.RoleID == 3;
            }
            else
            {
                return false;
            }
        }
    }
}