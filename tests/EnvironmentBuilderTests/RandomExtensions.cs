using System;
using EnvironmentBuilder.Abstractions;
using EnvironmentBuilder.Extensions;

namespace EnvironmentBuilderRandomContrib.Extensions
{
    // The static class that will hold the extension methods
    public static class RandomExtensions
    {
        // Unique keys to use in the configuration - they should be unique system wide, perhaps the assembly name and description
        public const string RngKey = "RandomExtensions.RngKey";
        public const string SaverKey = "RandomExtensions.NumberSaverKey";

        // This method will be used to add the RNG to the global configuation
        public static IEnvironmentConfiguration WithRandomNumberGenerator(this IEnvironmentConfiguration configuration,
            Random random)
        {
            return configuration.SetFactoryValue(RngKey, () => random.Next());
        }
        // this method will be used to get the next randomly incremented number
        public static IEnvironmentBuilder Random(this IEnvironmentBuilder builder)
        {
            if (!builder.Configuration.HasValue(SaverKey))
            {
                builder.WithConfiguration(cfg => cfg.SetValue(SaverKey, new NumberSaver()));
            }

            return builder.WithSource(config =>
            {
                //just a simple logic to increment and save values
                var nextValue = config.GetFactoryValue<int>(RngKey);
                var saver = config.GetValue<NumberSaver>(SaverKey);
                var oldKey = saver.Prev;
                saver.Prev += nextValue;
                return oldKey;

            }, config => config.WithTrace("my random value","my random source"));
        }
        // this is just an example class that holds the previous value
        //a more suitable approach would be to save the value itself to the configuration
        // but this shows the ability to add extra logic and save it in the configuration
        private class NumberSaver
        {
            public int Prev { get; set; } = 0;
        }

    }
}