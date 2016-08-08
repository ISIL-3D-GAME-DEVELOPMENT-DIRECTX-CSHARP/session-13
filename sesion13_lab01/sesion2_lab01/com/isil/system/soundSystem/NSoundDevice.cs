using SharpDX.MediaFoundation;
using SharpDX.XAudio2;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sesion2_Lab01.com.isil.system.soundSystem {
    public class NSoundDevice {

        private const int PREFERRED_INPUT_CHANNELS  = 1;
        private const int PREFERRED_SAMPLE_RATE     = 22050;

        private XAudio2 mAudioEngine;
        private MasteringVoice mMasteringVoice;

        private int mInputChannels;
        private int mSampleRate;

        private bool mIsReady;

        public bool isReady { get { return mIsReady; } }

        public int InputChannels             { get { return mInputChannels; } }
        public int SampleRate                { get { return mSampleRate; } }
        public XAudio2 AudioEngine           { get { return mAudioEngine; } }
        public MasteringVoice MasteringVoice { get { return mMasteringVoice; } }

        public NSoundDevice() {
            mIsReady = false;

            mInputChannels = NSoundDevice.PREFERRED_INPUT_CHANNELS;
            mSampleRate = NSoundDevice.PREFERRED_SAMPLE_RATE;

            // This is mandatory when using any of SharpDX.MediaFoundation classes
            MediaManager.Startup();

            // Starts The XAudio2 engine
            mAudioEngine = new XAudio2();
            mAudioEngine.StartEngine();

            mAudioEngine.CriticalError += OnInvalidate;

            mMasteringVoice = new MasteringVoice(mAudioEngine, mInputChannels, mSampleRate);
            
            mIsReady = true;
        }

        private void OnInvalidate(object sender, ErrorEventArgs e) {
            mIsReady = false;

            // just null the current pointers
            mAudioEngine = null;
            mMasteringVoice = null;

            mAudioEngine = new XAudio2();
            mAudioEngine.StartEngine();

            mAudioEngine.CriticalError += OnInvalidate;

            // give a time to re-create itself
            System.Threading.Thread.Sleep(25);
        }
    }
}
