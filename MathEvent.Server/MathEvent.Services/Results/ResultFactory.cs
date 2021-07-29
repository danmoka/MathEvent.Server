using MathEvent.Contracts;
using System.Collections.Generic;

namespace MathEvent.Services.Results
{
    /// <summary>
    /// Фабрика для создания результатов выполнения
    /// </summary>
    internal static class ResultFactory
    {
        /// <summary>
        /// Создает результат успеха с сообщениями (без указания сообщений)
        /// </summary>
        /// <typeparam name="E">Тип сущности</typeparam>
        /// <param name="entity">Возвращаемая сущность, как результат выполнения</param>
        /// <returns>Результат с сообщениями (без указания сообщений)</returns>
        internal static IResult<IMessage, E> GetSuccessfulResult<E>(E entity) where E : class
        {
            return GetMessageResult(true, null, entity);
        }

        /// <summary>
        /// Создает результат успеха с сообщениями
        /// </summary>
        /// <typeparam name="E">Тип сущности</typeparam>
        /// <param name="messages">Набор сообщений</param>
        /// <param name="entity">Возвращаемая сущность, как результат выполнения</param>
        /// <returns>Результат с сообщениями</returns>
        internal static IResult<IMessage, E> GetSuccessfulMessageResult<E>(IEnumerable<IMessage> messages, E entity) where E : class
        {
            return GetMessageResult(true, messages, entity);
        }

        /// <summary>
        /// Создает результат неудачи с сообщениями (без указания сущности)
        /// </summary>
        /// <typeparam name="E">Тип сущности</typeparam>
        /// <param name="messages">Набор сообщений</param>
        /// <returns>Результат с сообщениями</returns>
        internal static IResult<IMessage, E> GetUnsuccessfulMessageResult<E>(IEnumerable<IMessage> messages) where E : class
        {
            return GetMessageResult(false, messages, (E)null);
        }

        private static IResult<IMessage, E> GetMessageResult<E>(bool succeeded, IEnumerable<IMessage> messages, E entity) where E : class
        {
            return new MessageResult<E>
            {
                Succeeded = succeeded,
                Messages = messages,
                Entity = entity
            };
        }
    }
}
