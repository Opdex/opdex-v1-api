using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Models.Binders
{
    /// <summary>
    /// Binds a Unix timestamp to its UTC DateTime representation.
    /// </summary>
    public class UnixDateTimeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None) return Task.CompletedTask;

            var modelState = bindingContext.ModelState;
            modelState.SetModelValue(modelName, valueProviderResult);

            var metadata = bindingContext.ModelMetadata;
            var type = metadata.UnderlyingOrModelType;

            var value = valueProviderResult.FirstValue;
            if (!long.TryParse(value, out var unixTime)) modelState.TryAddModelError(modelName, "Value is not a valid UInt64.");

            try
            {
                var dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
                bindingContext.Result = ModelBindingResult.Success(dateTime);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                modelState.TryAddModelError(modelName, exception, metadata);
            }

            return Task.CompletedTask;
        }

    }
}
