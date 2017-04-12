using Kentor.AuthServices.Saml2P;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Exceptions
{
    /// <summary>
    /// Extended exception containing information about the status and status message SAML response.  
    /// </summary>
    [Serializable]
    public class UnsuccessfulSamlOperationException : AuthServicesException
    {
        /// <summary>
        /// Status of the SAML2Response
        /// </summary>
        public Saml2StatusCode Status { get; set; }
        /// <summary>
        /// Status message of SAML2Response
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Second level status of SAML2Response
        /// </summary>
        public string SecondLevelStatus { get; set; }

        /// <summary>
        /// Ctor, bundling the Saml2 status codes and message into the exception message.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="statusCode">Status of the SAML2Response</param>
        /// <param name="statusMessage">Status message of SAML2Response</param>
        /// <param name="secondLevelStatus">Second level status of SAML2Response</param>
        public UnsuccessfulSamlOperationException(string message, Saml2StatusCode statusCode, string statusMessage, string secondLevelStatus) : 
            base(message + "\n" +
                "  Saml2 Status Code: " + statusCode + "\n" +
                "  Saml2 Status Message: " + statusMessage + "\n" +
                "  Saml2 Second Level Status: " + secondLevelStatus)
        {
            this.Status = statusCode;
            this.StatusMessage = statusMessage;
            this.SecondLevelStatus = secondLevelStatus;
        }
        /// <summary>
        /// 
        /// </summary>
        public UnsuccessfulSamlOperationException() : base()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public UnsuccessfulSamlOperationException(string message) : base(message)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public UnsuccessfulSamlOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected UnsuccessfulSamlOperationException(SerializationInfo info, StreamingContext context): base(info, context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

    }
}
