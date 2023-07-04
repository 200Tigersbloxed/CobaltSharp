namespace CobaltSharp;

/// <summary>
/// Status of a response
/// </summary>
public enum Status
{
    Error,
    Redirect,
    Stream,
    Success,
    RateLimit,
    Picker
}

internal static class StatusHelper
{
    public static Status FomString(string s)
    {
        switch (s.ToLower())
        {
            case "error":
                return Status.Error;
            case "redirect":
                return Status.Redirect;
            case "stream":
                return Status.Stream;
            case "success":
                return Status.Success;
            case "rate-limit":
                return Status.RateLimit;
            case "picker":
                return Status.Picker;
        }
        return Status.Error;
    }
}