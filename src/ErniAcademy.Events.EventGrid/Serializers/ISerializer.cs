﻿namespace ErniAcademy.Events.EventGrid.Serializers;

/// <summary>
/// TODO: Omar. Replace this with https://github.com/ERNI-Academy/assets-serializers-abstraction ISerializer
/// </summary>
public interface ISerializer
{
    string ContentType { get; }

    void SerializeToStream<TItem>(TItem item, Stream stream);
    Task SerializeToStreamAsync<TItem>(TItem item, Stream stream, CancellationToken cancellationToken = default);
    string SerializeToString<TItem>(TItem item);
    TItem DeserializeFromStream<TItem>(Stream stream);
    TItem DeserializeFromString<TItem>(string item);
    ValueTask<TItem> DeserializeFromStreamAsync<TItem>(Stream stream, CancellationToken cancellationToken = default);
}