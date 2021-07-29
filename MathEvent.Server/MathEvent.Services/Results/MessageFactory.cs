using MathEvent.Contracts;
using MathEvent.Services.Messages;
using Microsoft.AspNetCore.Identity;

namespace MathEvent.Services.Results
{
    /// <summary>
    /// Фабрика для создания экземпляров сообщений
    /// </summary>
    internal static class MessageFactory
    {
        /// <summary>
        /// Создает сообщение
        /// </summary>
        /// <param name="code">Код (ошибки) сообщения</param>
        /// <param name="message">Сообщение</param>
        /// <returns>Сообщение</returns>
        internal static IMessage GetSimpleMessage(string code, string message)
        {
            return new SimpleMessage
            {
                Code = code,
                Message = message
            };
        }

        /// <summary>
        /// Создает Identity сообщение
        /// </summary>
        /// <param name="identityError">Ошибка из подсистемы Identity</param>
        /// <returns>Сообщение</returns>
        internal static IMessage GetIdentityMessage(IdentityError identityError)
        {
            return new IdentityMessage
            {
                IdentityError = identityError
            };
        }
    }
}
