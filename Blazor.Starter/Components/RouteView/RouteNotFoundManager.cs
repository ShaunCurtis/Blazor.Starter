using Blazor.Starter.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Blazor.Starter.Components
{
    /// <summary>
    /// Class to handle custom runtime routing
    /// where the dynamic routes are loaded into a routing table in the RouteViewService
    /// Route matching uses Regular Expressions, so is relatively expensive.
    /// </summary>
    public class RouteNotFoundManager : IComponent
    {
        /// <summary>
        /// Gets or sets the type of a layout to be used if the page does not
        /// declare any layout. If specified, the type must implement <see cref="IComponent"/>
        /// and accept a parameter named <see cref="LayoutComponentBase.Body"/>.
        /// </summary>
        [Parameter]
        public Type DefaultLayout { get; set; }

        /// <summary>
        /// Child content of the Component
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Injected Navigation Manager
        /// </summary>
        [Inject] NavigationManager NavManager { get; set; }

        /// <summary>
        /// Injected RouteViewService
        /// </summary>
        [Inject] RouteViewService RouteViewService { get; set; }

        /// <summary>
        ///  RenderFragment Delegate run when rendering the component
        /// </summary>
        private readonly RenderFragment _renderDelegate;

        /// <summary>
        /// RendleHandle we get from the Attach Process to queue render requests on
        /// </summary>
        private RenderHandle _renderHandle;

        /// <summary>
        /// Internal Route data obtained from RouteViewService
        /// </summary>
        private RouteData _routeData = null;

        /// <summary>
        /// The PageLayout to use in rendering the component
        /// </summary>
        private Type _pageLayoutType => _routeData?.PageType.GetCustomAttribute<LayoutAttribute>()?.LayoutType ?? DefaultLayout;

        /// <inheritdoc />
        public void Attach(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
        }

        /// <inheritdoc />
        public Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);
            var url = $"/{NavManager.Uri.Replace(NavManager.BaseUri, "")}";
            if (RouteViewService.GetRouteMatch(url, out var routedata))
                _routeData = routedata;
            if (_pageLayoutType == null)
                _renderHandle.Render(ChildContent);
            else
                _renderHandle.Render(_layoutViewFragment);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Layouted Render Fragment
        /// </summary>
        private RenderFragment _layoutViewFragment => builder =>
        {
            builder.OpenComponent<LayoutView>(0);
            builder.AddAttribute(1, nameof(LayoutView.Layout), _pageLayoutType);
            if (_routeData != null)
                builder.AddAttribute(2, nameof(LayoutView.ChildContent), _renderRouteWithParameters);
            else
                builder.AddAttribute(2, nameof(LayoutView.ChildContent), this.ChildContent);
            builder.CloseComponent();
        };

        /// <summary>
        /// Render Fragment built from the RouteData
        /// </summary>
        private RenderFragment _renderRouteWithParameters => builder =>
        {
            builder.OpenComponent(0, _routeData.PageType);
            foreach (var kvp in _routeData.RouteValues)
            {
                builder.AddAttribute(1, kvp.Key, kvp.Value);
            }
            builder.CloseComponent();
        };

    }
}
