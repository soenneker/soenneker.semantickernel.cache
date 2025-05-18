using Microsoft.SemanticKernel;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using Soenneker.SemanticKernel.Dtos.Options;

namespace Soenneker.SemanticKernel.Cache.Abstract;

/// <summary>
/// Providing async thread-safe singleton Semantic Kernel instances
/// </summary>
public interface ISemanticKernelCache : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Retrieves a <see cref="Kernel"/> instance asynchronously, creating it if necessary.
    /// </summary>
    /// <param name="id">The unique identifier of the kernel instance.</param>
    /// <param name="options">The options used to configure the kernel instance.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, returning the requested <see cref="Kernel"/>.</returns>
    ValueTask<Kernel> Init(string id, SemanticKernelOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a <see cref="Kernel"/> instance asynchronously, creating it if necessary.
    /// </summary>
    /// <param name="id">The unique identifier of the kernel instance.</param>
    /// <param name="options">The options used to configure the kernel instance.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, returning the requested <see cref="Kernel"/>.</returns>
    ValueTask<Kernel> Get(string id, SemanticKernelOptions? options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a <see cref="Kernel"/> instance synchronously, creating it if necessary.
    /// </summary>
    /// <param name="id">The unique identifier of the kernel instance.</param>
    /// <param name="options">The options used to configure the kernel instance.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The requested <see cref="Kernel"/> instance.</returns>
    Kernel GetSync(string id, SemanticKernelOptions? options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a <see cref="Kernel"/> instance from the cache asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the kernel instance to remove.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous removal operation.</returns>
    ValueTask Remove(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a <see cref="Kernel"/> instance from the cache synchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the kernel instance to remove.</param>
    /// <param name="cancellationToken"></param>
    void RemoveSync(string id, CancellationToken cancellationToken = default);

    ValueTask Clear(CancellationToken cancellationToken = default);

    ValueTask<Dictionary<string, Kernel>> GetAll(CancellationToken cancellationToken = default);
}