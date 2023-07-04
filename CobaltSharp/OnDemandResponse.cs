using SimpleJSON;

namespace CobaltSharp;

/// <summary>
/// Response object for OnDemand
/// </summary>
public struct OnDemandResponse
{
    /// <summary>
    /// API Response Status
    /// </summary>
    public Status status;
    /// <summary>
    /// Rendered block
    /// </summary>
    public string text;

    internal OnDemandResponse(string response)
    {
        JSONNode jsonObject = JSON.Parse(response);
        status = StatusHelper.FomString(jsonObject["status"].Value);
        text = jsonObject["text"].Value;
    }
}