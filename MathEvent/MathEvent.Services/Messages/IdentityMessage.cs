using MathEvent.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Service.Messages
{
    /// <summary>
    /// Сообщение на основе IdentityError
    /// </summary>
    public class IdentityMessage : IMessage
    {
        public IdentityError IdentityError { get; set; }

        public string Code { get => IdentityError.Code; }

        public string Message { get => IdentityError.Description; }
    }
}
