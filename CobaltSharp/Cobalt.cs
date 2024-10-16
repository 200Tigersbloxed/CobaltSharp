using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CobaltSharp.Internals;

namespace CobaltSharp;

/// <summary>
/// The main Object where Requests are Handled
/// </summary>
public class Cobalt : IDisposable
{
    private CobaltServer _server;
    private HttpClient _httpClient;

    public Cobalt()
    {
        _server = new CobaltServer();
        _httpClient = new HttpClient {Timeout = _server.Timeout};
    }
    public Cobalt(CobaltServer cobaltServer)
    {
        _server = cobaltServer;
        _httpClient = new HttpClient {Timeout = cobaltServer.Timeout};
    }

    /// <summary>
    /// Get info on where to download media
    /// </summary>
    /// <param name="getMedia"></param>
    /// <returns></returns>
    public MediaResponse GetMedia(GetMedia getMedia)
    {
        Uri e = new Uri(_server.APIUri, ((IAPIEndpoint) getMedia).Endpoint);
        string mt = ((IAPIEndpoint) getMedia).MediaType;
        StringContent c = new StringContent(((IAPIEndpoint) getMedia).GetBody(), Encoding.UTF8, mt);
        HttpRequestMessage requestMessage = new HttpRequestMessage
        {
            RequestUri = e,
            Content = c,
            Method = ((IAPIEndpoint) getMedia).Method
        };
        requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(mt);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mt));
        if(getMedia.OtherLanguage)
            requestMessage.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(getMedia.dubLang));
        HttpResponseMessage responseMessage = _httpClient.SendAsync(requestMessage).Result;
        return new MediaResponse(responseMessage.Content.ReadAsStringAsync().Result);
    }

    /// <summary>
    /// Asynchronously get info on where to download media
    /// </summary>
    /// <param name="getMedia"></param>
    /// <returns></returns>
    public async Task<MediaResponse> GetMediaAsync(GetMedia getMedia)
    {
        Uri e = new Uri(_server.APIUri, ((IAPIEndpoint) getMedia).Endpoint);
        string mt = ((IAPIEndpoint) getMedia).MediaType;
        StringContent c = new StringContent(((IAPIEndpoint) getMedia).GetBody(), Encoding.UTF8, mt);
        HttpRequestMessage requestMessage = new HttpRequestMessage
        {
            RequestUri = e,
            Content = c,
            Method = ((IAPIEndpoint) getMedia).Method
        };
        requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(mt);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mt));
        if(getMedia.OtherLanguage)
            requestMessage.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(getMedia.dubLang));
        HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);
        return new MediaResponse(await responseMessage.Content.ReadAsStringAsync());
    }

    private StreamResponse GetStream(string url)
    {
        HttpRequestMessage requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Get
        };
        HttpResponseMessage responseMessage = _httpClient.SendAsync(requestMessage).Result;
        StreamResponse streamResponse = new StreamResponse(responseMessage.Content.ReadAsStringAsync().Result);
        if (streamResponse.status != null)
            return streamResponse;
        if (responseMessage.Content.Headers.ContentDisposition == null)
        {
            Uri? uri = responseMessage.RequestMessage?.RequestUri;
            if (uri == null)
                return new StreamResponse(responseMessage.Content.ReadAsStreamAsync().Result, Guid.NewGuid().ToString());
            string fileName = Path.GetFileName(uri.AbsolutePath);
            Console.WriteLine(fileName);
            return new StreamResponse(responseMessage.Content.ReadAsStreamAsync().Result, fileName);
        }
        return new StreamResponse(responseMessage.Content.ReadAsStreamAsync().Result,
            RemoveQuotes(responseMessage.Content.Headers.ContentDisposition?.FileName));
    }

    private async Task<StreamResponse> GetStreamAsync(string url)
    {
        HttpRequestMessage requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Get
        };
        HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);
        StreamResponse streamResponse = new StreamResponse(await responseMessage.Content.ReadAsStringAsync());
        if (streamResponse.status != null)
            return streamResponse;
        if (responseMessage.Content.Headers.ContentDisposition == null)
        {
            Uri? uri = responseMessage.RequestMessage?.RequestUri;
            if (uri == null)
                return new StreamResponse(await responseMessage.Content.ReadAsStreamAsync(), Guid.NewGuid().ToString());
            string fileName = Path.GetFileName(uri.AbsolutePath);
            Console.WriteLine(fileName);
            return new StreamResponse(await responseMessage.Content.ReadAsStreamAsync(), fileName);
        }
        return new StreamResponse(await responseMessage.Content.ReadAsStreamAsync(),
            RemoveQuotes(responseMessage.Content.Headers.ContentDisposition?.FileName));
    }

    /// <summary>
    /// Gets a StreamResponse that contains a Stream and FileName
    /// </summary>
    /// <param name="mediaResponse"></param>
    /// <returns></returns>
    public StreamResponse GetStream(MediaResponse mediaResponse) => GetStream(mediaResponse.url!);
    
    /// <summary>
    /// Gets a StreamResponse that contains a Stream and FileName
    /// </summary>
    /// <param name="pickerItem"></param>
    /// <returns></returns>
    public StreamResponse GetStream(PickerItem pickerItem) => GetStream(pickerItem.url);

    /// <summary>
    /// Asynchronously gets a StreamResponse that contains a Stream and FileName
    /// </summary>
    /// <param name="mediaResponse"></param>
    /// <returns></returns>
    public async Task<StreamResponse> GetStreamAsync(MediaResponse mediaResponse) => await GetStreamAsync(mediaResponse.url!);
    
    /// <summary>
    /// Gets a StreamResponse that contains a Stream and FileName
    /// </summary>
    /// <param name="pickerItem"></param>
    /// <returns></returns>
    public async Task<StreamResponse> GetStreamAsync(PickerItem pickerItem) => await GetStreamAsync(pickerItem.url);
    
    /// <summary>
    /// Gets the On-demand website element loading. Useless without an HTML Parser/Renderer.
    /// </summary>
    /// <param name="onDemand"></param>
    /// <returns></returns>
    public OnDemandResponse OnDemand(OnDemand onDemand)
    {
        Uri e = new Uri(_server.APIUri, ((IAPIEndpoint) onDemand).Endpoint);
        string mt = ((IAPIEndpoint) onDemand).MediaType;
        StringContent c = new StringContent(((IAPIEndpoint) onDemand).GetBody(), Encoding.UTF8, mt);
        HttpRequestMessage requestMessage = new HttpRequestMessage
        {
            RequestUri = e,
            Content = c,
            Method = ((IAPIEndpoint) onDemand).Method
        };
        requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(mt);
        requestMessage.Headers.Accept.Clear();
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mt));
        HttpResponseMessage responseMessage = _httpClient.SendAsync(requestMessage).Result;
        return new OnDemandResponse(responseMessage.Content.ReadAsStringAsync().Result);
    }
    
    /// <summary>
    /// Asynchronously gets the On-demand website element loading. Useless without an HTML Parser/Renderer.
    /// </summary>
    /// <param name="onDemand"></param>
    /// <returns></returns>
    public async Task<OnDemandResponse> OnDemandAsync(OnDemand onDemand)
    {
        Uri e = new Uri(_server.APIUri, ((IAPIEndpoint) onDemand).Endpoint);
        string mt = ((IAPIEndpoint) onDemand).MediaType;
        StringContent c = new StringContent(((IAPIEndpoint) onDemand).GetBody(), Encoding.UTF8, mt);
        HttpRequestMessage requestMessage = new HttpRequestMessage
        {
            RequestUri = e,
            Content = c,
            Method = ((IAPIEndpoint) onDemand).Method
        };
        requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(mt);
        requestMessage.Headers.Accept.Clear();
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mt));
        HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);
        return new OnDemandResponse(await responseMessage.Content.ReadAsStringAsync());
    }

    public ServerInfo GetServerInfo()
    {
        Uri e = new Uri(_server.APIUri, "api/serverInfo");
        string mt = "application/json";
        HttpRequestMessage requestMessage = new HttpRequestMessage
        {
            RequestUri = e,
            Method = HttpMethod.Get
        };
        requestMessage.Headers.Accept.Clear();
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mt));
        HttpResponseMessage responseMessage = _httpClient.SendAsync(requestMessage).Result;
        return new ServerInfo(responseMessage.Content.ReadAsStringAsync().Result);
    }
    
    public async Task<ServerInfo> GetServerInfoAsync()
    {
        Uri e = new Uri(_server.APIUri, "api/serverInfo");
        string mt = "application/json";
        HttpRequestMessage requestMessage = new HttpRequestMessage
        {
            RequestUri = e,
            Method = HttpMethod.Get
        };
        requestMessage.Headers.Accept.Clear();
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mt));
        HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);
        return new ServerInfo(await responseMessage.Content.ReadAsStringAsync());
    }

    private static string RemoveQuotes(string? fileName)
    {
        if(fileName == null)
            return String.Empty;
        // dotnet, for the love of god, fix this issue, PLEASE!
        string ns = fileName.Substring(1);
        ns = ns.Substring(0, ns.Length - 1);
        return ns;
    }

    public void Dispose() => _httpClient.Dispose();
}