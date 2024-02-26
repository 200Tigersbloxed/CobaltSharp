# CobaltSharp

A C# wrapper for the [cobalt](https://github.com/wukko/cobalt) API

# Supported Runtimes

+ net7
+ net48
+ netstandard 2.1

# Using CobaltSharp

## Creating the CobaltServer

First, create a [CobaltServer](https://github.com/200Tigersbloxed/CobaltSharp/blob/main/CobaltSharp/CobaltServer.cs) object. This object is what will define which API server to use.

```cs
// Uses DEFAULT_API (https://co.wuk.sh/)
CobaltServer defaultServer = new CobaltServer();
// Defines specific server to use
CobaltServer specificServer = new CobaltServer("https://co.wuk.sh/");
// Defines specific server from a Uri (does the same thing as the constructor above)
CobaltServer uriServer = new CobaltServer(new Uri("https://co.wuk.sh/"));
```

If the server is experiencing high volume, adjust the TimeSpan. By default, the Timeout is `1 hour`.

> ## ℹ️ IMPORTANT ℹ️
> 
> Timeout can **NOT** be adjust after the Cobalt instance is created.

```cs
CobaltServer server = new CobaltServer();
server.Timeout = TimeSpan.FromHours(5);
```

## Creating the Cobalt Instance

Next, create a [Cobalt](https://github.com/200Tigersbloxed/CobaltSharp/blob/main/CobaltSharp/Cobalt.cs) object. This object will contain all of our methods for getting data from the API.

```cs
// Create a default Cobalt
Cobalt cobalt = new Cobalt();

// Create a Cobalt from a CobaltServer
CobaltServer server = new CobaltServer();
Cobalt cobalt = new Cobalt(server);
```

> ## ⚠️ DISPOSE WHEN FINISHED ⚠️
> 
> Cobalt uses resources that *should* be disposed when you are done with it!
> 
> ```cs
> Cobalt cobalt = new Cobalt();
> // Do something...
> cobalt.Dispose();
> ```

## Making Requests

Requests in Cobalt can be made synchronously or asynchronously. Pick whichever fits your application best. In these demos, it will be assumed synchronous. All methods of Cobalt have an asynchronous counterpart with `Async` attached to the end of the method name.

## Getting Media

[GetMedia](https://github.com/200Tigersbloxed/CobaltSharp/blob/main/CobaltSharp/GetMedia.cs) gets data on where to download media from the CobaltServer. This example demonstrates how to download the video at `https://www.youtube.com/watch?v=gkTb9GP9lVI` at `360p`

```cs
Cobalt cobalt = new Cobalt();
GetMedia getMedia = new GetMedia
{
    url = "https://www.youtube.com/watch?v=gkTb9GP9lVI",
    vQuality = VideoQuality.q360
};
MediaResponse mediaResponse = cobalt.GetMedia(getMedia);
if(mediaResponse.status == Status.Stream)
{
    // Stream means that only one video to download was found
    // Handle mediaResponse.url...
}
else if(mediaResponse.status == Status.Picker)
{
    // Picker means that multiple videos were found
    foreach(PickerItem item in mediaResponse.picker)
    {
        // Handle item.url...
    }
}
```

Getting Media will return a [MediaResponse](https://github.com/200Tigersbloxed/CobaltSharp/blob/main/CobaltSharp/MediaResponse.cs), which contains all the data where to download/stream the requested `url`.

## Streaming Media

The MediaResponse or PickerItem generated is where you can stream/download the media. This example demonstrates how to download and save a MediaResult to a file.

```cs
/*
 * Assume Cobalt was created
 * Assume a GetMedia request was sent
 * Assume a MediaResult exists and has a status of Status.Stream
*/

StreamResponse streamResponse = cobalt.GetStream(mediaResponse);
if(streamResponse.status == null || streamResponse.status != Status.Success)
{
    // Failed to get Stream
    // Handle this error...
}
else
{
    if (streamResponse.status != Status.Success)
        return;
    // Change streamResponse to any path you'd like to save to
    FileStream fileStream = new FileStream(streamResponse.FileName, FileMode.Create, FileAccess.Write,
        FileShare.ReadWrite | FileShare.Delete);
    using MemoryStream ms = new MemoryStream();
    // We can assume if status is Status.Success that Stream is *probably* not null
    streamResponse.Stream!.CopyTo(ms);
    byte[] data = ms.ToArray();
    fileStream.Write(data, 0, data.Length);
    fileStream.Dispose();
}
// Don't forget to Dispose when you're done!
streamResponse.Dispose();
```

Getting a Stream will return a [StreamResponse](https://github.com/200Tigersbloxed/CobaltSharp/blob/main/CobaltSharp/StreamResponse.cs), which contains mostly nullables that aren't null in certain scenarios.

+ **If status was not Successful**
  + `Stream` is null
  + `FileName` is Empty
+ **If status was Successful**
  + `Stream` is null
  + `FileName` is Empty
+ **If API returned the file**
  + `text` is Empty

> ## ⚠️ DISPOSE WHEN FINISHED ⚠️
> 
> StreamResponse may use resources that *should* be disposed when you are done with it!
> 
> ```cs
> GetStream getStream = new GetStream(mediaResponse);
> StreamResponse streamResponse = cobalt.GetStream(getStream);
> // Do stuff...
> streamResponse.Dispose();
> ```

## Getting On-Demand Blocks

[OnDemand](https://github.com/200Tigersbloxed/CobaltSharp/blob/main/CobaltSharp/OnDemand.cs) blocks are used for for website element loading. This is mostly useless unless you have some sort of HTTP parser or WebRenderer. This example demonstrates how to get the first block.

```cs
Cobalt cobalt = new Cobalt();
OnDemand onDemand = new OnDemand {blockId = 0};
OnDemandResponse onDemandResponse = cobalt.OnDemand(onDemand);
if(onDemandResponse.status == Status.Success)
{
    // Handle onDemandResponse.text...
}
```

Getting an OnDemand will return an [OnDemandResponse](https://github.com/200Tigersbloxed/CobaltSharp/blob/main/CobaltSharp/OnDemandResponse.cs) which contains the elements inside of the text field.

## Getting ServerInfo

Getting the ServerInfo is simple and it is a parameter-less function. This example demonstrates how to get the ServerInfo.

```cs
Cobalt cobalt = new Cobalt();
ServerInfo serverInfo = cobalt.GetServerInfo();
// Handle serverInfo...
```

Getting ServerInfo will return a [ServerInfo](https://github.com/200Tigersbloxed/CobaltSharp/blob/main/CobaltSharp/ServerInfo.cs) object containing the version, commit, branch, name, url, cors, and startTime.