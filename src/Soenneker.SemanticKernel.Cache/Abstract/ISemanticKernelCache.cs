using Microsoft.SemanticKernel;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using Soenneker.SemanticKernel.Dtos.Options;

namespace Soenneker.SemanticKernel.Cache.Abstract;

/// <summary>
/// Provides concurrent, keyed creation and reuse of Semantic Kernel instances.
/// </summary>
public interface ISemanticKernelCache : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Gets a <see cref="Kernel"/> by ID, creating and configuring it on first access.
    /// </summary>
    /// <param name="id">The unique identifier of the kernel instance.</param>
    /// <param name="options">The options used to configure the kernel instance.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, returning the requested <see cref="Kernel"/>.</returns>
    ValueTask<Kernel> Init(string id, SemanticKernelOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a <see cref="Kernel"/> by ID, creating and configuring it on first access.
    /// </summary>
    /// <param name="id">The unique identifier of the kernel instance.</param>
    /// <param name="options">The options used to configure the kernel instance.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, returning the requested <see cref="Kernel"/>.</returns>
    ValueTask<Kernel> Get(string id, SemanticKernelOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a <see cref="Kernel"/> instance synchronously, creating it if necessary.
    /// </summary>
    /// <param name="id">The unique identifier of the kernel instance.</param>
    /// <param name="options">The options used to configure the kernel instance.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The requested <see cref="Kernel"/> instance.</returns>
    Kernel GetSync(string id, SemanticKernelOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a <see cref="Kernel"/> instance from the cache asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the kernel instance to remove.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous removal operation.</returns>
    ValueTask<bool> Remove(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a <see cref="Kernel"/> instance from the cache synchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the kernel instance to remove.</param>
    /// <param name="cancellationToken"></param>
    void RemoveSync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all entries managed by the Semantic Kernel Cache.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the Semantic Kernel Cache has been cleared.</returns>
    ValueTask Clear(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a snapshot of all initialized kernels keyed by ID.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A dictionary snapshot of initialized kernels.</returns>
    ValueTask<Dictionary<string, Kernel>> GetAll(CancellationToken cancellationToken = default);
}
