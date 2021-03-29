using System.Collections.Generic;

namespace MathEvent.Contracts
{
    /// <summary>
    /// Описывает функционал результата выполнения
    /// </summary>
    /// <typeparam name="T">Тип сообщения</typeparam>
    /// <typeparam name="E">Тип сущности</typeparam>
    public interface IResult<T, E> where E : class
    {
        IEnumerable<T> Messages { get; set; }

        bool Succeeded { get; set; }

        E Entity { get; set; }
    }
}
