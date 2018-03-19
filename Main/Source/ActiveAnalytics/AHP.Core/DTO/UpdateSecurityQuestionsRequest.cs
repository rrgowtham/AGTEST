using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.DTO
{
    public class UpdateSecurityQuestionsRequest
    {

        public string Username { get; set; }

        public List<Core.DTO.UserSecurityOption> SecurityQuestions { get; set; }

    }
}
