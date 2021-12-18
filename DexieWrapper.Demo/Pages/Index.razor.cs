using DexieWrapper.Demo.Persons;
using Microsoft.AspNetCore.Components;

namespace DexieWrapper.Demo.Pages
{
    public partial class Index
    {
        private Person? _person;

        [Inject] public PersonRepository PersonRepository { get; set; } = null!;

        private async Task CreatePersonClicked()
        {
            _person = new Person()
            {
                Id = Guid.NewGuid(),
                FirstName = "Dario",
                SecondName = "Kuster"
            };

            await PersonRepository.CreateOrUpdate(_person);
        }

        private async Task ReadPersonClicked()
        {
            _person = (await PersonRepository.GetAll()).FirstOrDefault();
        }

        private async Task UpdatePersonClicked()
        {
            if (_person == null)
            {
                throw new Exception("No person exists. Create person before update");
            }

            _person.Birthday = new DateTime(2008, 2, 1);

            await PersonRepository.CreateOrUpdate(_person);
        }

        private async Task DeletePersonClicked()
        {
            if (_person == null)
            {
                throw new Exception("No person exists. Create person before update");
            }

            await PersonRepository.Delete(_person);
            _person = null;
        }
    }
}
