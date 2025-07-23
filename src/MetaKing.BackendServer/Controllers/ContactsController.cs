using MetaKing.BackendServer.Data;
using MetaKing.BackendServer.Helpers;
using MetaKing.BackendServer.Services;
using MetaKing.ViewModels.Contents;
using MetaKing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net;
using Azure.Core;

namespace MetaKing.BackendServer.Controllers
{
    [Route("api/contacts")]
    [Authorize]
    [ApiController]
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public ContactsController(ApplicationDbContext context,
            ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpPost]
        public async Task<IActionResult> PostContact([FromBody] ContactCreateRequest request)
        {
            var contact = new ContactModel()
            {
                Name = request.Name,
                Description = request.Description,
                Map = request.Map,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                CreatedDate = request.CreatedDate,
            };
            _context.Contacts.Add(contact);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Contacts");
                return CreatedAtAction(nameof(GetById), new { id = contact.Id }, request);
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse("Create Contact failed"));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetContacts()
        {
            var cachedData = await _cacheService.GetAsync<List<ContactViewModel>>("Contacts");
            if (cachedData == null)
            {
                var contacts = await _context.Contacts.ToListAsync();

                var contactViewModel = contacts.Select(c => CreateContactViewModel(c)).ToList();
                await _cacheService.SetAsync("Contacts", contactViewModel);
                cachedData = contactViewModel;
            }

            return Ok(cachedData);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetContactsPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.Contacts.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Name.Contains(filter));
            }
            var total = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = items.Select(c => CreateContactViewModel(c)).ToList();

            var pagination = new Pagination<ContactViewModel>
            {
                Items = data,
                Total = total,
            };
            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
                return NotFound(new ApiNotFoundResponse($"Contact with id: {id} is not found"));

            ContactViewModel contactViewModel = CreateContactViewModel(contact);

            return Ok(contactViewModel);
        }

        [HttpPut("{id}")]
        [ApiValidationFilter]
        public async Task<IActionResult> PutContact(int id, [FromBody] ContactCreateRequest request)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
                return NotFound(new ApiNotFoundResponse($"Contact with id: {id} is not found"));

            contact.Name = request.Name;
            contact.Description = request.Description;
            contact.Map = request.Map;
            contact.Email = request.Email;
            contact.Phone = request.Phone;
            contact.Address = request.Address;
            contact.CreatedDate = request.CreatedDate;

            _context.Contacts.Update(contact);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _cacheService.RemoveAsync("Contacts");

                return NoContent();
            }
            return BadRequest(new ApiBadRequestResponse("Update Contact failed"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
                return NotFound(new ApiNotFoundResponse($"Contact with id: {id} is not found"));

            _context.Contacts.Remove(contact);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _cacheService.RemoveAsync("Contacts");

                ContactViewModel contactViewModel = CreateContactViewModel(contact);
                return Ok(contactViewModel);
            }
            return BadRequest();
        }

        private static ContactViewModel CreateContactViewModel(ContactModel contact)
        {
            return new ContactViewModel()
            {
                Id = contact.Id,
                Name = contact.Name,
                Description = contact.Description,
                Map = contact.Map,
                Email = contact.Email,
                Phone = contact.Phone,
                Address = contact.Address,
                CreatedDate = contact.CreatedDate,
            };
        }
    }
}
