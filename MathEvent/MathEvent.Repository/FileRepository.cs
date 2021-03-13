using MathEvent.Contracts;
using MathEvent.Entities;
using MathEvent.Entities.Entities;

namespace MathEvent.Repository
{
    /// <summary>
    /// Репозиторий для Файлов
    /// </summary>
    public class FileRepository : RepositoryBase<File>, IFileRepository
    {
        public FileRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
