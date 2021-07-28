﻿using System.Net;
using System.Windows.Input;
using Avalonia.Controls.ApplicationLifetimes;
using LiteDB;
using Renci.SshNet.Common;
using Slithin.Controls;
using Slithin.Core;
using Slithin.UI.Views;

namespace Slithin.ViewModels
{
    public class ConnectionWindowViewModel : BaseViewModel
    {
        private string _ipAddress;

        private string _password;

        private bool _remember;

        public ConnectionWindowViewModel()
        {
            _ipAddress = string.Empty;
            _password = string.Empty;
            _remember = false;

            ConnectCommand = new DelegateCommand(Connect);
        }

        public ObjectId _id { get; set; }

        [BsonIgnore]
        public ICommand ConnectCommand { get; set; }

        public string IP
        {
            get { return _ipAddress; }
            set { SetValue(ref _ipAddress, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetValue(ref _password, value); }
        }

        public bool Remember
        {
            get { return _remember; }
            set { SetValue(ref _remember, value); }
        }

        private void Connect(object? obj)
        {
            ServiceLocator.Client = new Renci.SshNet.SshClient(IP, 22, "root", Password);
            ServiceLocator.Scp = new Renci.SshNet.ScpClient(IP, 22, "root", Password);

            ServiceLocator.Scp.ErrorOccurred += (s, _) =>
            {
                DialogService.OpenError(_.Exception.ToString());
            };

            if (IPAddress.TryParse(IP, out var addr))
            {
                try
                {
                    ServiceLocator.Client.Connect();
                    ServiceLocator.Scp.Connect();

                    if (ServiceLocator.Client.IsConnected)
                    {
                        if (Remember)
                        {
                            ServiceLocator.RememberLoginCredencials(this);
                        }

                        if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                        {
                            desktop.MainWindow.Hide();
                            desktop.MainWindow = new MainWindow();
                            //((MainWindowViewModel)desktop.MainWindow.DataContext).SyncService.SynchronizeCommand.Execute(null);
                            desktop.MainWindow.Show();
                        }
                    }
                    else
                    {
                        DialogService.OpenError("Could not connect to host");
                    }
                }
                catch (SshException ex)
                {
                    DialogService.OpenError(ex.Message);
                }
            }
            else
            {
                DialogService.OpenError("The given IP was not valid");
            }
        }
    }
}
