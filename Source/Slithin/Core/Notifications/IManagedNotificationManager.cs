﻿using System.Threading.Tasks;

namespace Slithin.Core.Notifications;

/// <summary>
/// Represents a notification manager that can show arbitrary content.
/// Managed notification managers can show any content.
/// </summary>
/// <remarks>
/// Because notification managers of this type are implemented purely in managed code, they
/// can display arbitrary content, as opposed to notification managers which display notifications
/// using the host operating system's notification mechanism.
/// </remarks>
public interface IManagedNotificationManager : INotificationManager
{
    /// <summary>
    /// Shows a notification.
    /// </summary>
    /// <param name="content">The content to be displayed.</param>
    Task<Avalonia.Controls.Notifications.NotificationCard> Show(object content);
}
