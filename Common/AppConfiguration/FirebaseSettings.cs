using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace Common.AppConfiguration
{
    public class FirebaseSettings
    {
        [ConfigurationKeyName("type")]
        [JsonProperty("type")]
        public string Type { get; set; }
        [ConfigurationKeyName("project_id")]
        [JsonProperty("project_id")]
        public string ProjectId { get; set; }
        [ConfigurationKeyName("private_key_id")]
        [JsonProperty("private_key_id")]
        public string PrivateKeyId { get; set; }
        [ConfigurationKeyName("private_key")]
        [JsonProperty("private_key")]
        public string PrivateKey { get; set; }
        [ConfigurationKeyName("client_email")]
        [JsonProperty("client_email")]
        public string ClientEmail { get; set; }
        [ConfigurationKeyName("client_id")]
        [JsonProperty("client_id")]
        public string ClientId { get; set; }
        [ConfigurationKeyName("auth_uri")]
        [JsonProperty("auth_uri")]
        public string AuthUri { get; set; }
        [ConfigurationKeyName("token_uri")]
        [JsonProperty("token_uri")]
        public string TokenUri { get; set; }
        [ConfigurationKeyName("auth_provider_x509_cert_url")]
        [JsonProperty("auth_provider_x509_cert_url")]
        public string AuthProviderX509CertUrl { get; set; }
        [ConfigurationKeyName("client_x509_cert_url")]
        [JsonProperty("client_x509_cert_url")]
        public string ClientX509CertUrl { get; set; }
        [ConfigurationKeyName("universe_domain")]
        [JsonProperty("universe_domain")]
        public string UniverseDomain { get; set; }
    }
}
