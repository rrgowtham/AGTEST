using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHP.Core.Model;
using AHP.Crypto;
using AHP.Core.Logger;
using AHP.Core.DTO;
using AHP.Core;

namespace AHP.Repository
{
    public class UserinfoManager : IUserinfoManager
    {

        #region -- Members --

        private readonly IActiveAnalyticsLogger _logger;

        private readonly StringBuilder _loggerMessages;

        private AHP.Crypto.Interfaces.ICryptoService _cryptoService;

        private AHP.Repository.IAuditEventManager _auditEventManager;

        private const string InternalUserType = "INTERNAL";

        private const string ExternalUserType = "EXTERNAL";

        #endregion

        #region -- Constructors --

        public UserinfoManager(IActiveAnalyticsLogger logger
            , AHP.Crypto.Interfaces.ICryptoService cryptoService
            , IAuditEventManager auditEventManager)
        {
            _logger = logger;
            _loggerMessages = new StringBuilder();
            _cryptoService = cryptoService;
            _auditEventManager = auditEventManager;
        }

        #endregion

        #region -- IUserinfoManager Members --

        //public bool IsPersonalInfoValidationSuccess(string username, string birthYearMonth, string zipCode, string favTeacher, string favPlaceAsKid)
        //{
        //    bool success = false;
        //    PersonalInfoQuestion usrSecurityQuestion = null;
        //    _loggerMessages.Append("Personal info validation for password reset.");
        //    try
        //    {
        //        if (string.IsNullOrEmpty(username))
        //        {
        //            _loggerMessages.Append("Username provided was empty. Terminating further processing.");
        //            _logger.Info(_loggerMessages.ToString());
        //            return false;
        //        }

        //        _loggerMessages.Append("Retrieving user info from database.");
        //        using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
        //        {
        //            usrSecurityQuestion = dbContext.PersonalInfoQuestions.FirstOrDefault(usrInfo => usrInfo.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        //            if (usrSecurityQuestion == null)
        //            {
        //                return false;
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _loggerMessages.AppendFormat("Error occurred during database operation. Error Message {0}.", ex.Message);
        //        _logger.Info(_loggerMessages.ToString());
        //        return false;
        //    }

        //    int numberOfAnswersProvided = 0;

        //    if (!string.IsNullOrEmpty(birthYearMonth))
        //    {
        //        if (usrSecurityQuestion.MonthYear.Equals(birthYearMonth, StringComparison.OrdinalIgnoreCase))
        //        {
        //            numberOfAnswersProvided = numberOfAnswersProvided + 1;
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(zipCode))
        //    {
        //        if (usrSecurityQuestion.ZipCode.Equals(zipCode, StringComparison.OrdinalIgnoreCase))
        //        {
        //            numberOfAnswersProvided = numberOfAnswersProvided + 1;
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(favPlaceAsKid))
        //    {
        //        if (usrSecurityQuestion.FavPlaceAsChild.Equals(favPlaceAsKid, StringComparison.OrdinalIgnoreCase))
        //        {
        //            numberOfAnswersProvided = numberOfAnswersProvided + 1;
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(favTeacher))
        //    {
        //        if (usrSecurityQuestion.FavTeacher.Equals(favTeacher, StringComparison.OrdinalIgnoreCase))
        //        {
        //            numberOfAnswersProvided = numberOfAnswersProvided + 1;
        //        }
        //    }

        //    return numberOfAnswersProvided == 2;
        //}

        //public UserSetupInfo GetUserSetupInfo(string username)
        //{
        //    UserSetupInfo response = new UserSetupInfo()
        //    {
        //        AreSecurityquestionsPresent = false,
        //        IsEmailPresent = false
        //    };
        //    int securityAnswerCount = 0;
        //    _loggerMessages.Append("Looking up oracle db for user's information");
        //    try
        //    {
        //        if (string.IsNullOrEmpty(username))
        //        {
        //            response.Success = false;
        //            response.Errors.Add("Username for the user info cannot be empty");
        //            return response;
        //        }
        //        using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
        //        {
        //            //check for prescence of email
        //            UserInfo userInfo = dbContext.UserInfo.FirstOrDefault(usrInfo => usrInfo.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        //            if (userInfo == null)
        //            {
        //                response.IsEmailPresent = false;
        //            }
        //            else
        //            {
        //                response.IsEmailPresent = !string.IsNullOrEmpty(userInfo.Email);
        //            }

        //            //check for prescence of security questions and answers
        //            PersonalInfoQuestion infoQuestion = dbContext.PersonalInfoQuestions.FirstOrDefault(usrInfo => usrInfo.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        //            if (infoQuestion == null)
        //            {
        //                _loggerMessages.Append("user information not present in the database.");
        //                response.AreSecurityquestionsPresent = false;
        //            }
        //            else
        //            {
        //                _loggerMessages.Append("user information present in the database.Checking if answers are present.");
        //                if (!string.IsNullOrEmpty(infoQuestion.FavPlaceAsChild))
        //                {
        //                    _loggerMessages.Append("user has input favorite place as child.");
        //                    securityAnswerCount = securityAnswerCount + 1;
        //                }

        //                if (!string.IsNullOrEmpty(infoQuestion.FavTeacher))
        //                {
        //                    _loggerMessages.Append("user has input favorite teacher.");
        //                    securityAnswerCount = securityAnswerCount + 1;
        //                }

        //                if (!string.IsNullOrEmpty(infoQuestion.MonthYear))
        //                {
        //                    _loggerMessages.Append("user has input month and year of birth.");
        //                    securityAnswerCount = securityAnswerCount + 1;
        //                }

        //                if (!string.IsNullOrEmpty(infoQuestion.ZipCode))
        //                {
        //                    _loggerMessages.Append("user has input zipcode of his location.");
        //                    securityAnswerCount = securityAnswerCount + 1;
        //                }
        //                //at the least two security questions needs to be answered
        //                response.AreSecurityquestionsPresent = securityAnswerCount >= 2;
        //            }
        //        }
        //        response.Success = true;
        //        response.Errors.Clear();
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        _loggerMessages.AppendFormat("Error occurred looking user information. Error Info : {0}.", ex.Message);
        //        response.IsEmailPresent = false;
        //        response.AreSecurityquestionsPresent = false;
        //    }
        //    _logger.Info(_loggerMessages.ToString());
        //    return response;
        //}

        //public bool SetupSecurityAnswers(string username, string birthYearMonth, string zipCode, string favTeacher, string favPlaceAsKid)
        //{
        //    if (string.IsNullOrEmpty(username))
        //    {
        //        return false;
        //    }

        //    int numberOfAnswersProvided = 0;

        //    try
        //    {
        //        //at the least two answers must be provided
        //        if (!string.IsNullOrEmpty(birthYearMonth))
        //        {
        //            numberOfAnswersProvided = numberOfAnswersProvided + 1;
        //        }

        //        if (!string.IsNullOrEmpty(zipCode))
        //        {
        //            numberOfAnswersProvided = numberOfAnswersProvided + 1;
        //        }

        //        if (!string.IsNullOrEmpty(favPlaceAsKid))
        //        {
        //            numberOfAnswersProvided = numberOfAnswersProvided + 1;
        //        }

        //        if (!string.IsNullOrEmpty(favTeacher))
        //        {
        //            numberOfAnswersProvided = numberOfAnswersProvided + 1;
        //        }

        //        if (numberOfAnswersProvided != 2)
        //        {
        //            //Only two questions are mandated to be filled in.
        //            return false;
        //        }

        //        using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
        //        {
        //            PersonalInfoQuestion infoQuestion = dbContext.PersonalInfoQuestions.FirstOrDefault(usrInfo => usrInfo.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        //            if (infoQuestion == null)
        //            {
        //                infoQuestion = new PersonalInfoQuestion();
        //                infoQuestion.UserName = username;
        //                infoQuestion.FavPlaceAsChild = favPlaceAsKid;
        //                infoQuestion.FavTeacher = favTeacher;
        //                infoQuestion.ZipCode = zipCode;
        //                infoQuestion.MonthYear = birthYearMonth;
        //                dbContext.PersonalInfoQuestions.Add(infoQuestion);
        //            }
        //            else
        //            {
        //                //changes to existing entity will be saved when savechanges is called
        //                infoQuestion.FavPlaceAsChild = favPlaceAsKid;
        //                infoQuestion.FavTeacher = favTeacher;
        //                infoQuestion.ZipCode = zipCode;
        //                infoQuestion.MonthYear = birthYearMonth;
        //            }
        //            return dbContext.SaveChanges() == 1;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //public bool UpsertEmailAddress(string username, string emailAddress, bool isEmailvalid)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(username))
        //        {
        //            return false;
        //        }

        //        if (string.IsNullOrEmpty(emailAddress))
        //        {
        //            return false;
        //        }

        //        using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
        //        {
        //            UserInfo userInformation = dbContext.UserInfo.FirstOrDefault(usrInfo => usrInfo.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        //            if (userInformation == null)
        //            {
        //                //insert
        //                userInformation = new UserInfo()
        //                {
        //                    Email = emailAddress,
        //                    Username = username,
        //                    IsEmailActivated = isEmailvalid ? "Y" : "N"
        //                };
        //                dbContext.UserInfo.Add(userInformation);
        //            }
        //            else
        //            {
        //                //update
        //                userInformation.Email = emailAddress;
        //                userInformation.IsEmailActivated = isEmailvalid ? "Y" : "N";
        //            }
        //            return dbContext.SaveChanges() == 1;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Info("Error occurred while updating/inserting users information", ex);
        //    }
        //    return false;
        //}

        public GenericResponse<List<AHP.Core.DTO.ExternalUserInfo>> ListUsers()
        {
            GenericResponse<List<AHP.Core.DTO.ExternalUserInfo>> usrInfo = new GenericResponse<List<ExternalUserInfo>>();
            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    usrInfo.Data = dbContext.UserInfo.ToList().Select(dbUser => new ExternalUserInfo()
                    {
                        ChangePasswordOnLogon = dbUser.ChangePasswordOnLogon.Equals("Y", StringComparison.OrdinalIgnoreCase),
                        Email = dbUser.Email.ToLower(),
                        Firstname = dbUser.FirstName,
                        IsEmailActivated = dbUser.IsEmailActivated.Equals("Y", StringComparison.OrdinalIgnoreCase),
                        IsLocked = dbUser.IsLocked.Equals("Y", StringComparison.OrdinalIgnoreCase),
                        Lastname = dbUser.LastName,
                        IsActive = dbUser.IsUserActive.Equals("Y", StringComparison.OrdinalIgnoreCase),
                        Company = dbUser.CompanyName.ToLower(),
                        Role = dbUser.Role,
                        SupplierId = dbUser.SupplierIds,
                        Username = dbUser.Username.ToLower(),
                        BirthMonth = short.Parse(string.IsNullOrEmpty(dbUser.MonthYear) ? "0" : dbUser.MonthYear.Split('/')[0]),
                        BirthYear = short.Parse(string.IsNullOrEmpty(dbUser.MonthYear) ? "0" : dbUser.MonthYear.Split('/')[1]),
                        ZipCode = dbUser.ZipCode,
                        CreatedBy = dbUser.CreatedBy,
                        CreatedDate = dbUser.CreatedDate.HasValue ? dbUser.CreatedDate.Value.ToShortDateString(): string.Empty,
                        LastLogonDate = dbUser.LastLoginDate.HasValue ? dbUser.LastLoginDate.Value.ToShortDateString():string.Empty
                    }).ToList();
                    usrInfo.Success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Error occurred retrieving user info " + ex.Message);
                usrInfo.Success = false;
                usrInfo.Errors.Add("Could not retrieve user information. Please try again");
            }
            return usrInfo;
        }

        public GenericResponse<bool> CreateUser(ExternalUserInfo externalUser)
        {
            //By default success is false
            GenericResponse<bool> response = new GenericResponse<bool>();
            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    //Check for existing username
                    if (externalUser == null)
                    {
                        response.Errors.Add("Please provide required user information.");
                        return response;
                    }

                    if (string.IsNullOrEmpty(externalUser.Username))
                    {
                        response.Errors.Add("Username is required");
                        return response;
                    }

                    UserInfo dbUser = dbContext.UserInfo.FirstOrDefault(user => user.Username.ToUpper().Equals(externalUser.Username.ToUpper(), StringComparison.OrdinalIgnoreCase));
                    if (dbUser != null)
                    {
                        response.Errors.Add("User by that username already exists");
                        return response;
                    }

                    //Check for existing email address
                    dbUser = dbContext.UserInfo.FirstOrDefault(user => user.Email.ToUpper().Equals(externalUser.Email.ToUpper(), StringComparison.OrdinalIgnoreCase));
                    if (dbUser != null)
                    {
                        response.Errors.Add("User by that email already exists");
                        return response;
                    }

                    if (string.IsNullOrEmpty(externalUser.Firstname))
                    {
                        response.Errors.Add("First name is required");
                        return response;
                    }

                    if (string.IsNullOrEmpty(externalUser.Lastname))
                    {
                        response.Errors.Add("Last name is required");
                        return response;
                    }

                    if (string.IsNullOrEmpty(externalUser.Role))
                    {
                        response.Errors.Add("Please specify role for the user");
                        return response;
                    }

                    if (!externalUser.Role.Equals("admin", StringComparison.OrdinalIgnoreCase) && !externalUser.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Errors.Add("Please specify either admin or user role.");
                        return response;
                    }

                    if (string.IsNullOrEmpty(externalUser.SupplierId))
                    {
                        response.Errors.Add("Please specify supplier id for the user");
                        return response;
                    }

                    dbUser = new UserInfo();
                    dbUser.FirstName = externalUser.Firstname;
                    dbUser.LastName = externalUser.Lastname;
                    dbUser.Email = externalUser.Email.ToLower();
                    dbUser.ChangePasswordOnLogon = "Y";
                    //Email is always activated for new user.
                    dbUser.IsEmailActivated = "Y";
                    dbUser.IsLocked = "Y";
                    dbUser.Role = externalUser.Role;
                    dbUser.SupplierIds = externalUser.SupplierId;
                    dbUser.Username = externalUser.Username.ToLower();
                    dbUser.IsUserActive = "Y";
                    dbUser.CompanyName = externalUser.Company;
                    dbUser.InvalidLogonAttemptCount = 0;
                    dbUser.CreatedDate = DateTime.Now;
                    dbUser.CreatedBy = externalUser.CreatedBy;
                    //account information
                    dbUser.ZipCode = externalUser.ZipCode;
                    dbUser.MonthYear = string.Format("{0}/{1}", externalUser.BirthMonth, externalUser.BirthYear);
                    dbUser.PasswordExpiresOn = DateTime.Now.AddDays(180);
                    dbContext.UserInfo.Add(dbUser);
                    response.Success = response.Data = dbContext.SaveChanges() == 1;
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Error occurred creating new user. Please try again");
                _logger.Error("An Error occurred creating new user.", ex);
            }
            return response;
        }

        public GenericResponse<ExternalUserInfo> GetUserDetails(string username)
        {
            //By default success is false
            GenericResponse<ExternalUserInfo> response = new GenericResponse<ExternalUserInfo>()
            {
                Data = new ExternalUserInfo()
            };
            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {

                    if (string.IsNullOrEmpty(username))
                    {
                        response.Errors.Add("Username is required");
                        return response;
                    }

                    UserInfo dbUser = dbContext.UserInfo.FirstOrDefault(user => user.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase));
                    if (dbUser == null)
                    {
                        response.Errors.Add("User by that username does not exists");
                        return response;
                    }

                    response.Data.Firstname = dbUser.FirstName;
                    response.Data.Lastname = dbUser.LastName;
                    response.Data.Email = dbUser.Email.ToLower();
                    response.Data.ChangePasswordOnLogon = dbUser.ChangePasswordOnLogon.Equals("Y", StringComparison.OrdinalIgnoreCase);
                    response.Data.IsEmailActivated = dbUser.IsEmailActivated.Equals("Y", StringComparison.OrdinalIgnoreCase);
                    response.Data.IsLocked = dbUser.IsLocked.Equals("Y", StringComparison.OrdinalIgnoreCase);
                    response.Data.Role = dbUser.Role;
                    response.Data.SupplierId = dbUser.SupplierIds;
                    response.Data.Username = dbUser.Username.ToLower();
                    response.Data.Company = dbUser.CompanyName;
                    response.Data.IsActive = dbUser.IsUserActive.Equals("Y", StringComparison.OrdinalIgnoreCase);
                    response.Data.BirthMonth = short.Parse(string.IsNullOrEmpty(dbUser.MonthYear) ? "0" : dbUser.MonthYear.Split('/')[0]);
                    response.Data.BirthYear = short.Parse(string.IsNullOrEmpty(dbUser.MonthYear) ? "0" : dbUser.MonthYear.Split('/')[1]);
                    response.Data.ZipCode = dbUser.ZipCode;
                    response.Data.InvalidLogonAttemptCount = dbUser.InvalidLogonAttemptCount;
                    response.Data.CreatedBy = dbUser.CreatedBy;
                    response.Data.CreatedDate = dbUser.CreatedDate.HasValue ? dbUser.CreatedDate.Value.ToShortDateString():string.Empty;
                    response.Data.LastLogonDate = dbUser.LastLoginDate.HasValue ? dbUser.LastLoginDate.Value.ToShortDateString() : string.Empty;
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Error occurred creating new user. Please try again");
                _logger.Error("An Error occurred creating new user.", ex);
            }
            return response;
        }

        public GenericResponse<bool> UnlockUserAccount(string username, string email)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            if (string.IsNullOrEmpty(username))
            {
                response.Success = false;
                response.Errors.Add("Username is required");
                return response;
            }

            if (string.IsNullOrEmpty(email))
            {
                response.Success = false;
                response.Errors.Add("Email is required");
                return response;
            }

            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    UserInfo user = dbContext.UserInfo.FirstOrDefault(usr => usr.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase) && usr.Email.ToUpper().Equals(email.ToUpper(), StringComparison.OrdinalIgnoreCase));
                    if (user == null)
                    {
                        response.Success = false;
                        response.Errors.Add("User could not be found");
                        return response;
                    }
                    //activate the user
                    user.IsLocked = "N";
                    user.InvalidLogonAttemptCount = 0;

                    //1 records must be updated
                    response.Data = response.Success = dbContext.SaveChanges() == 1;
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Deactivate user on username " + username + " failed. Exception message " + ex.Message);
                response.Success = false;
                response.Errors.Add("An error occurred activating user account.");
            }
            return response;
        }

        public GenericResponse<bool> LockUserAccount(string username, string email)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            if (string.IsNullOrEmpty(username))
            {
                response.Success = false;
                response.Errors.Add("Username is required");
                return response;
            }

            if (string.IsNullOrEmpty(email))
            {
                response.Success = false;
                response.Errors.Add("Email is required");
                return response;
            }

            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    UserInfo user = dbContext.UserInfo.FirstOrDefault(usr => usr.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase) && usr.Email.ToUpper().Equals(email.ToUpper(), StringComparison.OrdinalIgnoreCase));
                    if (user == null)
                    {
                        response.Success = false;
                        response.Errors.Add("User could not be found");
                        return response;
                    }
                    //deactivate the user
                    user.IsLocked = "Y";

                    //1 records must be updated
                    response.Data = response.Success = dbContext.SaveChanges() == 1;
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Deactivate user on username " + username + " failed. Exception message " + ex.Message);
                response.Success = false;
                response.Errors.Add("An error occurred deactivating user account.");
            }
            return response;
        }

        public GenericResponse<bool> ActivateEmail(string username, string email, bool activate = false)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            if (string.IsNullOrEmpty(username))
            {
                response.Success = false;
                response.Errors.Add("Username is required");
                return response;
            }

            if (string.IsNullOrEmpty(email))
            {
                response.Success = false;
                response.Errors.Add("Email is required");
                return response;
            }

            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    UserInfo user = dbContext.UserInfo.FirstOrDefault(usr => usr.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase) && usr.Email.ToUpper().Equals(email.ToUpper(), StringComparison.OrdinalIgnoreCase));
                    if (user == null)
                    {
                        response.Success = false;
                        response.Errors.Add("User could not be found");
                        return response;
                    }
                    //activate the user
                    user.IsEmailActivated = activate ? "Y" : "N";

                    //1 records must be updated
                    response.Data = response.Success = dbContext.SaveChanges() == 1;
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Activate email on username " + username + " failed. Exception message " + ex.Message);
                response.Success = false;
                response.Errors.Add("An error occurred activating email address.");
            }
            return response;
        }

        public GenericResponse<string> ResetPassword(string username, string email, bool changePwdOnLogon)
        {
            GenericResponse<string> response = new GenericResponse<string>();
            if (string.IsNullOrEmpty(username))
            {
                response.Success = false;
                response.Errors.Add("Username is required");
                return response;
            }

            if (string.IsNullOrEmpty(email))
            {
                response.Success = false;
                response.Errors.Add("Email is required");
                return response;
            }

            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    UserInfo user = dbContext.UserInfo.FirstOrDefault(usr => usr.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase) && usr.Email.ToUpper().Equals(email.ToUpper(), StringComparison.OrdinalIgnoreCase));
                    if (user == null)
                    {
                        response.Success = false;
                        response.Errors.Add("User could not be found");
                        return response;
                    }

                    if (user.IsEmailActivated.Equals("N", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Success = false;
                        response.Errors.Add("Please activate user's email before resetting password.");
                        return response;
                    }

                    //Generate the salt for the user
                    string salt = _cryptoService.GenerateSalt();

                    //generate the random password
                    string randomPassword = RandomPassword.Generate(10, PasswordGroup.Lowercase, PasswordGroup.Uppercase, PasswordGroup.Numeric);

                    //hashed password
                    string hashedPassword = _cryptoService.Compute(randomPassword, salt);

                    user.Salt = salt;
                    user.Password = hashedPassword;
                    user.ChangePasswordOnLogon = changePwdOnLogon ? "Y" : "N";
                    user.InvalidLogonAttemptCount = 0;

                    //user needs his account unlocked to login and change passwords
                    user.IsLocked = "N";

                    //user needs to change his pasword after 180 days'
                    user.PasswordExpiresOn = DateTime.Now.AddDays(180);

                    //1 records must be updated
                    response.Success = dbContext.SaveChanges() == 1;

                    //set the random password to be passed back to the caller
                    response.Data = randomPassword;
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Reset password on username " + username + " failed. Exception message " + ex.Message);
                response.Success = false;
                response.Errors.Add("An error occurred resetting the password.");
            }
            return response;
        }

        public GenericResponse<AHP.Core.DTO.ExternalUserInfo> Logon(string username, string password)
        {
            GenericResponse<AHP.Core.DTO.ExternalUserInfo> response = new GenericResponse<ExternalUserInfo>();

            if (string.IsNullOrEmpty(username))
            {
                response.Success = false;
                response.Errors.Add("Username is required");
                return response;
            }

            if (string.IsNullOrEmpty(password))
            {
                response.Success = false;
                response.Errors.Add("Password is required");
                return response;
            }

            try
            {
                _loggerMessages.AppendFormat("User {0} is trying to logon as External user", username);

                //retrieve user information by his username
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    List<UserInfo> matchedUsers = dbContext.UserInfo.Where(usr => usr.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase)).ToList();

                    _auditEventManager.AuditEvent(AuditEventType.LOGIN_ATTEMPT, username, string.Format("{0} tried to logon", username));

                    //Of course we should have only one user by that name, but then who knows
                    if (matchedUsers.Count != 1)
                    {
                        _loggerMessages.AppendFormat("User {0} is trying to logon as External user. No or more than one user found", username);
                        response.Success = false;
                        response.Errors.Add("Please check your username and password and try again");
                        _auditEventManager.AuditEvent(AuditEventType.INVALID_LOGIN, username, string.Format("{0} tried to logon. No account exists or more than one exists.", username));
                        _logger.Info(_loggerMessages.ToString());
                        return response;
                    }

                    UserInfo user = matchedUsers[0];

                    //he also needs his account to be activated
                    if (user.IsUserActive.Equals("N", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Success = false;
                        response.Errors.Add("Your account has been disabled. Please contact your account manager for assistance.");
                        return response;
                    }

                    //he needs to be unlocked first
                    if (user.IsLocked.Equals("Y", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Success = false;
                        response.Errors.Add("For security reasons your account has been locked because of too many unsuccessful login attempts. In order to unlock your account you must <<click here>> in order to verify your identity and reset your password");
                        return response;
                    }

                    //lets compute hash of the provided password and compare that to existing hash
                    string providedPwd = _cryptoService.Compute(password, user.Salt);
                    if (_cryptoService.Compare(providedPwd, user.Password))
                    {
                        //yay successful logon
                        response.Data = new ExternalUserInfo()
                        {
                            ChangePasswordOnLogon = user.ChangePasswordOnLogon.Equals("Y", StringComparison.OrdinalIgnoreCase),
                            Email = user.Email.ToLower(),
                            Firstname = user.FirstName,
                            IsEmailActivated = user.IsEmailActivated.Equals("Y", StringComparison.OrdinalIgnoreCase),
                            IsLocked = user.IsLocked.Equals("Y", StringComparison.OrdinalIgnoreCase),
                            Lastname = user.LastName,
                            Role = user.Role,
                            //InvalidLogonAttemptCount = user.InvalidLogonAttemptCount,
                            SupplierId = user.SupplierIds,
                            Username = user.Username.ToLower(),
                            Company = user.CompanyName,
                            IsActive = user.IsUserActive.Equals("Y", StringComparison.OrdinalIgnoreCase),
                            BirthMonth = short.Parse(string.IsNullOrEmpty(user.MonthYear) ? "0" : user.MonthYear.Split('/')[0]),
                            BirthYear = short.Parse(string.IsNullOrEmpty(user.MonthYear) ? "0" : user.MonthYear.Split('/')[1]),
                            LastLogonDate = user.LastLoginDate.HasValue? user.LastLoginDate.Value.ToString("MM/dd/yy"): DateTime.Now.ToString("MM/dd/yy"),
                            CreatedBy = user.CreatedBy,
                            CreatedDate = user.CreatedDate.HasValue ? user.CreatedDate.Value.ToShortDateString() : string.Empty,
                            ZipCode = user.ZipCode,
                            PasswordExpiresOn = user.PasswordExpiresOn
                        };

                        response.Success = true;
                        response.Errors.Clear();

                        _auditEventManager.AuditEvent(AuditEventType.LOGIN_SUCCESS, username, string.Format("{0} tried to logon and it was successfull", username));

                        //Reset password incorrect times after successfully login
                        user.InvalidLogonAttemptCount = 0;
                        user.LastLoginDate = DateTime.Now;
                        user.IsLocked = "N";
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        //Yeah 0 , 1 & 2 which is third attempt
                        if (user.InvalidLogonAttemptCount >= 2)
                        {
                            response.Success = false;
                            response.Errors.Add("For security reasons your account has been locked because of too many unsuccessful login attempts. In order to unlock your account you must <<click here>> in order to verify your identity and reset your password");

                            _auditEventManager.AuditEvent(AuditEventType.INVALID_LOGIN, username, string.Format("{0} tried to logon. Invalid credentials provided. Account locked as exceeded three attempts.", username));

                            //lock the account
                            user.IsLocked = "Y";
                            //reset it
                            user.InvalidLogonAttemptCount = 0;
                            dbContext.SaveChanges();
                            return response;
                        }

                        _auditEventManager.AuditEvent(AuditEventType.INVALID_LOGIN, username, string.Format("{0} tried to logon. Invalid credentials provided", username));

                        user.InvalidLogonAttemptCount = user.InvalidLogonAttemptCount + 1;
                        //boo not right password
                        response.Success = false;
                        response.Errors.Add("Please check your username and password and try again");
                        //response.Errors.Add(string.Format("Username or password provided is incorrect. You have {0} more attempts before we lock your account.", 3 - user.InvalidLogonAttemptCount));
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerMessages.AppendFormat("External user logon failed for user {0}. Exception message is {1}.", username, ex.Message);
                response.Success = false;
                response.Errors.Add("An error occurred. Please try again.");
            }

            return response;
        }

        public GenericResponse<ExternalUserInfo> UpdateUser(ExternalUserInfo userInfo)
        {
            //By default success is false
            GenericResponse<ExternalUserInfo> response = new GenericResponse<ExternalUserInfo>();
            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    //Check for existing username
                    if (userInfo == null)
                    {
                        response.Errors.Add("Please provide required user information.");
                        return response;
                    }

                    if (string.IsNullOrEmpty(userInfo.Username))
                    {
                        response.Errors.Add("Username is required");
                        return response;
                    }
                  
                    //Check if user exists
                    UserInfo dbUser = dbContext.UserInfo.FirstOrDefault(user => user.Username.ToUpper().Equals(userInfo.Username.ToUpper(), StringComparison.OrdinalIgnoreCase));
                    if (dbUser == null)
                    {
                        response.Errors.Add("User with the provided details could not be found to update. Please try again");
                        return response;
                    }

                    //If user isn't editing his own account, then check if email is already present
                    if (!dbUser.Email.Equals(userInfo.Email,StringComparison.OrdinalIgnoreCase))
                    {
                        UserInfo existingEmail = dbContext.UserInfo.FirstOrDefault(user => user.Email.ToUpper().Equals(userInfo.Email.ToUpper(), StringComparison.OrdinalIgnoreCase));
                        if (existingEmail != null)
                        {
                            response.Errors.Add("User with the provided email already exists.");
                            return response;
                        }
                    }

                    if (string.IsNullOrEmpty(userInfo.Firstname))
                    {
                        response.Errors.Add("First name is required");
                        return response;
                    }

                    if (string.IsNullOrEmpty(userInfo.Lastname))
                    {
                        response.Errors.Add("Last name is required");
                        return response;
                    }

                    if (string.IsNullOrEmpty(userInfo.Role))
                    {
                        response.Errors.Add("Please specify role for the user");
                        return response;
                    }

                    if (!userInfo.Role.Equals("admin", StringComparison.OrdinalIgnoreCase) && !userInfo.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Errors.Add("Please specify either admin or user role.");
                        return response;
                    }

                    if (string.IsNullOrEmpty(userInfo.SupplierId))
                    {
                        response.Errors.Add("Please specify supplier id for the user");
                        return response;
                    }

                    //only sypplier id and few fields are allowed to be changed
                    dbUser.SupplierIds = userInfo.SupplierId;
                    dbUser.Email = userInfo.Email;
                    dbUser.FirstName = userInfo.Firstname;
                    dbUser.LastName = userInfo.Lastname;
                    dbUser.Role = userInfo.Role;
                    dbUser.CompanyName = userInfo.Company;
                    dbUser.ChangePasswordOnLogon = userInfo.ChangePasswordOnLogon ? "Y" : "N";
                    dbUser.CompanyName = userInfo.Company;

                    //I am not sure if these are updateable
                    dbUser.ZipCode = userInfo.ZipCode;
                    dbUser.MonthYear = string.Format("{0}/{1}", userInfo.BirthMonth, userInfo.BirthYear);

                    dbContext.SaveChanges();
                    response.Success = true;
                    response.Data = new ExternalUserInfo()
                    {
                        ChangePasswordOnLogon = dbUser.ChangePasswordOnLogon == "Y",
                        Email = dbUser.Email,
                        Firstname = dbUser.FirstName,
                        IsEmailActivated = dbUser.IsEmailActivated == "Y",
                        IsLocked = dbUser.IsLocked == "Y",
                        IsActive = dbUser.IsUserActive == "Y",
                        InvalidLogonAttemptCount = dbUser.InvalidLogonAttemptCount,                        
                        Lastname = dbUser.LastName,
                        Role = dbUser.Role,
                        SupplierId = dbUser.SupplierIds,
                        Username = dbUser.Username,
                        ZipCode = dbUser.ZipCode,
                        BirthYear = short.Parse(string.IsNullOrEmpty(dbUser.MonthYear) ? "0" : dbUser.MonthYear.Split('/')[1]),
                        BirthMonth = short.Parse(string.IsNullOrEmpty(dbUser.MonthYear) ? "0" : dbUser.MonthYear.Split('/')[0]),
                        CreatedBy = dbUser.CreatedBy,
                        LastLogonDate = dbUser.LastLoginDate.HasValue ? dbUser.LastLoginDate.Value.ToShortDateString():string.Empty,
                        CreatedDate = dbUser.CreatedDate.HasValue ? dbUser.CreatedDate.Value.ToShortDateString():string.Empty,
                        Company = dbUser.CompanyName
                    };
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Error occurred creating new user. Please try again");
                _logger.Error("An Error occurred creating new user.", ex);
            }
            return response;
        }

        public GenericResponse<bool> ActivateUser(string username, string email)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            if (string.IsNullOrEmpty(username))
            {
                response.Success = false;
                response.Errors.Add("Username is required");
                return response;
            }

            if (string.IsNullOrEmpty(email))
            {
                response.Success = false;
                response.Errors.Add("Email is required");
                return response;
            }

            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    UserInfo user = dbContext.UserInfo.FirstOrDefault(usr => usr.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase) && usr.Email.ToUpper().Equals(email.ToUpper(), StringComparison.OrdinalIgnoreCase));
                    if (user == null)
                    {
                        response.Success = false;
                        response.Errors.Add("User could not be found");
                        return response;
                    }
                    //activate the user
                    user.IsUserActive = "Y";

                    //1 records must be updated
                    dbContext.SaveChanges();
                    response.Data = response.Success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Info("activate user on username " + username + " failed. Exception message " + ex.Message);
                response.Success = false;
                response.Errors.Add("An error occurred activating user account.");
            }
            return response;
        }

        public GenericResponse<bool> DeactivateUser(string username, string email)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            if (string.IsNullOrEmpty(username))
            {
                response.Success = false;
                response.Errors.Add("Username is required");
                return response;
            }

            if (string.IsNullOrEmpty(email))
            {
                response.Success = false;
                response.Errors.Add("Email is required");
                return response;
            }

            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    UserInfo user = dbContext.UserInfo.FirstOrDefault(usr => usr.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase) && usr.Email.ToUpper().Equals(email.ToUpper(), StringComparison.OrdinalIgnoreCase));
                    if (user == null)
                    {
                        response.Success = false;
                        response.Errors.Add("User could not be found");
                        return response;
                    }
                    //activate the user
                    user.IsUserActive = "N";

                    //1 records must be updated
                    response.Data = response.Success = dbContext.SaveChanges() == 1;
                }
            }
            catch (Exception ex)
            {
                _logger.Info("deactivate user on username " + username + " failed. Exception message " + ex.Message);
                response.Success = false;
                response.Errors.Add("An error occurred activating user account.");
            }
            return response;
        }

        public GenericResponse<List<UserSecurityOption>> GetSecurityOptionsForUser(string username)
        {
            GenericResponse<List<UserSecurityOption>> response = new GenericResponse<List<UserSecurityOption>>();
            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    List<UserSecurityAnswer> usersAnswers = dbContext.SecurityQuestionAnswer.Where(qans => qans.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase)).ToList();
                    response.Data = usersAnswers.Select(usrAns => new UserSecurityOption() { Question = usrAns.SecurityQuestion }).ToList();
                }
                response.Data = response.Data ?? new List<UserSecurityOption>();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("An Error occurred retrieving security questions.");
            }
            return response;
        }        

        public GenericResponse<bool> ChangePassword(string username, string oldPassword, string newPassword)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            _loggerMessages.AppendFormat("User {0} requested change password.", username);
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    _loggerMessages.Append("Username provided was empty.");
                    response.Errors.Add("Please provide username.");
                    response.Success = false;
                    _logger.Info(_loggerMessages.ToString());
                    return response;
                }

                if (string.IsNullOrEmpty(oldPassword))
                {
                    _loggerMessages.Append("Current password provided was empty.");
                    response.Errors.Add("Please provide current password.");
                    response.Success = false;
                    _logger.Info(_loggerMessages.ToString());
                    return response;
                }

                if (string.IsNullOrEmpty(newPassword))
                {
                    _loggerMessages.Append("New password provided was empty.");
                    response.Errors.Add("Please provide new password.");
                    response.Success = false;
                    _logger.Info(_loggerMessages.ToString());
                    return response;
                }

                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    List<UserInfo> matchedUsers = dbContext.UserInfo.Where(usrInfo => usrInfo.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase)).ToList();
                    if (matchedUsers.Count != 1)
                    {
                        response.Errors.Add("User info could not be found.");
                        response.Success = false;
                        return response;
                    }

                    UserInfo user = matchedUsers[0];
                    string oldPwdHash = _cryptoService.Compute(oldPassword, user.Salt);
                    if (!_cryptoService.Compare(user.Password,oldPwdHash))
                    {
                        response.Errors.Add("Old password does not match.");
                        response.Success = false;
                        return response;
                    }

                    string newSalt = _cryptoService.GenerateSalt();
                    string newPwdHash = _cryptoService.Compute(newPassword, newSalt);

                    user.Salt = newSalt;
                    user.Password = newPwdHash;
                    user.ChangePasswordOnLogon = "N";
                    user.InvalidLogonAttemptCount = 0;
                    user.IsLocked = "N";
                    user.IsUserActive = "Y";

                    //password expires 180 days from date of change
                    user.PasswordExpiresOn = DateTime.Now.AddDays(180);
                    response.Success = response.Data = dbContext.SaveChanges() == 1;
                }
            }
            catch (Exception ex)
            {
                _loggerMessages.AppendFormat("Error occurred changing user password. Error info {0}", ex.Message);
                response.Success = false;
                response.Errors.Add("Could not change your password. Please try again.");
                response.Data = false;
            }
            return response;
        }

        public List<string> GetSecurityQuestions()
        {
            List<string> response = new List<string>();
            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    response = dbContext.SecurityQuestions.Select(ques => ques.SecurityQuestion).ToList();
                }
            }
            catch (Exception ex)
            {
                _loggerMessages.AppendFormat("Error occurred getting security questions. Exception info {0}",ex.Message);
            }
            _logger.Info(_loggerMessages.ToString());
            return response;
        }

        public GenericResponse<bool> SetupQuestions(string username, List<UserSecurityOption> securityQuestionAnswers)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();

            try
            {
                _loggerMessages.AppendFormat("Username {0} is setting up his security questions.",username);
                _loggerMessages.Append("Checking if the user exists.");
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    List<UserInfo> matchedUsers = dbContext.UserInfo.Where(ctx => ctx.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase)).ToList();
                    if (matchedUsers.Count != 1)
                    {
                        response.Errors.Add("User could not be found");
                        response.Success = false;
                        _loggerMessages.Append("User could not be found or more than one user exists.");
                        _logger.Info(_loggerMessages.ToString());
                        return response;
                    }

                    List<UserSecurityAnswer> matchedAnswers = dbContext.SecurityQuestionAnswer.Where(ctx => ctx.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase)).ToList();
                    _loggerMessages.Append("Iterating users security question and answer and removing them.");
                    foreach (UserSecurityAnswer usersAnswers in matchedAnswers)
                    {
                        dbContext.SecurityQuestionAnswer.Remove(usersAnswers);
                    }

                    _loggerMessages.Append("Iterating users new security question and answer and adding them.");
                    foreach (UserSecurityOption securityOption in securityQuestionAnswers)
                    {
                        string salt = _cryptoService.GenerateSalt();
                        string answerHash = _cryptoService.Compute(securityOption.Answer.ToLowerInvariant(), salt);
                        UserSecurityAnswer userAnswerEntry = new UserSecurityAnswer();
                        userAnswerEntry.Username = username;
                        userAnswerEntry.Answer = answerHash;
                        userAnswerEntry.Salt = salt;
                        userAnswerEntry.Guid = Guid.NewGuid().ToString();
                        userAnswerEntry.SecurityQuestion = securityOption.Question;
                        dbContext.SecurityQuestionAnswer.Add(userAnswerEntry);
                    }                    
                    dbContext.SaveChanges();
                    _loggerMessages.Append("User security option update successfull.");
                    response.Success = true;
                    response.Data = true;
                }
            }
            catch (Exception ex)
            {
                _loggerMessages.AppendFormat("Error occurred updating user security answers.Error info {0}",ex.Message);
                response.Success = false;
                response.Data = false;
                response.Errors.Add("Error occurred updating security questions and answers. Please try again.");
            }
            _logger.Info(_loggerMessages.ToString());
            return response;
        }

        public GenericResponse<bool> ClearSecurityAnswers(string username)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();

            try
            {
                _loggerMessages.AppendFormat("Username {0} is setting up his security questions.", username);
                _loggerMessages.Append("Checking if the user exists.");
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    List<UserInfo> matchedUsers = dbContext.UserInfo.Where(ctx => ctx.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase)).ToList();
                    if (matchedUsers.Count != 1)
                    {
                        response.Errors.Add("User could not be found");
                        response.Success = false;
                        _loggerMessages.Append("User could not be found or more than one user exists.");
                        _logger.Info(_loggerMessages.ToString());
                        return response;
                    }

                    List<UserSecurityAnswer> matchedAnswers = dbContext.SecurityQuestionAnswer.Where(ctx => ctx.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase)).ToList();
                    _loggerMessages.Append("Iterating users security question and answer and removing them.");
                    foreach (UserSecurityAnswer usersAnswers in matchedAnswers)
                    {
                        dbContext.SecurityQuestionAnswer.Remove(usersAnswers);
                    }
                    
                    dbContext.SaveChanges();
                    _loggerMessages.Append("User security option update successfull.");
                    response.Success = true;
                    response.Data = true;
                }
            }
            catch (Exception ex)
            {
                _loggerMessages.AppendFormat("Error occurred updating user security answers.Error info {0}", ex.Message);
                response.Success = false;
                response.Data = false;
                response.Errors.Add("Error occurred updating security questions and answers. Please try again.");
            }
            _logger.Info(_loggerMessages.ToString());
            return response;
        }

        public GenericResponse<AHP.Core.DTO.PasswordResetResponse> ResetPassword(string username, List<UserSecurityOption> securityQuestionAnswers)
        {
            GenericResponse<PasswordResetResponse> response = new GenericResponse<PasswordResetResponse>();
            if (string.IsNullOrEmpty(username))
            {
                response.Success = false;
                response.Errors.Add("Username is required");
                return response;
            }

            if (securityQuestionAnswers == null)
            {
                response.Success = false;
                response.Errors.Add("Security questions and answers are required.");
                return response;
            }

            try
            {
                if (securityQuestionAnswers.Count != 3)
                {
                    response.Errors.Add("Please select security question and provide answer.");
                    response.Success = false;
                    return response;
                }

                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    UserInfo user = dbContext.UserInfo.FirstOrDefault(usr => usr.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase));
                    if (user == null)
                    {
                        response.Success = false;
                        response.Errors.Add("User could not be found");
                        return response;
                    }

                    if (user.IsUserActive.Equals("N",StringComparison.OrdinalIgnoreCase))
                    {
                        response.Success = false;
                        response.Errors.Add("Your account is deactivated. Please contact the administrator to activate your account.");
                        return response;
                    }

                    if (user.IsEmailActivated.Equals("N", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Success = false;
                        response.Errors.Add("Please activate your email before resetting password.");
                        return response;
                    }                    

                    //verify users security question and answer
                    List<UserSecurityAnswer> lstOfAnswers = dbContext.SecurityQuestionAnswer.Where(ctx => ctx.Username.ToUpper().Equals(username.ToUpper(),StringComparison.OrdinalIgnoreCase)).ToList();

                    int numOfRightAnswers = 0;

                    //for every question user has answered, check if answer matches as in database
                    foreach (UserSecurityOption userAnswerInfo in securityQuestionAnswers)
                    {
                        UserSecurityAnswer secAnswer = lstOfAnswers.FirstOrDefault(ans => userAnswerInfo.Question.ToUpper().Equals(ans.SecurityQuestion.ToUpper(), StringComparison.OrdinalIgnoreCase) && ans.Username.ToUpper().Equals(username.ToUpper(), StringComparison.OrdinalIgnoreCase));
                        if (secAnswer != null)
                        {
                            string answerHash = _cryptoService.Compute(userAnswerInfo.Answer.ToLowerInvariant(), secAnswer.Salt);
                            if (_cryptoService.Compare(secAnswer.Answer,answerHash))
                            {
                                numOfRightAnswers = numOfRightAnswers + 1;
                            }
                        }
                    }

                    if (numOfRightAnswers != 3)
                    {
                        response.Errors.Add(" We could not  process your password reset request. Your answers to the security challenge questions do not match what you entered during account setup. <<click here>>");
                        response.Success = false;
                        return response;
                    }

                    //Generate the salt for the user
                    string salt = _cryptoService.GenerateSalt();

                    //generate the random password
                    string randomPassword = RandomPassword.Generate(10, PasswordGroup.Lowercase, PasswordGroup.Uppercase, PasswordGroup.Numeric);

                    //hashed password
                    string hashedPassword = _cryptoService.Compute(randomPassword, salt);

                    user.Salt = salt;
                    user.Password = hashedPassword;
                   
                    //user needs his account unlocked to login and change passwords
                    user.ChangePasswordOnLogon = "Y";
                    user.InvalidLogonAttemptCount = 0;                    
                    user.IsLocked = "N";

                    //Password expires 180 days from day you change it
                    user.PasswordExpiresOn = DateTime.Now.AddDays(180);

                    //1 records must be updated
                    response.Success = dbContext.SaveChanges() == 1;

                    //set the random password to be passed back to the caller
                    response.Data = new PasswordResetResponse()
                    {
                        Email = user.Email,
                        NewPassword = randomPassword,
                        Username = user.Username
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Reset password on username " + username + " failed. Exception message " + ex.Message);
                response.Success = false;
                response.Errors.Add("An error occurred resetting password.");
            }
            return response;
        }

        public GenericResponse<List<InternalUserInfo>> ListAllInternalUsers()
        {
            GenericResponse<List<InternalUserInfo>> usrInfo = new GenericResponse<List<InternalUserInfo>>();
            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    usrInfo.Data = dbContext.InternalUserInfo.ToList().Select(dbUser => new InternalUserInfo()
                    {
                        ActiveHealthId = dbUser.ActiveHealthUserId,
                        TableauId = dbUser.TableauId
                    }).ToList();
                    usrInfo.Success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Error occurred retrieving user info " + ex.Message);
                usrInfo.Success = false;
                usrInfo.Errors.Add("Could not retrieve user information. Please try again");
            }
            return usrInfo;
        }

        public GenericResponse<bool> MapInternalUser(InternalUserInfo userInfo)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            try
            {
                _loggerMessages.Append("Mapping active health ID to Tableau ID");

                if (string.IsNullOrEmpty(userInfo.ActiveHealthId))
                {
                    _loggerMessages.Append("Active Health ID provided is empty.");
                    response.Errors.Add("Active Health employee ID is required");
                }

                if (string.IsNullOrEmpty(userInfo.TableauId))
                {
                    _loggerMessages.Append("Tableau ID provided is empty.");
                    response.Errors.Add("Tableau account ID is required");
                }

                if (response.Errors.Any())
                {
                    _loggerMessages.Append("Validation error occurred during mapping.");
                    response.Success = false;
                    _logger.Info(_loggerMessages.ToString());
                    return response;
                }

                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    List<InternalUserTableauInfo> allInternalUsers =  dbContext.InternalUserInfo.ToList();

                    _loggerMessages.Append("Checking if active health id already exists.");

                    //checking if user with ah ID already exists
                    bool isAhmIdPresent = allInternalUsers.Any(intUsr => intUsr.ActiveHealthUserId.Equals(userInfo.ActiveHealthId, StringComparison.OrdinalIgnoreCase));

                    _loggerMessages.Append("Checking if tableau ID already exists.");

                    //checking if user with that tableau ID already exists
                    bool isTableuIdPresent = !isAhmIdPresent && allInternalUsers.Any(intUsr => intUsr.TableauId.Equals(userInfo.TableauId, StringComparison.OrdinalIgnoreCase));

                    _loggerMessages.Append("Checking if user is already mapped.");

                    //checking if user with ah and tableau id combo already exists
                    bool isAlreadyMapped = !isAhmIdPresent && !isTableuIdPresent && allInternalUsers.Any(intUsr => intUsr.TableauId.Equals(userInfo.TableauId, StringComparison.OrdinalIgnoreCase) && intUsr.ActiveHealthUserId.Equals(userInfo.ActiveHealthId,StringComparison.OrdinalIgnoreCase));

                    if (isAhmIdPresent || isTableuIdPresent || isAlreadyMapped)
                    {
                        _loggerMessages.Append("Account by that information already exists.");
                        response.Errors.Add("Account by that information already exists");
                        response.Success = false;
                    }
                    else
                    {
                        InternalUserTableauInfo internalUser = new InternalUserTableauInfo();
                        internalUser.TableauId = userInfo.TableauId;
                        internalUser.ActiveHealthUserId = userInfo.ActiveHealthId;

                        dbContext.InternalUserInfo.Add(internalUser);
                        response.Success = dbContext.SaveChanges() != 0;
                        response.Data = true;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add("Could not process your request");
                _loggerMessages.Append("Exception occured mapping internal users to tableau ID. Exception info " +ex.Message);
                _logger.Error(ex.Message, ex);
            }
            _logger.Info(_loggerMessages.ToString());
            return response;
        }

        public GenericResponse<bool> UpdateInternalUser(InternalUserInfo userInfo)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            try
            {
                _loggerMessages.Append("Mapping active health ID to Tableau ID");

                if (string.IsNullOrEmpty(userInfo.ActiveHealthId))
                {
                    _loggerMessages.Append("Active Health ID provided is empty.");
                    response.Errors.Add("Active Health employee ID is required");
                }

                if (string.IsNullOrEmpty(userInfo.TableauId))
                {
                    _loggerMessages.Append("Tableau ID provided is empty.");
                    response.Errors.Add("Tableau account ID is required");
                }

                if (response.Errors.Any())
                {
                    _loggerMessages.Append("Validation error occurred during mapping.");
                    response.Success = false;
                    _logger.Info(_loggerMessages.ToString());
                    return response;
                }

                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    List<InternalUserTableauInfo> allInternalUsers = dbContext.InternalUserInfo.ToList();

                    _loggerMessages.Append("Checking if active health id already exists.");

                    //checking if user with ah ID already exists
                    var matchingIntUsers = allInternalUsers.Where(intUsr => intUsr.ActiveHealthUserId.Equals(userInfo.ActiveHealthId, StringComparison.OrdinalIgnoreCase));

                    if (matchingIntUsers.Count() == 1)
                    {
                                               
                        Core.Model.InternalUserTableauInfo user = matchingIntUsers.First();

                        if (user.TableauId.Equals(userInfo.TableauId,StringComparison.OrdinalIgnoreCase))
                        {
                            //if tableau ID wasn't changed don't bother updating
                            response.Success = true;
                            response.Data = true;
                        }
                        else
                        {
                            _loggerMessages.Append("Checking if tableau ID already exists.");

                            //checking if user with that tableau ID already exists
                            if(allInternalUsers.Any(intUsr => intUsr.TableauId.Equals(userInfo.TableauId, StringComparison.OrdinalIgnoreCase)))
                            {
                                _loggerMessages.Append("Account by that information already exists.");
                                response.Errors.Add("Account by that information already exists");
                                response.Success = false;
                            }
                            else
                            {
                                user.TableauId = userInfo.TableauId;
                                dbContext.SaveChanges();
                                response.Success = response.Data = true;
                                _loggerMessages.Append("Successfully updated the user information.");
                            }                            
                        }                        
                    }
                    else
                    {
                        response.Success = false;
                        response.Errors.Add("Unable to find user with that information");
                    }                    
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add("Could not process your request");
                _loggerMessages.Append("Exception occured mapping internal users to tableau ID. Exception info " + ex.Message);
                _logger.Error(ex.Message, ex);
            }
            _logger.Info(_loggerMessages.ToString());
            return response;
        }

        public GenericResponse<List<TableauViewInfo>> ListAllTableauViews()
        {
            GenericResponse<List<TableauViewInfo>> tableauViewInfo = new GenericResponse<List<TableauViewInfo>>();
            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    tableauViewInfo.Data = dbContext.WorkbookViews.ToList().Select(dbViewInfo => new TableauViewInfo()
                    {
                        ViewId = dbViewInfo.ViewId,
                        Disabled = dbViewInfo.IsDisabled,
                        IsDashboard = dbViewInfo.IsDashboard,
                        ViewName = dbViewInfo.ViewName,
                        ViewUrl = dbViewInfo.ViewUrl,
                        Description = dbViewInfo.Description
                    }).ToList();
                    tableauViewInfo.Success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Error occurred retrieving tableau workbook view info " + ex.Message);
                tableauViewInfo.Success = false;
                tableauViewInfo.Errors.Add("Could not retrieve tableau view information. Please try again");
            }
            return tableauViewInfo;
        }

        public GenericResponse<bool> AddTableauInfo(TableauViewInfo tabInfo)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    WorkbookViewInfo tableauInfo = new WorkbookViewInfo();
                    tableauInfo.Description = tabInfo.Description;
                    tableauInfo.IsDashboard = tabInfo.IsDashboard;
                    tableauInfo.IsDisabled = tabInfo.Disabled;
                    tableauInfo.ViewId = Guid.NewGuid().ToString();
                    tableauInfo.ViewName = tabInfo.ViewName;
                    tableauInfo.ViewUrl = tabInfo.ViewUrl;
                    dbContext.WorkbookViews.Add(tableauInfo);
                    response.Success = response.Data = dbContext.SaveChanges() == 1;
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Error occurred adding tableau workbook view info " + ex.Message);
                response.Success = false;
                response.Errors.Add("Could not add tableau view information. Please try again");
            }
            return response;
        }

        public GenericResponse<bool> UpdateTableauInfo(TableauViewInfo tabInfo)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {

                    List<WorkbookViewInfo> lstOfViews = dbContext.WorkbookViews.Where(vw => vw.ViewId.Equals(tabInfo.ViewId)).ToList();

                    //can't have views with same ID
                    if(lstOfViews.Count != 1)
                    {
                        response.Success = false;
                        response.Errors.Add("Could not find tableau view with that information");
                    }
                    else
                    {
                        var dbTabInfo = lstOfViews[0];
                        dbTabInfo.Description = tabInfo.Description;
                        dbTabInfo.IsDashboard = tabInfo.IsDashboard;
                        dbTabInfo.IsDisabled = tabInfo.Disabled;
                        dbTabInfo.ViewName = tabInfo.ViewName;
                        dbTabInfo.ViewUrl = tabInfo.ViewUrl;
                        dbContext.SaveChanges();
                        response.Success = response.Data = true;
                        response.Errors.Clear();
                    }                  
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Error occurred updating tableau workbook view info " + ex.Message);
                response.Success = false;
                response.Errors.Add("Could not update tableau view information. Please try again");
            }
            return response;
        }

        public GenericResponse<List<TableauViewUserAssociation>> GetUsersForView(string viewId)
        {
            GenericResponse<List<TableauViewUserAssociation>> dbResponse = new GenericResponse<List<TableauViewUserAssociation>>();
            try
            {

                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    List<TableauViewsForUser> lstOfUsers = dbContext.ViewsForUser.Where(vw => vw.ViewId == viewId).ToList();

                    List<string> existingExternalUsers = lstOfUsers.Where(usr => usr.UserType.ToLower().Equals("external")).Select(usr => usr.Username.ToLower()).ToList();

                    List<string> existingInternalUsers = lstOfUsers.Where(usr => usr.UserType.ToLower().Equals("internal")).Select(usr => usr.Username.ToLower()).ToList();

                    //get all internal users not in the already associated view username
                    List<InternalUserTableauInfo> internalUses = dbContext.InternalUserInfo.Where(intUsr => !existingInternalUsers.Contains(intUsr.ActiveHealthUserId.ToLower())).ToList();

                    //list all external users who aren't associated with a view yet
                    List<UserInfo> externalUsers = dbContext.UserInfo.Where(extUsr => !existingExternalUsers.Contains(extUsr.Username.ToLower())).ToList();

                    List<TableauViewUserAssociation> lstOfAssociatedUser = new List<TableauViewUserAssociation>();

                    lstOfAssociatedUser.AddRange(lstOfUsers.Select(vw => new TableauViewUserAssociation() { Selected = true,Username = vw.Username.ToLower(), UserType = vw.UserType,ViewId = vw.ViewId }));

                    //Add internal users
                    lstOfAssociatedUser.AddRange(internalUses.Select(vw => new TableauViewUserAssociation() { Selected = false, Username = vw.ActiveHealthUserId.ToLower(), UserType = InternalUserType, ViewId = viewId }));

                    //add external users
                    lstOfAssociatedUser.AddRange(externalUsers.Select(vw => new TableauViewUserAssociation() { Selected = false, Username = vw.Username.ToLower(), UserType = ExternalUserType, ViewId = viewId }));

                    dbResponse.Success = true;
                    dbResponse.Data = lstOfAssociatedUser;
                }
            }
            catch (Exception ex)
            {
                dbResponse.Success = false;
                dbResponse.Errors.Add("Could not process your request.");
                _logger.Error("Error occurred listing all users for a view",ex);
            }
            return dbResponse;
        }

        public GenericResponse<bool> UpdateUsersForView(string viewId,List<TableauViewUserAssociation> usrViewAssoc)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    if (string.IsNullOrEmpty(viewId))
                    {
                        response.Success = response.Data = false;
                        response.Errors.Add("View id provided is empty");
                        return response;
                    }

                    //remove all associations first
                    var existingAssociations = dbContext.ViewsForUser.Where(vw => vw.ViewId == viewId);
                    dbContext.ViewsForUser.RemoveRange(existingAssociations);                   
                    if (usrViewAssoc.Count != 0)
                    {
                        //add back the newly selected users
                        dbContext.ViewsForUser.AddRange(usrViewAssoc.Select(vw => new TableauViewsForUser()
                        {
                            Username = vw.Username.ToLower(),
                            UserType = vw.UserType.ToLower(),
                            ViewId = viewId
                        }));
                    }
                    dbContext.SaveChanges();
                    response.Data = response.Success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error updating the views for user",ex);
                response.Success = response.Data = false;
                response.Errors.Add("Error updating user information for view");
            }
            return response;
        }

        public GenericResponse<List<TableauViewInfo>> GetViewsOnUser(string username, string usertype)
        {
            GenericResponse<List<TableauViewInfo>> dbResponse = new GenericResponse<List<TableauViewInfo>>();
            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    List<string> lstOfViews = dbContext.ViewsForUser.
                        Where(vw => vw.Username.ToLower().Equals(username.ToLower()) && vw.UserType.ToLower().Equals(usertype.ToLower())).
                        Select(vw => vw.ViewId).ToList();

                    List<WorkbookViewInfo> workbookViews =  dbContext.WorkbookViews.Where(wbv => lstOfViews.Contains(wbv.ViewId) && wbv.IsDisabled == "N").ToList();

                    dbResponse.Data = workbookViews.Select(wbv => new TableauViewInfo
                    {
                        Description = wbv.Description??string.Empty,
                        Disabled = wbv.IsDisabled,
                        IsDashboard = wbv.IsDashboard,
                        ViewId = wbv.ViewId,
                        ViewName = wbv.ViewName,
                        ViewUrl = wbv.ViewUrl
                    }).ToList();
                    dbResponse.Success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Could not get views for user",ex);
                dbResponse.Success = false;
                dbResponse.Errors.Add("Could not process your request");
            }
            return dbResponse;
        }

        #endregion

    }
}

