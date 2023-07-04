using System.Text;
using SimpleJSON;

namespace CobaltSharp;

/// <summary>
/// Object containing stream result
/// </summary>
public struct StreamResponse : IDisposable
{
    /// <summary>
    /// If the operation completed successfully or not
    /// </summary>
    public Status? status;
    /// <summary>
    /// Response text
    /// </summary>
    public string? text;
    /// <summary>
    /// Stream containing data if status is Success
    /// </summary>
    public Stream? Stream;
    /// <summary>
    /// FileName of the Stream
    /// </summary>
    public string FileName;

    internal StreamResponse(string response)
    {
        try
        {
            JSONNode node = JSON.Parse(response);
            status = StatusHelper.FomString(node["status"].Value);
            text = node["text"].Value;
        }
        catch (Exception)
        {
            status = null;
            text = null;
        }
    }
    
    private string GenerateId(int length)
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        StringBuilder res = new StringBuilder();
        Random rnd = new Random();
        while (0 < length--)
        {
            res.Append(valid[rnd.Next(valid.Length)]);
        }
        return res.ToString();
    }

    internal StreamResponse(Stream s, string? fileName)
    {
        status = Status.Success;
        text = String.Empty;
        Stream = s;
        FileName = fileName ?? (FileName = GenerateId(25));
    }

    public void Dispose() => Stream?.Dispose();
}