﻿namespace MetaKing.BackendServer.Services.MoMo
{
    public class MoMoOptionModel
    {
        public string MoMoApiUrl { get; set; }
        public string SecretKey { get; set; }
        public string AccessKey { get; set; }
        public string ReturnUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string PartnerCode { get; set; }
        public string RequestType { get; set; }
    }
}
