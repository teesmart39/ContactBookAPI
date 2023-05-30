using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.API.Repository.Interface;
using Core.API.Services;
using Data.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.API.Entity;
using Model.API.Model;

namespace CONTACT_API.Controllers
{
    [Route("user/contact")]
    [ApiController]

    public class ContactController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;
      
      
        private readonly AppDbContext _appDbContext;

        public ContactController(IContactRepository contactRepository, IMapper mapper, AppDbContext appDbContext)
        {

            _contactRepository = contactRepository ?? throw new ArgumentNullException();
            _mapper = mapper ?? throw new ArgumentNullException();
          
           
            _appDbContext = appDbContext;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<ActionResult<Contacts>> AddContact(AddContactDto addContact)
        {
            var mapContactToDto = new Contacts()
            {
                FirstName = addContact.FirstName,
                LastName = addContact.LastName,
                Emails = addContact.Emails,
                Address = addContact.Address,
                PhoneNumber = addContact.PhoneNumber,
                WebSiteUrl = addContact.WebSiteUrl,
                Image = addContact.Image,
            };

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var contactAdd = await _contactRepository.AddContactAsync(mapContactToDto);
            return Ok(contactAdd);

        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteContact(int id)
        {
            if (id <= 0)
            {
                return BadRequest("This is not a valid id");
            }
            else
            {
                var delete = await _contactRepository.DeleteContactAsync(id);
                if (delete > 0)
                {
                    return NoContent();
                }
                else { return NotFound($"Contact not found"); }
            }
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateContact(UpdateContactDto updateContactDto, int id)
        {
            var mapUpdateItem = new Contacts()
            {
                Id = id,
                FirstName = updateContactDto.FirstName,
                LastName = updateContactDto.LastName,
                Emails = updateContactDto.Emails,
                Address = updateContactDto.Address,
                PhoneNumber = updateContactDto.PhoneNumber,
                WebSiteUrl = updateContactDto.WebSiteUrl,
                Image = updateContactDto.Image,
            };
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var updateItem = await _contactRepository.UpdateContactAsync(mapUpdateItem);
            if (updateItem > 0)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ContactDto>>> SearchContact(string firstname, string lastname)
        {
            var search = await _contactRepository.SearchContactAsync(firstname, lastname);
            if (search == null)
            {
                return NotFound(search);
            }
            return Ok(search);
        }

        [Authorize(Roles = "ADMIN,USER")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactDto>> GetSingleContactById(int id)
        {
            var contact = await _contactRepository.GetContactAsync(id);
            if (contact == null)
            {
                return NotFound(contact);
            }
            return Ok(contact);
        }

        [Authorize(Roles = "ADMIN, USER")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Contacts>>> GetAllContact(int pageNumber, int perPageSize)
        {

            var GetallContact = await _contactRepository.GetAllContactAsync();
            if (GetallContact != null && GetallContact.Count() > 0)
            {
                var paged = _contactRepository.PaginatedAsync(GetallContact.ToList(), pageNumber, perPageSize);
                return Ok(paged);
            }
            return Ok(GetallContact);

        }

        [HttpPost("photos")]
        public async Task<IActionResult> UploadPhoto( IFormFile file, [FromServices] CloudinaryService cloudinaryService)
        {
            var result = await cloudinaryService.UploadAsync(file);

            return Ok(new
            {
                PublicId = result.PublicId,
                Url = result.SecureUrl.ToString()
            });
        }


        [HttpPatch("photos")]
        public async Task<IActionResult> UploadPhoto2(IFormFile file, int id)
        {
            var contact = await _appDbContext.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound("Contact to upload picture to not available");
            }
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    Status = "No Image Uploaded"
                });
            }
            var cloudinary = new Cloudinary(new Account("druuoq2il", "239582452628213", "rDa5FQBZRsnJXdfaWAsZYM-nK9c"));
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = $"{id}"
            };
            var result = cloudinary.Upload(uploadParams);
            if (result == null)
            {
                return BadRequest(new
                {
                    Status = "Image not upload successfully"
                });
            }
            contact.Image = result.Url.ToString();
            _appDbContext.Contacts.Update(contact);
            await _appDbContext.SaveChangesAsync();
            return Ok(new
            {
                PublicId = result.PublicId,
                Url = result.SecureUrl.ToString(),
                Status = "Uploaded Successfully"
            });
        }
    }
}
