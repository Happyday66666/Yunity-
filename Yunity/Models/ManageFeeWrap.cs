using System;
using System.Collections.Generic;

namespace Yunity.Models;

public partial class ManageFeeWrap
{
    public ManageFee _ManageFee;
    public ManageFee ManageFee
    {
        get { return _ManageFee; }
        set { _ManageFee = value; }
    }

    public ManageFeeWrap()
    {
        _ManageFee = new ManageFee();
    }
    public int Id
    {
        get { return _ManageFee.Id; }
        set { _ManageFee.Id = value; }
    }

    public string? FeeName
    {
        get { return _ManageFee.FeeName; }
        set { _ManageFee.FeeName = value; }
    }

    public int? DoorNoId
    {
        get { return _ManageFee.DoorNoId; }
        set { _ManageFee.DoorNoId = value; }
    }

    public int? Price
    {
        get { return _ManageFee.Price; }
        set { _ManageFee.Price = value; }
    }

    public int? MotorPrice
    {
        get { return _ManageFee.MotorPrice; }
        set { _ManageFee.MotorPrice = value; }
    }

    public int? CarPrice
    {
        get { return _ManageFee.CarPrice; }
        set { _ManageFee.CarPrice = value; }
    }

    public DateTime? FeeEnd
    {
        get { return _ManageFee.FeeEnd; }
        set { _ManageFee.FeeEnd = value; }
    }

    public int? PayType
    {
        get { return _ManageFee.PayType; }
        set { _ManageFee.PayType = value; }
    }

    public DateTime? PayTime
    {
        get { return _ManageFee.PayTime; }
        set { _ManageFee.PayTime = value; }
    }

    public int? State
    {
        get { return _ManageFee.State; }
        set { _ManageFee.State = value; }
    }

    public DateTime? LogTime
    {
        get { return _ManageFee.LogTime; }
        set { _ManageFee.LogTime = value; }
    }

    public string? MerchantTradeNo
    {
        get { return _ManageFee.MerchantTradeNo; }
        set { _ManageFee.MerchantTradeNo = value; }
    }

    public string? DoorName { get ; set ; }

    public int? TotalPrice { get; set; }

    public string? Pay_Type { get; set; }
    public string? Pay_State { get; set; }

    public int? MF_Pricce { get; set; }

    public int? MF_Motor { get; set; }

    public int? MF_Car { get; set; }

    public string? Pay_Time { get; set; }

}

