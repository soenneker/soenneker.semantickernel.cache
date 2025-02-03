using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Soenneker.Facts.Local;
using Soenneker.SemanticKernel.Cache.Abstract;
using Soenneker.SemanticKernel.Cache.Dtos;
using Soenneker.Tests.FixturedUnit;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Soenneker.SemanticKernel.Cache.Tests;

[Collection("Collection")]
public class SemanticKernelCacheTests : FixturedUnitTest
{
    private readonly ISemanticKernelCache _util;

    public SemanticKernelCacheTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<ISemanticKernelCache>(true);
    }

    [Fact]
    public void Default()
    {
    }

    [LocalFact]
    public async ValueTask Get_should_retrieve_kernel()
    {
        var options = new SemanticKernelOptions
        {
            ModelId = "deepseek-r1:32b",
            Endpoint = "http://localhost:11434",
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

        Kernel kernel = await _util.Get("localOllamaChatTest", options, CancellationToken);

        kernel.Should().NotBeNull();
    }

    [LocalFact]
    public async ValueTask Chat_should_function()
    {
        var options = new SemanticKernelOptions
        {
            ModelId = "deepseek-r1:32b",
            Endpoint = "http://localhost:11434",
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

        Kernel kernel = await _util.Get("localOllamaChatTest", options, CancellationToken);

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
}