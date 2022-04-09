using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.Installation
{
    public abstract class Installation : Imya.Models.NotifyPropertyChanged.PropertyChangedNotifier, IProgress<float>
    {
        public float Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }
        protected float _progress = 0.1f;
        private (float, float) _progressRange = (0, 1);

        public bool IsInstalling
        {
            get => _isInstalling;
            set => SetProperty(ref _isInstalling, value);
        }
        protected bool _isInstalling = false;

        public IInstallationStatus Status { get; }

        public void Report(float value) => Progress = _progressRange.Item1 + value * (_progressRange.Item2 - _progressRange.Item1);
    }

    public interface IInstallationStatus
    {
        public IText Localized { get; }
    }
}
