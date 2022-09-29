using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Grid
{
    public class DTGEService : IDTGEService
    {

        protected IVmHelperService vmHelperService;
        private readonly MapPathAbstraction mapPath;

        public DTGEService(IVmHelperService vmHelperService, MapPathAbstraction mapPath)
        {
            this.vmHelperService = vmHelperService;
            this.mapPath = mapPath;
        }

        private static readonly string YuzuGridDirectory = "/App_Plugins/YuzuGrid/";
        private static readonly string ConfigPath = $"{YuzuGridDirectory}package.manifest";

        public virtual JObject GetByName(string name)
        {
            var config = Get();
            return config.Where(x => x["name"].ToString() == name).FirstOrDefault() as JObject;
        }

        public virtual void CreateOrUpdate(string name, string alias, string[] allowedDocTypesRef)
        {
            var allowedDocTypes = allowedDocTypesRef.Select(x => string.Format("\\b{0}\\b", vmHelperService.DocumentTypeAliasFromRef(x))).ToArray();

            var newDTGEConfig = new GridConfig
            {
                Name = name,
                Alias = alias,
            };

            newDTGEConfig.Config.AllowedTypes = allowedDocTypes;
            newDTGEConfig.Config.EnablePreview = true;

            var config = Get();

            var currentConfig = config.FirstOrDefault(x => x["alias"].ToString() == alias);
            if (currentConfig != null)
            {
                config.Remove(currentConfig);
            }
            config.Add(JObject.FromObject(newDTGEConfig));
            Save(config);
        }

        public virtual JArray Get()
        {
            JObject config;

            if (!File.Exists(mapPath.Get(ConfigPath)))
            {
                if (!Directory.Exists(mapPath.Get(YuzuGridDirectory)))
                {
                    Directory.CreateDirectory(mapPath.Get(YuzuGridDirectory));
                }

                config = JObject.Parse("{ 'gridEditors':[] }");
                File.WriteAllText(mapPath.Get(ConfigPath), JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            else
            {
                var configFile = File.ReadAllText(mapPath.Get(ConfigPath));
                config = JObject.Parse(configFile);
            }
            return config["gridEditors"].Value<JArray>();
        }

        public virtual void Save(JArray config)
        {
            try
            {
                var configFile = File.ReadAllText(mapPath.Get(ConfigPath));
                var configObj = JObject.Parse(configFile);
                configObj["gridEditors"] = config;
                configFile = JsonConvert.SerializeObject(configObj, Formatting.Indented);
                File.WriteAllText(mapPath.Get(ConfigPath), configFile);
            }
            catch { }
        }

        public class GridConfig
        {
            public GridConfig()
            {
                View = "/App_Plugins/DocTypeGridEditor/Views/doctypegrideditor.html";
                Render = "/App_Plugins/DocTypeGridEditor/Render/DocTypeGridEditor.cshtml";
                Icon = "icon-item-arrangement";

                Config = new DTGEConfig();

                Config.ViewPath = "/Views/Partials/Grid/Editors/DocTypeGridEditor/";
                Config.PreviewViewPath = "/Views/Partials/Grid/Editors/DocTypeGridEditor/Previews/";
            }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("alias")]
            public string Alias { get; set; }

            [JsonProperty("view")]
            public string View { get; set; }

            [JsonProperty("render")]
            public string Render { get; set; }

            [JsonProperty("icon")]
            public string Icon { get; set; }

            [JsonProperty("config")]
            public DTGEConfig Config { get; set; }

        }

        public class DTGEConfig
        {
            [JsonProperty("allowedDocTypes")]
            public string[] AllowedTypes { get; set; }

            [JsonProperty("enablePreview")]
            public bool EnablePreview { get; set; }

            [JsonProperty("viewPath")]
            public string ViewPath { get; set; }

            [JsonProperty("previewViewPath")]
            public string PreviewViewPath { get; set; }

            [JsonProperty("previewCssFilePath")]
            public string PreviewCssFilePath { get; set; }

            [JsonProperty("previewJsFilePath")]
            public string PreviewJsFilePath { get; set; }

        }
    }

    public interface IDTGEService
    {
        void CreateOrUpdate(string name, string alias, string[] allowedDocTypesRef);
        JObject GetByName(string name);
    }

}

