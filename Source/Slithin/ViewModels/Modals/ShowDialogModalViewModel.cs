﻿namespace Slithin.ViewModels.Modals;

public class ShowDialogModalViewModel : ModalBaseViewModel
{
    private object _content;
    private string _title;

    public object Content
    {
        get => _content;
        set => SetValue(ref _content, value);
    }

    public string Title
    {
        get => _title;
        set => SetValue(ref _title, value);
    }
}
