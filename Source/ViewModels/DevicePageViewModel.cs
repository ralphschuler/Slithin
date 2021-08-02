﻿using System;
using System.Linq;
using Slithin.Core;
using Slithin.Core.Remarkable;
using Slithin.Core.Remarkable.Cloud;

namespace Slithin.ViewModels
{
    public class DevicePageViewModel : BaseViewModel
    {
        public DevicePageViewModel()
        {
            SyncService.CustomScreens.Add(new CustomScreen { Title = "Starting", Filename = "starting.png" });
            SyncService.CustomScreens.Add(new CustomScreen { Title = "Power Off", Filename = "poweroff.png" });
            SyncService.CustomScreens.Add(new CustomScreen { Title = "Suspended", Filename = "suspended.png" });
            SyncService.CustomScreens.Add(new CustomScreen { Title = "Rebooting", Filename = "rebooting.png" });

            foreach (var cs in ServiceLocator.SyncService.CustomScreens)
            {
                cs.Load();
            }

            Version = ServiceLocator.Device.GetVersion().ToString();
        }

        public string Version { get; set; }
    }
}
