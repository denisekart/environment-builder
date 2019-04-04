﻿namespace EnvironmentBuilder.Abstractions
{
    public static class Constants
    {
        private const string KeyPrefix = "EnvironmentBuilder.Abstractions.Constants.";
        /// <summary>
        /// The key used to store the required type of the value being built
        /// </summary>
        public const string SourceRequiredTypeKey = KeyPrefix+"Build.Type";

        /// <summary>
        /// The key used to store the trace string for the current source
        /// </summary>
        public const string SourceTraceValueKey = KeyPrefix + "Source.Trace";
        /// <summary>
        /// The key used to store the description for the current source
        /// </summary>
        public const string SourceDescriptionValueKey = KeyPrefix + "Source.Description";
        /// <summary>
        /// The key used to store the environment variable target for the source
        /// </summary>
        public const string EnvironmentVariableTargetKey = KeyPrefix + "Source.EnvironmentVariableTarget";
    }
}
