using Crm.Api.Contracts;
using Crm.Api.Domain.Entities;
using Crm.Api.Infrastructure.Persistence;
using Crm.Api.Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/contacts")]
public sealed class ContactsController(CrmDbContext dbContext, CrmValidator validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ContactResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await dbContext.Contacts.AsNoTracking()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new ContactResponse(x.Id, x.FirstName, x.LastName, x.OwnerName, x.Email, x.PhoneNumber, x.JobTitle, x.AccountId, x.Status, x.CreatedAtUtc, x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContactResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Contacts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return Ok(new ContactResponse(entity.Id, entity.FirstName, entity.LastName, entity.OwnerName, entity.Email, entity.PhoneNumber, entity.JobTitle, entity.AccountId, entity.Status, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }

    [HttpPost]
    public async Task<ActionResult<ContactResponse>> Create(ContactRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateContactEmailUniqueAsync(request.Email, null, cancellationToken);

        if (request.AccountId.HasValue && !await dbContext.Accounts.AnyAsync(x => x.Id == request.AccountId.Value, cancellationToken))
        {
            return BadRequest("The referenced account does not exist.");
        }

        var entity = new Contact
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            OwnerName = request.OwnerName.Trim(),
            Email = request.Email?.Trim(),
            PhoneNumber = request.PhoneNumber?.Trim(),
            JobTitle = request.JobTitle?.Trim(),
            AccountId = request.AccountId,
            Status = request.Status
        };

        dbContext.Contacts.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new ContactResponse(entity.Id, entity.FirstName, entity.LastName, entity.OwnerName, entity.Email, entity.PhoneNumber, entity.JobTitle, entity.AccountId, entity.Status, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ContactResponse>> Update(Guid id, ContactRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Contacts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        await validator.ValidateContactEmailUniqueAsync(request.Email, id, cancellationToken);

        if (request.AccountId.HasValue && !await dbContext.Accounts.AnyAsync(x => x.Id == request.AccountId.Value, cancellationToken))
        {
            return BadRequest("The referenced account does not exist.");
        }

        entity.FirstName = request.FirstName.Trim();
        entity.LastName = request.LastName.Trim();
        entity.OwnerName = request.OwnerName.Trim();
        entity.Email = request.Email?.Trim();
        entity.PhoneNumber = request.PhoneNumber?.Trim();
        entity.JobTitle = request.JobTitle?.Trim();
        entity.AccountId = request.AccountId;
        entity.Status = request.Status;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new ContactResponse(entity.Id, entity.FirstName, entity.LastName, entity.OwnerName, entity.Email, entity.PhoneNumber, entity.JobTitle, entity.AccountId, entity.Status, entity.CreatedAtUtc, entity.UpdatedAtUtc));
    }
}
