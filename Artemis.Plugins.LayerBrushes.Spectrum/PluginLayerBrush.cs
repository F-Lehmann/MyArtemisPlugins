using Artemis.Core.LayerBrushes;
using SkiaSharp;
using Artemis.Plugins.LayerBrushes.Spectrum.PropertyGroups;


namespace Artemis.Plugins.LayerBrushes.Spectrum {
    // This is the layer brush, the plugin has provided it to Artemis via a descriptor
    // Artemis may create multiple instances of it, one instance for each profile element (folder/layer) it is applied to
    public class PluginLayerBrush : LayerBrush<MainPropertyGroup> {
        public override void EnableLayerBrush() {

        }

        public override void DisableLayerBrush() {
        }

        public override void Update(double deltaTime) {
        }

        public override void Render(SKCanvas canvas, SKImageInfo canvasInfo, SKPath path, SKPaint paint) {
        }
    }
}