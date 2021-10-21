using System;
using System.Collections.Generic;
using System.Linq;
using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace MiSmart.Infrastructure.Permissions
{
    public interface IPermission
    {
        Boolean HasPermission(ActionExecutingContext context);
    }
    public class HasPermissionAttribute : ActionFilterAttribute
    {
        public List<IPermission> permissions = new List<IPermission>();
        public HasPermissionAttribute(params Type[] permissionTypes)
        {
            this.permissions = GetPermission(permissionTypes);
        }
        private List<IPermission> GetPermission(Type[] permissionTypes)
        {
            List<IPermission> permissions = new List<IPermission>();
            foreach (var permissionType in permissionTypes)
            {
                if (typeof(IPermission).IsAssignableFrom(permissionType))
                {
                    permissions.Add((IPermission)Activator.CreateInstance(permissionType));
                }
                else
                {
                    throw new InvalidCastException($"{permissionType.ToString()} is not a IPermission");
                }
            }
            return permissions;
        }
        public HasPermissionAttribute()
        {
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ActionResponse response = context.HttpContext.RequestServices.GetRequiredService<IActionResponseFactory>().CreateInstance();
            if (permissions.Any(ww => !ww.HasPermission(context)))
            {
                response.AddNotAllowedErr();
                context.Result = response.ToIActionResult();
            }
        }
    }
}