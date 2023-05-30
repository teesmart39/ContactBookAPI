using Model.API.Entity;
using Model.API.Model;

namespace Core.API.Repository.Interface
{
    public interface IContactRepository
    {

        Task<Contacts> AddContactAsync(Contacts contacts);
        Task<int> DeleteContactAsync(int id);
         Task<int> UpdateContactAsync(Contacts updateContactEntity);
        Task<IEnumerable<Contacts>> SearchContactAsync(string firstname, string lastname);
        Task<Contacts> GetContactAsync(int contactId);
        Task<IEnumerable<Contacts>> GetAllContactAsync(); 
        PaginatedContacts PaginatedAsync(List<Contacts> contacts, int pageNumber, int perPageSize);
    }
}
