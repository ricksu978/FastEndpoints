﻿using Microsoft.Extensions.DependencyInjection;

namespace FastEndpoints;

internal static class EventPublisherStorage
{
    internal static bool IsInitialized { get; private set; }
    internal static Func<IEventStorageRecord> RecordFactory { get; private set; } = default!;
    internal static IEventPublisherStorageProvider Provider { get; private set; } = default!;

    static EventPublisherStorage()
    {
        _ = StaleSubscriberPurgingTask();
    }

    internal static void Initialize<TStorageRecord, TStorageProvider>(IServiceProvider serviceProvider)
        where TStorageRecord : IEventStorageRecord, new()
        where TStorageProvider : class, IEventPublisherStorageProvider
    {
        RecordFactory = () => new TStorageRecord();
        Provider = ActivatorUtilities.CreateInstance<TStorageProvider>(serviceProvider);
        IsInitialized = true;
    }

    private static async Task StaleSubscriberPurgingTask()
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromHours(1));
            try
            {
                await Provider.PurgeStaleRecordsAsync();
            }
            catch { }
        }
    }
}
