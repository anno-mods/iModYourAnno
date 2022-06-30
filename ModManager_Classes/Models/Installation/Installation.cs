﻿using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Installation
{
    public abstract class Installation : 
        Imya.Models.NotifyPropertyChanged.PropertyChangedNotifier,
        IProgress<float>,
        IInstallation
    {
        public float Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }
        protected float _progress = 0.0f;

        protected (float, float) _progressRange = (0, 1);

        public bool IsInstalling
        {
            get => _isInstalling;
            set => SetProperty(ref _isInstalling, value);
        }
        protected bool _isInstalling = false;

        public bool IsAbortable
        {
            get => _isAbortable;
            set => SetProperty(ref _isAbortable, value);
        }
        protected bool _isAbortable = false;

        public IInstallationStatus? Status { get; }

        public IText? HeaderText 
        { 
            get => _headerText;
            protected set 
            {
                SetProperty(ref _headerText, value);
                OnPropertyChanged(nameof(HeaderText));
            }
        }
        protected IText? _headerText;

        public IText? AdditionalText
        {
            get => _additional_text;
            protected set
            {
                SetProperty(ref _additional_text, value);
                OnPropertyChanged(nameof(AdditionalText));
            }
        }
        protected IText? _additional_text;

        public bool HasAdditionalText => AdditionalText is not null;

        public void Report(float value) => Progress = _progressRange.Item1 + value * (_progressRange.Item2 - _progressRange.Item1);

        public void SetProgressRange(float Min, float Max)
        {
            _progressRange = (Min, Max);
            Report(Progress);
        }

        public abstract Task<IInstallation> Setup();
        public abstract Task Finalize();

        public abstract void CleanUp();
    }

    public interface IInstallationStatus
    {
        public IText Localized { get; }
    }
}
