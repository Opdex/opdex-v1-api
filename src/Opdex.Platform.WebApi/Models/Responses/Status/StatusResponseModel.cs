using NJsonSchema.Annotations;

namespace Opdex.Platform.WebApi.Models.Responses.Status
{
    public class StatusResponseModel
    {
        [NotNull]
        public string Commit { get; set; }

        [NotNull]
        public string Identifier { get; set; }
    }
}
