using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Kentor.AuthServices.StubIdp.Models
{
    public class IdpConfigurationModel
    {
        public IdpConfigurationModel(string jsonData)
        {
            JsonData = jsonData;
            ETag = ComputeEtag(jsonData);
            DefaultAssertionConsumerServiceUrl = GetAcs(jsonData);
        }

        private static string GetAcs(string jsonData)
        {
            try
            {
                var parsedJson = JObject.Parse(jsonData);
                var acs = (string)parsedJson["DefaultAssertionConsumerServiceUrl"];
                return acs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string ComputeEtag(string jsonData)
        {
            var md5Hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(jsonData));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5Hash.Length; i++)
            {
                sb.Append(md5Hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        public string JsonData { get; private set; }

        public string ETag { get; private set; }

        public string DefaultAssertionConsumerServiceUrl { get; private set; }
    }
}