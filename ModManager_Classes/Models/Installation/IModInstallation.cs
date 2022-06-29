﻿using Imya.Models.NotifyPropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Installation
{
    public interface IModInstallation : IInstallation
    {
        ModCollection? Result { get; protected set; }
        ModInstallationOptions Options { get; protected init; }
    }

    public static class IModInstallationExtensions
    {
        public static Task RunMoveInto(this IModInstallation installation)
        {
            if (installation.Result is null)
            {
                Console.WriteLine("No ModCollection to install");
                return Task.CompletedTask;
            }

            Console.WriteLine($"Install zip: {installation.Result?.ModsPath}");

            return Task.Run(async () =>
            {
                await ModCollection.Global?.MoveIntoAsync(installation.Result!, installation.Options.AllowOldToOverwrite);
            }
            );
        }
    }

    public class ModInstallationOptions : PropertyChangedNotifier
    {
        public bool AllowOldToOverwrite
        {
            get => _allowOldToOverwrite;
            set
            {
                _allowOldToOverwrite = value;
                OnPropertyChanged(nameof(AllowOldToOverwrite));
            }
        }
        private bool _allowOldToOverwrite = false;
    }
}
