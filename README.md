# TestFlask

TestFlask is a set of components that manipulates (called weaving) your any backend .net method calls inside WCF service or REST API. In addition to weaving, TestFlask records your method request and responses, store them inside a document database, and replay them if requested. 

There is a nuget package called TestFlaskAddin.Fody inside the solution. If you reference that package in your backend service, you can mark your methods with [Playback] attribute as below

```csharp

[Playback(typeof(MovieNameIdentifier))]
public Movie GetMovieWithStockCount(string name)
{
    //simulate a delay
    Thread.Sleep(new Random().Next(500, 2000));
    //gets movie info from info service
    var movie = infoService.GetMovieInfo(name);
    //obtain stock info from stock service
    movie.StockCount = stockService.GetStock(name);
    
    return movie;
}

```

And after you build your project, TestFlask will weave your code and turn it into something like this. You can see it if you decompile your assembly with a decompiler tool. 

Thanks to wondeful [Fody](https://github.com/Fody/Fody) library for simplifying .net assembly weaving.
```csharp

[Playback(typeof (MovieNameIdentifier), null)]
public Movie GetMovieWithStockCount(string name)
{
    Player<string, Movie> player = new Player<string, Movie>("MovieRental.Models.Movie MovieRental.Business.RentalManager::GetMovieWithStockCount(System.String)", (IRequestIdentifier<string>) new MovieNameIdentifier(), (IResponseIdentifier<Movie>) null);
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
    Thread.Sleep(new Random().Next(500, 2000));
    Movie movieInfo = this.infoService.GetMovieInfo(name);
    movieInfo.StockCount = this.stockService.GetStock(name);
    return movieInfo;
}
```

This auto-wrapping enables TestFlask to intercept your method call and act as requested. It can record your request, replay your request or just calls original method. TestFlask determines what to do by looking up to custom https headers that the client sent to the service. There are actually four test modes 

    * Record: Calls original method and then persists request and response object through TestFlask.API into a mongoDB database
    * Play: Calls TestFlask.API to look for a recorded response for the current request and returns that response
    * NoMock: Calls original method with no mocking
    * Assert: Same as Play, however in this case TestFlask stores last response as an assertion result to assert later on.

As you can see TestFlask talks to a REST API to do recording and playing. That component is called TestFlask.API

## How to instantiate a TestFlask.API Host

In order to intercept calls from your weaved methods, you should initiate a TestFlask.API host. 

This project is actually an ASP.NET MVC API project. As a persistence mechanism, it uses mongoDB. Therefore, configure your TestFlask.API project's web.config with a running mongoDB instance

```xml
<appSettings>
    <add key="testFlaskMongoDbServer" value="mongodb://localhost" />
    <add key="testFlaskMongoDbName" value="test" />
</appSettings>
```
## How will I send proper TestFlask HttpHeaders to determine testing mode

TestFlask.Assistant project (nuget package) is an ASP.NET MVC extension to ease integrating your ASP.NET MVC client to send proper headers to your TestFlask ready backend service.

Please examine [Movie Rental Sample App](https://github.com/FatihSahin/test-flask-sample) to further investigate how to use TestFlask with scenarios, steps and also manage your tests with [TestFlask Manager](https://github.com/FatihSahin/test-flask-web). You can also examine a SoapUI project inside that sample app to trigger your backend without UI.

