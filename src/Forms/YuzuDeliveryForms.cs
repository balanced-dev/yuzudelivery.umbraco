using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuzuDelivery.Umbraco.Forms
{

    public class YuzuDeliveryForms
    {
        private const string InstanceNotSet = "Yuzu forms definition instance not valid";

        private const string ConfigurationNotSet = "Yuzu forms definition configuration not set";
        private const string ConfigurationAlreadySet = "Yuzu forms definition configuration already set, can't initialise more than once";

        private static IYuzuDeliveryFormsConfiguration _configuration;
        private static YuzuDeliveryForms _instance;

        /// <summary>
        /// Configuration provider for performing maps
        /// </summary>
        public static IYuzuDeliveryFormsConfiguration Configuration
        {
            get => _configuration ?? throw new InvalidOperationException(ConfigurationNotSet);
            private set => _configuration = (_configuration == null) ? value : throw new InvalidOperationException(ConfigurationAlreadySet);
        }

        /// <summary>
        /// Initialize static configuration instance
        /// </summary>
        /// <param name="config">Configuration action</param>
        public static void Initialize(IYuzuDeliveryFormsConfiguration config)
        {
            Instance = new YuzuDeliveryForms(config);

        }

        /// <summary>
        /// Resets the mapper configuration. Not intended for production use, but for testing scenarios.
        /// </summary>
        public static void Reset()
        {
            _configuration = null;
            _instance = null;
        }

        public static YuzuDeliveryForms Instance
        {
            get => _instance ?? throw new InvalidOperationException(InstanceNotSet);
            private set => _instance = value;
        }

        public YuzuDeliveryForms(IYuzuDeliveryFormsConfiguration _configuration)
        {
            Configuration = _configuration ?? throw new ArgumentNullException(nameof(_configuration));
        }
    }
}
