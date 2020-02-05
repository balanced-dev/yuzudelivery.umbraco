using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuzuDelivery.Umbraco.Grid
{
    public class YuzuDeliveryGrid
    {
        private const string InstanceNotSet = "Yuzu grids definition instance not valid";

        private const string ConfigurationNotSet = "Yuzu grids definition configuration not set";
        private const string ConfigurationAlreadySet = "Yuzu grids definition configuration already set, can't initialise more than once";

        private static IYuzuDeliveryGridConfiguration _configuration;
        private static YuzuDeliveryGrid _instance;

        /// <summary>
        /// Configuration provider for performing maps
        /// </summary>
        public static IYuzuDeliveryGridConfiguration Configuration
        {
            get => _configuration ?? throw new InvalidOperationException(ConfigurationNotSet);
            private set => _configuration = (_configuration == null) ? value : throw new InvalidOperationException(ConfigurationAlreadySet);
        }

        /// <summary>
        /// Initialize static configuration instance
        /// </summary>
        /// <param name="config">Configuration action</param>
        public static void Initialize(IYuzuDeliveryGridConfiguration config)
        {
            Instance = new YuzuDeliveryGrid(config);

        }

        /// <summary>
        /// Resets the mapper configuration. Not intended for production use, but for testing scenarios.
        /// </summary>
        public static void Reset()
        {
            _configuration = null;
            _instance = null;
        }

        public static YuzuDeliveryGrid Instance
        {
            get => _instance ?? throw new InvalidOperationException(InstanceNotSet);
            private set => _instance = value;
        }

        public YuzuDeliveryGrid(IYuzuDeliveryGridConfiguration _configuration)
        {
            Configuration = _configuration ?? throw new ArgumentNullException(nameof(_configuration));
        }
    }
}
