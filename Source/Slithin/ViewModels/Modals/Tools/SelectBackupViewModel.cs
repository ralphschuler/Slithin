﻿using System.Collections.ObjectModel;
using Slithin.Models;

namespace Slithin.ViewModels.Modals.Tools;

public class SelectBackupViewModel : ModalBaseViewModel
{
    private Backup _selectedBackup;
    public ObservableCollection<Backup> Backups { get; set; } = new();

    public Backup SelectedBackup
    {
        get => _selectedBackup;
        set => SetValue(ref _selectedBackup, value);
    }
}
