using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core.Utility
{
    public class ReflectUtility
    {
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool TryCreateInstance<T>(string type, out T result)
        {
            Type instanceType = null;
            result = default(T);

            if (!TryGetType(type, out instanceType))
                return false;

            try
            {
                object instance = Activator.CreateInstance(instanceType);
                result = (T)instance;
                return true;
            }
            catch 
            {
                return false;
            }
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool TryGetType(string type, out Type result)
        {
            try
            {
                result = Type.GetType(type, true);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
