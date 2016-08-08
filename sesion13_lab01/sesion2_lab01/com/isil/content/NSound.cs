using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.IO;
using SharpDX.MediaFoundation;

namespace Sesion2_Lab01.com.isil.content {
    public class NSound {

        public const int DEFAULT_SAMPLE_RATE = 22050;

        private AudioDecoder mAudioDecoder;
        private NativeFileStream mAudioStream;

        public AudioDecoder AudioDecoder            { get { return mAudioDecoder; } }
        public NativeFileStream NativeFileStream    { 
            get {
                mAudioStream.Position = 0;
                return mAudioStream;
            }
        }

        public NSound(string path) {
            mAudioStream = new NativeFileStream(path, NativeFileMode.Open, NativeFileAccess.Read);
            mAudioDecoder = new AudioDecoder(mAudioStream);
        }
    }
}
