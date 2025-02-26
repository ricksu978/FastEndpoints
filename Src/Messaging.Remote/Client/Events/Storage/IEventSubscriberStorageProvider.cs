﻿namespace FastEndpoints;

/// <summary>
/// interface for implementing a storage provider for event subscription client app (gRPC client)
/// </summary>
public interface IEventSubscriberStorageProvider
{
    /// <summary>
    /// store the event storage record however you please. ideally on a nosql database.
    /// </summary>
    /// <param name="e">the event storage record which contains the actual event object as well as some metadata</param>
    /// <param name="ct">cancellation token</param>
    ValueTask StoreEventAsync(IEventStorageRecord e, CancellationToken ct);

    /// <summary>
    /// fetch the next pending event storage record that needs to be processed.
    /// <code>
    ///   Where(e => e.SubscriberID == subscriberID &amp;&amp; !e.IsComplete &amp;&amp; DateTime.UtcNow &lt;= e.ExpireOn)
    ///   OrderAscending(e => e.Id)
    ///   Take(1)
    /// </code>
    /// </summary>
    /// <param name="subscriberID">the id of the subscriber who's next event that should be retrieved</param>
    /// <param name="ct">cancellation token</param>
    ValueTask<IEventStorageRecord?> GetNextEventAsync(string subscriberID, CancellationToken ct);

    /// <summary>
    /// mark the event storage record as complete by either replacing the entity on storage with the supplied instance or
    /// simply update the <see cref="IEventStorageRecord.IsComplete"/> field to true with a partial update operation.
    /// </summary>
    /// <param name="e"></param>
    /// <param name="ct">cancellation token</param>
    ValueTask MarkEventAsCompleteAsync(IEventStorageRecord e, CancellationToken ct);

    /// <summary>
    /// this method will be called hourly. implement this method to remove stale records (completed or (expired and incomplete)) from storage.
    /// or instead of removing them, you can move them to some other location (dead-letter-queue maybe) or for inspection by a human.
    /// or if you'd like to retry expired events, update the <see cref="IEventStorageRecord.ExpireOn"/> field to a future date/time.
    /// </summary>
    ValueTask PurgeStaleRecordsAsync();
}