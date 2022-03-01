using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _usersRepository;

        public UserService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }


        public async Task<IEnumerable<UserDomainModel>> GetAllAsync()
        {
            var data = await _usersRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<UserDomainModel> result = new List<UserDomainModel>();
            UserDomainModel model;
            foreach (var item in data)
            {
                model = new UserDomainModel
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    UserName = item.UserName,
                    Role = item.Role,
                    BonusPoints = item.BonusPoints
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<UserDomainModel> GetUserByIdAsync(Guid id)
        {
            var data = await _usersRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            UserDomainModel domainModel = new UserDomainModel
            {
                Id = data.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                UserName = data.UserName,
                Role = data.Role,
                BonusPoints = data.BonusPoints
            };

            return domainModel;
        }

        public async Task<UserDomainModel> GetUserByUserName(string username)
        {
            var data = _usersRepository.GetByUserName(username);

            if (data == null)
            {
                return null;
            }

            UserDomainModel domainModel = new UserDomainModel
            {
                Id = data.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                UserName = data.UserName,
                Role = data.Role,
                BonusPoints = data.BonusPoints
            };

            return domainModel;
        }

        public async Task<ResponseModel<UserDomainModel>> AddUser(UserDomainModel newUser)
        {
            User userToCreate = new User()
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                UserName = newUser.UserName,
                Role = newUser.Role,
                BonusPoints = 0,
            };

            if(!userToCreate.Role.Equals("admin") && !userToCreate.Role.Equals("user") && !userToCreate.Role.Equals("super-user"))
                return new ResponseModel<UserDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.UNKNOWN_USER_ROLE
                };

            //provera da li postoji user sa tim usernameom vec
            var userExists = _usersRepository.GetByUserName(userToCreate.UserName);
            if (userExists != null)
            {
                return new ResponseModel<UserDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.USER_ALREADY_EXISTS
                };
            }

            var data = _usersRepository.Insert(userToCreate);
            //provera da li je uspesno dodat
            if (data == null)
            {
                return new ResponseModel<UserDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.USER_CREATION_ERROR
                };
            }

            _usersRepository.Save();

            ResponseModel<UserDomainModel> result = new ResponseModel<UserDomainModel>
            {
                IsSuccessful = true,
                ErrorMessage = null,
                DomainModel = new UserDomainModel()
                {
                    Id = data.Id,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    UserName = data.UserName,
                    BonusPoints = data.BonusPoints,
                    Role = data.Role
                }
            };

            return result;
        }
        public int AddBonusPoints(Guid userId, int bonusPoints)
        {
            var bonusPointState = _usersRepository.AddBonusPoints(userId, bonusPoints);
            if (bonusPointState == null || bonusPointState < 0)
            {
                return -1;
            }
            _usersRepository.Save();

            return bonusPointState;
        }

    }
}
