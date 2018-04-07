namespace OpenTl.ClientApi.FunctinonalTests
{
    using System.Threading.Tasks;

    using OpenTl.ClientApi.FunctinonalTests.Framework;
    using OpenTl.Schema;

    using Xunit;
    using Xunit.Abstractions;

    public sealed class UsersTests: TestWithAuth
    {
        public UsersTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task GetCurrentUserFull()
        {
            var userFull = await ClientApi.UsersService.GetCurrentUserFullAsync().ConfigureAwait(false);
            
            Assert.NotNull(userFull.User);
        }
        
        [Fact]
        public async Task GetUserPhotos()
        {
            var photos = await ClientApi.UsersService.GetUserPhotosAsync(new TInputUserSelf()).ConfigureAwait(false);
            
            Assert.NotNull(photos.Users);
        }
        
        [Fact]
        public async Task GetUsersAsync()
        {
            var users = await ClientApi.UsersService.GetUsersAsync(new []{new TInputUserSelf()} ).ConfigureAwait(false);
            
            Assert.NotEmpty(users);
        }
        
        [Fact]
        public async Task CheckUsername()
        {
            var isAvailable = await ClientApi.UsersService.CheckUsernameAsync("test12356767").ConfigureAwait(false);
            
            Assert.True(isAvailable);
        }
        
        [Fact]
        public async Task UpdateUsername()
        {
            var currentUser = await ClientApi.UsersService.GetCurrentUserFullAsync().ConfigureAwait(false);

            var username = currentUser.User.Is<TUser>().Username + "1";
            
            var user = await ClientApi.UsersService.UpdateUsernameAsync(username).ConfigureAwait(false);
            
            Assert.Equal(username, user.Username);
        }
    }
}