using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using YuzuDelivery.Umbraco.Forms;

namespace $rootnamespace$
{ 
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    [ComposeBefore(typeof(YuzuFormsStartup))]
    public class YuzuFormsComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            var assembly = Assembly.GetAssembly(typeof(YuzuFormsComposer));

            var config = new YuzuDeliveryFormsConfiguration()
            {
                FormElementAssemblies = new Assembly[] { assembly }
            };

            YuzuDeliveryForms.Initialize(config);
        }
    }
}
