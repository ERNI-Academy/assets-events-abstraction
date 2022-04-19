# About Events Abstraction
 Define a contract and many implementations (Azure EventGrid, Azure ServiceBus, Azure Storage Queues, Azure Redis) for publish events

ERNI Academy StarterKit, PoC, or Gidelines. This is an about description of your repository.

<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-1-orange.svg?style=flat-square)](#contributors)
<!-- ALL-CONTRIBUTORS-BADGE:END -->

## Built With

This section should list any major frameworks that you built your project using. Leave any add-ons/plugins for the acknowledgements section. Here are a few examples.

- [.Net 6.0](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-6)
- [c# 11](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-11)
- [assets-serializers-abstraction (refereced as git submodule)](https://github.com/ERNI-Academy/assets-serializers-abstraction)

## Features

- PublishAsync

## Getting Started

This is an example of how you may give instructions on setting up your project locally. To get a local copy up and running follow these simple example steps.

## Prerequisites

.net 6
Visual Studio or Visual Studio Code

## Installation

Installation instructions Events Abstraction by running:

1. Clone the repo

```sh
git clone --recurse-submodules https://github.com/ERNI-Academy/assets-events-abstraction.git
```

2. Basic use

```c#
class MyEvent : EventBase
{
	public string MyCustomProperty { get; set; }
}

var @event = new MyEvent { MyCustomProperty = "hi" };

//you can choose between many impl
IEventPublisher publisher = new ErniAcademy.Events.EventGrid.EventGridPublisher();
IEventPublisher publisher = new ErniAcademy.Events.Redis.RedisPublisher();
IEventPublisher publisher = new ErniAcademy.Events.ServiceBus.ServiceBusPublisher();
IEventPublisher publisher = new ErniAcademy.Events.StorageQueues.StorageQueuePublisher();

//just publish your event 
await publisher.PublishAsync(@event);
```

3. Depency injection (ServiceCollection) use

```c#
class MyEvent : EventBase
{
	public string MyCustomProperty { get; set; }
}

//when configuring your ServiceCollection use the extension methods defined in each library for easy of use. This sample is provided with no arguments, take a look on the extensions to see the rest of the arguments, like IConfiguration, ISerializer etc.
services.AddEventsEventGrid();
services.AddEventsRedis();
services.AddEventsServiceBus();
services.AddEventsStorageQueues();

//then just inject IEventPublisher directly in your classes

class MyService
{
  private readonly IEventPublisher _publisher;

  public MyService(IEventPublisher publisher)
  {
    _publisher = publisher;
  }

  public async Task SomeMethod()
  {
      //... some logic
      
      var @event = new MyEvent { MyCustomProperty = "hi" };

      //just publish your event 
      await _publisher.PublishAsync(@event);
  }
}
```

## Contributing

Please see our [Contribution Guide](CONTRIBUTING.md) to learn how to contribute.

## License

![MIT](https://img.shields.io/badge/License-MIT-blue.svg)

(LICENSE) © {{Year}} [ERNI - Swiss Software Engineering](https://www.betterask.erni)

## Code of conduct

Please see our [Code of Conduct](CODE_OF_CONDUCT.md)

## Stats

Check [https://repobeats.axiom.co/](https://repobeats.axiom.co/) for the right URL

## Follow us

[![Twitter Follow](https://img.shields.io/twitter/follow/ERNI?style=social)](https://www.twitter.com/ERNI)
[![Twitch Status](https://img.shields.io/twitch/status/erni_academy?label=Twitch%20Erni%20Academy&style=social)](https://www.twitch.tv/erni_academy)
[![YouTube Channel Views](https://img.shields.io/youtube/channel/views/UCkdDcxjml85-Ydn7Dc577WQ?label=Youtube%20Erni%20Academy&style=social)](https://www.youtube.com/channel/UCkdDcxjml85-Ydn7Dc577WQ)
[![Linkedin](https://img.shields.io/badge/linkedin-31k-green?style=social&logo=Linkedin)](https://www.linkedin.com/company/erni)

## Contact

📧 [esp-services@betterask.erni](mailto:esp-services@betterask.erni)

## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="https://github.com/omaramalfi"><img src="https://avatars.githubusercontent.com/u/85349124?v=4?s=100" width="100px;" alt=""/><br /><sub><b>omaramalfi</b></sub></a><br /><a href="https://github.com/ERNI-Academy/assets-events-abstraction/commits?author=omaramalfi" title="Code">💻</a> <a href="#content-omaramalfi" title="Content">🖋</a> <a href="https://github.com/ERNI-Academy/assets-events-abstraction/commits?author=omaramalfi" title="Documentation">📖</a> <a href="#design-omaramalfi" title="Design">🎨</a> <a href="#ideas-omaramalfi" title="Ideas, Planning, & Feedback">🤔</a> <a href="#maintenance-omaramalfi" title="Maintenance">🚧</a> <a href="https://github.com/ERNI-Academy/assets-events-abstraction/commits?author=omaramalfi" title="Tests">⚠️</a> <a href="#example-omaramalfi" title="Examples">💡</a> <a href="https://github.com/ERNI-Academy/assets-events-abstraction/pulls?q=is%3Apr+reviewed-by%3Aomaramalfi" title="Reviewed Pull Requests">👀</a></td>
  </tr>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->
This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!
