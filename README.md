# TestFlask 

![Erlenmeyer](Icons/package_icon.png)

[![Build status](https://ci.appveyor.com/api/projects/status/iwmii0fmtpyopkgu?svg=true)](https://ci.appveyor.com/project/FatihSahin/test-flask)
[![Join the chat at https://gitter.im/test-flask/Lobby](https://badges.gitter.im/test-flask/Lobby.svg)](https://gitter.im/test-flask/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

TestFlask is a tool to create tests for your .net WCF Service or REST API. It is based on a very simple idea. 
Record your any .net method invocation in runtime with arguments and the return type. After recording, replay the same method invocation without actually calling the real implementation when testing or debugging.

It is super useful for creating auto-mocks by actually calling your WCF service or your REST API in an integration testing or real production environment (if you supply proper TestFlask Http headers). The end result is that you get a lot of recorded method calls that are invoked in a hierarchy. 

All you need to do is add a Playback attribute on top of the method that you want to record/replay. Just look at this

```csharp
[Playback(typeof(MovieNameIdentifier))]
public Movie GetMovieWithStockCount(string name)
{
    //gets movie info from info service
    var movie = infoService.GetMovieInfo(name);
    //obtain stock info from stock service
    movie.StockCount = stockService.GetStock(name);
    return movie;
}
```
After you build your project, TestFlask will weave your code and turn it into something like below . You can see it if you decompile your assembly with a decompiler tool. 

```csharp
[Playback(typeof (MovieNameIdentifier), null)]
public Movie GetMovieWithStockCount(string name)
{
    FuncPlayer<string, Movie> player = new FuncPlayer<string, Movie>("MovieRental.Models.Movie MovieRental.Business.RentalManager::GetMovieWithStockCount(System.String)", (IRequestIdentifier<string>) new MovieNameIdentifier(), (IResponseIdentifier<Movie>) null);
    player.BeginInvocation(name);
    switch (player.DetermineTestMode(name))
    {
        case TestModes.NoMock:
            return player.CallOriginal(name, new Func<string, Movie>(this.GetMovieWithStockCount__Original));
        case TestModes.Record:
            return player.Record(name, new Func<string, Movie>(this.GetMovieWithStockCount__Original));
        case TestModes.Play:
            return player.Play(name);
        default:
            throw new Exception("Invalid TestFlask test mode detected!");
    }
}

public Movie GetMovieWithStockCount__Original(string name)
{
    Movie movieInfo = this.infoService.GetMovieInfo(name);
    movieInfo.StockCount = this.stockService.GetStock(name);
    return movieInfo;
}
```
TestFlask uses [Fody](https://github.com/Fody/Fody) library to plug-in to MS Build process for weaving. It also depends on [Mono.Cecil](https://github.com/jbevain/cecil) library to manipulate IL.

Then, you can now either replay like a mock while you are trigerring a regression test or just developing a new feature while replaying a previously recorded scenario and having the comfort that you already easily created (recorded) mocks for all your marked external dependencies while debugging.

The advantage of using TestFlask over service virtualization is that you actually do not have to deal with protocols or external testing environments, tools. It all happens inside your code and your IDE. You are also not limited to mocking your external SOAP, XML or JSON services. You can now mock a database call, or a totally different external resource (ftp, tcp, sna you name it). It actually does not matter, because your method is now intercepted and mocked. It will not do IO over the wire now.

TestFlask not just records your method invocations but also provides a way to create organized business scenarios just like you are creating a isolated testing session. Look at how a recorded scenario looks like

![assistant1](https://github.com/FatihSahin/test-flask-sample/blob/master/Docs/manager_2.png)

Notice that little Replay checkbox? It gives you flexibility to mock or not to mock a .net method depending on your choice next time. Also you can alter the response of a method and replay as your method will now return that object the we way you modified. Test data is encapsulated in that scenario with a scenario number and will not affect your other recorded scenarios.

Using the combination of these alterations, you can easily and flexibly develop a complex and highly integrated project without worrying about actual integration problems and test data. It is now all isolated for you and encapsulated in a TestFlask scenario. TestFlask is never a tool to replace your real unit tests, but complement them and provide you a hybrid testing power inside your IDE.

Go check out [Movie Rental Sample App](https://github.com/FatihSahin/test-flask-sample) for a complete living demo. 
I will also create to demonstrate a YouTube video asap.

TestFlask also has built-in CLI tooling (checkout TestFlask.CLI) for auto creating unit-tests that trigger your recorded scenario, so that you can instantiate a testing scenario with mocks ready inside your IDE. MS Tests are supported right now, but I will add NUnit test support when requested.

TestFlask determines what to do by looking up to custom https headers that the client has sent to the service. There are actually five test modes. 

TestFlask-Mode  | Description
------------- | -------------
Record | Calls the original method and then persists request and response objects through TestFlask.API into a mongoDB database.
Play | Calls TestFlask.API to look for a recorded response for the current request and returns that response.
NoMock | Calls the original method with no mocking.
Assert | Same as Play, however in this case TestFlask stores last response as an assertion result to assert later on.
IntelliRecord | Just like record, however in this case if a matching invocation already exists it just replays it, but patches missing ones on to existing invocation tree.

As you can see TestFlask talks to a REST API to do recording and playing. This component is called TestFlask.API

## How to instantiate a TestFlask.API Host

In order to persist your intercepted calls from your weaved methods, you should initiate a TestFlask.API host. 

This project is actually an ASP.NET MVC API project. As a persistence mechanism, it uses mongoDB. Therefore, configure your TestFlask.API project's web.config with a running mongoDB instance

```xml
<appSettings>
    <add key="testFlaskMongoDbServer" value="mongodb://localhost" />
    <add key="testFlaskMongoDbName" value="test" />
</appSettings>
```
## How will I send proper TestFlask HttpHeaders to determine testing mode?

There are four http headers that TestFlask looks up to determine how to store your intercepted request & response.

Http Header         | Description
--------------------| -------------
TestFlask-Mode      | See above
TestFlask-ProjectKey| This is the key that you created for your backend service to categorize scenarios
TestFlask-ScenarioNo| This is the number for your test scenario. It is the main test data encapsulation identifier.
TestFlask-StepNo    | This is optional, if you provide a step no, it will override that step. If you do not, TestFlask will create an auto step under that scenario.

You can also examine a SoapUI project inside that sample app to trigger your backend without UI.

## Where does TestFlask actually persist all the stuff?

It persists all scenarios, steps and invocations in a MongoDB database. 


Lastly, TestFlask.Assistant.Mvc project (nuget package) is an ASP.NET MVC extension to ease integrating your ASP.NET MVC client to send proper headers to your TestFlask ready backend service.

TestFlask still needs a lot of development and hopefully it will go on. It is close to v1 release now. I did not upload v1.0.0 packages to nuget.org yet, but as soon as I feel it is ready for v1, I will. Please feel free to contribute with PRs.



### Icon

[Erlenmeyer Flask](https://thenounproject.com/search/?q=flasks&i=707717) by Rockicon from the Noun Project
