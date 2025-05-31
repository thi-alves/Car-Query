namespace CarQuery.Services
{
    public interface ISeedUserRoleInitial
    {
        Task SeedRoles();
        Task SeedUsers();
    }
}
