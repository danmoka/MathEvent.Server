using MathEvent.Contracts;
using MathEvent.Database;
using MathEvent.Entities.Entities;

namespace MathEvent.Repository
{
    /// <summary>
    /// Репозиторий для Файлов
    /// </summary>
    public class FileRepository : RepositoryBase<File>, IFileRepository
    {
        public FileRepository(ApplicationContext applicationContext) : base(applicationContext)
        {

        }
    }
}
