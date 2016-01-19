using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core.Config
{
    /// <summary>
    /// socket  type config
    /// </summary>
    public class AtonSocketType : ConfigurationElement
    {
        public AtonSocketType() { }
        public AtonSocketType(string typeName, string typeAssembly)
        {
            base["TypeName"] = typeName;
            base["TypeAssembly"] = typeAssembly;
        }

        [ConfigurationProperty("TypeName")]
        public bool TypeName
        {
            get { return (bool)base["TypeName"]; }
            set { base["TypeName"] = value; }
        }

        [ConfigurationProperty("TypeAssembly")]
        public bool TypeAssembly
        {
            get { return (bool)base["TypeAssembly"]; }
            set { base["TypeAssembly"] = value; }
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
