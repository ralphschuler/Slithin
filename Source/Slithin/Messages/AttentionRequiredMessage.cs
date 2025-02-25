﻿using System;
using Slithin.Core;
using Slithin.Core.Messaging;

namespace Slithin.Messages;

public class AttentionRequiredMessage : AsynchronousMessage
{
    public Action<object> Action { get; internal set; }
    public string Text { get; internal set; }
    public string Title { get; internal set; }
}