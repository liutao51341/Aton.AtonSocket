using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core
{
    /// <summary>
    /// server status
    /// </summary>
    public enum ServerStatus : int
    {
        /// <summary>
        /// not init
        /// </summary>
        NotInit =1,
        /// <summary>
        /// initialized
        /// </summary>
        initialized,
        /// <summary>
        /// running
        /// </summary>
        Running,
        /// <summary>
        /// stopped
        /// </summary>
        Stopped
    }
}
