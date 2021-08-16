using System.Collections.Generic;
using System.Linq;

namespace YuzuDelivery.Umbraco.Import
{
    public class BlockListGridRowConfigToContent
    {
        private readonly IYuzuDeliveryImportConfiguration importConfig;

        protected static string ColumnSettingsName { get; private set; }

        protected static string ContentGroup { get; private set; }
        protected static string SettingsGroup { get; private set; }


        public BlockListGridRowConfigToContent(IYuzuDeliveryImportConfiguration importConfig)
        {
            this.importConfig = importConfig;

            ColumnSettingsName = "Column Settings";

            ContentGroup = "Content";
            SettingsGroup = "Settings";
        }

        public GridRowConfig[] GetSectionBlocks(ContentPropertyConfig config)
        {
            var sizes = config.Grid.Sizes;
            var output = new List<GridRowConfig>();

            foreach (var f in importConfig.GridRowConfigs)
            {
                if (f.IsDefault && !sizes.Any())
                {
                    output.Add(f);
                }
                else
                {
                    if (sizes.Intersect(f.DefinedSizes).Any())
                    {
                        output.Add(f);
                    }
                }
            }
            return output.ToArray();
        }

        public Property[] GetProperties(GridRowConfig[] configs)
        {
            var output = new List<Property>();
            var types = configs.ToDictionary(x => x.ActualSizes.Count(), y => y.Name);

            foreach(var i in configs)
            {
                var size = i.ActualSizes.Length;

                if(size == 1)
                {
                    output.Add(new Property(size, false, types) { Name = "Full Width", Alias = "w100", Size = 1 });
                    output.Add(new Property(size, true, types) { Alias = "w100Settings", Size = 1 });
                }
                if (size == 2)
                {
                    output.Add(new Property(size, false, types) { Name = "Left", Alias = "l50", Size = 2 });
                    output.Add(new Property(size, false, types) { Name = "Right", Alias = "r50", Size = 2 });

                    output.Add(new Property(size, true, types) { Alias = "l50Settings", Size = 2 });
                    output.Add(new Property(size, true, types) { Alias = "r50Settings", Size = 2 });
                }
                if (size == 3)
                {
                    output.Add(new Property(size, false, types) { Name = "Left", Alias = "l33", Size = 3 });
                    output.Add(new Property(size, false, types) { Name = "Middle", Alias = "m33", Size = 3 });
                    output.Add(new Property(size, false, types) { Name = "Right", Alias = "r33", Size = 3 });

                    output.Add(new Property(size, true, types) { Alias = "l33Settings", Size = 3 });
                    output.Add(new Property(size, true, types) { Alias = "m33Settings", Size = 3 });
                    output.Add(new Property(size, true, types) { Alias = "r33Settings", Size = 3 });
                }
                if (size == 4)
                {
                    output.Add(new Property(size, false, types) { Name = "Quarter Width", Alias = "l25", Size = 4 });
                    output.Add(new Property(size, false, types) { Name = "Quarter Width", Alias = "ml25", Size = 4 });
                    output.Add(new Property(size, false, types) { Name = "Quarter Width", Alias = "mr25", Size = 4 });
                    output.Add(new Property(size, false, types) { Name = "Quarter Width", Alias = "r25", Size = 4 });

                    output.Add(new Property(size, true, types) { Alias = "l25Settings", Size = 4 });
                    output.Add(new Property(size, true, types) { Alias = "ml25Settings", Size = 4 });
                    output.Add(new Property(size, true, types) { Alias = "mr25Settings", Size = 4 });
                    output.Add(new Property(size, true, types) { Alias = "r25Settings", Size = 4 });
                }
            }
            return output.ToArray();
        }

        public class Property
        {
            public Property(int size, bool isSettings, Dictionary<int, string> types)
            {
                GroupName = isSettings ? SettingsGroup : ContentGroup;
                if (isSettings) Name = ColumnSettingsName;
                Type = types[size];
                IsSettings = isSettings;
            }

            public string Type { get; set; }
            public string Name { get; set; }
            public string Alias { get; set; }
            public string GroupName { get; set; }
            public bool IsSettings { get; set; }
            public int Size { get; set; }
        }
    }

}
