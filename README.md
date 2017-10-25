# AzureServiceBusMeetsRabbitMq

This solution is just a quick and dirty scratchpad which is comprised of two console applications one that creates simple messages and publishes them on to the queue and another that reads the messages placed on the queue.

##### NOTE
---
The solution can be configured to use either *Azure Service Bus* or *RabbitMq* as the queue provider by setting the following appSetting to true or false in both app.config files:

```xml
<add key="RunAzureSolution" value="true"/>
```

When set to true you will need to ensure that the connectionString setting (again in both app.config files) has the correct Azure Service Bus connection string values

```xml
<add name="Microsoft.ServiceBus.ConnectionString" connectionString="Endpoint=sb://[yournamespace].servicebus.windows.net;SharedAccessKeyName=[yoursharedaccessKeyName];SharedAccessKey=[yoursecret]"/>
```

If you run the solution targeting the *RabbitMq* configuration it is assumed that the *RabbitMq* instance is configured as follows:
+ **Hostname** = *localhost*
+ **Port** = *5672*
+ **UserName** = *guest*
+ **Password** = *guest*
+ **VirtualHost** = */*

If you wish to run *RabbitMq* in a Docker container then you can run the following Docker command to spin up an appropriate container, which will clean up after itself when terminated:

```sh
docker run --rm -it --hostname rabbit-hostname --name rabbit-name -p 5672:5672 -p 8080:15672 rabbitmq:3-management
```

You can monitor the queue satistics via the RabbitMq web console by browsing to http://localhost:8080 and logging in with the default username and password: ***guest***

