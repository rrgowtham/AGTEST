using System.Xml;

namespace AHP.Core.Repository
{
    /// <summary>
    /// Business Intelligence REST service helper
    /// </summary>
    public interface IBIRepository
    {
        /// <summary>
        /// Sends request to the specified <paramref name="URI"/> and <paramref name="URIExtension"/>
        /// </summary>
        /// <param name="send">Required xml to be sent in body</param>
        /// <param name="recv">Response received from the server</param>
        /// <param name="method">Only GET, PUT and POST allowed</param>
        /// <param name="URI">Valid url to the host with protocol</param>
        /// <param name="URIExtension">Valid path to host must be preceded with forward slash</param>
        /// <param name="pLogonToken">Token to be passed to the BO server</param>
        /// <returns>Instance of <see cref="WebRequestResponseStatus"/></returns>
        WebRequestResponseStatus CreateWebRequest(XmlDocument send, XmlDocument recv, string method, string URI, string URIExtension, string pLogonToken);

        /// <summary>
        /// Gets the html response after performing get request
        /// </summary>
        /// <param name="URI">Valid url to the host with protocol</param>
        /// <param name="URIExtension">Valid path to host must be preceded with forward slash</param>
        /// <param name="pLogonToken">Token to be passed to the BO server</param>
        /// <returns>Response from the request</returns>
        string GetHTML(string URI, string URIExtension, string pLogonToken);

        /// <summary>
        /// Log off from the system
        /// </summary>
        /// <param name="URI">Valid url to the host with protocol</param>
        /// <param name="authToken">Token to be passed to the BO server</param>
        /// <returns>Instance of <see cref="WebRequestResponseStatus"/></returns>
        WebRequestResponseStatus Logoff(string URI, string authToken);
    }
}
