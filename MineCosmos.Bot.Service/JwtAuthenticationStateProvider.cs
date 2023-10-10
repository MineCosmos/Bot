using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace MineCosmos.Bot.Service
{

    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private JwtSecurityTokenHandler _tokenHandler;
        private JwtSecurityToken _token;

        public JwtAuthenticationStateProvider()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = GetCurrentUser();

            var identity = user != null
                ? new ClaimsIdentity(user.Claims, "jwt")
                : new ClaimsIdentity();

            var principal = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(principal);

            return Task.FromResult(state);
        }

        public void MarkUserAsAuthenticated(string token)
        {
            _token = _tokenHandler.ReadJwtToken(token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void MarkUserAsLoggedOut()
        {
            _token = null;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private ClaimsPrincipal GetCurrentUser()
        {
            if (_token != null && _token.ValidTo > DateTime.UtcNow)
            {
                var identity = new ClaimsIdentity(_token.Claims, "jwt");
                return new ClaimsPrincipal(identity);
            }

            return null;
        }
    }
}
