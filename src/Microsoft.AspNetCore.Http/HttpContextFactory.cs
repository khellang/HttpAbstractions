// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Http
{
    public class HttpContextFactory : IHttpContextFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly FormOptions _formOptions;

        [Obsolete("This constructor is obsolete and will be removed in a future version. Use the overloads without an ObjectPoolProvider")]
        public HttpContextFactory(ObjectPoolProvider poolProvider, IOptions<FormOptions> formOptions)
            : this(formOptions)
        {
        }

        public HttpContextFactory(IOptions<FormOptions> formOptions)
            : this(formOptions, httpContextAccessor: null)
        {
        }

        [Obsolete("This constructor is obsolete and will be removed in a future version. Use the overloads without an ObjectPoolProvider")]
        public HttpContextFactory(ObjectPoolProvider poolProvider, IOptions<FormOptions> formOptions, IHttpContextAccessor httpContextAccessor)
            : this(formOptions, httpContextAccessor)
        {
        }

        public HttpContextFactory(IOptions<FormOptions> formOptions, IHttpContextAccessor httpContextAccessor)
        {
            if (formOptions == null)
            {
                throw new ArgumentNullException(nameof(formOptions));
            }

            _formOptions = formOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpContext Create(IFeatureCollection featureCollection)
        {
            if (featureCollection == null)
            {
                throw new ArgumentNullException(nameof(featureCollection));
            }

            var httpContext = new DefaultHttpContext(featureCollection);
            if (_httpContextAccessor != null)
            {
                _httpContextAccessor.HttpContext = httpContext;
            }

            var formFeature = new FormFeature(httpContext.Request, _formOptions);
            featureCollection.Set<IFormFeature>(formFeature);

            return httpContext;
        }

        public void Dispose(HttpContext httpContext)
        {
            if (_httpContextAccessor != null)
            {
                _httpContextAccessor.HttpContext = null;
            }
        }
    }
}