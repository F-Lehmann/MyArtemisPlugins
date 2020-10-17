using Artemis.Core.DataModelExpansions;
using Artemis.Plugins.DataModelExpansions.Audio.DataModels;
using CSCore;
using CSCore.Codecs;
using CSCore.DSP;
using CSCore.SoundOut;
using CSCore.SoundIn;
using CSCore.Streams;
using CSCore.Streams.Effects;
using CSCore.CoreAudioAPI;
using System.Linq;

namespace Artemis.Plugins.DataModelExpansions.Audio {
    public class PluginDataModelExpansion : DataModelExpansion<PluginDataModel> {
        private WasapiCapture _soundIn;
        private IWaveSource _source;
        private BasicSpectrumProvider _spectrumProvider;
        private const FftSize fftSize = FftSize.Fft64;

        public override void EnablePlugin() {
            //open the default device 
            _soundIn = new WasapiLoopbackCapture();
            //Our loopback capture opens the default render device by default so the following is not needed
            //_soundIn.Device = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Console);
            _soundIn.Initialize();

            var soundInSource = new SoundInSource(_soundIn);
            ISampleSource source = soundInSource.ToSampleSource(); //.AppendSource(x => new PitchShifter(x), out _pitchShifter);

            //create a spectrum provider which provides fft data based on some input
            _spectrumProvider = new BasicSpectrumProvider(source.WaveFormat.Channels,
                source.WaveFormat.SampleRate, fftSize);


            //the SingleBlockNotificationStream is used to intercept the played samples
            var notificationSource = new SingleBlockNotificationStream(source);
            //pass the intercepted samples as input data to the spectrumprovider (which will calculate a fft based on them)
            notificationSource.SingleBlockRead += (s, a) => _spectrumProvider.Add(a.Left, a.Right);

            _source = notificationSource.ToWaveSource(16);

            // We need to read from our source otherwise SingleBlockRead is never called and our spectrum provider is not populated
            byte[] buffer = new byte[_source.WaveFormat.BytesPerSecond / 2];
            soundInSource.DataAvailable += (s, aEvent) => {
                int read;
                while ((read = _source.Read(buffer, 0, buffer.Length)) > 0) ;
            };


            //play the audio
            _soundIn.Start();
        }

        public override void DisablePlugin() {
            if (_soundIn != null) {
                _soundIn.Stop();
                _soundIn.Dispose();
                _soundIn = null;
            }
            if (_source != null) {
                _source.Dispose();
                _source = null;
            }
        }

        public override void Update(double deltaTime) {

            var fftBuffer = new float[(int)fftSize];

            //get the fft result from the spectrum provider
            if (_spectrumProvider.GetFftData(fftBuffer, this)) {
                DataModel.FFTBins = fftBuffer.ToList();
            }

            // You can access your data model here and update it however you like
            DataModel.TemplateDataModelString = $"The last delta time was {deltaTime} seconds";
        }
    }
}