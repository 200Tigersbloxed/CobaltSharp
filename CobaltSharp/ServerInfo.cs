using SimpleJSON;

namespace CobaltSharp;

public struct ServerInfo
{
    public string version;
    public string commit;
    public string branch;
    public string name;
    public string url;
    public string cors;
    public DateTime startTime;
    
    internal ServerInfo(string response)
    {
        JSONNode node = JSON.Parse(response);
        version = node["version"].Value;
        commit = node["commit"].Value;
        branch = node["branch"].Value;
        name = node["name"].Value;
        url = node["url"].Value;
        cors = node["cors"].Value;
        string st = node["startTime"].Value;
        ulong v = Convert.ToUInt64(st);
        startTime = DateTimeOffset.FromUnixTimeMilliseconds((long)v).DateTime;
    }
}