using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Opdex.Platform.Common.Models;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Models.Binders;

public class Sha256ModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext is null) throw new ArgumentNullException(nameof(bindingContext));

        var modelName = bindingContext.ModelName;

        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
        if (valueProviderResult == ValueProviderResult.None) return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        if (Sha256.TryParse(value, out var hash))
        {
            bindingContext.Result = ModelBindingResult.Success(hash);
        }
        else
        {
            bindingContext.ModelState.AddModelError(modelName, "Invalid SHA256 hash.");
        }

        return Task.CompletedTask;
    }
}

public class Sha256ModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        if (context.Metadata.ModelType == typeof(Sha256)) return new BinderTypeModelBinder(typeof(Sha256ModelBinder));

        return null;
    }
}