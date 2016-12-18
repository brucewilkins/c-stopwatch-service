#Chama Assignment

The source code can be found in [GitHub] (https://github.com/brucewilkins/c-stopwatch-service)
The deployed service can be found [here] (http://c-stopwatch-service.azurewebsites.net/swagger/ui)


## Architecture
The solution is built using:

- asp.net core 
- signal r core *(preview)*
- swagger *(preview)*
- automapper

## Completed tasks
- [x] Create Azure App Service
- [x] Implement `POST /api/stopwatch`
- [x] Implement `GET /api/stopwatch`
- [x] Limit access to API using BASIC authentication
- [x] Store data in Azure Table Storage
- [x] Implement SignalR set/ reset stopwatch
- [x] Implement SignalR get stopwatches (for user)
- [x] Implement SignalR push stopwatch updates (for all connected users)
- [x] Limit access to SignalR Hub using BASIC authentication

### Improvements

The basic structure provided by the `TimerEntryService` would typically be implemented using a more scalable pattern, e.g.: CQRS. This would also allow for cleaner code re-use between the logic in `StopwatchController` and `StopwatchHub`.

Due to the browser's websocket client not allowing custom headers, it is apparently not possible to add custom authorization headers to the SignalR javascript client. Due to this limitation there is no test client provided.

In a typical production solution, I would expect the relevant tests and automated build/ deployment setup.

In the preview version of swagger for asp.net core (ahoy) it does not seem possible to currently provide a way of adding an authorization header, so although the api is discoverable the methods cannot be invoked using the swagger ui.



