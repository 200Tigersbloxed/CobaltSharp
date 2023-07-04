using SimpleJSON;

namespace CobaltSharp;

/// <summary>
/// Object handling selections of media to get
/// </summary>
public struct PickerItem
{
    /// <summary>
    /// Type of media (Various PickerType Only)
    /// </summary>
    public string? type;
    /// <summary>
    /// Direct link to file or link to cobalt's stream
    /// </summary>
    public string url;
    /// <summary>
    /// Item thumbnail (Video Type Only)
    /// </summary>
    public string? thumb;

    internal PickerItem(JSONNode node)
    {
        url = node["url"].Value;
        if (node.HasKey("type"))
            type = node["type"].Value;
        if (node.HasKey("thumb"))
            thumb = node["thumb"].Value;
    }
}