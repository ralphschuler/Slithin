﻿using System.Windows.Input;
using Slithin.Core;
using Slithin.Core.MVVM;

namespace Slithin.ViewModels.Modals;

public class PromptModalViewModel : BaseViewModel
{
    private string _header;
    private string _input;
    private string _watermark;
    public ICommand AcceptCommand { get; set; }
    public ICommand CancelCommand { get; set; }

    public string Header
    {
        get => _header;
        set => SetValue(ref _header, value);
    }

    public string Input
    {
        get => _input;
        set => _input = value;
    }

    public string Watermark
    {
        get => _watermark;
        set => SetValue(ref _watermark, value);
    }
}
