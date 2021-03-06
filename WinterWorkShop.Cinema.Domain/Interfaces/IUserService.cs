using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of Users</returns>
        Task<IEnumerable<UserDomainModel>> GetAllAsync();

        /// <summary>
        /// Get a user by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User</returns>
        Task<UserDomainModel> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>User</returns>
        Task<UserDomainModel> GetUserByUserName(string username);

        /// <summary>
        /// Adds new user to DB
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        Task<ResponseModel<UserDomainModel>> AddUser(UserDomainModel newUser);

        public int AddBonusPoints(Guid userId, int bonusPoints);
    }
}
