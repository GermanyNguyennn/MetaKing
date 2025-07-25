﻿namespace MetaKing.BackendServer.Services.VNPay
{
    public class VNPayInformationModel
    {
        public string OrderType { get; set; }
        public string OrderId { get; set; }
        public string? OrderInfo { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
