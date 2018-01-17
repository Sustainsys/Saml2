using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Sustainsys.Saml2.StubIdp.Models
{
    public class IdpConfigurationModel
    {
        public IdpConfigurationModel(string jsonData)
        {
            JsonData = jsonData;
            ETag = ComputeEtag(jsonData);
            DefaultAssertionConsumerServiceUrl = GetJsonValue(jsonData, "DefaultAssertionConsumerServiceUrl");
            DefaultAudience = GetJsonValue(JsonData, "DefaultAudience");
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
        public string JsonData { get; }
        public string ETag { get; }

        public string DefaultAssertionConsumerServiceUrl { get; }

        public string DefaultAudience { get; }

        public string IdpDescription { get;}

        public bool HideDetails { get; }
    }
}