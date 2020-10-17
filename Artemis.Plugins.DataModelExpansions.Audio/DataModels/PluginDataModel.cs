using System.Collections.Generic;
using Artemis.Core.DataModelExpansions;

namespace Artemis.Plugins.DataModelExpansions.Audio.DataModels {
    public class PluginDataModel : DataModel {
        public PluginDataModel() {
            FFTBins = new List<float>();
        }

        // Your datamodel can have regular properties and you can annotate them if you'd like
        [DataModelProperty(Name = "A test string", Description = "It doesn't do much, but it's there.")]
        public string TemplateDataModelString { get; set; }


        [DataModelProperty(Name = "FFT bins", Description = "holds FFT bins")]
        public List<float> FFTBins { get; set; }
    }
}