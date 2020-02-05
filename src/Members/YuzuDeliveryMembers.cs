using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuzuDelivery.Umbraco.Members
{

    public class YuzuDeliveryMembers
    {
        private const string InstanceNotSet = "Yuzu members definition instance not valid";

        private const string ConfigurationNotSet = "Yuzu members definition configuration not set";
        private const string ConfigurationAlreadySet = "Yuzu members definition configuration already set, can't initialise more than once";

        private static IYuzuDeliveryMembersConfiguration _configuration;
        private static YuzuDeliveryMembers _instance;

        /// <summary>
        /// Configuration provider for performing maps
        /// </summary>
        public static IYuzuDeliveryMembersConfiguration Configuration
        {
            get => _configuration ?? throw new InvalidOperationException(ConfigurationNotSet);
            private set => _configuration = (_configuration == null) ? value : throw new InvalidOperationException(ConfigurationAlreadySet);
        }

        /// <summary>
        /// Initialize static configuration instance
        /// </summary>
        /// <param name="config">Configuration action</param>
        public static void Initialize(IYuzuDeliveryMembersConfiguration config)
        {
            Instance = new YuzuDeliveryMembers(config);

        }

        /// <summary>
        /// Resets the mapper configuration. Not intended for production use, but for testing scenarios.
        /// </summary>
        public static void Reset()
        {
            _configuration = null;
            _instance = null;
        }

        public static YuzuDeliveryMembers Instance
        {
            get => _instance ?? throw new InvalidOperationException(InstanceNotSet);
            private set => _instance = value;
        }

        public YuzuDeliveryMembers(IYuzuDeliveryMembersConfiguration _configuration)
        {
            Configuration = _configuration ?? throw new ArgumentNullException(nameof(_configuration));
        }
    }
}
