using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public sealed class BooleanEventArgs : EventArgs
    {
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
