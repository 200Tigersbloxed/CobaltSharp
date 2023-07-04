using CobaltSharp.Internals;
using SimpleJSON;

namespace CobaltSharp;

/// <summary>
/// Object containing data on what media should be obtained by the CobaltServer
/// </summary>
public struct GetMedia : IAPIEndpoint
{
    /// <summary>
    /// The URL of the Media to download
    /// </summary>
    public string url;
    /// <summary>
    /// (YouTube only) The video codec to use
    /// </summary>
    public VideoCodec vCodec;
    /// <summary>
    /// Quality of the video
    /// </summary>
    public VideoQuality vQuality;
    /// <summary>
    /// Audio format
    /// </summary>
    public AudioFormat aFormat;
    /// <summary>
    /// Should the video be removed
    /// </summary>
    public bool isAudioOnly;
    /// <summary>
    /// Removes watermark in TikTOk and Douyin videos
    /// </summary>
    public bool isNoTTWatermark;
    /// <summary>
    /// Downloads the original sound used in a TikTok video
    /// </summary>
    public bool isTTFullAudio;
    /// <summary>
    /// Mutes audio track(s) in videos
    /// </summary>
    public bool isAudioMuted;
    /// <summary>
    /// Accept-Language header, if not empty, for YouTube video audio tracks
    /// </summary>
    public string dubLang;

    internal bool OtherLanguage { get; private set; }
    
    string IAPIEndpoint.Endpoint => "api/json/";
    HttpMethod IAPIEndpoint.Method => HttpMethod.Post;
    string IAPIEndpoint.MediaType => "application/json";

    string IAPIEndpoint.GetBody()
    {
        JSONObject jsonObject = new JSONObject();
        jsonObject.Add("url", url);
        jsonObject.Add("vCodec", vCodec.ToString());
        jsonObject.Add("vQuality", vQuality.ToString().Substring(1));
        jsonObject.Add("aFormat", aFormat.ToString());
        jsonObject.Add("isAudioOnly", isAudioOnly);
        jsonObject.Add("isNoTTWatermark", isNoTTWatermark);
        jsonObject.Add("isTTFullAudio", isTTFullAudio);
        jsonObject.Add("isAudioMuted", isAudioMuted);
        OtherLanguage = !string.IsNullOrEmpty(dubLang);
        jsonObject.Add("dubLang", OtherLanguage);
        return jsonObject.ToString();
    }
}