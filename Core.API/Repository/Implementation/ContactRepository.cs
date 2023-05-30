using Core.API.Repository.Interface;
using Data.API;
using Microsoft.EntityFrameworkCore;
using Model.API.Entity;
using Model.API.Model;

namespace Core.API.Repository.Implementation
{
    public class ContactRepository : IContactRepository

    {
        private readonly AppDbContext _dbContext;

        public ContactRepository(AppDbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException();
        }
        public async Task<Contacts> AddContactAsync(Contacts newContact)
        {
            await _dbContext.Contacts.AddAsync(newContact);
            var result = await _dbContext.SaveChangesAsync();
            if (result > 0)
            {
                return newContact;
            }
            else
            {
                throw new Exception("Contact Not Added Successfully");
            }

        }

        public async Task<int> DeleteContactAsync(int id)
        {
            var contactToDelete = await _dbContext.Contacts.FindAsync(id);
            if (contactToDelete != null)
            {
                _dbContext.Contacts.Remove(contactToDelete);

                var result = await _dbContext.SaveChangesAsync();
                return result;      
            }
            else
            {
                return 0;
            }

        }

        public async Task<int> UpdateContactAsync(Contacts updateContact)
        {
            _dbContext.Contacts.Update(updateContact);
            var updatedContact = await _dbContext.SaveChangesAsync();
            return updatedContact;
        }

        public async Task<IEnumerable<Contacts>> SearchContactAsync(string firstname, string lastname)
        {
            if (string.IsNullOrEmpty(firstname) && string.IsNullOrEmpty(lastname))
            {
                return await GetAllContactAsync();
            }
            var searchedContact = await _dbContext.Contacts.Where(item =>
            item.FirstName.ToLower().Contains(firstname.ToLower().Trim()) &&
            item.LastName.ToLower().Contains(lastname.ToLower().Trim())).ToListAsync();
            return searchedContact;
        }
        public async Task<Contacts> GetContactAsync(int contactId)
        {

            return await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == contactId);
        }

        public async Task<IEnumerable<Contacts>> GetAllContactAsync()
        {
            return await _dbContext.Contacts.ToListAsync();

        }

       


        public PaginatedContacts PaginatedAsync(List<Contacts> contacts, int pageNumber, int perPageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            perPageSize = perPageSize < 1 ? 5 : perPageSize;
            var totalCount = contacts.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / perPageSize);
            var paginated = contacts.Skip((pageNumber - 1) * perPageSize).Take(perPageSize).ToList();
            var result = new PaginatedContacts
            {
                CurrentPage = pageNumber,
                PerPageSize = perPageSize,
                TotalPages = totalPages,
                Contacts = paginated
            };
            return result;
        }


       

      

       
    }
}
