﻿using System.Collections.ObjectModel;
using System.Linq;
using Slithin.Core.Remarkable;
using Slithin.Core.MVVM;
using Slithin.Core.Remarkable.Models;

namespace Slithin.Core.Sync;

public class NotebooksFilter : NotifyObject
{
    private ObservableCollection<Metadata> _documents = new();
    private string _folder = "";

    public ObservableCollection<Metadata> Documents
    {
        get => _documents;
        set => SetValue(ref _documents, value);
    }

    public string Folder
    {
        get => _folder;
        set => SetValue(ref _folder, value);
    }

    public void SortByFolder()
    {
        var ordered = Documents.OrderByDescending(_ => _.IsPinned);
        ordered = ordered.OrderByDescending(_ => _.Type == "CollectionType");
        ordered = ordered.OrderByDescending(_ => _.VisibleName?.Equals("Up .."));

        Documents = new ObservableCollection<Metadata>(ordered);
    }
}
