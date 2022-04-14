﻿using System;

namespace ErniAcademy.Events.Contracts.UnitTests.Utils;

public class DummyEvent : EventBase
{
    public DummyEvent()
    {
    }

    public DummyEvent(Guid correlationId) : base(correlationId)
    {
    }

    public DummyEvent(string eventType) : base(eventType)
    {
    }

    public DummyEvent(Guid eventId, Guid correlationId, string eventType, DateTimeOffset createdAt) : base(eventId, correlationId, eventType, createdAt)
    {
    }
}
