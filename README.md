[![](https://img.shields.io/nuget/v/Soenneker.SemanticKernel.Cache.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.SemanticKernel.Cache/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.cache/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.cache/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.SemanticKernel.Cache.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.SemanticKernel.Cache/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.cache/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.cache/actions/workflows/codeql.yml)

# Soenneker.SemanticKernel.Cache

A concurrent, keyed cache that creates and reuses Microsoft Semantic Kernel `Kernel` instances.

## Installation

```bash
dotnet add package Soenneker.SemanticKernel.Cache
```

## Registration

```csharp
using Soenneker.SemanticKernel.Cache.Registrars;

services.AddSemanticKernelCacheAsSingleton();
```

Singleton registration shares kernels across the application. `AddSemanticKernelCacheAsScoped()` instead creates an independent cache per DI scope and clears it when that scope is disposed.

## Creating a kernel

```csharp
using Microsoft.SemanticKernel;
using Soenneker.SemanticKernel.Cache.Abstract;
using Soenneker.SemanticKernel.Dtos.Options;

var options = new SemanticKernelOptions
{
    ModelId = "chat-model",
    Endpoint = "https://model.example.com",
    ApiKey = configuration["Models:ApiKey"],
    KernelFactory = static (options, cancellationToken) =>
    {
        IKernelBuilder builder = Kernel.CreateBuilder();

        // Add the connector required by your application to builder here.

        return ValueTask.FromResult(builder);
    },
    ConfigureBuilder = builder =>
    {
        // Register plugins or services before Build(), if needed.
    },
    ConfigureKernel = async (kernel, cancellationToken) =>
    {
        // Perform asynchronous post-build configuration, if needed.
        await ValueTask.CompletedTask;
    }
};

Kernel kernel = await cache.Get("primary-chat", options, cancellationToken);
```

`KernelFactory` produces the builder. `ConfigureBuilder` runs before `Build()`, and `ConfigureKernel` runs after the kernel is built. If no factory is supplied, the cache builds an empty `Kernel.CreateBuilder()`; connector-specific options such as `ModelId`, `Endpoint`, and `ApiKey` are not applied automatically.

## Key behavior

The ID is the cache identity. Concurrent requests for the same ID share one initialization, and subsequent calls return that kernel. The options passed during the first successful creation win; later options for the same ID do not reconfigure the existing kernel. Use distinct IDs for distinct configurations.

```csharp
Kernel sameKernel = await cache.Get("primary-chat", options, cancellationToken);

bool removed = await cache.Remove("primary-chat", cancellationToken);
Kernel rebuilt = await cache.Get("primary-chat", replacementOptions, cancellationToken);
```

`Init` and `Get` both initialize on first access and return the cached instance. `GetSync` blocks while performing the same operation; prefer the asynchronous API when a factory or post-build configuration can perform asynchronous work.

`Remove`, `Clear`, and cache disposal remove and dispose cached kernels through the underlying singleton dictionary. Do not continue using a kernel after its key has been removed or its cache has been disposed. `GetAll` returns the initialized kernels as a dictionary snapshot for inspection; it does not initialize missing keys.
