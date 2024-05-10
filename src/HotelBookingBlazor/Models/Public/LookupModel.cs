namespace HotelBookingBlazor.Models.Public
{
    public readonly record struct LookupModel<TId> (TId Id, string Name);
    public readonly record struct LookupModel<TId, TAdditionalData> (TId Id, string Name, TAdditionalData AdditionalData)
    {
        public bool IsEmpty => string.IsNullOrWhiteSpace(Name);
    }
}
