#region -- File Header --

/*
 * Author: James Deepak
 * Date: 11 May 2017
 * Purpose: Provides user management features
 * 
 */
#endregion

#region -- Using Directive --
using AHP.Core.Logger;
using AHP.Web.Helpers;
using AHP.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
#endregion

namespace AHP.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagerController : BaseController
    {
        #region -- Members --

        private readonly IActiveAnalyticsLogger _logger;

        private readonly IServerDataRestClient _restClient;

        #endregion

        #region -- Constructors --

        public UserManagerController(IActiveAnalyticsLogger logger, IServerDataRestClient restClient)
        {
            _logger = logger;
            _restClient = restClient;
        }

        #endregion

        #region -- Action Methods --

        // GET: UserManager
        public ActionResult Index()
        {
            //skeleton to load angular files and list all users
            return View();
        }

        [HttpPost]
        public ActionResult GetUsersList()
        {
            GenericAjaxResponse<List<AHP.Core.DTO.ExternalUserInfo>> response = new GenericAjaxResponse<List<AHP.Core.DTO.ExternalUserInfo>>();
            try
            {
                response = _restClient.GetAllUsers();
                if (response == null || response.Data == null)
                {
                    response = new GenericAjaxResponse<List<Core.DTO.ExternalUserInfo>>();
                    response.Success = false;
                    response.Errors.Add("Error occurred. Please try again.");
                    return Json(response);
                }
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Error occurred getting users. Please try again");
                _logger.Info("Could not retrieve users from database. Error " + ex.Message);
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult CreateUser(ViewModel.UserinfoViewModel userInfo)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            try
            {
                if (ModelState.IsValid)
                {
                    if (!userInfo.Role.Equals("admin", StringComparison.OrdinalIgnoreCase) && !userInfo.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Errors.Add("Role can be only admin or user");
                        response.Success = false;
                        return Json(response);
                    }

                    AHP.Core.DTO.ExternalUserInfo externalUserAccount = new Core.DTO.ExternalUserInfo();
                    //Only subset of properties are to be filled. Since others are maintained internally
                    externalUserAccount.Username = userInfo.Username;
                    externalUserAccount.Email = userInfo.Email;
                    externalUserAccount.Firstname = userInfo.Firstname;
                    externalUserAccount.Lastname = userInfo.Lastname;
                    externalUserAccount.Role = userInfo.Role;
                    externalUserAccount.SupplierId = string.Join(",", userInfo.SupplierId.Split(',').Distinct());
                    externalUserAccount.BirthMonth = short.Parse(userInfo.BirthMonth);
                    externalUserAccount.BirthYear = short.Parse(userInfo.BirthYear);
                    externalUserAccount.ZipCode = userInfo.ZipCode;
                    externalUserAccount.Company = userInfo.Company;
                    externalUserAccount.CreatedBy = Identity.UserName;

                    response = _restClient.CreateUser(externalUserAccount);
                    if (response == null)
                    {
                        response = new GenericAjaxResponse<bool>();
                        response.Success = false;
                        response.Errors.Add("An error occurred. Please try again.");
                    }
                    else
                    {
                        response.Success = response.Data;
                    }
                }
                else
                {
                    foreach (var modelKey in ModelState.Keys)
                    {
                        response.Errors.AddRange(ModelState[modelKey].Errors.Select(err => err.ErrorMessage));
                    }
                    response.Success = response.Errors.Count == 0;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Error occurred getting users. Please try again");
                _logger.Info("Could not retrieve users from database. Error " + ex.Message);
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult LockUser(AHP.Core.DTO.ExternalUserInfo userInfo)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            try
            {
                if (string.IsNullOrEmpty(userInfo.Username))
                {
                    response.Success = false;
                    response.Errors.Add("Username is required");
                    return Json(response);
                }

                if (string.IsNullOrEmpty(userInfo.Email))
                {
                    response.Success = false;
                    response.Errors.Add("Email is required");
                    return Json(response);
                }

                response = _restClient.LockUser(userInfo.Username, userInfo.Email);
                if (response == null)
                {
                    response = new GenericAjaxResponse<bool>();
                    response.Success = false;
                    response.Errors.Add("An error occurred. Please try again.");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Error occurred. Please try again");
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult UnlockUser(AHP.Core.DTO.ExternalUserInfo userInfo)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            try
            {
                if (string.IsNullOrEmpty(userInfo.Username))
                {
                    response.Success = false;
                    response.Errors.Add("Username is required");
                    return Json(response);
                }

                if (string.IsNullOrEmpty(userInfo.Email))
                {
                    response.Success = false;
                    response.Errors.Add("Email is required");
                    return Json(response);
                }

                response = _restClient.UnlockUserAccount(userInfo.Username, userInfo.Email);

                if (response == null)
                {
                    response = new GenericAjaxResponse<bool>();
                    response.Success = false;
                    response.Errors.Add("An error occurred. Please try again.");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Error occurred. Please try again");
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult ResetPassword(AHP.Core.DTO.ExternalUserInfo userInfo)
        {
            //email template needs these details {CopyrightYear}{ServerUrl}{RandomPassword}{Username}
            //should check email address and also reset his password and also force change pwd on first logon
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            try
            {
                if (string.IsNullOrEmpty(userInfo.Username))
                {
                    response.Success = false;
                    response.Errors.Add("Username is required");
                    return Json(response);
                }

                if (string.IsNullOrEmpty(userInfo.Email))
                {
                    response.Success = false;
                    response.Errors.Add("Email is required");
                    return Json(response);
                }

                //reset password from admin will always force user to set new password on logon
                response = _restClient.ResetPassword(userInfo.Username, userInfo.Email, true);
                if (response == null)
                {
                    response = new GenericAjaxResponse<bool>();
                    response.Success = false;
                    response.Errors.Add("An error occurred. Please try again.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred resetting user password", ex);
                response.Success = false;
                response.Errors.Add("Error occurred. Please try again");
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult Userdetail(string username)
        {
            GenericAjaxResponse<AHP.Core.DTO.ExternalUserInfo> response = new GenericAjaxResponse<Core.DTO.ExternalUserInfo>();
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    response.Success = false;
                    response.Errors.Add("Please provider a username");
                    return Json(response);
                }
                response = _restClient.GetUserDetails(username);
                if (response == null)
                {
                    response = new GenericAjaxResponse<Core.DTO.ExternalUserInfo>();
                    response.Success = false;
                    response.Errors.Add("An error occurred. Please try again.");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Error occurred.");
                _logger.Error("Error occurred getting user details", ex);
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult ActivateEmail(AHP.Core.DTO.ExternalUserInfo user)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            try
            {
                if (string.IsNullOrEmpty(user.Username))
                {
                    response.Success = false;
                    response.Errors.Add("Username is required");
                    return Json(response);
                }

                if (string.IsNullOrEmpty(user.Email))
                {
                    response.Success = false;
                    response.Errors.Add("Email is required");
                    return Json(response);
                }

                response = _restClient.ActivateEmail(user.Username, user.Email);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Error occurred. Please try again");
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult UpdateUser(AHP.Web.ViewModel.UserinfoViewModel userInfo)
        {
            GenericAjaxResponse<AHP.Core.DTO.ExternalUserInfo> response = new GenericAjaxResponse<Core.DTO.ExternalUserInfo>();
            try
            {
                if (ModelState.IsValid)
                {
                    if (!userInfo.Role.Equals("admin", StringComparison.OrdinalIgnoreCase) && !userInfo.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Errors.Add("Role can be only admin or user");
                        response.Success = false;
                        return Json(response);
                    }

                    AHP.Core.DTO.ExternalUserInfo externalUserAccount = new Core.DTO.ExternalUserInfo();
                    //Only subset of properties are to be filled. Since others are maintained internally
                    externalUserAccount.Username = userInfo.Username;
                    externalUserAccount.Email = userInfo.Email;
                    externalUserAccount.Firstname = userInfo.Firstname;
                    externalUserAccount.Lastname = userInfo.Lastname;
                    externalUserAccount.Role = userInfo.Role;
                    externalUserAccount.SupplierId = string.Join(",", userInfo.SupplierId.Split(',').Distinct());
                    externalUserAccount.ChangePasswordOnLogon = userInfo.ChangePasswordOnLogon;
                    externalUserAccount.BirthMonth = short.Parse(userInfo.BirthMonth);
                    externalUserAccount.BirthYear = short.Parse(userInfo.BirthYear);
                    externalUserAccount.ZipCode = userInfo.ZipCode;
                    externalUserAccount.Company = userInfo.Company;
                    response = _restClient.UpdateUser(externalUserAccount);
                    if (response == null)
                    {
                        response = new GenericAjaxResponse<Core.DTO.ExternalUserInfo>();
                        response.Success = false;
                        response.Errors.Add("An error occurred. Please try again.");
                    }
                }
                else
                {
                    foreach (var modelKey in ModelState.Keys)
                    {
                        response.Errors.AddRange(ModelState[modelKey].Errors.Select(err => err.ErrorMessage));
                    }
                    response.Success = response.Errors.Count == 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred updating the user", ex);
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult ActivateUser(AHP.Core.DTO.ExternalUserInfo userInfo)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            try
            {
                if (string.IsNullOrEmpty(userInfo.Username))
                {
                    response.Success = false;
                    response.Errors.Add("Username is required");
                    return Json(response);
                }

                if (string.IsNullOrEmpty(userInfo.Email))
                {
                    response.Success = false;
                    response.Errors.Add("Email is required");
                    return Json(response);
                }

                response = _restClient.ActivateUser(userInfo.Username, userInfo.Email);
                if (response == null)
                {
                    response = new GenericAjaxResponse<bool>();
                    response.Success = false;
                    response.Errors.Add("An error occurred. Please try again.");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Error occurred. Please try again");
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult DeactivateUser(AHP.Core.DTO.ExternalUserInfo userInfo)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            try
            {
                if (string.IsNullOrEmpty(userInfo.Username))
                {
                    response.Success = false;
                    response.Errors.Add("Username is required");
                    return Json(response);
                }

                if (string.IsNullOrEmpty(userInfo.Email))
                {
                    response.Success = false;
                    response.Errors.Add("Email is required");
                    return Json(response);
                }

                response = _restClient.DeactivateUser(userInfo.Username, userInfo.Email);

                if (response == null)
                {
                    response = new GenericAjaxResponse<bool>();
                    response.Success = false;
                    response.Errors.Add("An error occurred. Please try again.");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Error occurred. Please try again");
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult GetInternalUsers()
        {
            GenericAjaxResponse<List<AHP.Core.DTO.InternalUserInfo>> apiResponse = new GenericAjaxResponse<List<AHP.Core.DTO.InternalUserInfo>>();
            try
            {
                apiResponse = _restClient.GetInternalUsers();
            }
            catch (Exception ex)
            {
                _logger.Info("Error occured getting internal user list", ex);
                apiResponse.Success = false;
                apiResponse.Errors.Add("Could not process your request");
            }
            return Json(apiResponse);
        }

        [HttpPost]
        public ActionResult MapInternalUser(ViewModel.InternalUserViewModel user)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            try
            {
                if (ModelState.IsValid)
                {
                    Core.DTO.InternalUserInfo userInfo = new Core.DTO.InternalUserInfo();
                    userInfo.ActiveHealthId = user.ActiveHealthId;
                    userInfo.TableauId = user.TableauId;
                    response = _restClient.MapInternalUser(userInfo);
                    if (response == null)
                    {
                        response = new GenericAjaxResponse<bool>();
                        response.Success = false;
                        response.Errors.Add("Could not process your request.");
                    }
                    else
                    {
                        response.Success = response.Data;
                    }
                }
                else
                {
                    foreach (var modelKey in ModelState.Keys)
                    {
                        response.Errors.AddRange(ModelState[modelKey].Errors.Select(err => err.ErrorMessage));
                    }
                    response.Success = response.Errors.Count == 0;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Could not process your request");
                _logger.Info("Could not map internal user to tableau account. Error " + ex.Message);
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult UpdateInternalUser(ViewModel.InternalUserViewModel user)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            try
            {
                if (ModelState.IsValid)
                {
                    Core.DTO.InternalUserInfo userInfo = new Core.DTO.InternalUserInfo();
                    userInfo.ActiveHealthId = user.ActiveHealthId;
                    userInfo.TableauId = user.TableauId;
                    response = _restClient.UpdateInternalUser(userInfo);
                    if (response == null)
                    {
                        response = new GenericAjaxResponse<bool>();
                        response.Success = false;
                        response.Errors.Add("Could not process your request.");
                    }
                    else
                    {
                        response.Success = response.Data;
                    }
                }
                else
                {
                    foreach (var modelKey in ModelState.Keys)
                    {
                        response.Errors.AddRange(ModelState[modelKey].Errors.Select(err => err.ErrorMessage));
                    }
                    response.Success = response.Errors.Count == 0;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add("Could not process your request");
                _logger.Info("Could not map internal user to tableau account. Error " + ex.Message);
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult ListTableauViews()
        {
            GenericAjaxResponse<List<AHP.Core.DTO.TableauViewInfo>> apiResponse = new GenericAjaxResponse<List<AHP.Core.DTO.TableauViewInfo>>();
            try
            {
                apiResponse = _restClient.GetTableauViews();
            }
            catch (Exception ex)
            {
                _logger.Info("Error occured getting tableau workbook views list", ex);
                apiResponse.Success = false;
                apiResponse.Errors.Add("Could not process your request");
            }
            return Json(apiResponse);
        }

        [HttpPost]
        public ActionResult GetTableauUrl()
        {
            string url = string.Empty;
            try
            {
                url = ConfigurationManager.AppSettings["tableauServerUrl"];
            }
            catch (Exception ex)
            {
                url = string.Empty;
            }
            return Content(url);
        }

        [HttpPost]
        public ActionResult AddTableauView(AHP.Web.ViewModel.TableauWorkbookViewModel tableauView)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            try
            {
                if (ModelState.IsValid)
                {
                    AHP.Core.DTO.TableauViewInfo dto = new Core.DTO.TableauViewInfo();
                    dto.Description = string.IsNullOrEmpty(tableauView.Description) ? string.Empty : tableauView.Description.Substring(0, Math.Min(400, tableauView.Description.Length));
                    dto.Disabled = string.IsNullOrEmpty(tableauView.Disabled) ? "N" : tableauView.Disabled;
                    dto.IsDashboard = string.IsNullOrEmpty(tableauView.IsDashboard) ? "N" : tableauView.IsDashboard;
                    dto.ViewId = tableauView.ViewId;
                    dto.ViewName = tableauView.ViewName;
                    dto.ViewUrl = tableauView.ViewUrl;
                    response = _restClient.AddTableauView(dto);
                }
                else
                {
                    foreach (var modelKey in ModelState.Keys)
                    {
                        response.Errors.AddRange(ModelState[modelKey].Errors.Select(err => err.ErrorMessage));
                    }
                    response.Success = response.Errors.Count == 0;
                }
            }
            catch (Exception ex)
            {
                response.Success = response.Data = false;
                response.Errors.Add("Could not process your request. Please try again.");
                _logger.Error("Add tableau view ended up with an error", ex);
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult UpdateableauView(AHP.Web.ViewModel.TableauWorkbookViewModel tableauView)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            try
            {
                if (ModelState.IsValid)
                {
                    Guid parsedViewId;
                    if (!string.IsNullOrEmpty(tableauView.ViewId) && Guid.TryParse(tableauView.ViewId, out parsedViewId))
                    {
                        AHP.Core.DTO.TableauViewInfo dto = new Core.DTO.TableauViewInfo();
                        dto.Description = string.IsNullOrEmpty(tableauView.Description)?string.Empty: tableauView.Description.Substring(0,Math.Min(400, tableauView.Description.Length));
                        dto.Disabled = string.IsNullOrEmpty(tableauView.Disabled) ? "N" : tableauView.Disabled;
                        dto.IsDashboard = string.IsNullOrEmpty(tableauView.IsDashboard) ? "N" : tableauView.IsDashboard;
                        dto.ViewId = tableauView.ViewId;
                        dto.ViewName = tableauView.ViewName;
                        dto.ViewUrl = tableauView.ViewUrl;
                        response = _restClient.UpdateTableauView(dto);
                    }
                    else
                    {
                        response.Success = false;
                        response.Errors.Add("View Id is required. Please refresh screen and try again.");
                    }
                }
                else
                {
                    foreach (var modelKey in ModelState.Keys)
                    {
                        response.Errors.AddRange(ModelState[modelKey].Errors.Select(err => err.ErrorMessage));
                    }
                    response.Success = response.Errors.Count == 0;
                }
            }
            catch (Exception ex)
            {
                response.Success = response.Data = false;
                response.Errors.Add("Could not process your request. Please try again.");
                _logger.Error("Update tableau view ended up with an error", ex);
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult UpdateTableauUserInfo(string viewId,List<AHP.Web.ViewModel.TableauViewUserAssociation> userInfo)
        {
            GenericAjaxResponse<bool> response = new GenericAjaxResponse<bool>();
            try
            {               
                List<Core.DTO.TableauViewUserAssociation> usrVwAssoc = new List<Core.DTO.TableauViewUserAssociation>();
                if (userInfo != null && userInfo.Count > 0)
                {
                    //add only selected or checked users only
                    usrVwAssoc.AddRange(userInfo.Where(usrInf => usrInf.Selected).Select(usrInf => new Core.DTO.TableauViewUserAssociation()
                    {
                        Username = usrInf.Username,
                        UserType = usrInf.UserType,
                        ViewId = viewId
                    }));
                }                
                response = _restClient.UpdateTableauUserAssociation(viewId, usrVwAssoc);
            }
            catch (Exception ex)
            {
                response.Success = response.Data = false;
                response.Errors.Add("Could not process your request");
                _logger.Error("Error occurred updating tableau view user association", ex);
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult GetUsersForView(string viewId)
        {
            GenericAjaxResponse<List<Web.ViewModel.TableauViewUserAssociation>> webResponse = new GenericAjaxResponse<List<Web.ViewModel.TableauViewUserAssociation>>();
            try
            {
                if (string.IsNullOrEmpty(viewId))
                {
                    webResponse.Errors.Add("View id provided is empty");
                    webResponse.Success = false;
                    return Json(webResponse);
                }
                GenericAjaxResponse<List<Core.DTO.TableauViewUserAssociation>> apiResponse = _restClient.ListViewAssociations(viewId);
                webResponse.Errors = apiResponse.Errors;
                webResponse.Success = apiResponse.Success;
                if (apiResponse.Data != null)
                {
                    webResponse.Data = apiResponse.Data.Select(view => new Web.ViewModel.TableauViewUserAssociation()
                    {
                        Selected = view.Selected,
                        Username = view.Username,
                        UserType = view.UserType,
                        ViewId = view.ViewId
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                webResponse.Success = false;
                webResponse.Errors.Add("Could not process the request.");
                _logger.Error("Error occurred getting list of all users for view", ex);
            }
            return Json(webResponse);
        }

        #endregion
    }
}