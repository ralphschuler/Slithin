﻿using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Ionic.Zip;
using Slithin.Core;
using Slithin.Core.ItemContext;
using Slithin.Core.Remarkable;
using Slithin.Core.Services;

namespace Slithin.ContextMenus;

[Context(UIContext.Notebook)]
public class BackupContextMenu : IContextProvider
{
    private readonly ILocalisationService _localisationService;
    private readonly IMailboxService _mailboxService;
    private readonly IPathManager _pathManager;

    public BackupContextMenu(
        ILocalisationService localisationService,
        IPathManager pathManager,
        IMailboxService mailboxService)
    {
        _localisationService = localisationService;
        _pathManager = pathManager;
        _mailboxService = mailboxService;
    }

    public object ParentViewModel { get; set; }

    public bool CanHandle(object obj)
    {
        return obj is Metadata;
    }

    public ICollection<MenuItem> GetMenu(object obj)
    {
        return new[] {
            new MenuItem()
            {
                Header = _localisationService.GetString("Backup"),
                Command = new DelegateCommand((_)=> Backup((Metadata)obj)),
            }
        };
    }

    private void Backup(Metadata md)
    {
        _mailboxService.PostAction(() =>
        {
            NotificationService.Show(_localisationService.GetStringFormat("Start Compressing {0}", md.VisibleName));

            var zip = new ZipFile();

            BackupNotebookOrFolder(md, zip);

            zip.Comment = "This backup was generated by Slithin";

            zip.SaveProgress += (s, e) =>
            {
                if (e.EventType == ZipProgressEventType.Saving_BeforeWriteEntry)
                {
                    NotificationService.ShowProgress(_localisationService.GetStringFormat("Compressing '{0}'", e.CurrentEntry.FileName), e.EntriesSaved, e.EntriesTotal);
                }
            };

            zip.Save(Path.Combine(_pathManager.BackupsDir,
                $"Backup_{md.VisibleName}_{DateTime.Now:yyyy-dd-M--HH-mm-ss}.zip"));

            zip.Dispose();
            NotificationService.Hide();
        });
    }

    private void BackupNotebookOrFolder(Metadata md, ZipFile zip)
    {
        if (md.Type == "CollectionType")
        {
            foreach (var sub in MetadataStorage.Local.GetByParent(md.ID))
            {
                BackupNotebookOrFolder(sub, zip);
            }
        }

        var files = Directory.GetFiles(_pathManager.NotebooksDir, $"{md.ID}*");
        foreach (var file in files)
        {
            zip.AddFile(file, "/");
        }

        var directories = Directory.GetDirectories(_pathManager.NotebooksDir, $"{md.ID}*");
        foreach (var dir in directories)
        {
            zip.AddDirectory(dir, new DirectoryInfo(dir).Name);
        }
    }
}