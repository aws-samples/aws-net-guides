// <copyright file="S3ObjectCreateEvent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DomainModel.EventBridgeEvent
{
    using Amazon.Lambda.CloudWatchEvents;

    /// <summary>
    /// This class represents an S3 object create event sent via EventBridge.
    /// </summary>
    public class S3ObjectCreateEvent : CloudWatchEvent<S3ObjectCreate>
    {
    }
}