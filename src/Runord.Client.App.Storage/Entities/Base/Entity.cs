using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runord.Client.App.Storage.Entities.Base
{
    public abstract class Entity
    {
        public Guid Id { get; init; } = Guid.NewGuid();
    }
}
