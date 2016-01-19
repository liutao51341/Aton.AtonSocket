using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Aton.AtonSocket.Core.Loader
{
    /// <summary>
    /// 加载器
    /// </summary>
    public class ReflectionLoader
    {
        /// <summary>
        /// 加载单个类型对象
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="baseTypes"></param>
        /// <param name="defaultType"></param>
        /// <returns></returns>
        public object ReflectionLoad(Assembly assembly, IList<Type> baseTypes, Type defaultType)
        {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (Type ntype in baseTypes)
                {
                    if (type.BaseType == ntype)
                    {
                        return Activator.CreateInstance(type);
                    }
                }
            }
            return Activator.CreateInstance(defaultType) ;
        }
        /// <summary>
        /// 加载多个类型对象
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="baseTypes"></param>
        /// <param name="defaultType"></param>
        /// <returns></returns>
        public List<object> ReflectionLoadList(Assembly assembly, IList<Type> baseTypes, Type defaultType)
        {
            List<object> m_typeList = new List<object>();
            foreach (Type type in assembly.GetTypes())
            {
                foreach (Type ntype in baseTypes)
                {
                    if (type.BaseType == ntype)
                    {
                        m_typeList.Add(Activator.CreateInstance(type));
                    }
                }
            }
            if (m_typeList.Count == 0) m_typeList.Add(Activator.CreateInstance(defaultType));
            return m_typeList;
        }
    }
}
