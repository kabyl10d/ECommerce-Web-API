namespace SampleCrud.Common
{
    public class Enums
    {
        public enum Category
        {
            Electronics = 1,
            Mobiles,
            HomeKitchen,
            Fashion,
            Beauty,
            Health,
            BabyProducts,
            Stationary
        }

        public enum OrderStatus
        {
            Processing, Delivered, NotDelivered, Cancelled
        }

        public enum PaymentMode
        {
            Card, UPI
        }
        public enum UPI
        {
            GooglePay = 1,
            PhonePe,
            PayTM,

        }
        public enum ReviewType
        {
            Critical = 1,
            NotBad,
            Good,
            VeryGood,
            Excellent
        }
    }
}
