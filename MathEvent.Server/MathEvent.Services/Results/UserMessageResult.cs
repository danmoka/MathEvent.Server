using MathEvent.Services.Messages;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MathEvent.Services.Results
{
    /// <summary>
    /// Результат выполнения с сущностью пользователя
    /// </summary>
    public class UserMessageResult<T> : MessageResult<T> where T : class
    {
        /// <summary>
        /// Преобразует перечисление IdentityError в перечисление IdentityMessage
        /// </summary>
        /// <param name="errors">Перечисление ошибок подсистемы Identity</param>
        /// <returns></returns>
        public static IEnumerable<IdentityMessage> GetMessagesFromErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                yield return new IdentityMessage
                {
                    IdentityError = error
                };
            }
        }
    }
}
