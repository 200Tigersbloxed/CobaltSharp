using CobaltSharp.Internals;

namespace CobaltSharp;

/// <summary>
/// Object used for getting the On-Demand elements
/// </summary>
public class OnDemand : IAPIEndpoint
{
    /// <summary>
    /// Block ID to be rendered on the server
    /// </summary>
    public int blockId;
    
    string IAPIEndpoint.Endpoint => "api/onDemand?blockId=" + blockId;
    HttpMethod IAPIEndpoint.Method => HttpMethod.Get;
    string IAPIEndpoint.MediaType => "application/json";
    string IAPIEndpoint.GetBody() => String.Empty;
}