using AHP.Core.Logger;
using AHP.Core.Model;
using AHP.Core.Repository;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AHP.Repository
{
    public class LDAPAuthenticationRepository : ILDAPAuthenticationRepository
    {
        #region -- Members --

        private readonly IActiveAnalyticsLogger _logger;

        private readonly StringBuilder _logMessages;

        private readonly LDAPUser _userDetails;

        #endregion

        #region -- Constructors --
        public LDAPAuthenticationRepository(IActiveAnalyticsLogger logger)
        {
            _logger = logger;
            _logMessages = new StringBuilder();
            _userDetails = new LDAPUser()
            {
                AccountName = string.Empty,
                LDAPAccessStatus = LDAPAccessStatus.UserLogonUnsuccessful
            };
        }

        #endregion

        #region -- ILDAPAuthenticationRepository Members --

        public LDAPUser AuthenticateUser(LDAPAuthentication ldapAuth)
        {
            //TODO: Cleanup to be undestandable and readable
            _userDetails.AccountName = ldapAuth.UserName;
            string domainAndUsername = string.Format(@"{0}\{1}",ldapAuth.DomainName,ldapAuth.UserName);
            try
            {
                _logMessages.Append("LDAP Authentication. Performing binding.");
                using (DirectoryEntry entry = new DirectoryEntry(ldapAuth.LDAPUrlPath, domainAndUsername, ldapAuth.Password))
                {
                    _logMessages.AppendFormat("Directory bind to {0} for user {1} successfull.", 
                        ldapAuth.LDAPUrlPath,
                        domainAndUsername);
                    //Bind to the native AdsObject to force authentication.
                    object obj = entry.NativeObject;

                    using (DirectorySearcher search = new DirectorySearcher(entry))
                    {
                        search.Filter = "(SAMAccountName=" + ldapAuth.UserName + ")";

                        _logMessages.AppendFormat("Searching for the user with filter {0}.", search.Filter);

                        // Filter for properties to load. Not required, when loading all properties.
                        // For complete list, refer http://www.kouti.com/tables/userattributes.htm .
                        search.PropertiesToLoad.Add("givenName");
                        search.PropertiesToLoad.Add("sn");
                        search.PropertiesToLoad.Add("displayName");

                        SearchResult result = search.FindOne();
                        if (null == result)
                        {
                            _logMessages.Append("Filter did not find any user Or user logon was unsuccessfull.");
                            _userDetails.LDAPAccessStatus = LDAPAccessStatus.UserLogonUnsuccessful;
                        }
                        else
                        {
                            _logMessages.Append("Trying to load givenname, sn and displayname from AD. Default status is set to user logon successfull.");
                            _userDetails.LDAPAccessStatus = LDAPAccessStatus.UserLogonSuccessful;
                            if (result.Properties.Contains("givenName") && result.Properties["givenName"].Count == 1)
                            {
                                _userDetails.FirstName = Convert.ToString(result.Properties["givenName"][0]);
                            }

                            if (result.Properties.Contains("sn") && result.Properties["sn"].Count == 1)
                            {
                                _userDetails.LastName = Convert.ToString(result.Properties["sn"][0]);
                            }

                            if (result.Properties.Contains("displayName") && result.Properties["displayName"].Count == 1)
                            {
                                _userDetails.DisplayName = Convert.ToString(result.Properties["displayName"][0]);
                            }
                        }
                    }
                }
                _logger.Info(_logMessages.ToString());                
            }
            catch (DirectoryServicesCOMException exc)
            {
                _logMessages.AppendFormat("Directory service com exception occurred Exception message {0}.", exc.Message);
                string errCodeHex = string.Empty;
                try
                {
                    // Unfortunately, the only place to get the LDAP bind error code is in the "data" field of the 
                    // extended error message, which is in this format:
                    // 80090308: LdapErr: DSID-0C09030B, comment: AcceptSecurityContext error, data 52e, v893
                    if (!string.IsNullOrEmpty(exc.ExtendedErrorMessage))
                    {
                        Match match = Regex.Match(exc.ExtendedErrorMessage, @" data (?<errCode>[0-9A-Fa-f]+),");
                        if (match.Success)
                        {
                            errCodeHex = match.Groups["errCode"].Value;
                        }
                    }
                }
                catch (NullReferenceException ex)
                {
                    throw;
                }

                if (errCodeHex == "775")
                {
                    _userDetails.LDAPAccessStatus = LDAPAccessStatus.UserAccountLocked;
                }
                else
                {
                    _userDetails.LDAPAccessStatus = LDAPAccessStatus.UserLogonUnsuccessful;
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                _logMessages.AppendFormat("Interop services com exception occurred Exception message {0}.", ex.Message);
                _userDetails.LDAPAccessStatus = LDAPAccessStatus.UserLogonUnsuccessful;
            }
            catch (Exception ex)
            {
                _logMessages.AppendFormat("Exception occurred Exception message {0}.", ex.Message);
                if (ex.Message.ToLower(System.Globalization.CultureInfo.InvariantCulture).Contains("unknown user name or bad password"))
                {
                    _userDetails.LDAPAccessStatus = LDAPAccessStatus.UserLogonUnsuccessful;
                }
                //// Check for whether account is locked on not
                //if (Convert.ToBoolean(entry.InvokeGet("IsAccountLocked")))
                //{
                //    retValueUser.LDAPAccessStatus = LDAPAccessStatus.UserAccountLocked;
                //}
                _userDetails.LDAPAccessStatusMessage = "Error authenticating user: " + ex.Message;                
            }
            _logger.Info(_logMessages.ToString());
            return _userDetails;
        }

        //public string GetGroups(LDAPAuthentication ldapAuth)
        //{
        //    StringBuilder groupNames = new StringBuilder();
        //    _logMessages.Append("Retrieving domain groups. Using directory searcher.");
        //    using (DirectorySearcher search = new DirectorySearcher(ldapAuth.LDAPUrlPath))
        //    {
        //        _logMessages.Append("Directory search bind successfull.");
        //        search.Filter = "(cn=" + ldapAuth.FilterAttribute + ")";
        //        search.PropertiesToLoad.Add("memberOf");
        //        try
        //        {
        //            SearchResult result = search.FindOne();
        //            int propertyCount = result.Properties["memberOf"].Count;
        //            string dn;
        //            int equalsIndex, commaIndex;

        //            for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
        //            {
        //                dn = (string)result.Properties["memberOf"][propertyCounter];
        //                equalsIndex = dn.IndexOf("=", 1);
        //                commaIndex = dn.IndexOf(",", 1);
        //                if (-1 == equalsIndex)
        //                {
        //                    //TODO : Check invoking method if it handles NULL
        //                    return null;
        //                }
        //                groupNames.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
        //                groupNames.Append("|");
        //            }
        //            _logMessages.Append("Completed loading all groups.");
        //        }
        //        catch (Exception ex)
        //        {
        //            _logMessages.AppendFormat("Error occurred getting groups, exception message {0}.", ex.Message);
        //            _logger.Info(_logMessages.ToString());
        //            //TODO: Remove below or throw back the exception. Prob a new exception
        //            throw new Exception("Error obtaining group names. " + ex.Message);
        //        }
        //    }
        //    _logger.Info(_logMessages.ToString());
        //    return groupNames.ToString();
        //} 

        #endregion
    }
    
}
