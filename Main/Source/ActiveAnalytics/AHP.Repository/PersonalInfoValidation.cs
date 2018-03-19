//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using AHP.Core.Repository;
//using AHP.Core.Model;
//using System.Reflection;
//using AHP.Core.Logger;

//namespace AHP.Repository
//{
//   public class PersonalInfoValidation: IPersonalInfoValidation
//    {
//        #region -- Members --

//        private readonly IActiveAnalyticsLogger _logger;

//        private readonly StringBuilder _logMessages;

//        #endregion

//        #region -- Constructors --

//        public PersonalInfoValidation(IActiveAnalyticsLogger logger)
//        {
//            _logger = logger;
//            _logMessages = new StringBuilder();
//        }

//        #endregion

//        #region -- IPersonalInfoValidation Implementation --
//        public bool IsPersonalInfoValidationSuccess(PersonalInfoQuestion personalInfoQuestion)
//        {
//            bool isValidated = false;
//            using (ActiveAnalyticsOracleDatabaseContext context = new ActiveAnalyticsOracleDatabaseContext())
//            {
//                try
//                {
//                    _logMessages.Append("Checking for any personal info questions.");
//                    if (context.PersonalInfoQuestions.Any())
//                    {
//                        _logMessages.Append("Personal info questions exists.");
//                        var answers = context.PersonalInfoQuestions.First(a => a.UserName.Equals(personalInfoQuestion.UserName,StringComparison.OrdinalIgnoreCase));
//                        if (answers == null)
//                        {
//                            _logMessages.Append("No Answers setup for the user.");
//                            isValidated = false;
//                        }
//                        else
//                        {
//                            _logMessages.Append("Iterating every answer to check all answers are correct.");
//                            List<PropertyInfo> differences = new List<PropertyInfo>();
//                            PropertyInfo[] personalQuestionProperties = personalInfoQuestion.GetType().GetProperties();
//                            foreach (PropertyInfo property in personalQuestionProperties)
//                            {
//                                object value1 = property.GetValue(personalInfoQuestion, null);
//                                object value2 = property.GetValue(answers, null);

//                                if (value1 is string)
//                                {
//                                    value1 = value1.ToString().ToUpper(System.Globalization.CultureInfo.InvariantCulture);
//                                    value2 = value2.ToString().ToUpper(System.Globalization.CultureInfo.InvariantCulture);

//                                    if (!value1.Equals(value2))
//                                    {
//                                        differences.Add(property);
//                                        isValidated = false;
//                                        _logMessages.Append("One of the answers is incorrect.");
//                                        _logger.Info(_logMessages.ToString());
//                                        return isValidated;
//                                    }
//                                    else
//                                    {
//                                        isValidated = true;
//                                    }
//                                }
//                                else
//                                {
//                                    isValidated = false;
//                                }                               
//                            }                            
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    _logger.Error("An error occurred getting personal questions and validating for password reset.", ex);
//                    isValidated = false;
//                }
//            }
//            if (isValidated)
//            {
//                _logMessages.Append("All answers are correct.");
//            }
//            _logger.Info(_logMessages.ToString());
//            return isValidated;

//        } 
//        #endregion

//    }
//}
