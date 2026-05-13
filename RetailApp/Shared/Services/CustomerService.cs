using Domain.Entities;
using Persistence.Interfaces;
using Shared.Interfaces;

namespace Shared.Services;

public class CustomerService(IRepository<Customer> customerRepository) : ICustomerService
{
    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await customerRepository.GetAllAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await customerRepository.GetByIdAsync(id);
    }
}