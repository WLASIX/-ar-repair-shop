using BlazorServerAuthenticationAndAuthorization.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorServerAuthenticationAndAuthorization.Authentication
{
    public class UserAccountService : DbContext
    {
        private List<UserAccount> _users = new List<UserAccount>();
        private DB db;
        private List<DBItems>? items;
  
        public UserAccountService(IConfiguration config)
        {
            db = new(config);
            GetClientsAsync().Wait();
        }

        public UserAccount? GetByUserName(string userName)
        {
            _users.Clear();
            GetClientsAsync().Wait();
            return _users.FirstOrDefault(x => x.UserName == userName);
        }

        // Получение всех пользователей с БД
        private async Task GetClientsAsync()
        {
            items = await db.GetClientsFromDB();

            if (items != null)
            {
                foreach (DBItems i in items)
                {
                    _users.Add(new UserAccount
                    {
                        UserName = $"{i.Login}",
                        Password = $"{i.Password}",
                        Role = $"{(i.IsAdmin == false ? "User" : "Administrator")}",
                        UserId = $"{i.Id.ToString()}",
                    });
                }
            }
        }

        // Добавление нового пользователя в БД
        public async Task SetClientAsync(BlazorServerAuthenticationAndAuthorization.Pages.Register.Model model)
        {
            await db.Reg(model);
        }
    }
}
