using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    /// <summary>
    /// Defines response for different layers with error information
    /// </summary>
    /// <typeparam name="T">Generic open type</typeparam>
    public class GenericResponse<T>
    {
        /// <summary>
        /// Data response
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// List of errors on the operation
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// True when operation succeeded otherwise false
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="GenericResponse{T}"/> class
        /// </summary>
        public GenericResponse()
        {
            Success = false;
            Errors = new List<string>();           
            Data = default(T); 
        }
    }
}
