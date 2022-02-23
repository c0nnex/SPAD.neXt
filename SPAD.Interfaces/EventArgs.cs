using SPAD.neXt.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public delegate void SPADEventHandler<TSender,TArgument>(TSender sender, TArgument e);
    public delegate void SPADEventHandler(object sender, ISPADEventArgs e);

    public sealed class BooleanEventArgs : EventArgs
    {
        public static readonly BooleanEventArgs True = new BooleanEventArgs(true);
        public static readonly BooleanEventArgs False = new BooleanEventArgs(false);
        public bool Value { get; }
        public BooleanEventArgs()
        {

        }
        public BooleanEventArgs(bool value)
        {
            Value = value;
        }
    }

    public sealed class ValidationEventArgs : EventArgs
    {
        public bool IsValid { get; set; }
        public object Value { get; }
        public string Error { get; set; }

        public ValidationEventArgs()
        {

        }

        public ValidationEventArgs(object value) : this()
        {
            Value = value;
        }
    }
}
