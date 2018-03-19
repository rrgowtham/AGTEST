using AHP.Core;
using AHP.Core.Logger;
using AHP.Core.Repository;
using System.IO;
using System.Net;
using System.Xml;

namespace AHP.Repository
{
    public class BIRepository : IBIRepository
    {
        #region -- Members --
        private readonly IActiveAnalyticsLogger _logger; 
        #endregion

        #region -- Constructors --

        public BIRepository(IActiveAnalyticsLogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region -- IBIRepository Implementation --
       
        public WebRequestResponseStatus CreateWebRequest(XmlDocument send, XmlDocument recv, string method, string URI, string URIExtension, string pLogonToken)
        {
            //TODO: Need to refactor using single responsibility.

            HttpWebRequest request;
            HttpWebResponse response;

            if (method != "GET" && method != "PUT" && method != "POST")
            {
                // unsupported method
                return WebRequestResponseStatus.Unsupported;
            }

            request = (HttpWebRequest)WebRequest.Create(URI + URIExtension);
            request.Method = method;
            request.ContentType = "application/xml";
            request.Accept = "application/xml";

            if (pLogonToken != "")
            {
                request.Headers["X-SAP-LogonToken"] = pLogonToken;
            }

            // if the method is post or put, the body must be prepared
            if (method == "POST" || method == "PUT")
            {
                // turn the send xml into a stream and put it in request
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(send.OuterXml);
                try
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                    }                        
                }
                catch(System.Exception ex)
                {
                    _logger.Error("An Error occurred trasforming input xml to bytes.", ex);
                    return WebRequestResponseStatus.Failure;
                }
            }

            // send the request and retrieve the response
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if (response != null)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // logging off has no response stream and an attempt to read it will fail
                        if (URIExtension != "/biprws/logoff")
                        {
                            recv.Load(response.GetResponseStream());
                        }
                    }
                    else
                    {
                        return WebRequestResponseStatus.InvalidResponse;
                    }
                }
                else
                {
                    return WebRequestResponseStatus.InvalidResponse;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string error = reader.ReadToEnd();
                    _logger.Error("An Error occurred during CreateWebRequest , Error detail "+ error+".",ex);
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error("An Error occurred during CreateWebRequest. Inside Catch exception block.", ex);
                return WebRequestResponseStatus.Failure;
            }

            return WebRequestResponseStatus.Success;
        }

        public string GetHTML(string URI, string URIExtension, string pLogonToken)
        {
            string output = string.Empty;
            string hostUrl = string.Format("{0}{1}",URI,URIExtension);
            HttpWebRequest GetRequest = (HttpWebRequest)WebRequest.Create(hostUrl);

            try
            {
                GetRequest.Method = "GET";
                GetRequest.Accept = "text/html";
                GetRequest.Headers.Set("X-SAP-LogonToken", pLogonToken);
                using (HttpWebResponse GETResponse = (HttpWebResponse)GetRequest.GetResponse())
                {
                    using (Stream GETResponseStream = GETResponse.GetResponseStream())
                    {
                        StreamReader sr = new StreamReader(GETResponseStream);
                        output = sr.ReadToEnd();
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error("Error occurred getting Report details.", ex);
            }
            return output;
        }
        
        public WebRequestResponseStatus Logoff(string URI, string authToken)
        {
            XmlDocument request = new XmlDocument();
            XmlDocument response = new XmlDocument();
            return CreateWebRequest(request, response, "POST", URI, "/biprws/logoff", authToken);
        } 

        #endregion

    }
}
