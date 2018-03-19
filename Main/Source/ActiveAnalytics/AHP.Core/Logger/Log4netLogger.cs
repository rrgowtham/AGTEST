#region -- File Header --

/*
 * Author: James Deepak
 * Date: 10 February 2017
 * Purpose: Implements logger functionality for active analytics
 * 
 */

#endregion

#region -- Using Statements --
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace AHP.Core.Logger
{
    public class Log4netLogger : IActiveAnalyticsLogger
    {

        #region -- Members --
        private readonly ILog _logger;
        #endregion

        #region -- Constructors --
        public Log4netLogger(string loggerType)
        {
            _logger = LogManager.GetLogger(loggerType);

        }
        #endregion

        #region -- Interface Implementation --

        public void Debug(string message, Exception optionalError = null)
        {
            if (optionalError == null)
                _logger.Debug(message);
            else
                _logger.Debug(message, optionalError);
        }

        public void Error(string message, Exception optionalError = null)
        {
            if (optionalError == null)
                _logger.Error(message);
            else
                _logger.Error(message, optionalError);
        }

        public void Fatal(string message, Exception optionalError = null)
        {
            if (optionalError == null)
                _logger.Fatal(message);
            else
                _logger.Fatal(message, optionalError);
        }

        public void Info(string message, Exception optionalError = null)
        {
            if (optionalError == null)
                _logger.Info(message);
            else
                _logger.Info(message, optionalError);
        }

        public void Warn(string message, Exception optionalError = null)
        {
            if (optionalError == null)
                _logger.Warn(message);
            else
                _logger.Warn(message, optionalError);
        } 

        #endregion

    }
}
