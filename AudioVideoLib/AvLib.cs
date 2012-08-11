using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vlc.DotNet.Core;
using Vlc.DotNet;
using Vlc.DotNet.Core.Interops;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Core.Interops.Signatures.LibVlc.AsynchronousEvents;

namespace Sematena.AudioVideoLib
{
    public class AvLib
    {
        private IntPtr _vlcInstancePtr;
        private IntPtr _vlcPlayerPtr;
        private IntPtr _vlcMediaPtr;
        private IntPtr _vlcEventManagerPtr;

        private LibVlcMediaPlayer _libPlayer;
        private LibVlcMedia _libMedia;
        private LibVlcAsynchronousEvents _libEvents;

        private EventCallbackDelegate _eventCallbackDelegate;

        public event EventHandler Playing;
        public event EventHandler Stopped;

        public void Initialize()
        {
            VlcContext.LibVlcDllsPath = getVlcLibPath();
            //VlcContext.LibVlcDllsPath = @"C:\Program Files (x86)\VideoLAN\VLC";
            VlcContext.LibVlcPluginsPath = getVlcPluginsPath();
            //VlcContext.LibVlcPluginsPath = @"C:\Program Files (x86)\VideoLAN\VLC\plugins";

            var loggingEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["LoggingEnabled"]);
            VlcContext.StartupOptions.IgnoreConfig = true;
            VlcContext.StartupOptions.LogOptions.LogInFile = loggingEnabled;
            VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = loggingEnabled;
            VlcContext.StartupOptions.LogOptions.Verbosity = loggingEnabled ? VlcLogVerbosities.Debug : VlcLogVerbosities.None;

            _vlcInstancePtr = VlcContext.Initialize();

            _libPlayer = VlcContext.InteropManager.MediaPlayerInterops;
            _libMedia = VlcContext.InteropManager.MediaInterops;
            _libEvents = VlcContext.InteropManager.EventInterops;

            var funcNewPlayerInst = _libPlayer.NewInstance;
            _vlcPlayerPtr = funcNewPlayerInst.Invoke(_vlcInstancePtr);

            _vlcEventManagerPtr = _libPlayer.EventManagerNewIntance.Invoke( _vlcPlayerPtr );
            _eventCallbackDelegate = new EventCallbackDelegate(this.OnVlcEvent);
            _libEvents.Attach.Invoke(_vlcEventManagerPtr, EventTypes.MediaPlayerPlaying, _eventCallbackDelegate, IntPtr.Zero);
            _libEvents.Attach.Invoke(_vlcEventManagerPtr, EventTypes.MediaPlayerStopped, _eventCallbackDelegate, IntPtr.Zero);
       }

        private void OnVlcEvent(ref LibVlcEventArgs eventData, IntPtr userData)
        {
            switch (eventData.Type)
            {
                case EventTypes.MediaPlayerPlaying:
                    Playing.RaiseSafe(this, EventArgs.Empty); // TODO
                    break;
                case EventTypes.MediaPlayerStopped:
                    Stopped.RaiseSafe(this, EventArgs.Empty); // TODO
                    break;
                default:
                    break;
            }
        }

        private string getVlcLibPath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var libDir = ConfigurationManager.AppSettings["VlcLibRelDir"];
            return Path.Combine(baseDir, libDir);
        }

        private string getVlcPluginsPath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var libDir = ConfigurationManager.AppSettings["VlcPluginsRelDir"];
            return Path.Combine(baseDir, libDir);
        }

        public void LoadMedia(string path)
        {
            var funcNewMediaInst = _libMedia.NewInstanceFromPath;
            _vlcMediaPtr = funcNewMediaInst.Invoke(_vlcInstancePtr, System.Text.Encoding.ASCII.GetBytes(path));
        }

        public void Play()
        {
            if (_vlcMediaPtr == IntPtr.Zero)
                return;

            _libPlayer.SetMedia.Invoke(_vlcPlayerPtr, _vlcMediaPtr);
            if ( !LibVlcHelpers.RetOk( _libPlayer.Play.Invoke(_vlcPlayerPtr) ) )
                return; // TODO: raise exception
        }

        public void Pause()
        {

        }

        public void Stop()
        {
            _libPlayer.Stop.Invoke(_vlcPlayerPtr);
        }

        public void Close()
        {
            _libEvents.Detach.Invoke(_vlcEventManagerPtr, EventTypes.MediaPlayerStopped, _eventCallbackDelegate, IntPtr.Zero);
            _libEvents.Detach.Invoke(_vlcEventManagerPtr, EventTypes.MediaPlayerPlaying, _eventCallbackDelegate, IntPtr.Zero);

            // any VLC object instance that we create directly via
            // the interop layer needs to be explicitly released

            if (_vlcEventManagerPtr != IntPtr.Zero)
                _libPlayer.ReleaseInstance.Invoke(_vlcEventManagerPtr);

            if (_vlcMediaPtr != IntPtr.Zero)
                _libMedia.ReleaseInstance.Invoke(_vlcMediaPtr);

            if (_vlcPlayerPtr != IntPtr.Zero)
                _libPlayer.ReleaseInstance.Invoke(_vlcPlayerPtr);

            VlcContext.CloseAll();
        }
    }
}
