namespace CobaltSharp.Internals;

internal interface IAPIEndpoint
{
    internal string Endpoint { get; }
    internal HttpMethod Method { get; }
    internal string MediaType { get; }
    internal string GetBody();
}