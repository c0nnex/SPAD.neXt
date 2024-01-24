using System;
using System.Collections.Generic;
using System.Threading;

namespace SPAD.Interfaces.Remote
{
    public sealed class RemoteError
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class RemoteAction
    {
        private static long _ActionIdCounter = 0;
        public string Type { get; set; }
        public string Tag { get; set; }
        public long Id { get; set; }
        public RemoteError Error { get; set; } = null;
        public RemoteAction()
        {
            Type = this.GetType().Name;
            Id = Interlocked.Increment(ref _ActionIdCounter);
        }

        public RemoteAction(string tag) : this()
        {
            Tag = tag;
        }

        public RemoteAction(string tag, string type) : this() 
        {
            Type = type;
            Tag = tag;
        }

        public RemoteAction WithError(int code, string message)
        {
            Error = new RemoteError() { ErrorCode = code, ErrorMessage = message };
            return this;
        }
    }

    public class RemoteQuery<T> : RemoteAction 
    {
        public int Id { get; set; }
        public List<T> QueryResults { get; set; } = null;
    }

    public class RemotePage
    {
        public string PageName { get; set; }
        public Guid PageId { get; set; }
        public bool IsActivePage { get; set; }
        public bool IsDefaultPage { get; set; }
    }

    public class RemoteQueryPages : RemoteQuery<RemotePage>
    { }

    public sealed class RemoteImage : RemoteAction
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public byte[] Image { get; set; }

        public RemoteImage() : base()
        {
        }

        public RemoteImage(string tag, int width, int height, byte[] image) : base(tag)
        {
            Width = width;
            this.Height = height;
            Image = image;
        }
    }

    public sealed class RemoteStateEvent : RemoteAction
    {
        public int State { get; set; }
        public RemoteStateEvent() : base()
        {
        }

        public RemoteStateEvent(String tag, int state) : base(tag)
        {
            State = state;
        }

    }

    public sealed class RemoteEvent : RemoteAction
    {
        public string Trigger { get; set; }
        public int Value { get; set; } = 0;
        public string Parameter { get; set; } = null;

        public RemoteEvent() : base()
        {
        }

        public RemoteEvent(String tag, String trigger) : base(tag)
        {
            Trigger = trigger;
        }

        public RemoteEvent(String tag, String trigger, int value) : base(tag)
        {
            Trigger = trigger;
            Value = value;
        }
        public RemoteEvent(String tag, String trigger, string parameter) : base(tag)
        {
            Tag = tag;
            Trigger = trigger;
            Parameter = parameter;
        }
    }
}
