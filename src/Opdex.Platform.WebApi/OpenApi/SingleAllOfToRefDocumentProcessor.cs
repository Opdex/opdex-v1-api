using NJsonSchema;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi;

/// <summary>
/// Takes the schemas in a document which contain allOf and transforms them to directly reference the parent
/// Required for README.io docs and other tools which don't support polymorphism.
/// </summary>
public class SingleAllOfToRefDocumentProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        var schemas = context.Document.Components.Schemas.Select(schema => schema.Value);
        foreach (var schema in schemas)
        {
            JsonSchema reference = null;

            switch (schema.AllOf.Count)
            {
                // when allOf.Length == 1 we're looking for a child object with no additional properties and no description
                case 1 when schema.AllOf.Single().Reference is not null:
                    reference = schema.AllOf.Single().Reference;
                    break;
                // when allOf.Length == 2 we're looking for a child object with no additional properties, which does have a description
                case 2 when schema.AllOf.Count(p => p.HasReference) == 1:
                    {
                        var referencingItem = schema.AllOf.First(p => p.HasReference);
                        var descriptiveItem = schema.AllOf.First(p => !p.HasReference);

                        if (descriptiveItem.Properties?.Count > 0) break;

                        reference = referencingItem.Reference;
                        reference.Description = descriptiveItem.Description;
                        break;
                    }
            }

            if (reference is null) continue;

            schema.Reference = reference;
            schema.AllOf.Clear();
        }
    }
}
