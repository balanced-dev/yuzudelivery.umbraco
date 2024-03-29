﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Autofac;
using Rhino.Mocks;
using Umb = Umbraco.Core.Services;
using YuzuDelivery.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YuzuDelivery.Umbraco.Import.Tests.Integration;

namespace YuzuDelivery.Umbraco.Grid.Test
{

    public class DGTEServiceMocks
    {
        private UmbracoApiTestMocks parent;

        public JArray GridConfig { get; set; }

        public List<DTGEService.GridConfig> Configs { get; private set; }

        public DTGEService DGTEService { get; set; }

        public DGTEServiceMocks(IContainer container, UmbracoApiTestMocks parent)
        {
            this.parent = parent;

            GridConfig = new JArray();

            DGTEService = container.Resolve<IDTGEService>() as DTGEService;

            Configs = new List<DTGEService.GridConfig>();

            Func<JArray> getConfig = () => {

                var json = JsonConvert.SerializeObject(Configs);
                return JArray.Parse(json);
            };

            Action<JArray> saveConfig = (JArray config) => {

                Configs = config.Select(x => x.ToObject<DTGEService.GridConfig>()).ToList();
            };

            DGTEService.Stub(x => x.Save(null)).IgnoreArguments().Do(saveConfig);
            DGTEService.Stub(x => x.Get()).IgnoreArguments().Do(getConfig);

        }
    }
    
}
