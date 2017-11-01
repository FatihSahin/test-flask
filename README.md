# TestFlask 

![Erlenmeyer](Icons/package_icon.png)

[![Build status](https://ci.appveyor.com/api/projects/status/iwmii0fmtpyopkgu?svg=true)](https://ci.appveyor.com/project/FatihSahin/test-flask)
[![Join the chat at https://gitter.im/test-flask/Lobby](https://badges.gitter.im/test-flask/Lobby.svg)](https://gitter.im/test-flask/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

TestFlask is a set of components that manipulate (a.k.a. weaving) your any backend .net method call inside a WCF service or a REST API. In addition to weaving, TestFlask records your methods' requests and responses, store them inside a document database, and replay them if requested. 

There is a nuget package called TestFlaskAddin.Fody inside the solution. If you reference that package in your backend service, you can mark your methods with [Playback] attribute as below.

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
    player.StartInvocation(name);
    switch (player.DetermineTestMode(name))
    {
        case TestModes.NoMock:
            return player.CallOriginal(name, new Func<string, Movie>(this.GetMovieWithStockCount__Original));
        case TestModes.Record:
            return player.Record(name, new Func<string, Movie>(this.GetMovieWithStockCount__Original));
        case TestModes.Play:
            return player.Play(name);
        default:
            return (Movie) null;
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

This auto-wrapping enables TestFlask to intercept your method calls and to act as requested. It can record your request, replay your request or just calls the original method. TestFlask determines what to do by looking up to custom https headers that the client has sent to the service. There are actually four test modes. 

TestFlask-Mode  | Description
------------- | -------------
Record | Calls the original method and then persists request and response objects through TestFlask.API into a mongoDB database.
Play | Calls TestFlask.API to look for a recorded response for the current request and returns that response.
NoMock | Calls the original method with no mocking.
Assert | Same as Play, however in this case TestFlask stores last response as an assertion result to assert later on.

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
TestFlask-ScenarioNo| This is the number for your test scenario
TestFlask-StepNo    | This is optional, if you provide a step no, it will override that step. If you do not, TestFlask will create an auto step under that scenario.

Please examine [Movie Rental Sample App](https://github.com/FatihSahin/test-flask-sample) to further investigate how to use TestFlask with scenarios, steps and also manage your scenarios with [TestFlask Manager](https://github.com/FatihSahin/test-flask-web). 

You can also examine a SoapUI project inside that sample app to trigger your backend without UI.

Lastly, TestFlask.Assistant.Mvc project (nuget package) is an ASP.NET MVC extension to ease integrating your ASP.NET MVC client to send proper headers to your TestFlask ready backend service.

TestFlask still needs a lot of development and hopefully it will go on. It is on a beta phase. Please feel free to contribute with PRs.

### Icon

[Erlenmeyer Flask](https://thenounproject.com/search/?q=flasks&i=707717) by Rockicon from the Noun Project
