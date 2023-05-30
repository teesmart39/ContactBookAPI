using AutoMapper;
using Model.API.Entity;
using Model.API.Model;

namespace CONTACT_API.Profiles
{
    public class ContactMapper: Profile
    {
        public ContactMapper()
        {
            CreateMap<Contacts, ContactDto>();
            CreateMap<AddContactDto, Contacts>();
        }
    }
}
