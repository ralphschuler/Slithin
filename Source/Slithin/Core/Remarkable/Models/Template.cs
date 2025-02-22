﻿using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using LiteDB;
using Newtonsoft.Json;
using Renci.SshNet;
using Slithin.Core.Services;
using Slithin.Core.MVVM;
using Slithin.Core.Remarkable;

namespace Slithin.Core.Remarkable.Models;

public class Template : INotifyPropertyChanged
{
    private IImage _image;

    public Template()
    {
        TransferCommand = new DelegateCommand(Transfer);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [JsonProperty("categories")] public string[] Categories { get; set; }

    [JsonProperty("filename")] public string Filename { get; set; }

    [JsonProperty("iconCode")] public string IconCode { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public IImage Image
    {
        get => _image;
        set
        {
            if (_image == value)
            {
                return;
            }

            _image = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Image)));
        }
    }

    [JsonProperty("landscape")] public bool Landscape { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [BsonIgnore]
    [JsonIgnore]
    public ICommand TransferCommand { get; set; }

    public void Load()
    {
        var templatesDir = ServiceLocator.Container.Resolve<IPathManager>().TemplatesDir;

        if (!Directory.Exists(templatesDir) || Image is not null)
        {
            return;
        }

        var path = Path.Combine(templatesDir, Filename);

        if (!File.Exists(path + ".png"))
        {
            return;
        }

        Image = Bitmap.DecodeToWidth(File.OpenRead(path + ".png"), 150);
    }

    private void Transfer(object obj)
    {
        var mailboxService = ServiceLocator.Container.Resolve<IMailboxService>();
        var pathManager = ServiceLocator.Container.Resolve<IPathManager>();
        var scp = ServiceLocator.Container.Resolve<ScpClient>();
        var xochitl = ServiceLocator.Container.Resolve<Xochitl>();

        mailboxService.PostAction(() =>
        {
            NotificationService.Show($"Uploading Template '{Name}'");

            scp.Upload(new FileInfo(Path.Combine(pathManager.TemplatesDir, Filename + ".png")), PathList.Templates + Filename + ".png");

            var tmpStorage = new TemplateStorage();

            var ms = new MemoryStream();

            scp.Download(PathList.Templates + "templates.json", ms);

            ms.Seek(0, SeekOrigin.Begin);

            var remoteTemplatesContent = new StreamReader(ms).ReadToEnd();
            tmpStorage.Templates = JsonConvert.DeserializeObject<TemplateStorage>(remoteTemplatesContent).Templates;
            tmpStorage.AppendTemplate(this);
            tmpStorage.Save();

            scp.Upload(new FileInfo(Path.Combine(pathManager.ConfigBaseDir, "templates.json")), PathList.Templates + "templates.json");

            xochitl.ReloadDevice();
        });
    }
}
