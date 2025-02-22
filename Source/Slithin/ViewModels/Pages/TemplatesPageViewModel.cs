﻿using System.Linq;
using System.Windows.Input;
using Slithin.Controls;
using Slithin.Core;
using Slithin.Core.Remarkable;
using Slithin.Core.Services;
using Slithin.Core.Sync.Repositorys;
using Slithin.UI.Modals;
using Slithin.ViewModels.Modals;
using Slithin.Commands;
using Slithin.Core.MVVM;
using Slithin.Core.Remarkable.Models;
using Slithin.Validators;

namespace Slithin.ViewModels.Pages;

public class TemplatesPageViewModel : BaseViewModel
{
    private readonly ILoadingService _loadingService;
    private readonly IMailboxService _mailboxService;

    private Template _selectedTemplate;

    public TemplatesPageViewModel(ILoadingService loadingService,
        IMailboxService mailboxService,
        LocalRepository localRepository,
        IPathManager pathManager,
        DeviceRepository deviceRepository,
        ILocalisationService localisationService,
        AddTemplateValidator validator)
    {
        OpenAddModalCommand = new DelegateCommand(_ =>
        {
            DialogService.Open(new AddTemplateModal(),
                new AddTemplateModalViewModel(pathManager, localRepository, localisationService, validator));
        });

        RemoveTemplateCommand = new RemoveTemplateCommand(this, localisationService,
            deviceRepository, localRepository);

        _loadingService = loadingService;
        _mailboxService = mailboxService;
    }

    public ICommand OpenAddModalCommand { get; set; }

    public ICommand RemoveTemplateCommand { get; set; }

    public Template SelectedTemplate
    {
        get => _selectedTemplate;
        set => SetValue(ref _selectedTemplate, value);
    }

    public override void OnLoad()
    {
        base.OnLoad();

        _mailboxService.PostAction(() =>
        {
            NotificationService.Show("Loading Templates");

            _loadingService.LoadTemplates();

            SyncService.TemplateFilter.SelectedCategory = SyncService.TemplateFilter.Categories.First();
        });
    }
}
