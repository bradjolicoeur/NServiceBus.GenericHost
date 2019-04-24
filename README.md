# NServiceBusGenericHost
This project contains a set of dotnet new Templates for creating an NSB endpoints that run as console applications and use the .NET Core Generic host.  Endpoints leveraging these templates are expected to run in Docker containers, Pivotal Cloud Foundry applications or Azure Service Fabric applications.  In each of these cases windows services are not supported and this lightweight method for hosting an NSB endpoint will work well.

Templates:
NServiceBusGenericHost => This is a simple endpoint that uses the LearningTransport and LearningPersistence.  This project will need a production grade transport and persistence implemented after it is used to create an endpoint.

Planned Templates
NServiceBusGenericHostAzure => This will use Generic host with Azure Service Bus and Azure Storage Provider as Transport and Persistence respectively
