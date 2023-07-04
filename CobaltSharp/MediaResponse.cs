using SimpleJSON;

namespace CobaltSharp;

/// <summary>
/// The object used to get info on where to download/stream media
/// </summary>
public struct MediaResponse
{
    /// <summary>
    /// API Response Status
    /// </summary>
    public Status status;
    /// <summary>
    /// Text
    /// </summary>
    public string text;
    /// <summary>
    /// Direct link to a file or to cobalt's stream
    /// </summary>
    public string? url;
    /// <summary>
    /// Type of Picker
    /// </summary>
    public PickerType? pickerType;
    /// <summary>
    /// Picker Items
    /// </summary>
    public PickerItem[] picker;
    /// <summary>
    /// Direct link to a file or to cobalt's stream
    /// </summary>
    public string? audio;

    internal MediaResponse(string response)
    {
        JSONNode jsonObject = JSON.Parse(response);
        status = StatusHelper.FomString(jsonObject["status"].Value);
        text = jsonObject["text"].Value;
        if (jsonObject.HasKey("url"))
            url = jsonObject["url"].Value;
        if (jsonObject.HasKey("pickerType"))
        {
            PickerType pt;
            Enum.TryParse(jsonObject["pickerType"].Value, out pt);
            pickerType = pt;
        }
        if (jsonObject.HasKey("picker"))
        {
            List<PickerItem> items = new();
            foreach (JSONNode jsonNode in jsonObject["picker"].Values)
                items.Add(new PickerItem(jsonNode));
            picker = items.ToArray();
        }
        else
            picker = Array.Empty<PickerItem>();
        if (jsonObject.HasKey("audio"))
            audio = jsonObject["audio"].Value;
    }
}