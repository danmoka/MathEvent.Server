using MathEvent.Contracts;
using System.Collections.Generic;

namespace MathEvent.Services.Results
{
    /// <summary>
    /// Родительский класс результата выполнения
    /// </summary>
    /// <typeparam name="T">Тип сообщения</typeparam>
    /// <typeparam name="E">Тип сущности</typeparam>
    public abstract class AResult<T, E> : IResult<T, E> where T : IMessage where E : class
    {
        public bool Succeeded { get; set; }

        public IEnumerable<T> Messages { get; set; }

        public E Entity { get; set; }
    }
}

