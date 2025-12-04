using HealthCTX.Domain;
using HealthCTX.Domain.OperationOutcomes;
using System.Text.Json;

namespace HealthServices.Gateway.Helpers;

internal class FhirResponseHelper
{
    internal static async Task<TResource?> GetResource<TResource>(
        string query,
        HttpContent content,
        CancellationToken cancellationToken,
        ILogger logger)
        where TResource : IResource
    {
        try
        {
            var resource = await content.ReadFromJsonAsync<TResource>(cancellationToken);
            if (resource is null)
            {
                logger.LogError("Deserialization of {ResourceType} with {Query} returned null", typeof(TResource).Name, query);
                return default;
            }

            return resource;
        }
        catch (JsonException jsonException)
        {
            logger.LogError(jsonException, "Deserialization of {ResourceType} with {Query} failed, error message: {ErrorMessages}", typeof(TResource).Name, query, jsonException.Message);
            return default;
        }
    }

    internal static string? HandleSerializationResult<TResource>(
        (string? FhirJson, OperationOutcome Outcome) serializationResult,
        ILogger logger)
        where TResource : IResource
    {
        if (serializationResult.FhirJson is null)
        {
            string diagnostics = serializationResult.Outcome.ToFhirJsonString().Item1 ?? "No diagnostics available";
            logger.LogError("Conversion to Fhir json failed for {ResourceType} - Outcome {Diagnostics}", typeof(TResource).Name, diagnostics);
            return default;
        }

        return serializationResult.FhirJson;
    }
}
