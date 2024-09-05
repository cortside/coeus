# Azure Service Bus

## Configuration

Configuration for `Topic`, `Subscriptions`, and `Queues` is managed in [config.json](config.json).

At a high level:

- `Topics` belong to a service (e.g. `identityserver`).
- `Topics` are thus named `<service>.<event name>`.
- `Subscriptions` are used to associate a `Topic` with a `Queue`.
- `Subscriptions` are named `<service>.subscription`
- `Queues` are named `<service>.queue`

The configuration values correspond to the settings you'd be able to apply through the azure portal. See [the quickstart](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-quickstart-topics-subscriptions-portal) for a quick primer, or read the reference documents linked in the [official documentation](#official-documentation) section below.

## Script

You can run the powershell script locally:

```sh
pwsh deploy-service-bus.ps1 -config config.json -namespace cortside-dev -resourcegroup pubsub -Force
```

This can also be hooked into whichever build & deploy process being used.

### What it does

The script will:

1. Attempt to authenticate to Azure
2. Create the given `-resourcegroup` if it doesn't exist
3. Create the given `-namespace` if it doesn't exist
4. Create the `Topics`, `Subscriptions`, and `Queues` defined in `config.json`

Additionally, if the `-Force` flag is passed, the script will delete any existing `Topics`, `Subscriptions`, and `Queues` that are not defined in `config.json`.

## Naming Rules

### Service Bus Namespace

The namespace name should adhere to the following naming conventions:

- The name must be unique **_across Azure_**. The system immediately checks to see if the name is available.
- The name length is at least 6 and at most 50 characters.
- The name can contain only letters, numbers, hyphens “-“.
- The name must start with a letter and end with a letter or number.
- The name doesn't end with “-sb“ or “-mgmt“.

### Service Bus Topic:

Topic names can contain letters, numbers, periods (.), hyphens (-), underscores (\_), and slashes (/), up to 260 characters. Topic names are also case-insensitive.

### Service Bus Queue:

Queue names can contain letters, numbers, periods (.), hyphens (-), underscores (\_), and slashes (/), up to 260 characters. Queue names are also case-insensitive.

### Service Bus Topic Subscriptions:

Subscription names can contain letters, numbers, periods (.), hyphens (-), and underscores (\_), up to 50 characters. Subscription names are also case-insensitive.

# Official Documentation:

- [Queues, Topics, and Subscriptions](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-queues-topics-subscriptions)
- [Service Bus Quickstart](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-quickstart-topics-subscriptions-portal)
- [Topic Reference](https://learn.microsoft.com/en-us/cli/azure/servicebus/topic?view=azure-cli-latest)
- [Topic Subscription Reference](https://learn.microsoft.com/en-us/cli/azure/servicebus/topic/subscription?view=azure-cli-latest)
- [Queue Reference](https://learn.microsoft.com/en-us/cli/azure/servicebus/queue?view=azure-cli-latest)

# RabbitMq in Docker for local development use

run `create-rabbit-config.ps1` to update `./rabbitmq/rabbit_configuration.json`, which uses the official `config.json`.

run `./start-rabbitmq.ps1` to start a container and configure it to have all the equivalent queues.

local micro service repos _should_ have rabbitmq settings in the ServiceBus section of `appsettings.json` by default

admin UI can be accessed at http://localhost:15672/ with admin/password as credentials
