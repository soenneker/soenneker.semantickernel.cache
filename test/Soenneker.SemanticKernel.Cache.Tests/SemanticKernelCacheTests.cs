using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Soenneker.Tests.Attributes.Local;
using Soenneker.SemanticKernel.Cache.Abstract;
using Soenneker.Tests.HostedUnit;
using System;
using System.Threading.Tasks;
using Soenneker.SemanticKernel.Dtos.Options;

namespace Soenneker.SemanticKernel.Cache.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class SemanticKernelCacheTests : HostedUnitTest
{
    private readonly ISemanticKernelCache _util;

    private const string _model = "gemma3:27b";
    private const string _endpoint = "http://localhost:11434";

    public SemanticKernelCacheTests(Host host) : base(host)
    {
        _util = Resolve<ISemanticKernelCache>(true);
    }

    [Test]
    public void Default()
    {
    }

    [LocalOnly]
    public async ValueTask Get_should_retrieve_kernel()
    {
        var options = new SemanticKernelOptions
        {
            ModelId = _model,
            Endpoint = _endpoint,
            ApiKey = "",
            KernelFactory = (opts, ct) =>
            {
                // Create a kernel builder configured for local Ollama.
#pragma warning disable SKEXP0070
                IKernelBuilder builder = Kernel.CreateBuilder().AddOllamaChatCompletion(opts.ModelId, new Uri(opts.Endpoint));
#pragma warning restore

                return ValueTask.FromResult(builder);
            }
        };

        Kernel kernel = await _util.Get("localOllamaChatTest", options, System.Threading.CancellationToken.None);

        kernel.Should().NotBeNull();
    }

    [LocalOnly]
    public async ValueTask Chat_should_function()
    {
        var options = new SemanticKernelOptions
        {
            ModelId = _model,
            Endpoint = _endpoint,
            ApiKey = "",
            KernelFactory = (opts, ct) =>
            {
                // Create a kernel builder configured for local Ollama.
#pragma warning disable SKEXP0070
                IKernelBuilder builder = Kernel.CreateBuilder().AddOllamaChatCompletion(opts.ModelId, new Uri(opts.Endpoint));
#pragma warning restore

                return ValueTask.FromResult(builder);
            }
        };

        Kernel kernel = await _util.Get("localOllamaChatTest", options, System.Threading.CancellationToken.None);

        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        var history = new ChatHistory();

        string input = "hello";

        history.AddUserMessage(input);

        Logger.LogInformation(input);

        ChatMessageContent result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            kernel: kernel);

        Logger.LogInformation(result.ToString());

        result.Should().NotBeNull();
    }

    [LocalOnly]
    public async ValueTask Init_with_null_options_on_Get_should_return_kernel()
    {
        var options = new SemanticKernelOptions
        {
            ModelId = _model,
            Endpoint = _endpoint,
            ApiKey = "",
            KernelFactory = (opts, ct) =>
            {
                // Create a kernel builder configured for local Ollama.
#pragma warning disable SKEXP0070
                IKernelBuilder builder = Kernel.CreateBuilder().AddOllamaChatCompletion(opts.ModelId, new Uri(opts.Endpoint));
#pragma warning restore

                return ValueTask.FromResult(builder);
            }
        };

        Kernel kernel1 = await _util.Init("localOllamaChatTest", options, System.Threading.CancellationToken.None);

        Kernel kernel2 = await _util.Get("localOllamaChatTest", null, System.Threading.CancellationToken.None);

        kernel1.Should().NotBeNull();
        kernel2.Should().NotBeNull();
    }
}
