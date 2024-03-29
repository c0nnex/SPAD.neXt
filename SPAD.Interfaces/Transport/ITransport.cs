﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Transport
{
    /// <summary> Interface for transport layer.  </summary>
    public interface ITransport : IDisposable
    {
        /// <summary>
        /// Connect transport 
        /// </summary>
        /// <returns></returns>
        bool Connect();

        /// <summary>
        /// Disconnect transport 
        /// </summary>
        /// <returns></returns>
        bool Disconnect();

        /// <summary>
        /// Returns connection status
        /// </summary>
        /// <returns>true when connected</returns>
        bool IsConnected();

        /// <summary>
        /// Bytes read over transport
        /// </summary>
        /// <returns></returns>
        byte[] Read();

        /// <summary>
        /// Write bytes over transport
        /// </summary>
        /// <param name="buffer"></param>
        void Write(byte[] buffer);

        /// <summary>
        /// Bytes have been received event. 
        /// </summary>
        event EventHandler DataReceived;
    }
}
