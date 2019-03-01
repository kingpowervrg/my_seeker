using GOEngine.Implement;
namespace fastJSON
{
    using System;
    using System.Collections.Generic;

    public sealed class JSONParameters
    {
        public bool EnableAnonymousTypes;
        public List<JsonFieldTypes> IgnoreAttributes = new List<JsonFieldTypes> { JsonFieldTypes.UnEditable };
        public bool IgnoreCaseOnDeserialize;
        public bool KVStyleStringDictionary;
        public bool ParametricConstructorOverride;
        public bool SerializeNullValues = true;
        public bool ShowReadOnlyProperties;
        public bool UseEscapedUnicode = true;
        public bool UseExtensions = true;
        public bool UseFastGuid = true;
        public bool UseOptimizedDatasetSchema = true;
        public bool UseUTCDateTime = true;
        public bool UseValuesOfEnums;
        public bool UsingGlobalTypes = true;

        public void FixValues()
        {
            if (!this.UseExtensions)
            {
                this.UsingGlobalTypes = false;
            }
            if (this.EnableAnonymousTypes)
            {
                this.ShowReadOnlyProperties = true;
            }
        }
    }
}

