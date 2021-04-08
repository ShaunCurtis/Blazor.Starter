using Blazor.Starter.Components;
using Blazor.Starter.Pages;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blazor.Starter.Services
{
    public class RouteViewService
    {
        public SortedDictionary<string, RouteData> Routes { get; private set; } = new SortedDictionary<string, RouteData>();

        public Type Layout { get; set; }

        public RouteViewService()
        {
            var route = new RouteData(typeof(Counter), new Dictionary<string, object>());
            Routes.Add(@"^\/counted\/(\d+)", route);
            Routes.Add(@"^\/counters", route);
        }

        public bool GetRouteMatch(string url, out RouteData routeData)
        {
            var route = Routes.FirstOrDefault(item => IsMatch(url, item.Key));
            if (!EqualityComparer<RouteData>.Default.Equals(route))
            {
                routeData = route.Value;
                return true;
            }
            else
            {
                routeData = null;
                return false;
            }
        }
        public bool IsMatch(string url, string matchstring)
        {
            var match = Regex.Match(url, matchstring);
            return match.Success;
        }
    }
}
