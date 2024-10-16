namespace CobaltSharp;

/// <summary>
/// Info for which Cobalt API server to connect to
/// </summary>
public struct CobaltServer
{
    private const string DEFAULT_API = "https://api.cobalt.tools/";
    /// <summary>
    /// The API's Uri
    /// </summary>
    public Uri APIUri { get; }
    /// <summary>
    /// How long before a request should timeout. Timed out requests will throw an exception.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromHours(1);

    public CobaltServer() => APIUri = new Uri(DEFAULT_API);
    public CobaltServer(string url) => APIUri = new Uri(url);
    public CobaltServer(Uri uri) => APIUri = uri;
}