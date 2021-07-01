# Captr - Event Sourcing Library

Captr is a simple .NET Event Sourcing library.

It was written partly as a personal project to learn about the Event Sourcing pattern, partly to fill a need for a work related issue and partly to improve in the art of coding.

I fully recognise there are some excellent Event Sourcing libraries already in existance (e.g. [Eventflow](http://geteventflow.net/)), but most of them either had so many features I didn't need or they didn't have enough.  I didn't need all the bells and whistles (e.g. Sagas, CQRS, etc) but I needed something that would allow me to choose any data storage I wanted (e.g. MS SQL, Cosmos, MySQL, etc).

## Getting Started

If you would like to play around with this library, you only need to set up two things ... dependency injection and your aggregate models.

For dependency injection ...

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddLogging();

    services.AddCaptr(options =>
    {
        options.AddEventStorage(cob => cob.UseAzureTableStorageAsEventStore("<connection string>", "<event table name>"));
        options.AddSnapshotStorage(cob => cob.UseAzureTableStorageAsSnapshotStorage("<connection string>", "<snapshot table name>"));
        options.SnapshotInterval = 10; // default is 100
    });
}
```

For aggregate ...

```
using Captr.Aggregates;
using Captr.EventStorage;

namespace SimpleBank.Domain.Aggregates
{
    public class Account : AggregateRoot<Account>
    {
        ...
    }
}
```

## Future Work

I would like to improve on this library by implementing the following:

- [ ] Improve the dependency injection process and make it a little more elegant
- [ ] Add more storage options, like MS SQL, or maybe even make it extensible so custom providers could be written (similar concept to how [Dapr](https://dapr.io) handles components)
- [ ] Add more, and improve, testing
- [ ] Add, and improve, this documentation!! :smile:

## Feedback Wanted!

As I mentioned earlier, I did this project partly to learn about the Event Sourcing pattern as well as to improve in the art of coding.  So if there is something that you can see that could be improved, done differently, or is just plain wrong, let me know.  Hopefully we can all learn and get better at our craft.