﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using ReactiveUI.Fody.Helpers;
using Slithin.Controls;
using Slithin.Core;
using Slithin.Core.Remarkable;
using Slithin.Core.Services;
using Slithin.Core.Sync;
using Slithin.Core.Validators;
using Slithin.Tools;

namespace Slithin.ViewModels.Modals
{
    public class CreateNotebookModalViewModel : BaseViewModel
    {
        private readonly IPathManager _pathManager;
        private readonly CreateNotebookValidator _validator;

        public CreateNotebookModalViewModel(IPathManager pathManager,
                                            CreateNotebookValidator validator,
                                            ILoadingService loadingService)
        {
            if (TemplateStorage.Instance.Templates == null)
            {
                loadingService.LoadTemplates();
            }

            Templates = new ObservableCollection<Template>(TemplateStorage.Instance.Templates);

            AddPagesCommand = new DelegateCommand(AddPages);
            OKCommand = new DelegateCommand(OK);

            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();

            Cover = new Bitmap(assets.Open(new Uri($"avares://Slithin/Resources/Covers/Folder-DBlue.png")));
            Filename = "internal:Folder-DBlue.png";

            var coverNames = GetType().Assembly.GetManifestResourceNames().
                Where(_ => _.StartsWith("Slithin.Resources.Covers.")).
                Select(_ => _.Replace(".png", "").Substring("Slithin.Resources.Covers.".Length));

            DefaultCovers = new ObservableCollection<string>(coverNames);
            _pathManager = pathManager;
            _validator = validator;
        }

        public ICommand AddPagesCommand { get; set; }

        [Reactive]
        public IImage Cover { get; set; }

        public ObservableCollection<string> DefaultCovers { get; set; }

        [Reactive]
        public string Filename { get; set; }

        [Reactive]
        public string Name { get; set; }

        public ICommand OKCommand { get; set; }

        [Reactive]
        public string PageCount { get; set; }

        public ObservableCollection<NotebookPage> Pages { get; set; } = new();

        [Reactive]
        public bool RenderName { get; set; }

        [Reactive]
        public Template SelectedTemplate { get; set; }

        public ObservableCollection<Template> Templates { get; set; } = new();

        public void LoadCover()
        {
            if (Filename.StartsWith("custom:"))
            {
                using var strm = File.OpenRead(Filename.Substring("custom:".Length));
                Cover = Bitmap.DecodeToWidth(strm, 150, Avalonia.Visuals.Media.Imaging.BitmapInterpolationMode.HighQuality);
            }
            else
            {
                using var strm = GetType().Assembly.GetManifestResourceStream("Slithin.Resources.Covers." + Filename.Substring("internal:".Length) + ".png");
                Cover = Bitmap.DecodeToWidth(strm, 150, Avalonia.Visuals.Media.Imaging.BitmapInterpolationMode.HighQuality);
            }
        }

        private void AddPages(object obj)
        {
            if (int.TryParse(PageCount, out var pcount) && SelectedTemplate != null)
            {
                Pages.Add(new NotebookPage(SelectedTemplate, int.Parse(PageCount)));

                SelectedTemplate = null;
                PageCount = null;
            }
            else
            {
                //ToDo: show error
            }
        }

        private void OK(object obj)
        {
            //ToDo: use validator
            var validationResult = _validator.Validate(this);

            if (validationResult.IsValid)
            {
                var document = new PdfDocument
                {
                    PageLayout = PdfPageLayout.SinglePage,
                    PageMode = PdfPageMode.FullScreen
                };
                document.Info.Author = "Slithin";
                document.Info.Title = Name;

                var size = PageSizeConverter.ToSize(PageSize.A4);

                var coverPage = document.AddPage();
                var coverGfx = XGraphics.FromPdfPage(coverPage);

                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();

                Stream coverStream = null;
                if (Filename.StartsWith("custom:"))
                {
                    coverStream = File.OpenRead(Filename.Substring("custom:".Length));
                }
                else
                {
                    coverStream = GetType().Assembly.GetManifestResourceStream("Slithin.Resources.Covers." + Filename.Substring("internal:".Length) + ".png");
                }

                if (coverStream == null)
                {
                    coverStream = GetType().Assembly.GetManifestResourceStream("Slithin.Resources.Cover.png");
                }

                var coverImage = XImage.FromStream(() => coverStream);
                coverGfx.DrawImage(coverImage, 3, 3, coverPage.Width - 6, coverPage.Height - 6);

                if (RenderName)
                {
                    var font = new XFont("Arial", 125);
                    var fontSize = coverGfx.MeasureString(Name, font);
                    coverGfx.DrawString(Name, font, XBrushes.Black, new XPoint(coverPage.Width / 2 - fontSize.Width / 2, 260));
                }

                foreach (var p in Pages)
                {
                    var t = XImage.FromFile(_pathManager.TemplatesDir + "\\" + p.Template.Filename + ".png");

                    for (var i = 0; i < p.Count; i++)
                    {
                        var page = document.AddPage();
                        page.Size = PageSize.A4;

                        var gfx = XGraphics.FromPdfPage(page);

                        gfx.DrawImage(t, 0, 0, page.Width, page.Height);
                    }
                }

                var md = new Metadata
                {
                    ID = Guid.NewGuid().ToString().ToLower(),
                    VisibleName = Name,
                    Type = "DocumentType",
                    Version = 1,
                    Parent = "",

                    Content = new() { FileType = "pdf", CoverPageNumber = 0, PageCount = document.Pages.Count }
                };

                md.Save();

                document.Save(_pathManager.NotebooksDir + $"\\{md.ID}.pdf");

                MetadataStorage.Local.Add(md, out var alreadyAdded);

                SyncService.NotebooksFilter.Documents.Add(md);
                SyncService.NotebooksFilter.SortByFolder();

                var syncItem = new SyncItem
                {
                    Action = SyncAction.Add,
                    Data = md,
                    Direction = SyncDirection.ToDevice,
                    Type = SyncType.Notebook
                };

                SyncService.SyncQueue.Insert(syncItem);

                DialogService.Close();
            }
            else
            {
                //ToDo: show error
            }
        }
    }
}