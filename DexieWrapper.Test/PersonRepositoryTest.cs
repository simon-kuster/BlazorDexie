using DexieWrapper.Persons;
using DexieWrapper.Test.ModuleWrappers;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DexieWrapper.Test
{
    public class PersonRepositoryTest
    {
        private INodeJSService _nodeJSService;

        public PersonRepositoryTest()
        {
            var services = new ServiceCollection();
            services.AddNodeJS();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            _nodeJSService = serviceProvider.GetRequiredService<INodeJSService>();
        }

        [Fact]
        public async Task GetAll()
        {
            // arrange
            var personRepostiry = new PersonRepository(new ModuleWrapperFactory(_nodeJSService, "../../DexieWrapper/wwwroot"));

            var p0 = await personRepostiry.CreateOrUpdate(new Person()
            {
                Id = Guid.NewGuid(),
                FirstName = "Dario",
                SecondName = "Kuster",
                Birthday = new DateTime(2008, 2, 1)
            });

            var p1 = await personRepostiry.CreateOrUpdate(new Person()
            {
                Id = Guid.NewGuid(),
                FirstName = "Simon",
                SecondName = "Kuster",
                Birthday = new DateTime(1978, 8, 28)
            });

            // act
            var persons = await personRepostiry.GetAll();

            // assert
            var p0Check = persons.First(p => p.Id == p0.Id);
            var p1Check = persons.First(p => p.Id == p1.Id);

            Assert.Equal(2, persons.Count);
            Assert.Equal(p0.FirstName, p0Check.FirstName);
            Assert.Equal(p0.SecondName, p0Check.SecondName);
            Assert.Equal(p0.Birthday, p0Check.Birthday);

            Assert.Equal(p1.FirstName, p1Check.FirstName);
            Assert.Equal(p1.SecondName, p1Check.SecondName);
            Assert.Equal(p1.Birthday, p1Check.Birthday);
        }

        [Fact]
        public async Task GetById()
        {
            // arrange
            var personRepostiry = new PersonRepository(new ModuleWrapperFactory(_nodeJSService, "../../DexieWrapper/wwwroot"));

            await personRepostiry.CreateOrUpdate(new Person()
            {
                Id = Guid.NewGuid(),
                FirstName = "Dario",
                SecondName = "Kuster",
                Birthday = new DateTime(2008, 2, 1)
            });

            var p1 = await personRepostiry.CreateOrUpdate(new Person()
            {
                Id = Guid.NewGuid(),
                FirstName = "Simon",
                SecondName = "Kuster",
                Birthday = new DateTime(1978, 8, 28)
            });

            // act
            var person = await personRepostiry.GetById(p1.Id);

            // assert
            Assert.Equal(p1.FirstName, person!.FirstName);
            Assert.Equal(p1.SecondName, person.SecondName);
            Assert.Equal(p1.Birthday, person.Birthday);

        }

        [Fact]
        public async Task CreateOrUpdate()
        {
            // arrange
            var personRepostiry = new PersonRepository(new ModuleWrapperFactory(_nodeJSService, "../../DexieWrapper/wwwroot"));

            var person = new Person()
            {
                Id = Guid.NewGuid(),
                FirstName = "Dario",
                SecondName = "Kuster",
                Birthday = new DateTime(2008, 2, 1)
            };

            // act
            await personRepostiry.CreateOrUpdate(person);

            // assert
            var checkPerson = await personRepostiry.GetById(person.Id);

            Assert.NotNull(person);
            Assert.Equal(person.FirstName, checkPerson!.FirstName);
            Assert.Equal(person.SecondName, checkPerson.SecondName);
            Assert.Equal(person.Birthday, checkPerson.Birthday);
        }

        [Fact]
        public async Task Delete()
        {
            // arrange
             var personRepostiry = new PersonRepository(new ModuleWrapperFactory(_nodeJSService, "../../DexieWrapper/wwwroot"));

            var p0 = await personRepostiry.CreateOrUpdate(new Person()
            {
                Id = Guid.NewGuid(),
                FirstName = "Dario",
                SecondName = "Kuster",
                Birthday = new DateTime(2008, 2, 1)
            });

            // act
            await personRepostiry.Delete(p0);

            // assert
            var checkPerson = await personRepostiry.GetById(p0.Id);
            Assert.Null(checkPerson);
        }
    }
}