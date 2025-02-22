﻿using System;

namespace Slithin.Core.Notifications;

/// <summary>
/// A notification that can be shown in a window or by the host operating system.
/// </summary>
/// <remarks>
/// This class represents a notification that can be displayed either in a window using
/// <see cref="WindowNotificationManager"/> or by the host operating system (to be implemented).
/// </remarks>
public class Notification : INotification
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Notification"/> class.
    /// </summary>
    /// <param name="title">The title of the notification.</param>
    /// <param name="message">The message to be displayed in the notification.</param>
    /// <param name="type">The <see cref="NotificationType"/> of the notification.</param>
    /// <param name="expiration">The expiry time at which the notification will close.
    /// Use <see cref="TimeSpan.Zero"/> for notifications that will remain open.</param>
    /// <param name="onClick">An Action to call when the notification is clicked.</param>
    /// <param name="onClose">An Action to call when the notification is closed.</param>
    public Notification(string? title,
        object? message,
        Avalonia.Controls.Notifications.NotificationType type = Avalonia.Controls.Notifications.NotificationType.Information,
        TimeSpan? expiration = null,
        Action? onClick = null,
        Action? onClose = null)
    {
        Title = title;
        Message = message;
        Type = type;
        Expiration = expiration.HasValue ? expiration.Value : TimeSpan.FromSeconds(5);
        OnClick = onClick;
        OnClose = onClose;
    }

    public Avalonia.Controls.Notifications.NotificationCard Control { get; set; }

    /// <inheritdoc/>
    public TimeSpan Expiration { get; private set; }

    /// <inheritdoc/>
    public object? Message { get; private set; }

    /// <inheritdoc/>
    public Action? OnClick { get; private set; }

    /// <inheritdoc/>
    public Action? OnClose { get; private set; }

    /// <inheritdoc/>
    public string? Title { get; private set; }

    /// <inheritdoc/>
    public Avalonia.Controls.Notifications.NotificationType Type { get; private set; }
}
