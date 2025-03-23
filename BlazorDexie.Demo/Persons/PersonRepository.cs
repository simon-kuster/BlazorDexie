using BlazorDexie.Demo.Database;
using BlazorDexie.Options;

namespace BlazorDexie.Demo.Persons
{
    public class PersonRepository
    {
        private readonly MyDb _db;

        public PersonRepository(BlazorDexieOptions blazorDexieOptions)
        {
            _db = new MyDb(blazorDexieOptions);
        }

        public async Task<List<Person>> GetAll()
        {
            return await _db.Persons.ToList();
        }

        public async Task<Person?> GetById(Guid id)
        {
            return await _db.Persons.Get(id);
        }

        public async Task<Person> CreateOrUpdate(Person person)
        {
            await _db.Persons.Put(person);
            return await Task.FromResult(person);
        }

        public async Task Delete(Person person)
        {
            await _db.Persons.Delete(person.Id);
        }
    }
}
