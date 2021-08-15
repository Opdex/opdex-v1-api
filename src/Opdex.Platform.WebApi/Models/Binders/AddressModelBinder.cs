using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Opdex.Platform.Common.Models;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Models.Binders
{
    public class AddressModelBinder : IModelBinder
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
                var address = new Address(value);
                bindingContext.Result = ModelBindingResult.Success(address);
            }
            catch (Exception e)
            {
                bindingContext.ModelState.AddModelError(modelName, e.Message);
            }

            return Task.CompletedTask;
        }
    }

    public class AddressModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(Address)) return new BinderTypeModelBinder(typeof(AddressModelBinder));

            return null;
        }
    }
}
