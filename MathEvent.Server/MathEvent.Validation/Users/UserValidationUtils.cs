using MathEvent.Contracts.Services;
using MathEvent.Contracts.Validators;
using MathEvent.Validation.Common;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MathEvent.Validation.Users
{
    /// <summary>
    /// Предназначен для описания нестандартной валидации данных, связанных с пользователем
    /// </summary>
    public class UserValidationUtils
    {
        private readonly IUserService _userService;

        private const string _emailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

        private const int _passwordMinLength = 6;

        private const int _passwordMaxLength = 255;

        private const int _nameMaxLength = 50;

        private const int _surnameMaxLength = 50;

        private const int _usernameMaxLength = 62;

        private const int _patronymicMaxLength = 50;

        public UserValidationUtils(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Проверяет то, что существует ли пользователь с данным идентификатором
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <returns>Ошибки валидации</returns>
        public async Task<IEnumerable<IValidationError>> ValidateUserId(string id)
        {
            var validationErrors = new List<IValidationError>();

            if (id is null)
            {
                // TODO: ValidationError - в Models проект
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(id),
                    Message = "Идентификатор пользователя не задан",
                });
            }
            else
            {
                var user = await _userService.RetrieveAsync(id);

                if (user is null)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(id),
                        Message = $"Пользователя с id = {id} не существует",
                    });
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Проверяет то, что существуют ли пользователи с данными идентификатором
        /// </summary>
        /// <param name="userIds">Идентификаторы пользователей</param>
        /// <returns>Ошибки валидации</returns>
        public async Task<IEnumerable<IValidationError>> ValidateUserIds(ICollection<string> userIds, string userType)
        {
            var validationErrors = new List<IValidationError>();

            if (userIds is null)
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(userIds),
                    Message = userType is not null ? $"Пользователи типа {userType} должны быть заданы" : "Пользователи должны быть заданы",
                });
            }
            else
            {
                foreach (var userId in userIds)
                {
                    validationErrors.AddRange(await ValidateUserId(userId));
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Проверяет корректность email адреса
        /// </summary>
        /// <param name="email">Email адрес</param>
        /// <param name="checkUserExistence">Требуется ли проверка существования пользователя с таким email</param>
        /// <returns>Ошибки валидации</returns>
        public async Task<IEnumerable<IValidationError>> ValidateEmail(string email, bool checkUserExistence = true)
        {
            var validationErrors = new List<IValidationError>();

            if (string.IsNullOrEmpty(email))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(email),
                    Message = "Введите email"
                });
            }
            else
            {
                if (!Regex.IsMatch(email, _emailPattern))
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(email),
                        Message = "Неверный формат email"
                    });
                }

                if (checkUserExistence && await _userService.GetUserByEmail(email) != null)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(email),
                        Message = "Пользователь с данным email уже существует"
                    });
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Проверяет корректность пароля
        /// </summary>
        /// <param name="password">Пароль</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<IValidationError> ValidatePassword(string password)
        {
            var validationErrors = new List<IValidationError>();

            if (string.IsNullOrEmpty(password))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(password),
                    Message = "Введите пароль"
                });
            }
            else
            {
                if (password.Length < _passwordMinLength)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(password),
                        Message = $"Минимальная длина составляет {_passwordMinLength} символов"
                    });
                }

                if (password.Length > _passwordMaxLength)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(password),
                        Message = $"Длина не должна превышать {_passwordMaxLength} символов"
                    });
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Валидация имени пользователя
        /// </summary>
        /// <param name="name">Имя</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<IValidationError> ValidateName(string name)
        {
            var validationErrors = new List<IValidationError>();

            if (string.IsNullOrEmpty(name))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(name),
                    Message = "Введите имя"
                });
            }
            else
            {
                if (name.Length > _nameMaxLength)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(name),
                        Message = $"Длина имени не должна превышать {_nameMaxLength} символов"
                    });
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Валидация фамилии пользователя
        /// </summary>
        /// <param name="surname">Фамилия</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<IValidationError> ValidateSurname(string surname)
        {
            var validationErrors = new List<IValidationError>();

            if (string.IsNullOrEmpty(surname))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(surname),
                    Message = "Введите фамилию"
                });
            }
            else
            {
                if (surname.Length > _surnameMaxLength)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(surname),
                        Message = $"Длина фамилии не должна превышать {_surnameMaxLength} символов"
                    });
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Валидация логина пользователя
        /// </summary>
        /// <param name="username">Логин</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<IValidationError> ValidateUsername(string username)
        {
            var validationErrors = new List<IValidationError>();

            if (string.IsNullOrEmpty(username))
            {
                validationErrors.Add(new ValidationError { Field = nameof(username), Message = "Введите логин" });
            }
            else
            {
                if (username.Length > _usernameMaxLength)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(username),
                        Message = $"Длина логина не должна превышать {_usernameMaxLength} символов"
                    });
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Валидация отчества пользователя
        /// </summary>
        /// <param name="patronymic">Отчество</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<IValidationError> ValidatePatronymic(string patronymic)
        {
            var validationErrors = new List<IValidationError>();

            if (string.IsNullOrEmpty(patronymic))
            {
                validationErrors.Add(new ValidationError { Field = nameof(patronymic), Message = "Введите отчество" });
            }
            else
            {
                if (patronymic.Length > _patronymicMaxLength)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(patronymic),
                        Message = $"Длина отчества не должна превышать {_patronymicMaxLength} символов"
                    });
                }
            }

            return validationErrors;
        }
    }
}
