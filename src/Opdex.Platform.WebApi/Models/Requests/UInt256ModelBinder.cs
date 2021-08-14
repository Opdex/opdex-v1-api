using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Opdex.Platform.Common.Models.UInt;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Models.Requests
{
    public class UInt256ModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext is null) throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None) return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            try
            {
                var result = UInt256.Parse(value);
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (Exception e)
            {
                bindingContext.ModelState.AddModelError(modelName, e.Message);
            }

            return Task.CompletedTask;
        }
    }

    public class UInt256ModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(UInt256)) return new BinderTypeModelBinder(typeof(UInt256ModelBinder));

            return null;
        }
    }
}
