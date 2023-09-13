﻿using static Mango.Web.Utility.StaticDetails;

namespace Mango.Web.Models
{
    public class RequestType
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
    }
}
