﻿using System.Windows.Input;
using Slithin.Core;
using Slithin.Core.MVVM;

namespace Slithin.ViewModels;

public class ActionNotificationViewModel : BaseViewModel
{
    private string _cancelButtonText;
    private string _message;

    private string _okButtonText;

    public string CancelButtonText
    {
        get { return _cancelButtonText; }
        set { SetValue(ref _cancelButtonText, value); }
    }

    public ICommand CancelCommand { get; set; }

    public string Message
    {
        get { return _message; }
        set
        {
            SetValue(ref _message, value);
        }
    }

    public string OKButtonText
    {
        get { return _okButtonText; }
        set { SetValue(ref _okButtonText, value); }
    }

    public ICommand OKCommand { get; set; }
}
