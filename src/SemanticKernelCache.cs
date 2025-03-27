using Soenneker.SemanticKernel.Cache.Abstract;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Soenneker.Extensions.ValueTask;
using Soenneker.SemanticKernel.Cache.Dtos;
using Soenneker.Utils.SingletonDictionary;

namespace Soenneker.SemanticKernel.Cache;

/// <inheritdoc cref="ISemanticKernelCache"/>
public class SemanticKernelCache : ISemanticKernelCache
{
    private readonly ILogger<SemanticKernelCache> _logger;
    private readonly SingletonDictionary<Kernel> _kernels;

    public SemanticKernelCache(ILogger<SemanticKernelCache> logger)
    {
        _logger = logger;
        _kernels = new SingletonDictionary<Kernel>(async (id, token, args) =>
        {
            SemanticKernelOptions options = args.FirstOrDefault() as SemanticKernelOptions
                                            ?? throw new ArgumentException("SemanticKernelOptions must be provided.");

            _logger.LogInformation("Creating a new Semantic Kernel instance for model ({ModelId})...", options.ModelId);

            Kernel kernel = await CreateKernelInternal(options, token).NoSync();

            await ConfigureKernel(kernel, options, token).NoSync();

            _logger.LogDebug("Semantic Kernel instance ({ModelId}) has been initialized", options.ModelId);

            return kernel;
        });
    }

    /// <summary>
    /// Creates a new Kernel instance using the supplied options.
    /// Always calls the KernelFactory delegate (if provided) to obtain an IKernelBuilder,
    /// applies any additional configuration via ConfigureBuilder, and then calls Build().
    /// </summary>
    private static async ValueTask<Kernel> CreateKernelInternal(SemanticKernelOptions options, CancellationToken cancellationToken)
    {
        IKernelBuilder builder;

        // Always call the KernelFactory if provided; otherwise, create a default builder.
        if (options.KernelFactory != null)
            builder = await options.KernelFactory(options, cancellationToken).NoSync();
        else
            builder = Kernel.CreateBuilder();

        // Optionally apply additional builder configuration.
        options.ConfigureBuilder?.Invoke(builder);

        return builder.Build();
    }

    /// <summary>
    /// Applies any additional asynchronous configuration on the kernel.
    /// </summary>
    private static async ValueTask ConfigureKernel(Kernel kernel, SemanticKernelOptions options, CancellationToken cancellationToken)
    {
        if (options.ConfigureKernel != null)
            await options.ConfigureKernel(kernel, cancellationToken).NoSync();
    }

    public ValueTask<Kernel> Init(string id, SemanticKernelOptions options, CancellationToken cancellationToken = default)
    {
        return _kernels.Get(id, cancellationToken, options);
    }

    public ValueTask<Kernel> Get(string id, SemanticKernelOptions? options, CancellationToken cancellationToken = default)
    {
        return _kernels.Get(id, cancellationToken, options!);
    }

    public Kernel GetSync(string id, SemanticKernelOptions? options, CancellationToken cancellationToken = default)
    {
        return _kernels.GetSync(id, cancellationToken, options!);
    }

    public ValueTask Remove(string id)
    {
        _logger.LogInformation("Removing Semantic Kernel instance ({KernelId})", id);
        return _kernels.Remove(id);
    }

    public void RemoveSync(string id)
    {
        _logger.LogInformation("Removing Semantic Kernel instance ({KernelId}) synchronously...", id);
        _kernels.RemoveSync(id);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return _kernels.DisposeAsync();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _kernels.Dispose();
    }
}