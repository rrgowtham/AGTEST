using AHP.Core.Logger;
using AHP.Core.Model;
using AHP.Core.Repository;
using AHP.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AHP.Service
{
    public class BOReportService : IBOReportService
    {

        #region -- Members --

        private readonly IBIRepository _biRepository;

        private readonly IActiveAnalyticsLogger _logger;

        private readonly StringBuilder _logMessages;

        private void GetChildren(BOAuthentication authModel, ReportCategory parentCat)
        {
            XmlDocument docRecv = new System.Xml.XmlDocument();

            _biRepository.CreateWebRequest
                (
                    send: null,
                    recv: docRecv,
                    method: "GET",
                    URI: authModel.URI,
                    URIExtension: string.Format("/biprws/infostore/{0}/children", parentCat.InfoStoreId),
                    pLogonToken: authModel.LogonToken
                );

            XmlNamespaceManager nsmgrGET = new XmlNamespaceManager(docRecv.NameTable);
            nsmgrGET.AddNamespace("rest", authModel.NameSpace);

            XmlNodeList list = docRecv.GetElementsByTagName("entry");
            for (int i = 1; i <= list.Count; i++)
            {
                XmlNode x = list[i - 1];
                string format = "(//rest:attr[@name='{0}'])[{1}]";
                if (x.SelectSingleNode(string.Format(format, "type", i), nsmgrGET).InnerText == "Folder" && x.SelectSingleNode(string.Format(format, "name", i), nsmgrGET).InnerText != "Dashboards")
                {
                    ReportCategory subCat = new ReportCategory()
                    {
                        Name = x.SelectSingleNode(string.Format(format, "name", i), nsmgrGET).InnerText,
                        InfoStoreId = x.SelectSingleNode(string.Format(format, "id", i), nsmgrGET).InnerText,
                        Description = x.SelectSingleNode(string.Format(format, "description", i), nsmgrGET).InnerText,
                        Cuid = x.SelectSingleNode(string.Format(format, "cuid", i), nsmgrGET).InnerText
                    };

                    parentCat.SubCategories.Add(subCat);
                    GetChildren(authModel, subCat);
                }
                else if (x.SelectSingleNode(string.Format(format, "name", i), nsmgrGET).InnerText != "Dashboards")
                {
                    parentCat.Reports.Add(new Report()
                    {
                        Name = x.SelectSingleNode(string.Format(format, "name", i), nsmgrGET).InnerText,
                        InfoStoreId = x.SelectSingleNode(string.Format(format, "id", i), nsmgrGET).InnerText,
                        Description = x.SelectSingleNode(string.Format(format, "description", i), nsmgrGET).InnerText,
                        Cuid = x.SelectSingleNode(string.Format(format, "cuid", i), nsmgrGET).InnerText
                    });
                }
            }
        }

        private bool GetOpenDocumentUri(XmlNode x, ref string openDocumentUri)
        {
            if (x.Attributes["title"] != null && x.Attributes["title"].Value == "OpenDocument")
            {
                openDocumentUri = x.Attributes["href"].Value;
                return true;
            }

            return false;
        }

        private List<string> GetFolders(BOAuthentication authModel)
        {

            XmlDocument docRecv = new System.Xml.XmlDocument();
            List<string> _lstfolderIds = new List<string>();
            _biRepository.CreateWebRequest(send: null, recv: docRecv, method: "GET", URI: authModel.URI, URIExtension: "/biprws/infostore/Root%20Folder/children",
                                                                                                                                pLogonToken: authModel.LogonToken);

            XmlNamespaceManager nsmgrGET = new XmlNamespaceManager(docRecv.NameTable);
            nsmgrGET.AddNamespace("rest", authModel.NameSpace);

            ReportCategory category = new ReportCategory();
            XmlNodeList nodeList = docRecv.SelectNodes("//rest:attr[@name='type']", nsmgrGET);

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList.Item(i).InnerText == "Folder")
                {
                    category.Name = docRecv.SelectNodes("//rest:attr[@name='name']", nsmgrGET)[i].InnerText;
                    category.InfoStoreId = docRecv.SelectNodes("//rest:attr[@name='id']", nsmgrGET)[i].InnerText;
                    category.Description = docRecv.SelectNodes("//rest:attr[@name='description']", nsmgrGET)[i].InnerText;
                    category.Cuid = docRecv.SelectNodes("//rest:attr[@name='cuid']", nsmgrGET)[i].InnerText;
                    _lstfolderIds.Add(category.InfoStoreId);
                    //GetChildren(authModel, category);
                }
            }

            return _lstfolderIds;

        }

        #endregion

        #region -- Constructors --

        public BOReportService(IBIRepository biRepository, IActiveAnalyticsLogger logger)
        {
            _biRepository = biRepository;
            _logger = logger;
            _logMessages = new StringBuilder();
        }

        #endregion

        #region -- IBOReportService Members --
        
        public ReportCategory GetReportList(BOAuthentication authModel, string defaultFolderId)
        {
            //TODO: Unit test it or Understand refactor it for readability
            ReportCategory category = new ReportCategory();
            XmlDocument docRecv = new System.Xml.XmlDocument();
            try
            {
                _logMessages.Append("Getting reports list from BO System.");
                //Get all folders from Root Folder
                List<string> _lstfolderIds = GetFolders(authModel);

                //For every folder
                for (int i = 0; i < _lstfolderIds.Count(); i++)
                {
                    //Get the list of reports within that folder
                    //TODO: Check with the send param,it should not be null
                    _biRepository.CreateWebRequest(send: null, recv: docRecv, method: "GET", URI: authModel.URI, URIExtension: "/biprws/infostore/" + _lstfolderIds[i],
                                                                                                                                        pLogonToken: authModel.LogonToken);

                    XmlNamespaceManager nsmgrGET = new XmlNamespaceManager(docRecv.NameTable);                    
                    nsmgrGET.AddNamespace("rest", authModel.NameSpace);
                    XmlNodeList nodeList = docRecv.SelectNodes("//rest:attr[@name='type']", nsmgrGET);
                    if (nodeList.Item(0).InnerText.Equals("Folder",StringComparison.OrdinalIgnoreCase))
                    {
                        ReportCategory parentCat = new ReportCategory()
                        {
                            Name = docRecv.SelectSingleNode("//rest:attr[@name='name']", nsmgrGET).InnerText,
                            InfoStoreId = docRecv.SelectSingleNode("//rest:attr[@name='id']", nsmgrGET).InnerText,
                            Description = docRecv.SelectSingleNode("//rest:attr[@name='description']", nsmgrGET).InnerText,
                            Cuid = docRecv.SelectSingleNode("//rest:attr[@name='cuid']", nsmgrGET).InnerText
                        };
                        category.ParentCategories.Add(parentCat);

                        //Find sub reports of this report
                        GetChildren(authModel, parentCat);
                    }
                }
                _logMessages.Append("Finished Iterating all reports folder");
            }
            catch (Exception ex)
            {
                _logMessages.AppendFormat("An Error occurred getting reports list Exception message {0}.",ex.Message);
                _logger.Info(_logMessages.ToString());
                throw;
            }
            _logger.Info(_logMessages.ToString());
            return category;
        }        

        public string GetReport(BOAuthentication authModel, string reportId)
        {
            _logMessages.AppendFormat("Retrieving opendocument url for report {0}.",reportId);
            // Get OpenDocumentURI
            XmlDocument docRecv = new System.Xml.XmlDocument();
            _biRepository.CreateWebRequest(send: null, recv: docRecv, method: "GET", URI: authModel.URI, URIExtension: "/biprws/infostore/" + reportId,
                                                                                                                                pLogonToken: authModel.LogonToken);

            var links = docRecv.GetElementsByTagName("link");
            string openDocumentUri = string.Empty;
            for (int counter = 0, reverseCounter = links.Count - 1; counter < links.Count / 2; counter++, reverseCounter--)
            {
                if (GetOpenDocumentUri(links[counter], ref openDocumentUri) || GetOpenDocumentUri(links[reverseCounter], ref openDocumentUri))
                {
                    _logMessages.Append("BO REST responded back with valid opendocument url.");
                    break;
                }
            }                      

            //append open doc url with session since it occupies only one license
            openDocumentUri = string.Format("{0}&serSes={1}", openDocumentUri, System.Web.HttpUtility.UrlEncode(authModel.BOSesssionID));

            _logMessages.AppendFormat("Final Opendocument url is {0}.",openDocumentUri);

            _logger.Info(_logMessages.ToString());
            return openDocumentUri;
        }


        #endregion

       
    }
}
