    namespace Common.Constants
{
    public enum UserRole
    {
        Visitor = 3, TourOperator = 2, Admin = 1
    }
    public enum DayOfWeekEnum
    {
        Monday = 2, Tuesday = 3, Wednesday = 4, Thursday = 5, Friday = 6, Saturday = 7, Sunday = 1,
    }
    public enum BookingStatus
    {
        ToPay = 0, Active = 1, Ongoing = 2, IsRating = 3, Completed = 4, Cancel =5
    }
    public enum TourOperatorStatus
    {
        Deactive = 0, Active = 1
    }
    public enum VisitorStatus
    {
        Deactive = 0, Active = 1
    }
    public enum TransactionStatus
    {
        Cancel = 0, Processing = 1, Completed = 2,
    }
    public enum BookingPlaceStatus
    {
        Future = 0, Completed = 1 ,
    }
    public enum PaymentMethodsEnum
    {
        MasterCard = 1, Banking = 2, PayPal = 3,
    }
    public enum JourneyStatus
    {
        Future = 0, OnGoing = 1, WaitRating = 2, Completed = 3,
    }
    public enum JourneyPlaceStatus
    {
        Future = 0, Completed = 1,
    }
    public enum NotificationTypes
    {
        ContactRequest = 1, Chat = 2, PaymentSuccessful = 3, Call = 4, BookingCancelled = 5,
    }
    public enum LanguageStatus
    {
        deactive = 0, prepared = 1, active = 2
    }
    public enum PlaceStatus
    {
        deactive = 0, prepared = 1, active = 2
    }
    public enum PlaceDescStatus
    {
        deactive = 0, converting = 1, active = 2, error = 3
    }
    public enum TourStatus
    {
        deactive = 0, active = 1
    }
    public enum TourDescStatus
    {
        deactive = 0, active = 1
    }
}
