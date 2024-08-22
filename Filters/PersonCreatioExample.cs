using CineMatrix_API.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace CineMatrix_API.Filters
{
    public class PersonCreatioExample : IExamplesProvider<PersonCreationDTO>
    {
        public PersonCreationDTO GetExamples()
        {
            return new PersonCreationDTO
            {
                Name = "John Doe",
                Biography = "A brief biography of the actor.",
                Picture = null,
            };
        }
    }
}
