#define ASYNC

using CobaltSharp;

Cobalt cobalt = new Cobalt();
#if ASYNC
LogServerInfo(await cobalt.GetServerInfoAsync());
#else
LogServerInfo(cobalt.GetServerInfo());
#endif
GetMedia getMedia = PromptGetMedia();
MediaResponse mediaResponse;
#if ASYNC
mediaResponse = await cobalt.GetMediaAsync(getMedia);
#else
mediaResponse = cobalt.GetMedia(getMedia);
#endif
if (mediaResponse.status is Status.Success or Status.Stream)
{
    if (mediaResponse.status == Status.Success)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("Got MediaResponse, but was not a Stream response");
        Console.ForegroundColor = ConsoleColor.White;
        return;
    }
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Got media!");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("Would you like to download the media? (y/n) Default: y");
    bool download = Console.ReadLine()?.ToLower() != "n";
    if (download)
    {
        StreamResponse streamResponse;
        DateTime before = DateTime.Now;
#if ASYNC
        streamResponse = await cobalt.GetStreamAsync(mediaResponse);
#else
        streamResponse = cobalt.GetStream(mediaResponse);
#endif
        TimeSpan after = DateTimeOffset.Now - before;
        if (streamResponse.status == null || streamResponse.status != Status.Success)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Failed to GetStream!");
            Console.WriteLine(mediaResponse.text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Got stream! (Took {after.Milliseconds}ms)");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Writing to " + streamResponse.FileName + "...");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            SaveStreamResponseToFile(streamResponse);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done!");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Goodbye!");
        }
    }
}
else
{
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine("Failed to GetMedia!");
    Console.WriteLine(mediaResponse.text);
    Console.ForegroundColor = ConsoleColor.White;
}

void LogServerInfo(ServerInfo serverInfo)
{
    Console.WriteLine("Server Info");
    Console.WriteLine("=============================================================");
    Console.WriteLine($"Version: {serverInfo.version}");
    Console.WriteLine($"Commit: {serverInfo.commit}");
    Console.WriteLine($"Branch: {serverInfo.branch}");
    Console.WriteLine($"Name: {serverInfo.name}");
    Console.WriteLine($"URL: {serverInfo.url}");
    Console.WriteLine($"CORS: {serverInfo.cors}");
    Console.WriteLine($"startTime: {serverInfo.startTime.ToLongDateString() + serverInfo.startTime.ToLongTimeString()}");
    Console.WriteLine("=============================================================");
}

string SelectLanguage()
{
    Console.WriteLine("=============================================================");
    Console.WriteLine("Languages usually look like a language code, followed by a country code.");
    Console.WriteLine("For example, English United States would be en-US");
    Console.WriteLine("Another example, fr-CA would be French Canada");
    Console.WriteLine("What Language?");
    string r = Console.ReadLine() ?? "en-US";
    Console.WriteLine("=============================================================");
    return r;
}

GetMedia PromptGetMedia()
{
    Console.WriteLine("What is the url of the media?");
    string url = Console.ReadLine() ?? String.Empty;
    Console.WriteLine("What codec? (h264/av1/vp9) Default: h264 (YouTube only)");
    string vc = Console.ReadLine() ?? String.Empty;
    VideoCodec videoCodec;
    switch (vc.ToLower())
    {
        case "av1":
            videoCodec = VideoCodec.av1;
            break;
        case "vp9":
            videoCodec = VideoCodec.vp9;
            break;
        default:
            videoCodec = VideoCodec.h264;
            break;
    }
    Console.WriteLine("What video quality? (144/240/360/480/720/1080/1440/2160/4320) Default: 720");
    string vq = Console.ReadLine() ?? String.Empty;
    VideoQuality videoQuality;
    switch (vq.ToLower())
    {
        case "144":
            videoQuality = VideoQuality.q144;
            break;
        case "240":
            videoQuality = VideoQuality.q240;
            break;
        case "360":
            videoQuality = VideoQuality.q360;
            break;
        case "480":
            videoQuality = VideoQuality.q480;
            break;
        case "720":
            videoQuality = VideoQuality.q720;
            break;
        case "1080":
            videoQuality = VideoQuality.q1080;
            break;
        case "1440":
            videoQuality = VideoQuality.q1440;
            break;
        case "2160":
            videoQuality = VideoQuality.q2160;
            break;
        case "4320":
            videoQuality = VideoQuality.q4320;
            break;
        default:
            // Recommended quality for phones
            videoQuality = VideoQuality.q720;
            break;
    }
    Console.WriteLine("What audio format? (best/mp3/ogg/wav/opus) Default: mp3");
    string af = Console.ReadLine() ?? String.Empty;
    AudioFormat audioFormat;
    switch (af.ToLower())
    {
        case "best":
            audioFormat = AudioFormat.best;
            break;
        case "mp3":
            audioFormat = AudioFormat.mp3;
            break;
        case "ogg":
            audioFormat = AudioFormat.ogg;
            break;
        case "wav":
            audioFormat = AudioFormat.wav;
            break;
        case "opus":
            audioFormat = AudioFormat.opus;
            break;
        default:
            audioFormat = AudioFormat.mp3;
            break;
    }
    Console.WriteLine("Download audio only? (y/n) Default: n");
    bool audioOnly = Console.ReadLine()?.ToLower() == "y";
    Console.WriteLine("Remove the TikTok/Douyin video watermarks? (y/n) Default: n (TikTok/Douyin only)");
    bool removeWatermark = Console.ReadLine()?.ToLower() == "y";
    Console.WriteLine("Download the original sound from a TikTok video? (y/n) Default: n (TikTok only)");
    bool fullAudio = Console.ReadLine()?.ToLower() == "y";
    Console.WriteLine("Mute the audio? (y/n) Default: n");
    bool muteAudio = Console.ReadLine()?.ToLower() == "y";
    Console.WriteLine("Use a specified language? (y/n) Default: n (YouTube only)");
    bool useLang = Console.ReadLine()?.ToLower() == "y";
    return new GetMedia
    {
        url = url,
        vCodec = videoCodec,
        vQuality = videoQuality,
        aFormat = audioFormat,
        isAudioOnly = audioOnly,
        isNoTTWatermark = removeWatermark,
        isTTFullAudio = fullAudio,
        isAudioMuted = muteAudio,
        dubLang = useLang ? SelectLanguage() : String.Empty
    };
}

#if ASYNC
async void SaveStreamResponseToFile(StreamResponse streamResponse)
{
    if (streamResponse.status != Status.Success)
        return;
    FileStream fileStream = new FileStream(streamResponse.FileName, FileMode.Create, FileAccess.Write,
        FileShare.ReadWrite | FileShare.Delete);
    using MemoryStream ms = new MemoryStream();
    await streamResponse.Stream!.CopyToAsync(ms);
    byte[] data = ms.ToArray();
    await fileStream.WriteAsync(data, 0, data.Length);
    fileStream.Dispose();
}
#else
void SaveStreamResponseToFile(StreamResponse streamResponse)
{
    if (streamResponse.status != Status.Success)
        return;
    FileStream fileStream = new FileStream(streamResponse.FileName, FileMode.Create, FileAccess.Write,
        FileShare.ReadWrite | FileShare.Delete);
    using MemoryStream ms = new MemoryStream();
    streamResponse.Stream!.CopyTo(ms);
    byte[] data = ms.ToArray();
    fileStream.Write(data, 0, data.Length);
    fileStream.Dispose();
}
#endif