namespace ApiCos.Services.IRepositories
{
    public interface IUnitOfWork
    {
        IUserRepository User { get; }
        ICompanyRepository Company { get; }
        Task CompleteAsync();
    }
}
