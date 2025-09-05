using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RX.PharmacyBusiness.ETL.RXS516
{
    public class POSMessaging
    {
        public long m_messageHeaderId { get; set; }
        public string m_destinationId { get; set; }
        public string m_securityTokenType { get; set; }
        public string m_securityTokenEncoding { get; set; }

        public int m_defaultTransactionSequenceNumber { get; set; }
        public short m_defaultPOSTerminalNumber { get; set; }
        public int m_defaultPOSTransactionNumber { get; set; }
        public string m_defaultUserId { get; set; }

        public string m_erxFinalizationURL { get; set; }

        // HACK since 4.0 doesn't expose TLS 1.2 nor 1.1
        private static SecurityProtocolType Tls = SecurityProtocolType.Tls;
        private static SecurityProtocolType Tls11 = (SecurityProtocolType)768;
        private static SecurityProtocolType Tls12 = (SecurityProtocolType)3072;

        #region "Constructor"
        /// <summary>
        /// Default Constructor
        /// </summary>
        public POSMessaging()
        {
            m_destinationId = "TREX"; // Read from configuration
            m_messageHeaderId = (long)1.0; //Read from configuration
            m_securityTokenType = "X509v1";
            m_securityTokenEncoding = "Base64Binary";
            m_erxFinalizationURL = ConfigurationManager.AppSettings["RXS516_FinalizationURL"];

            m_defaultTransactionSequenceNumber = 1;
            m_defaultPOSTransactionNumber = 101;
            m_defaultPOSTerminalNumber = 100;
            m_defaultUserId = "Manual";
        }

        #endregion //Constructor

        public bool SellPrescription(string storeNumber,
                             int prescriptionNumber,
                             short refillNumber,
                             short partialFillSequenceNumber,
                             DateTime soldDateTime,
                             decimal copayAmount)
        {
            return SendPOSFinalizationRequest(m_defaultUserId,
                                              m_defaultPOSTerminalNumber,
                                              m_defaultPOSTransactionNumber,
                                              m_defaultTransactionSequenceNumber,
                                              storeNumber,
                                              prescriptionNumber,
                                              refillNumber,
                                              partialFillSequenceNumber,
                                              soldDateTime,
                                              copayAmount,
                                              TxnHeaderInfoTypeTxnTypeInd.S);
        }//SellPrescription

        public bool RefundPrescription(string storeNumber,
                               int prescriptionNumber,
                               short refillNumber,
                               short partialFillSequenceNumber,
                               DateTime soldDateTime,
                               decimal copayAmount)
        {
            return SendPOSFinalizationRequest(m_defaultUserId,
                                              m_defaultPOSTerminalNumber,
                                              m_defaultPOSTransactionNumber,
                                              m_defaultTransactionSequenceNumber,
                                              storeNumber,
                                              prescriptionNumber,
                                              refillNumber,
                                              partialFillSequenceNumber,
                                              soldDateTime,
                                              copayAmount,
                                              TxnHeaderInfoTypeTxnTypeInd.R);
        }//RefundPrescription

        /// <summary>
        /// This routine sends a Point of Sale finalization message
        /// </summary>
        /// <param name="storeNumber"></param>
        /// <param name="prescriptionNumber"></param>
        /// <param name="refillNumber"></param>
        /// <param name="partialFillSequenceNumber"></param>
        /// <param name="soldDateTime"></param>
        /// <param name="copayAmount"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        private bool SendPOSFinalizationRequest(string posUserId,
                                                short posTerminalNumber,
                                                int posTransactionNumber,
                                                int posTransactionSequenceNumber,
                                                string storeNumber,
                                                int prescriptionNumber,
                                                short refillNumber,
                                                short partialFillSequenceNumber,
                                                DateTime soldDateTime,
                                                decimal copayAmount,
                                                TxnHeaderInfoTypeTxnTypeInd transactionType)
        {
            bool posMessageSentSuccessfully = false;

            #region "Load Digital Signature"

            //Retrieve configuration
            string executionPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string certificateFile = ConfigurationManager.AppSettings["RXS516_ERxCert_Path"];
            string certificateFilePath = string.Empty;

            //Build path to certificate file
            if (certificateFile.IndexOf(Path.DirectorySeparatorChar).Equals(0))
                certificateFilePath = executionPath + certificateFile;
            else
                certificateFilePath = Path.Combine(executionPath, certificateFile);
            X509Certificate certificate = new X509Certificate(certificateFilePath);

            #endregion

            #region "Prescription Identication"
            RxIDType rxIdentifier = new RxIDType();
            rxIdentifier.RxNum = prescriptionNumber.ToString();
            rxIdentifier.RxTxnNum = refillNumber;
            rxIdentifier.PartialFillSeqNum = partialFillSequenceNumber;
            #endregion

            #region "Sold Prescription Information"
            RxSoldInfoType rxSoldInfo = new RxSoldInfoType();
            rxSoldInfo.TimeStamp = String.Format("{0:yyyy-MM-ddTHH:mm:ss.000zzz}", soldDateTime);
            rxSoldInfo.UserID = posUserId;
            rxSoldInfo.AmountCollected = copayAmount;
            #endregion

            // Signature information is not included in the POS
            #region "Signature Capture Information"
            SigCaptureInfoType signatureCaptureInfo = new SigCaptureInfoType();
            signatureCaptureInfo.EasyCapSigCapture = false;
            signatureCaptureInfo.HIPAASigCapture = false;
            signatureCaptureInfo.RxAckSigCapture = false;
            signatureCaptureInfo.SigImageFile = string.Empty;
            signatureCaptureInfo.SigCapReasonCode = 0;
            #endregion

            #region "Transcation Header Information"
            TxnHeaderInfoType txHeaderInfo = new TxnHeaderInfoType();

            //Cash Register Terminal Number 
            txHeaderInfo.RegisterNum = posTerminalNumber;

            //Point of Sale (POS) Transaction Number
            txHeaderInfo.TxnNum = posTransactionNumber;

            //Transction Sequence Number
            txHeaderInfo.TxnSeqNum = posTransactionSequenceNumber;

            //Transaction indicator type (S)ale, (R)efund, (P)ost Void
            txHeaderInfo.TxnTypeInd = transactionType;
            #endregion //"Transaction Header Information"

            #region "Build POS finalization Request"
            POSFinalizationReq posFinalRequest = new POSFinalizationReq();
            posFinalRequest.RxID = rxIdentifier;
            posFinalRequest.RxSoldInfo = rxSoldInfo;
            posFinalRequest.SigCaptureInfo = signatureCaptureInfo;
            posFinalRequest.TxnHeaderInfo = txHeaderInfo;
            #endregion 

            #region "Message Header"

            MsgHeaderType messageHeader = new MsgHeaderType();
            messageHeader.DestinationID = m_destinationId;

            //Message Sequence Number
            messageHeader.MsgID = m_messageHeaderId;

            //Name of the message body or type of message being sent
            messageHeader.MsgName = MsgHeaderTypeMsgName.POSFinalizationReq;

            //Source of the message, this is typically the store
            messageHeader.SourceID = storeNumber;

            //Marked as optional field
            messageHeader.TimeStamp = String.Format("{0:yyyy-MM-ddTHH:mm:ss.000zzz}", DateTime.Now);

            //Version of the message
            messageHeader.Version = (decimal)1.0;
            #endregion

            #region "WS Security"

            BinarySecurityTokenType securityToken = new BinarySecurityTokenType();
            securityToken.Id = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(certificate.Subject));
            securityToken.Value = Convert.ToBase64String(certificate.GetPublicKey()).Substring(4);
            securityToken.ValueType = m_securityTokenType;
            securityToken.EncodingType = m_securityTokenEncoding;

            #endregion

            #region "Build POS Message"
            POS pos = new POS();
            pos.Security = new SecurityHeaderType();
            pos.Security.BinarySecurityToken = securityToken;
            pos.RequestEncoding = System.Text.UTF8Encoding.Default;
            pos.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Default;
            pos.MsgHeader1 = messageHeader;
            pos.Url = m_erxFinalizationURL;
            #endregion

            ServicePointManager.SecurityProtocol = Tls | Tls11 | Tls12;

            #region "Send Message"
            try
            {
                trustedCertificatePolicy myPolicy = new trustedCertificatePolicy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = myPolicy.RemoteCertificateValidationCallback;

                GenericSuccessRsp posResponse = pos.POSFinalizationRequest(posFinalRequest);
                if (posResponse != null)
                {
                    posMessageSentSuccessfully = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion

            return posMessageSentSuccessfully;

        }
    }

    public class trustedCertificatePolicy : System.Net.ICertificatePolicy
    {
        public trustedCertificatePolicy() { }

        public bool RemoteCertificateValidationCallback(Object sender,
            X509Certificate certificate,
            X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #region ICertificatePolicy Members

        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return true;
        }

        #endregion
    }
}
