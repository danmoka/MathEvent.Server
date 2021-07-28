using MathEvent.Services.Messages;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MathEvent.Services.Results
{
    internal static class ResultUtils
    {
        /// <summary>
        /// Преобразует перечисление IdentityError в перечисление IdentityMessage
        /// </summary>
        /// <param name="errors">Перечисление ошибок подсистемы Identity</param>
        /// <returns>Перечисление ошибок подсистемы Identity в виде сообщений</returns>
        internal static IEnumerable<IdentityMessage> MapIdentityErrorsToMessages(IEnumerable<IdentityError> errors)
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
