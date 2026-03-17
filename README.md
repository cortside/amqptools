[![Build status](https://ci.appveyor.com/api/projects/status/f3tnsngabv2dx0t3?svg=true)](https://ci.appveyor.com/project/cortside/cortside-restapiclient)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=cortside_amqptools&metric=alert_status)](https://sonarcloud.io/dashboard?id=cortside_amqptools)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=cortside_amqptools&metric=coverage)](https://sonarcloud.io/dashboard?id=cortside_amqptools)

# amqptools

CLI tools for interacting with service bus queues.
NOTE: Requires .NET SDK 10.0 or greater

## Install

```powershell
dotnet tool install --global AmqpTools
dotnet tool install --global AmqpTools --version <version>
```

### amqptools.json

```json
{
  "environments": [
    {
      "name": "prod",
      "key": "secret=",
      "namespace": "acme-prod.servicebus.windows.net",
      "policyname": "SendListen"
    },
    {
      "name": "dev",
      "key": "secret=",
      "namespace": "acme-dev.servicebus.windows.net",
      "policyname": "SendListen"
    }
  ]
}
```

## Commands

Will show available and required options

```powershell
dotnet amqptools --help
dotnet amqptools <command> --help
```

### delete

```powershell
dotnet amqptools delete --config c:\path\to\amqptools.json --environment dev -q shoppingcart.queue --messageType deadletter --messageId ca097856-295c-49d4-a0c1-86e4806c17e7
```

### peek

```powershell
dotnet amqptools peek --config c:\path\to\amqptools.json --environment dev -q shoppingcart.queue --messageType deadletter --count 10
```

### shovel

```powershell
$policyname = "SendListen"
$namespace = "acme.servicebus.windows.net"
$key = "secret=="
$queue = "shoppingcart.queue"

dotnet amqptools shovel --queue $queue --namespace $namespace --policyname=$policyname --key=$key
dotnet amqptools shovel shovel --config c:\path\to\amqptools.json --environment dev -q shoppingcart.queue --max 10 --verbose
```

### publish

```powershell
$policyname = "SendListen"
$namespace = "acme.servicebus.windows.net"
$key = "secret=="
$queue = "shoppingcart.queue"
$event = "Acme.ShoppingCartUpdatedEvent"

dotnet amqptools publish --queue $queue --namespace $namespace --policyname=$policyname --key=$key --eventtype $event --data '{\"ShoppingCartResourceId\":\"e25d2090-d890-4b8a-a904-5feebf4b6436\"}'
dotnet amqptools publish --config c:\path\to\amqptools.json --environment dev -q shoppingcart.queue --eventtype "Acme.DomainEvent.Events.ShoppingCartCreationEvent" --data '{\"ShoppingCartResourceId\":\"e25d2090-d890-4b8a-a904-5feebf4b6436\"}'

OR

dotnet amqptools publish --queue $queue --namespace $namespace --policyname=$policyname --key=$key --eventtype $event --file "event.json"
dotnet amqptools publish --config c:\path\to\amqptools.json --environment dev -q shoppingcart.queue --eventtype "Acme.DomainEvent.Events.ShoppingCartCreationEvent" --file "event.json"
```

### queue

```powershell
$policyname = "SendListen"
$namespace = "acme.servicebus.windows.net"
$key = "secret=="
$queue = "shoppingcart.queue"

dotnet amqptools queue --queue $queue --namespace $namespace --policyname=$policyname --key=$key
dotnet amqptools queue --config c:\path\to\amqptools.json --environment dev -q shoppingcart.queue
```

Output:
```json
{
  "Path": "onlineapplication.queue",
  "MessageCount": 6,
  "MessageCountDetails": {
    "ActiveMessageCount": 0,
    "DeadLetterMessageCount": 6,
    "ScheduledMessageCount": 0,
    "TransferMessageCount": 0,
    "TransferDeadLetterMessageCount": 0
  },
  "SizeInBytes": 807,
  "CreatedAt": "2023-02-15T21:27:27.202248",
  "UpdatedAt": "2024-08-09T04:02:57.2991904",
  "AccessedAt": "2025-03-26T02:14:33.6177786"
}
```