using Hangfire.Dashboard;

namespace ImpulseAPI.Extensions
{
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return httpContext.User.Identity.IsAuthenticated; /*true;*/ // Never publish 'true' in production (use it for debug only) !!!!!!!!
        }
    }
}
