using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sesion2_Lab01.com.isil.content;

using SharpDX.XAudio2;
using SharpDX.Multimedia;
using SharpDX.XAPO.Fx;

namespace Sesion2_Lab01.com.isil.system.soundSystem {

    public class NSoundInstance {

        private XAudio2 mAudioEngine;
        private NSoundDevice mSoundDevice;

        private SourceVoice mSourceVoice;
        private SoundStream mSoundStream;
        private AudioBuffer mAudioBuffer;

        private bool mIsSoundFinished;
        private Action mOnFinishSound;

        public bool IsSoundFinished { get { return mIsSoundFinished; } }

        public NSoundInstance(NSoundDevice soundDevice) {
            mSoundDevice = soundDevice;
            mAudioEngine = soundDevice.AudioEngine;

            mIsSoundFinished = false;
        }

        public void BindDataBuffer(NSound soundNode) {
            this.BindDataBuffer(soundNode, 0);
        }

        public void BindDataBuffer(NSound soundNode, int loopCount) {
            mSoundStream = new SoundStream(soundNode.NativeFileStream);

            mAudioBuffer = new AudioBuffer(mSoundStream);
            mAudioBuffer.AudioBytes = (int)mSoundStream.Length;
            mAudioBuffer.Flags = SharpDX.XAudio2.BufferFlags.EndOfStream;
            mAudioBuffer.LoopCount = loopCount;

            mSourceVoice = new SourceVoice(mAudioEngine, mSoundStream.Format);

            Reverb reverb = new Reverb();
            EffectDescriptor effectDescriptor = new EffectDescriptor(reverb);
            mSourceVoice.SetEffectChain(effectDescriptor);
            mSourceVoice.EnableEffect(0);

            mSourceVoice.SubmitSourceBuffer(mAudioBuffer, mSoundStream.DecodedPacketsInfo);  
        }

        public void Play(Action onFinishSound) {
            mOnFinishSound = onFinishSound;

            //mSourceVoice.Start();

            var waveFormat = new WaveFormat(44100, 32, 2);
            var sourceVoice = new SourceVoice(mAudioEngine, waveFormat);

            int bufferSize = waveFormat.ConvertLatencyToByteSize(60000);
            var dataStream = new SharpDX.DataStream(bufferSize, true, true);

            int numberOfSamples = bufferSize / waveFormat.BlockAlign;

            for (int i = 0; i < numberOfSamples; i++) {
                double vibrato = Math.Cos(2 * Math.PI * 10.0 * i / waveFormat.SampleRate);
                float value = (float)(Math.Cos(2 * Math.PI * (220.0 + 4.0 * vibrato) * i / waveFormat.SampleRate) * 0.5);
                dataStream.Write(value);
                dataStream.Write(value);

                //double vibrato = Math.Sin(2 * i * Math.Sin(i) / waveFormat.SampleRate) * 2;
                //float value = (float)(Math.Cos(2 * Math.PI * (220.0 + 4.0 * vibrato) * i / waveFormat.SampleRate) * 0.5);
                //dataStream.Write(value);
                //dataStream.Write(value);
            }

            dataStream.Position = 0;

            var audioBuffer = new AudioBuffer { Stream = dataStream, Flags = SharpDX.XAudio2.BufferFlags.EndOfStream, AudioBytes = bufferSize };

            /*var reverb = new Reverb();
            var effectDescriptor = new EffectDescriptor(reverb);
            sourceVoice.SetEffectChain(effectDescriptor);
            sourceVoice.EnableEffect(0);*/

            var effectDescriptorReverb = new EffectDescriptor(new Reverb());
            var effectDescriptorEcho = new EffectDescriptor(new Echo());

            sourceVoice.SetEffectChain(new EffectDescriptor[2] { effectDescriptorReverb, effectDescriptorEcho });
            sourceVoice.EnableEffect(0);
            sourceVoice.EnableEffect(1);

            sourceVoice.SubmitSourceBuffer(audioBuffer, null);

            sourceVoice.Start();
        }

        public void Stop() {
            mSourceVoice.Stop();
        }

        public void Update(int dt) {
            if (!mIsSoundFinished) {
                if (mSourceVoice.State.BuffersQueued == 0) {
                    mIsSoundFinished = true;

                    // recycle the sound
                    this.Recycle();

                    if (mOnFinishSound != null) {
                        mOnFinishSound();
                        mOnFinishSound = null;
                    }
                }
            }
        }

        private void Recycle() {
            if (mAudioBuffer != null) {
                if (mAudioBuffer.Stream != null) {
                    mAudioBuffer.Stream.Dispose();
                }

                mAudioBuffer = null;
            }

            mSoundStream = null;

            if (mSourceVoice != null) {
                if (!mSourceVoice.IsDisposed) {
                    mSourceVoice.DestroyVoice();
                    mSourceVoice.Dispose();
                }

                mSourceVoice = null;
            }
        }
    }
}
