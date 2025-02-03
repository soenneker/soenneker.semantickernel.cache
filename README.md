[![](https://img.shields.io/nuget/v/Soenneker.SemanticKernel.Cache.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.SemanticKernel.Cache/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.cache/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.cache/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.SemanticKernel.Cache.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.SemanticKernel.Cache/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.SemanticKernel.Cache

### Providing async thread-safe singleton Semantic Kernel instances

### Why?

When using `Microsoft.SemanticKernel`, it's recommended to maintain long-lived kernel instances rather than re-creating them for each consumer or request. This avoids the overhead of reconfiguring connectors or plugins every time you need to perform a semantic operation. The `SemanticKernelCache` provides a thread-safe singleton cache per key via dependency injection. Kernel instances are created lazily using customizable options and disposed on application shutdown (or manually if needed).

## Installation

Install the package via the .NET CLI:

```bash
dotnet add package Soenneker.SemanticKernel.Cache
```

## Usage

### 1. Register the Cache in Dependency Injection

In your `Program.cs` (or equivalent startup file), register the cache with the DI container:

```csharp
using Soenneker.SemanticKernel.Cache;

public static async Task Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Register SemanticKernelCache as a singleton service.
    builder.Services.AddSemanticKernelCacheAsSingleton();

    // Other configuration...
}
```

### 2. Inject and Retrieve a Kernel Instance

Inject `ISemanticKernelCache` into your classes and retrieve a Microsoft.SemanticKernel.Kernel instance by providing the required options.

```csharp
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Chat;
using Soenneker.SemanticKernel.Cache;

public class TestClass
{
    private readonly ISemanticKernelCache _semanticKernelCache;
    private readonly SemanticKernelOptions _options;

    public TestClass(ISemanticKernelCache semanticKernelCache)
    {
        _semanticKernelCache = semanticKernelCache;
        
        // Create the options object once. Replace these with your actual values.
        var options = new SemanticKernelOptions
        {
            ModelId = "deepseek-r1:32b",
            Endpoint = "http://localhost:11434",
            KernelFactory = (opts, ct) =>
            {
                IKernelBuilder builder = Kernel.CreateBuilder().AddOllamaChatCompletion(opts.ModelId, new Uri(opts.Endpoint));

                return ValueTask.FromResult(builder);
            }
        };
    }

    public async async ValueTask<string> GetKernelResponse(string input, CancellationToken cancellationToken = default)
    {
        // Retrieve (or create) the kernel instance using a key (here, nameof(TestClass)).
        Kernel kernel = await _semanticKernelCache.Get(nameof(TestClass), _options, cancellationToken);

        // Retrieve the chat completion service from the kernel.
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        // Create a chat history and add the user's message.
        var history = new ChatHistory();
        history.AddUserMessage(input);

        // Request a chat completion using the chat service.
        var chatResult = await chatCompletionService.GetChatMessageContentAsync(history, kernel: kernel);

        // Return the chat result (or process it further as needed).
        return chatResult.ToString();
    }
}
```

### Extending for Different Connectors/Plugins

The `SemanticKernelOptions` class includes an optional `KernelFactory` delegate. This allows you to override the default behavior (which uses the Azure Text Completion service) and create the kernel using a different connector or plugin. For example:

```csharp
var openAiOptions = new SemanticKernelOptions
{
    ModelId = "openai-model-id",
    Endpoint = "https://api.openai.com/v1/",
    ApiKey = "your-openai-api-key",
    KernelFactory = (opts, ct) =>
    {
        Kernel kernel = new KernelBuilder().AddOpenAITextCompletionService(opts.ModelId, opts.Endpoint, opts.ApiKey);

        return ValueTask.FromResult(kernel);
    },
    ConfigureKernelAsync = async kernel =>
    {
        // Optionally, import skills or perform additional configuration.
        await ValueTask.CompletedTask;
    }
};

Kernel openAiKernel = await semanticKernelCache.Get("openaiKernel", openAiOptions);
```

This design makes it straightforward to support multiple types of Semantic Kernel configurations using the same caching mechanism.