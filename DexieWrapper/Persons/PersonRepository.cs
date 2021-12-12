using DexieWrapper.JsModule;

namespace DexieWrapper.Persons
{
    public class PersonRepository
    {
        private readonly IJsModule _jsModule;

        public PersonRepository(IJsModuleFactory jsModuleFactory)
        {
            _jsModule = jsModuleFactory.CreateModule("personRepository.js"); 
        }

        public async Task<List<Person>> GetAll()
        {
            return await _jsModule.InvokeAsync<List<Person>>("getAll");
        }

        public async Task<Person?> GetById(Guid id)
        {
            return await _jsModule.InvokeAsync<Person?>("getById", id);
        }

        public async Task<Person> CreateOrUpdate(Person person)
        {
            await _jsModule.InvokeVoidAsync("createOrUpdate", person);
            return await Task.FromResult(person);
        }

        public async Task Delete(Person person)
        {
            await _jsModule.InvokeVoidAsync("remove", person);
        }
    }
}
