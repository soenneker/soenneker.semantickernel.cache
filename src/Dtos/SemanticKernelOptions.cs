using Microsoft.SemanticKernel;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.SemanticKernel.Cache.Dtos;

/// <summary>
/// Options for creating a Microsoft.SemanticKernel.Kernel instance.
/// </summary>
public class SemanticKernelOptions
{
    /// <summary>
    /// The model identifier (if applicable).
    /// </summary>
    public string ModelId { get; set; } = string.Empty;

    /// <summary>
    /// The endpoint (if applicable).
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// The API key required to authenticate (if applicable).
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Optional asynchronous delegate to further configure the kernel after creation.
    /// </summary>
    public Func<Kernel, ValueTask>? ConfigureKernelAsync { get; set; }

    /// <summary>
    /// Optional delegate that creates a custom KernelBuilder.
    /// This delegate is always called (if provided) and allows you to inject connectors, plugins, etc.
    /// </summary>
    public Func<SemanticKernelOptions, CancellationToken, ValueTask<IKernelBuilder>>? KernelFactory { get; set; }

    /// <summary>
    /// Optional delegate to further customize the KernelBuilder before building the kernel.
    /// Leave unset if no additional configuration is needed.
    /// </summary>
    public Action<IKernelBuilder>? ConfigureBuilder { get; set; }
}