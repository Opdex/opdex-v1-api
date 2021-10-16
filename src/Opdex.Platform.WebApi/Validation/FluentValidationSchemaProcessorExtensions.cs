using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZymLabs.NSwag.FluentValidation;

namespace Opdex.Platform.WebApi.Validation
{
    // Todo: PR into library
    public static class FluentValidationSchemaProcessorExtensions
    {
        /// <summary>
        /// Applies fluent validation rules to the OpenAPI generator.
        /// </summary>
        public static void AddFluentValidationSchemaProcessor(this AspNetCoreOpenApiDocumentGeneratorSettings settings,
                                                              IServiceProvider provider,
                                                              Action<FluentValidationRuleConfig> configuration = null)
        {
            var config = new FluentValidationRuleConfig();
            configuration.Invoke(config);

            List<FluentValidationRule> validationRules = null;
            if (config.AssembliesToScan.Count > 0)
            {
                validationRules = new List<FluentValidationRule>();
                foreach (var assembly in config.AssembliesToScan)
                {
                    var ruleTypes = assembly.DefinedTypes.Where(type => typeof(FluentValidationRule).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
                    validationRules.AddRange(ruleTypes.Select(type => Activator.CreateInstance(type)).Cast<FluentValidationRule>());
                }
            }

            var factory = provider.CreateScope().ServiceProvider.GetService<IValidatorFactory>();
            settings.SchemaProcessors.Add(new FluentValidationSchemaProcessor(factory, validationRules));
        }
    }

    /// <summary>
    /// Configures fluent validation schema processing.
    /// </summary>
    public class FluentValidationRuleConfig
    {
        internal List<Assembly> AssembliesToScan { get; }

        public FluentValidationRuleConfig()
        {
            AssembliesToScan = new List<Assembly>();
        }

        /// <summary>
        /// Registers an assembly to scan for validation rules.
        /// </summary>
        /// <typeparam name="TMarker">An assembly marker.</typeparam>
        public FluentValidationRuleConfig RegisterRulesFromAssemblyContaining<TMarker>() => RegisterRulesFromAssemblyContaining(typeof(TMarker));

        /// <summary>
        /// Registers an assembly to scan for validation rules.
        /// </summary>
        /// <param name="type">An assembly marker.</param>
        public FluentValidationRuleConfig RegisterRulesFromAssemblyContaining(Type type) => RegisterRulesFromAssembly(type.Assembly);

        /// <summary>
        /// Registers an assembly to scan for validation rules.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public FluentValidationRuleConfig RegisterRulesFromAssembly(Assembly assembly)
        {
            AssembliesToScan.Add(assembly);
            return this;
        }
    }
}
