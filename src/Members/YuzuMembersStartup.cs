using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Members
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class YuzuMembersStartup : IUserComposer
    {
        public void Compose(Composition composition)
        {
        }
    }
}