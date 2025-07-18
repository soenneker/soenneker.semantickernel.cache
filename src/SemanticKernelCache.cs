﻿using Soenneker.SemanticKernel.Cache.Abstract;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Soenneker.Extensions.ValueTask;
using Soenneker.SemanticKernel.Dtos.Options;
using Soenneker.Utils.SingletonDictionary;
using System.Collections.Generic;

namespace Soenneker.SemanticKernel.Cache;

/// <inheritdoc cref="ISemanticKernelCache"/>
public sealed class SemanticKernelCache : ISemanticKernelCache
{
    private readonly ILogger<SemanticKernelCache> _logger;
    private readonly SingletonDictionary<Kernel> _kernels;

    public SemanticKernelCache(ILogger<SemanticKernelCache> logger)
    {
        _logger = logger;
        _kernels = new SingletonDictionary<Kernel>(async (id, token, args) =>
        {
            SemanticKernelOptions options = args.FirstOrDefault() as SemanticKernelOptions
                                            ?? throw new ArgumentException($"{nameof(SemanticKernelOptions)} must be provided.");

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

    public ValueTask<Kernel> Get(string id, SemanticKernelOptions options, CancellationToken cancellationToken = default)
    {
        return _kernels.Get(id, cancellationToken, options);
    }

    public Kernel GetSync(string id, SemanticKernelOptions options, CancellationToken cancellationToken = default)
    {
        return _kernels.GetSync(id, cancellationToken, options);
    }

    public ValueTask Remove(string id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing Semantic Kernel instance ({id})...", id);
        return _kernels.Remove(id, cancellationToken);
    }

    public ValueTask Clear(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Clearing Semantic Kernel instances...");
        return _kernels.Clear(cancellationToken);
    }

    public ValueTask<Dictionary<string, Kernel>> GetAll(CancellationToken cancellationToken = default)
    {
        return _kernels.GetAll(cancellationToken);
    }

    public void RemoveSync(string id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing Semantic Kernel instance ({id}) synchronously...", id);
        _kernels.RemoveSync(id, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _kernels.DisposeAsync();
    }

    public void Dispose()
    {
        _kernels.Dispose();
    }
}