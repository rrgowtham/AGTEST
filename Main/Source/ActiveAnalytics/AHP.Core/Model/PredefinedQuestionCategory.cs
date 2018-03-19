using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Model
{
    /// <summary>
    /// Gets predefined values of security questions and an identifier for the questions
    /// </summary>
    public sealed class PredefinedQuestionCategory
    {
        /// <summary>
        /// Provides an identifier for drop down value favorite teacher for the user
        /// </summary>
        public const string FavoriteTeacher = "FAVTEACHER";

        /// <summary>
        /// Provides an identifier for drop down value birth month and year for the user
        /// </summary>
        public const string MonthYear = "BIRTHMONTHYEAR";

        /// <summary>
        /// Provides an identifier for drop down value zip code for the user
        /// </summary>
        public const string ZipCode = "ZIPCODE";

        /// <summary>
        /// Provides an identifier for drop down value favorite place as kid for the user
        /// </summary>
        public const string FavoritePlace = "FAVPLACE";

    }
}
