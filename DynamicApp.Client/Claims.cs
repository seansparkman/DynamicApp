using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DynamicApp.Client
{
    public class Claims
    {
        [JsonProperty("user")]
        public string UserUuid { get; set; }
        [JsonProperty("capabilities")]
        public List<int> Capabilities { get; set; }
        [JsonProperty("tci")]
        public string Tci { get; set; }
        [JsonProperty("jti")]
        public string Jti { get; set; }
        [JsonProperty("iat")]
        public long TokenIssuedAt { get; set; }
        [JsonProperty("exp")]
        public long TokenExpiration { get; set; }

        [JsonIgnore]
        public long TokenHalfLife
        {
            get { return (TokenExpiration - TokenIssuedAt) / 2 + TokenIssuedAt; }
        }
        [JsonIgnore]
        public bool IsPastHalfLife
        {
            get
            {
                return TokenHalfLife < Claims.GetCurrentEpochTime();
            }
        }

        private static long GetCurrentEpochTime()
        {
            var timeDiff = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return Convert.ToInt64(timeDiff.TotalSeconds);
        }
    }
}
