using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub.Util
{

    public static class DsarExtensions
    {
        private const string _rawTlogContainerName = "raw-tlog";

        public static IEnumerable<string> GetJsonUrisFromBlobContent(this Stream downloadResult)
        {
            StreamReader sr = new(downloadResult);
            while (sr.Peek() >= 0)
            {
                var jsonUri = sr.ReadLine();

                yield return jsonUri;
            }

        }

        public static string GetTlogUriFromBlobContent(this string downloadResult)
        {

            if (string.IsNullOrEmpty(downloadResult) || (!downloadResult.Contains(".xml")) || (downloadResult.Split("/").Length == 4))
                return string.Empty;                

            var tlogUri = downloadResult.Split(_rawTlogContainerName + "/").Last();

            return tlogUri;
         }

        public static XmlDocument DeidentifyCustomerIdFromTlog(this XmlDocument downloadResult)
        {
            if (downloadResult is null)
                return new XmlDocument();

            XmlNode tlogRoot = downloadResult.DocumentElement;

            XmlNodeList tlogNodeList = tlogRoot.SelectNodes("TransactionRecord");

            if (tlogNodeList is null)
                return new XmlDocument();

            // De-identify customer id number from every TransactionRecord child node
            foreach (XmlNode tlogNode in tlogNodeList)
            {
                XmlNode tlogChildNode = tlogNode.SelectSingleNode("CustomerAccountID");
                if (tlogChildNode != null)
                {
                    tlogChildNode.InnerText = "1111111";
                }
                XmlNode tlogChildNode2 = tlogNode.SelectSingleNode("CustomerNumberAsEntered");
                if (tlogChildNode2 != null)
                {
                    tlogChildNode2.InnerText = "11111111111";
                }
            }

            return downloadResult;

        }
    }
}