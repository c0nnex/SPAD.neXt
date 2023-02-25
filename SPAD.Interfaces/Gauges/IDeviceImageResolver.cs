using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.Extension;
using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

namespace SPAD.neXt.Interfaces
{
    public interface IDeviceImageResolver
    {
        IDeviceImage ResolveImage(Guid imgId);
    }

    public enum GaugeLayerType
    {
        NONE,
        COLOR,
        IMAGE,
        TEXT
    }

    /* Important: Don't forget to adjust GaugeImageLayer-Rendering if modes are added */
    public enum GaugeImageDistortionMode
    {
        NONE,
        ROTATE,
        SHIFT_X,
        SHIFT_Y,
        FLIP_X,
        FLIP_Y,
        FLIP_XY
        //        ROTATESHIFT,
        //        SHIFTROTATE,
    }

    public enum GaugeImageSizeMode
    {
        NONE,
        STRETCH,
        SIZE
    }

    public sealed class GaugeImageDistortion : GenericOptionObject
    {
        [XmlAttribute("DistortionType")]
        public GaugeImageDistortionMode GaugeImageDistortionType { get; set; } = GaugeImageDistortionMode.NONE;
    }

    public sealed class GaugeSize
    {        
        [XmlAttribute]
        public int Width { get; set; }
        [XmlAttribute]
        public int Height { get; set; }

        public GaugeSize(int sizeX, int sizeY)
        {
            Width = sizeX;
            Height = sizeY;
        }

    }

    public interface ISimpleGauge : IHasImageReferences,IVersionableObject, IXmlAnyObject, ICloneable<ISimpleGauge>
    {
        bool Disabled { get; set; }
        bool IsConfigured { get; }
        bool IsTurnedOn { get; set; }
        bool DefaultsToOff { get; set; }
        IGaugeRenderer Renderer { get; }
        T GetLayer<T>(int layerNumber) where T : ISimpleGaugeLayerConfig;
        IReadOnlyList<ISimpleGaugeLayerConfig> GetLayers();
        void SetParentID(Guid newParentID);
        void Reconfigure(IEnumerable<ISimpleGaugeLayerConfig> layers);
        IGaugeImageLayerConfig AddImageLayer(int layerNumber, Guid imageId);
        IGaugeTextLayerConfig AddTextLayer(int layerNumber, string text, ICustomLabel textConfig);
        IGaugeRenderer CreateRenderer(string belongsTo, int sizeX, int sizeY, IDeviceImageResolver deviceImageResolver);
    }

    public interface ISimpleGaugeLayerConfig : ICloneable
    {
        bool IsDisabled { get; }
        string LayerName { get; set; }
        string LayerSelectionName { get; }
        int LayerNumber { get; set; }

        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
       
        GaugeLayerType LayerType { get; }

        void SetName(string name);
        T As<T>() where T : ISimpleGaugeLayerConfig;
        [Browsable(false)]
        object RenderObject { get; }
        void SetRenderParameter(object value);
        object GetRenderParameter();

    }
    public interface IGaugeNoPositionEdit { }
    public interface IGaugeColorLayerConfig : ISimpleGaugeLayerConfig, IGaugeNoPositionEdit
    {
        string Color { get; set; }
    }
    public interface IGaugeImageLayerConfig : ISimpleGaugeLayerConfig
    {
        Guid ImageId { get; set; }
        GaugeImageSizeMode SizeMode { get; set; }
        GaugeImageDistortionMode DistortionMode { get; set; }
        int AxisX { get; set; }
        int AxisY { get; set; }
        int MaxRotationValue { get; set; }
        int MaxValue { get; set; }
        int MinRotationValue { get; set; }
        int MinValue { get; set; }
        int PointsTo { get; set; }
        string RotationExpressionText { get; set; }
    }

    public interface IGaugeTextLayerConfig : ISimpleGaugeLayerConfig,ICustomLabel
    {
    }


    public interface IGaugeRenderer : IDisposable
    {
        bool IsDirty { get; }
        bool IsInDesignMode { get; set; }
        GaugeSize Size { get; }

        void SetRenderRequestCallback(Action<IGaugeRenderer> renderingRequested);
        IDeviceImageResolver DeviceImageResolver { get; }
        void Reconfigure(IEnumerable<ISimpleGaugeLayerConfig> layers);
        IGaugeRenderLayer AddLayer(ISimpleGaugeLayerConfig config);
        IGaugeRenderLayer GetLayer(int layerNumber);
        bool RenderAsPixels(Action<byte[]> renderCompletedCallback, Func<object, IGaugeRenderLayer, bool> renderLayerCallback = null);
        bool RenderAsImage(Action<byte[]> renderCompletedCallback, Func<object, IGaugeRenderLayer, bool> renderLayerCallback = null);

        void UpdateColorLayer(int layerNumber, int layerScope, string newColor);
        void UpdateImageLayer(int layerNumber, Guid newImage);
        void UpdateTextLayer(int layerNumber, string newText);
        void DisableLayer(int layerNumber);
        void EnableLayer(int layerNumber);
        bool HasLayer(int layerNumber);

        void SetDirty();
    }

    public interface IGaugeRenderLayer : IDisposable
    {
        int LayerNumber { get; }
        GaugeLayerType LayerType { get; }
        bool IsDisabled { get; }
        void Disable();
        void Enable();
        bool Render(object renderTarget);
        bool RenderDesign(object renderTarget, object renderObject, object renderParameter);
        void RenderBorder(object renderTarget);
        void Initialize(IObserverTicket observerTicket, ISimpleGaugeLayerConfig config, int defaultWidth, int defaultHeight);
    }

    public interface IGaugeImageLayer : IGaugeRenderLayer
    {
    }

    public interface IGaugeColorLayer : IGaugeRenderLayer
    {
    }

    public interface IGaugeTextLayer : IGaugeRenderLayer
    {
        void UpdateRenderColors(string foreground, string background);
    }
}
