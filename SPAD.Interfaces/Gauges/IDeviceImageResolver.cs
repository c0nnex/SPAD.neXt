using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.Profile;
using System;
using System.Collections.Generic;
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

    public interface ISimpleGauge : IVersionableObject, IXmlAnyObject
    {
        bool Disabled { get; set; }
        bool IsConfigured { get; }
        T GetLayer<T>(int layerNumber) where T : ISimpleGaugeLayerConfig;
        IReadOnlyList<ISimpleGaugeLayerConfig> GetLayers();
        void SetParentID(Guid newParentID);

        ISimpleGaugeLayerConfig AddImageLayer(int layerNumber, Guid imageId);
        ISimpleGaugeLayerConfig AddTextLayer(int layerNumber, string text, ICustomLabel textConfig);
        IGaugeRenderer CreateRenderer(string belongsTo, int sizeX, int sizeY, IDeviceImageResolver deviceImageResolver);
    }

    public interface ISimpleGaugeLayerConfig
    {
        bool IsDisabled { get; }
        string LayerName { get; }
        int LayerNumber { get; }
        GaugeLayerType LayerType { get; }

        void SetName(string name);
        T As<T>() where T : ISimpleGaugeLayerConfig;
    }

    public interface IGaugeColorLayerConfig : ISimpleGaugeLayerConfig
    {
        string Color { get; set; }
    }
    public interface IGaugeImageLayerConfig : ISimpleGaugeLayerConfig
    {
        Guid ImageId { get; set; }
    }

    public interface IGaugeTextLayerConfig : ISimpleGaugeLayerConfig,ICustomLabel
    {
    }


    public interface IGaugeRenderer : IDisposable
    {
        bool IsDirty { get; }
        bool IsInDesignMode { get; set; }
        GaugeSize Size { get; }

        void AddLayer(GaugeLayerType layerType, int layerNumber, object renderObject, object renderParam = null);
        IGaugeRenderLayer GetLayer(int layerNumber);
        bool RenderAsPixels(Action<byte[]> renderCompletedCallback, Func<object, IGaugeRenderLayer, bool> renderLayerCallback = null);
        bool RenderAsImage(Action<byte[]> renderCompletedCallback, Func<object, IGaugeRenderLayer, bool> renderLayerCallback = null);

        void UpdateColorLayer(int layerNumber, string newColor);
        void UpdateImageLayer(int layerNumber, Guid newImage);
        void UpdateTextLayer(int layerNumber, ICustomLabel newTextConfig);
        void UpdateTextLayer(int layerNumber, string newText, string foreground = null, string background = null);
        void DisableLayer(int layerNumber);
        void EnableLayer(int layerNumber);
        bool HasLayer(int layerNumber);
    }

    public interface IGaugeRenderLayer : IDisposable
    {
        int LayerNumber { get; }
        GaugeLayerType LayerType { get; }
        bool IsDisabled { get; }
        void Disable();
        void Enable();
        bool Render(object renderTarget);
        void Initialize(GaugeSize size);
    }

    public interface IGaugeImageLayer : IGaugeRenderLayer
    {
        void Render(object renderTarget, IDeviceImage image, int rotation = 0);
    }

    public interface IGaugeColorLayer : IGaugeRenderLayer
    {
        void Render(object g, string color);
    }

    public interface IGaugeTextLayer : IGaugeRenderLayer
    {
        bool Render(object g, string text, ICustomLabel targetLabelConfig = null);
        void UpdateRenderColors(string foreground, string background);
    }
}
