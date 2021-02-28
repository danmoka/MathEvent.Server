using MathEvent.Contracts;
using System.Collections.Generic;

namespace Service.Results
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

//public class Builder
//{
//    private AResult<T> _result;

//    public Builder()
//    {
//        _result = new AResult<T>();
//    }

//    public Builder IsSucceeded(bool value)
//    {
//        _result._succeeded = value;

//        return this;
//    }

//    public Builder WithMessages(IEnumerable<T> messages)
//    {
//        _result._messages = messages;

//        return this;
//    }

//    public AResult<T> Build()
//    {
//        return _result;
//    }
//}
