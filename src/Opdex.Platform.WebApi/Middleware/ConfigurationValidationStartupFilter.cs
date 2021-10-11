using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Opdex.Platform.Common.Configurations;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Middleware
{
    public class ConfigurationValidationStartupFilter : IStartupFilter
    {
        readonly IEnumerable<IValidatable> _validatableObjects;

        public ConfigurationValidationStartupFilter(IEnumerable<IValidatable> validatableObjects)
        {
            _validatableObjects = validatableObjects;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            foreach (var validatableObject in _validatableObjects)
            {
                validatableObject.Validate();
            }

            return next;
        }
    }
}
