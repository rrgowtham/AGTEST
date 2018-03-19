using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AHP.Web.Models
{
    /// <summary>
    /// Generic Ajax Response for ajax calls
    /// </summary>
    /// <typeparam name="T">Data to </typeparam>
    public class GenericAjaxResponse<T>:AHP.Core.Model.GenericResponse<T>
    {

        /// <summary>
        /// An Url to redirect to, when the session timesout
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="GenericAjaxResponse{T}"/>
        /// </summary>
        public GenericAjaxResponse()
        {
            Data = default(T);
            Errors = new List<string>();
            Success = false;
            RedirectUrl = string.Empty;
        }
    }
}