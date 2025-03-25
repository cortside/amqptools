[![Build status](https://ci.appveyor.com/api/projects/status/f3tnsngabv2dx0t3?svg=true)](https://ci.appveyor.com/project/cortside/cortside-restapiclient)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=cortside_amqptools&metric=alert_status)](https://sonarcloud.io/dashboard?id=cortside_amqptools)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=cortside_amqptools&metric=coverage)](https://sonarcloud.io/dashboard?id=cortside_amqptools)

# amqptools

CLI tools for interacting with service bus queues.

## Help

Will show available commands

```powershell
./AmqpTools.exe --help
```

## amqptools.json

```json
{
  "environments": [
    {
      "name": "prod",
      "key": "secret=",
      "namespace": "acme-prod.servicebus.windows.net",
      "policyname": "RootManageSharedAccessKey"
    },
    {
      "name": "dev",
      "key": "secret=",
      "namespace": "acme-dev.servicebus.windows.net",
      "policyname": "RootManageSharedAccessKey"
    }
  ]
}
```

### Command help

Will show available and required options

```powershell
./AmqpTools.exe shovel --help
```

## delete

.\src\AmqpTools\bin\Debug\net8.0\AmqpTools.exe delete --environment dev -q shoppingcart.queue --messageType deadletter --config c:\work\cortside\amqptools\amqptools.json --messageId ca097856-295c-49d4-a0c1-86e4806c17e7

## peek

peek --environment dev -q shoppingcart.queue --messageType deadletter --count 10 --config c:\work\cortside\amqptools\amqptools.json

## Shovel

```powershell
$policyname = "SendListen"
$namespace = "acme.servicebus.windows.net"
$key = "secret=="
$queue = "shoppingcart.queue"

./AmqpTools.exe shovel --queue $queue --namespace $namespace --policyname=$policyname --key=$key
shovel --environment dev -q shoppingcart.queue --config c:\work\cortside\amqptools\amqptools.json --max 10 --verbose
```

## Publish

```powershell
$policyname = "SendListen"
$namespace = "acme.servicebus.windows.net"
$key = "secret=="
$queue = "shoppingcart.queue"
$event = "Acme.ShoppingCartUpdatedEvent"

./AmqpTools.exe publish --queue $queue --namespace $namespace --policyname=$policyname --key=$key --eventtype $event --data '{\"ShoppingCartResourceId\":\"e25d2090-d890-4b8a-a904-5feebf4b6436\"}'
.\src\AmqpTools\bin\Debug\net8.0\AmqpTools.exe publish --environment dev -q shoppingcart.queue --config c:\work\cortside\amqptools\amqptools.json --eventtype "Acme.DomainEvent.Events.ShoppingCartCreationEvent" --data '{\"ShoppingCartResourceId\":\"e25d2090-d890-4b8a-a904-5feebf4b6436\"}'
OR

./AmqpTools.exe publish --queue $queue --namespace $namespace --policyname=$policyname --key=$key --eventtype $event --file "event.json"
.\src\AmqpTools\bin\Debug\net8.0\AmqpTools.exe publish --environment dev -q shoppingcart.queue --config c:\work\cortside\amqptools\amqptools.json --eventtype "Acme.DomainEvent.Events.ShoppingCartCreationEvent" --file "event.json"
```

## Queue details

```powershell
$policyname = "SendListen"
$namespace = "acme.servicebus.windows.net"
$key = "secret=="
$queue = "shoppingcart.queue"

./AmqpTools.exe queue --queue $queue --namespace $namespace --policyname=$policyname --key=$key
queue --environment dev -q shoppingcart.queue --config c:\work\cortside\amqptools\amqptools.json
```
