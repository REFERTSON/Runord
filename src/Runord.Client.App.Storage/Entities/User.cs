using Runord.Client.App.Storage.Entities.Base;
using Runord.Client.Shared.Enums;
using Runord.Client.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Runord.Client.App.Storage.Entities
{
    public class User : Entity
    {
        public string Login { get; init; }
        public UserRole Role { get; init; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; init; }
        public HubNode Node { get; init; }
        public string JWTToken { get; set; } = string.Empty;

        public User(string login, UserRole role, string name, DateTime createdAt, HubNode node) 
        {
            Login = login;
            Role = role;
            Name = name;
            CreatedAt = createdAt;
            Node = node;
        }
    }
}
