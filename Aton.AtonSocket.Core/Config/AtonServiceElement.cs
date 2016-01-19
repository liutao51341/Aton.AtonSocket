using System.Collections.Specialized;
using System.Configuration;

namespace Aton.AtonSocket.Core.Config
{
    /// <summary>
    /// 服务元素
    /// </summary>
     class AtonSocketServer : ConfigurationElement
    {
        public AtonSocketServer() { }
        public AtonSocketServer(string name, string desc, string type)
        {
            base["Name"] = name;
            base["Type"] = type;
            base["Desc"] = desc;
        }

        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["Name"]; }
            set { base["Name"] = value; } 
        }
        [ConfigurationProperty("Desc", IsRequired = true)]
        public string Desc
        {
            get { return (string)base["Desc"]; }
            set { base["Desc"] = value; }
        }
        [ConfigurationProperty("Type", IsRequired = true)]
        public string Type
        {
            get { return (string)base["Type"]; }
            set { base["Type"] = value; } 
        }

        [ConfigurationProperty("disabled", DefaultValue = "false")]
        public bool Disabled
        {
            get { return (bool)base["disabled"]; }
            set { base["disabled"] = value; } 
        }

        public NameValueCollection Options { get; private set; }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            if (Options == null)
            {
                Options = new NameValueCollection();
            }

            Options.Add(name, value);
            return true;
        }
    }
}
