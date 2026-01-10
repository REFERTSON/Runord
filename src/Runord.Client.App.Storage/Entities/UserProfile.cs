using Runord.Client.App.Storage.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runord.Client.App.Storage.Entities
{
    public class UserProfile : Entity
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public Guid UserId { get; init; }
        public User? User { get; set; }

        public UserProfile(string name, string avatar, Guid user) 
        { 
            Name = name;
            Avatar = avatar;
            UserId = user;
        }
    }
}
