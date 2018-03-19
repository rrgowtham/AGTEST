#region -- File Headers --

/*
 * Author: James Deepak
 * Purpose: Status codes for web request response
 * Date: 16-February-2016
 * 
 */

#endregion

#region -- Using Statements --

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace AHP.Core
{
    /// <summary>
    /// Represents the status for the web request performed
    /// </summary>
    public enum WebRequestResponseStatus
    {
        /// <summary>
        /// Request failed for some reason
        /// </summary>
        Failure = 0,
        /// <summary>
        /// Request succeeded
        /// </summary>
        Success = 1,
        /// <summary>
        /// Request is unsupported
        /// </summary>
        Unsupported = 2,
        /// <summary>
        /// Invalid response from host or error occurred
        /// </summary>
        InvalidResponse = 3
    }
}
