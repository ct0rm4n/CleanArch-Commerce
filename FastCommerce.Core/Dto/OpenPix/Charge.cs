
namespace Core.Dto.OpenPix
{
    
    public class Charge
    {
        public int value { get; set; }
        public string comment { get; set; }
        public string identifier { get; set; }
        public string correlationID { get; set; }
        public string transactionID { get; set; }
        public string status { get; set; }
        public List<object> additionalInfo { get; set; }
        public int fee { get; set; }
        public int discount { get; set; }
        public int valueWithDiscount { get; set; }
        public DateTime expiresDate { get; set; }
        public string type { get; set; }
        public string paymentLinkID { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string brCode { get; set; }
        public int expiresIn { get; set; }
        public string pixKey { get; set; }
        public string paymentLinkUrl { get; set; }
        public string qrCodeImage { get; set; }
        public string globalID { get; set; }
        public PaymentMethods paymentMethods { get; set; }
    }

    public class PaymentMethods
    {
        public Pix pix { get; set; }
    }

    public class Pix
    {
        public string method { get; set; }
        public string txId { get; set; }
        public int value { get; set; }
        public string status { get; set; }
        public int fee { get; set; }
        public string brCode { get; set; }
        public string transactionID { get; set; }
        public string identifier { get; set; }
        public string qrCodeImage { get; set; }
    }

    public class ChargeRoot
    {
        public Charge charge { get; set; }
        public string correlationID { get; set; }
        public string brCode { get; set; }
    }


}
