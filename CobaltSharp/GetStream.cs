using System.Collections.Specialized;
using System.Web;
using CobaltSharp.Internals;

namespace CobaltSharp;

/// <summary>
/// The object used for downloading a stream from a MediaRequest
/// </summary>
public struct GetStream : IAPIEndpoint
{
    /// <summary>
    /// Probing rate limit
    /// </summary>
    public string? p;
    /// <summary>
    /// Stream token
    /// </summary>
    public string? t;
    /// <summary>
    /// Combined hash of hashed IP, stream token, expiry timestamp, and service name
    /// </summary>
    public string? h;
    /// <summary>
    /// Expiry timestamp
    /// </summary>
    public string? e;

    public GetStream(MediaResponse mediaResponse)
    {
        if(mediaResponse.url == null)
            return;
        Uri uri = new Uri(mediaResponse.url);
        NameValueCollection vals = HttpUtility.ParseQueryString(uri.Query);
        p = vals.Get("p");
        t = vals.Get("t");
        h = vals.Get("h");
        e = vals.Get("e");
    }

    public GetStream(string url)
    {
        Uri uri = new Uri(url);
        NameValueCollection vals = HttpUtility.ParseQueryString(uri.Query);
        p = vals.Get("p");
        t = vals.Get("t");
        h = vals.Get("h");
        e = vals.Get("e");
    }

    string IAPIEndpoint.Endpoint => "api/stream?p=" + p + "&t=" + t + "&h=" + h + "&e=" + e;
    HttpMethod IAPIEndpoint.Method => HttpMethod.Get;
    string IAPIEndpoint.MediaType => "application/json";
    string IAPIEndpoint.GetBody() => String.Empty;
}