using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Models.Binders
{
    /// <summary>
    /// Binds ISO-8601 DateTime values to their UTC representation.
    /// </summary>
    /// <remarks>
    /// Inspired by https://github.com/dotnet/aspnetcore/issues/11584.
    /// This may no longer be necessary using .NET 5 or later.
    /// </remarks>
    public class UtcAwareDateTimeModelBinder : IModelBinder
    {
        private const DateTimeStyles StyleSettings = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal |
                                                     DateTimeStyles.AllowWhiteSpaces;

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

            try
            {
                var value = valueProviderResult.FirstValue;
                var culture = valueProviderResult.Culture;

                object model;
                if (string.IsNullOrWhiteSpace(value)) model = null;
                else if (type == typeof(DateTime)) model = DateTime.Parse(value, culture, StyleSettings);
                else throw new NotSupportedException();

                // When converting value, a null model may indicate a failed conversion for an otherwise required
                // model (can't set a ValueType to null). This detects if a null model value is acceptable given the
                // current bindingContext. If not, an error is logged.
                if (model == null && !metadata.IsReferenceOrNullableType)
                {
                    var message = metadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor(valueProviderResult.ToString());
                    modelState.TryAddModelError(modelName, message);
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Success(model);
                }
            }
            catch (FormatException exception)
            {
                modelState.TryAddModelError(modelName, exception, metadata);
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                {
                    // Unlike TypeConverters, floating point types do not seem to wrap FormatExceptions. Preserve
                    // this code in case a cursory review of the CoreFx code missed something.
                    exception = ExceptionDispatchInfo.Capture(exception.InnerException).SourceException;
                }

                modelState.TryAddModelError(modelName, exception, metadata);
            }

            return Task.CompletedTask;
        }

    }

    public class UtcAwareDateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(DateTime)) return new UtcAwareDateTimeModelBinder();

            return null;
        }
    }
}
