using MathEvent.Contracts;

namespace MathEvent.Services.Results
{
    /// <summary>
    /// Результат выполнения с сообщениями IMessage
    /// </summary>
    /// <typeparam name="E">Тип сущности</typeparam>
    public class MessageResult<E> : AResult<IMessage, E> where E : class
    {
    }
}
