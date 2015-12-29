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
            DefaultAssertionConsumerServiceUrl = GetJsonValue(jsonData, "DefaultAssertionConsumerServiceUrl");
            IdpDescription = GetJsonValue(jsonData, "IdpDescription");
            HideDetails = (GetJsonValue(jsonData, "HideDetails") ?? "true").ToLower() == "true";
        }

        private static string GetJsonValue(string jsonData, string key)
        {
            try
            {
                var parsedJson = JObject.Parse(jsonData);
                var value = (string)parsedJson[key];
                return value;
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

        public string IdpDescription { get; private set; }

        public bool HideDetails { get; private set; }
    }
}