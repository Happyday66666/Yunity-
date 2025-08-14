using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class EcpayOrder
{
    public int Id { get; set; }

    public string MerchantTradeNo { get; set; } = null!;

    public string MemberId { get; set; } = null!;

    public int? RtnCode { get; set; }

    public string? RtnMsg { get; set; }

    public string TradeNo { get; set; } = null!;

    public int? TradeAmt { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentTypeChargeFee { get; set; }

    public string? TradeDate { get; set; }

    public int? SimulatePaid { get; set; }

    public string? PaymentType { get; set; }
}
