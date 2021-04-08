﻿// ==========================================================
//  Original code:
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Mods by Cold Elm Coders
// ============================================================

#nullable disable warnings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Blazor.Starter.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazor.Starter.Components
{
    /// <summary>
    /// Displays the specified page component, rendering it inside its layout
    /// and any further nested layouts.
    /// Customized replacement version of RouteView Component
    /// Handles Dynamic Layout changes and changing RouteViews with no routing
    /// </summary>
    public class RouteViewManager : IComponent
    {

        /// <summary>
        /// Gets or sets the route data. This determines the page that will be
        /// displayed and the parameter values that will be supplied to the page.
        /// </summary>
        [Parameter]
        public RouteData RouteData { get; set; }

        /// <summary>
        /// Gets or sets the type of a layout to be used if the page does not
        /// declare any layout. If specified, the type must implement <see cref="IComponent"/>
        /// and accept a parameter named <see cref="LayoutComponentBase.Body"/>.
        /// </summary>
        [Parameter]
        public Type DefaultLayout { get; set; }

        /// <summary>
        /// The size of the History list.
        /// </summary>
        [Parameter] public int ViewHistorySize { get; set; } = 10;

        /// <summary>
        /// Injected RouteViewService 
        /// Used to get the override Layout if one is specified
        /// </summary>
        [Inject] private RouteViewService RouteViewService { get; set; }

        /// <summary>
        /// Gets or sets the view data.
        /// </summary>
        public ViewData ViewData
        {
            get => this._ViewData;
            protected set
            {
                this.AddViewToHistory(this._ViewData);
                this._ViewData = value;
            }
        }

        /// <summary>
        /// Property that stores the View History.  It's size is controlled by ViewHistorySize
        /// </summary>
        public SortedList<DateTime, ViewData> ViewHistory { get; private set; } = new SortedList<DateTime, ViewData>();

        /// <summary>
        /// Gets or sets the view data.
        /// </summary>
        public ViewData LastViewData
        {
            get
            {
                var newest = ViewHistory.Max(item => item.Key);
                if (newest != default) return ViewHistory[newest];
                else return null;
            }
        }

        /// <summary>
        /// Method to check if <param name="view"> is the current View
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public bool IsCurrentView(Type view) => this.ViewData?.ViewType == view;

        /// <summary>
        /// Boolean to check if we have a View set
        /// </summary>
        public bool HasView => this._ViewData?.ViewType != null;


        /// <summary>
        /// ViewData used by the component
        /// </summary>
        private ViewData _ViewData { get; set; }

        /// <summary>
        /// Boolean Flag to track if there's a pending render event queued
        /// </summary>
        private bool _RenderEventQueued;

        /// <summary>
        ///  RenderFragment Delegate run when rendering the component
        /// </summary>
        private readonly RenderFragment _renderDelegate;

        /// <summary>
        /// RendleHandle we get from the Attach Process to queue render requests on
        /// </summary>
        private RenderHandle _renderHandle;

        private Type _pageLayoutType => RouteViewService.Layout
            ?? RouteData?.PageType.GetCustomAttribute<LayoutAttribute>()?.LayoutType 
            ?? DefaultLayout;

        /// <summary>
        /// Initializes a new instance of <see cref="RouteView"/>.
        /// </summary>
        public RouteViewManager()
        {
            // Cache the delegate instances
            _renderDelegate = Render;
            // TODO - sort
            //_renderPageWithParametersDelegate = RenderPageWithParameters;
        }

        /// <inheritdoc />
        public void Attach(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
        }

        /// <inheritdoc />
        public Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            // Check if we have either RouteData or ViewData
            if (RouteData == null && this.ViewData == null)
            {
                throw new InvalidOperationException($"The {nameof(RouteView)} component requires a non-null value for the parameter {nameof(RouteData)}.");
            }
            // If we have RouteData then we've routed and need to clear the ViewData
            else if (RouteData != null)
                this.ViewData = null;
            // Render the component
            _renderHandle.Render(_renderDelegate);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Method tp load a new view
        /// </summary>
        /// <param name="viewData"></param>
        /// <returns></returns>
        public Task LoadViewAsync(ViewData viewData = null)
        {
            if (viewData != null) this.ViewData = viewData;
            if (ViewData == null)
            {
                throw new InvalidOperationException($"The {nameof(RouteViewManager)} component requires a non-null value for the parameter {nameof(ViewData)}.");
            }
            this.Render();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Method tp load a new view
        /// </summary>
        /// <param name="viewtype"></param>
        /// <returns></returns>
        public Task LoadViewAsync(Type viewtype)
        {
            var viewData = new ViewData(viewtype, new Dictionary<string, object>());
            return this.LoadViewAsync(viewData);
        }

        /// <summary>
        /// Method tp load a new view
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task LoadViewAsync<TView>(Dictionary<string, object> data = null)
        {
            var viewData = new ViewData(typeof(TView), data);
            return this.LoadViewAsync(viewData);
        }

        /// <summary>
        /// Method tp load a new view
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task LoadViewAsync<TView>(string key, object value)
        {
            var data = new Dictionary<string, object>();
            data.Add(key, value);
            var viewData = new ViewData(typeof(TView), data);
            return this.LoadViewAsync(viewData);
        }

        /// <summary>
        /// Renders the component.
        /// Cascades this instance of RouteViewManager and gets the layout Render Fragment
        /// </summary>
        /// <param name="builder">The <see cref="RenderTreeBuilder"/>.</param>
        protected virtual void Render(RenderTreeBuilder builder)
        {
            _RenderEventQueued = false;
            // Adds cascadingvalue for the ViewManager
            builder.OpenComponent<CascadingValue<RouteViewManager>>(0);
            builder.AddAttribute(1, "Value", this);
            // Get the layout render fragment
            builder.AddAttribute(2, "ChildContent", this._layoutViewFragment);
            builder.CloseComponent();
        }

        /// <summary>
        /// Render Fragment to build the layout with either the Routed component or the View Component
        /// </summary>
        private RenderFragment _layoutViewFragment => builder =>
        {
            builder.OpenComponent<LayoutView>(0);
            builder.AddAttribute(1, nameof(LayoutView.Layout), _pageLayoutType);
            if (this.HasView)
                builder.AddAttribute(2, nameof(LayoutView.ChildContent), _renderViewWithParameters);
            else
                builder.AddAttribute(2, nameof(LayoutView.ChildContent), _renderRouteWithParameters);
            builder.CloseComponent();
        };

        /// <summary>
        /// Render Fragment to build the routed component
        /// </summary>
        private RenderFragment _renderRouteWithParameters => builder =>
        {
            builder.OpenComponent(0, RouteData.PageType);
            foreach (var kvp in RouteData.RouteValues)
            {
                builder.AddAttribute(1, kvp.Key, kvp.Value);
            }
            builder.CloseComponent();
        };

        /// <summary>
        /// Render Fragment to build the view component
        /// </summary>
        private RenderFragment _renderViewWithParameters => builder =>
        {
            // Adds the defined view with any defined parameters
            builder.OpenComponent(0, _ViewData.ViewType);
            if (this._ViewData.ViewParameters != null)
            {
                foreach (var kvp in _ViewData.ViewParameters)
                {
                    builder.AddAttribute(1, kvp.Key, kvp.Value);
                }
            }
            builder.CloseComponent();
        };

        /// <summary>
        /// Method to force a UI update
        /// Queues a render of the component
        /// </summary>
        public void Render() => InvokeAsync(() =>
        {
            if (!this._RenderEventQueued)
            {
                this._RenderEventQueued = true;
                _renderHandle.Render(_renderDelegate);
            }
        }
        );

        /// <summary>
        /// Executes the supplied work item on the associated renderer's
        /// synchronization context.
        /// </summary>
        /// <param name="workItem">The work item to execute.</param>
        protected Task InvokeAsync(Action workItem) => _renderHandle.Dispatcher.InvokeAsync(workItem);

        /// <summary>
        /// Executes the supplied work item on the associated renderer's
        /// synchronization context.
        /// </summary>
        /// <param name="workItem">The work item to execute.</param>
        protected Task InvokeAsync(Func<Task> workItem) => _renderHandle.Dispatcher.InvokeAsync(workItem);

        /// <summary>
        /// Method to add a View to the View History and manage it's size
        /// </summary>
        /// <param name="value"></param>
        private void AddViewToHistory(ViewData value)
        {
            while (this.ViewHistory.Count >= this.ViewHistorySize)
            {
                var oldest = ViewHistory.Min(item => item.Key);
                this.ViewHistory.Remove(oldest);
            }
            this.ViewHistory.Add(DateTime.Now, value);
        }

    }
}
