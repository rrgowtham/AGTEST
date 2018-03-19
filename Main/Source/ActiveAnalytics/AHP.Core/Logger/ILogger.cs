#region -- File Header --

/*
 * Author: James Deepak
 * Date: February 10 2017
 * Purpose: Shell for implementing logging functionality
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

namespace AHP.Core.Logger
{
    /// <summary>
    /// Provides methods to be implemented by an logger
    /// to store error or warning information in any logger store
    /// </summary>
    public interface IActiveAnalyticsLogger
    {
        /// <summary>
        /// Stores the provided message as error
        /// </summary>
        /// <param name="message">An error information</param>
        /// <param name="optionalError">Exception information that could be associated with the message</param>
        void Error(string message,Exception optionalError = null);

        /// <summary>
        /// Stores the provided message as warning
        /// </summary>
        /// <param name="message">Warning information</param>
        /// <param name="optionalError">Exception information that could be associated with the message</param>
        void Warn(string message, Exception optionalError = null);

        /// <summary>
        /// Stores the provided message as debug
        /// </summary>
        /// <param name="message">Debug information</param>
        /// <param name="optionalError">Exception information that could be associated with the message</param>
        void Debug(string message, Exception optionalError = null);

        /// <summary>
        /// Stores the provided message as information
        /// </summary>
        /// <param name="message">Information message</param>
        /// <param name="optionalError">Exception information that could be associated with the message</param>
        void Info(string message, Exception optionalError = null);

        /// <summary>
        /// Stores the provided information as fatal
        /// </summary>
        /// <param name="message">Fatal error information</param>
        /// <param name="optionalError">Exception information that could be associated with the message</param>
        void Fatal(string message, Exception optionalError = null);

    }
}
