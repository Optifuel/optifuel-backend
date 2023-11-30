using ApiCos.Models.Entities;

namespace ApiCos.Services.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> Add(User user, string businessName, string password);
        Task<User?> GetByEmailAndPassword(string email, string password);

        Task<User?> EditUser(User user);
        Task ValidationUser(string email, int token);
        Task ChangePasswordRequest(string email, string oldPassword, string newPassword);
        Task ChangePassword(string email, int token);
        Task<List<Vehicle>> GetListVehicleByUser(string email);

    }
}
