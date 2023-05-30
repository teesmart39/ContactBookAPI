namespace Model.API.Entity
{
    public class Phone
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public Contacts Contacts { get; set; }
        public int ContactsId { get; set; }
    }
}
