using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    public interface IErrorProvider
    {

        bool Success { get; set; }

        List<string> Errors { get; set; }

    }
}
