using FluentValidation;
using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.WebApi.Validation;

public static class EnumRules
{
    public static IRuleBuilderOptions<T, TEnum> MustBeValidEnumValue<T, TEnum>(this IRuleBuilder<T, TEnum> ruleBuilder) where TEnum : Enum
    {
        return ruleBuilder.Must(value => value.IsValid()).WithMessage("Value must be valid for the the enumeration values.");
    }
}