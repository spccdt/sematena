using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmFoundation.Wpf;
using Sematena.AudioVideoLib;

namespace Sematena.ViewModel
{
    class PlayerViewModel : ObservableObject
    {
        private AvLib _avLib;

        public bool Playing { get; set; }

        public PlayerViewModel( AvLib avLib )
        {
            _avLib = avLib;

            _avLib.Playing += OnPlaying;
            _avLib.Stopped += OnStopped;

            // FOR TESTING ONLY
            _avLib.LoadMedia(@"C:\Users\Cameron\Desktop\BBC\48502_1745.mp4");
        }

        void OnStopped(object sender, EventArgs e)
        {
            if (!Playing)
                return;

            Playing = false;
            RaisePropertyChanged("Playing");
        }

        void OnPlaying(object sender, EventArgs e)
        {
            if (Playing)
                return;

            Playing = true;
            RaisePropertyChanged("Playing");
        }

        private ICommand _playCommand;
        public ICommand PlayCommand
        {
            get { return _playCommand ?? (_playCommand = new RelayCommand(() => this.togglePlay())); }
        }

        private void togglePlay()
        {
            if (!Playing)
            {
                _avLib.Play();
            }
            else
            {
                _avLib.Stop();
            }
        }
    }
}
