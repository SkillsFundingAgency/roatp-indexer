using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Sfa.Roatp.Indexer.WorkerRole
{
    [DebuggerNonUserCode]
    static class SafeRoleEnvironment
    {
        static bool _isAvailable = true;
        static Type _roleEnvironmentType;
        static Type _roleInstanceType;
        static Type _roleType;
        static Type _localResourceType;

        static SafeRoleEnvironment()
        {
            try
            {
                TryLoadRoleEnvironment();
            }
            catch
            {
                _isAvailable = false;
            }

        }

        public static bool IsAvailable => _isAvailable;

        public static string CurrentRoleInstanceId
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new Exception("Role environment is not available, check IsAvailable before calling this property");
                }

                var instance = _roleEnvironmentType.GetProperty("CurrentRoleInstance")?.GetValue(null, null);
                return (string)_roleInstanceType.GetProperty("Id")?.GetValue(instance, null);
            }
        }

        public static string DeploymentId
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new Exception("Role environment is not available, check IsAvailable before calling this property");
                }

                return (string)_roleEnvironmentType.GetProperty("DeploymentId")?.GetValue(null, null);
            }
        }
        public static string CurrentRoleName
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new Exception("Role environment is not available, check IsAvailable before calling this property");
                }

                var instance = _roleEnvironmentType.GetProperty("CurrentRoleInstance").GetValue(null, null);
                var role = _roleInstanceType.GetProperty("Role").GetValue(instance, null);
                return (string)_roleType.GetProperty("Name").GetValue(role, null);
            }
        }

        public static string GetConfigurationSettingValue(string name)
        {
            if (!IsAvailable)
            {
                throw new Exception("Role environment is not available, check IsAvailable before calling this method");
            }

            return (string)_roleEnvironmentType.GetMethod("GetConfigurationSettingValue").Invoke(null, new object[] { name });
        }

        public static bool TryGetConfigurationSettingValue(string name, out string setting)
        {
            if (!IsAvailable)
            {
                throw new Exception("Role environment is not available, check IsAvailable before calling this method");
            }

            setting = string.Empty;
            try
            {
                setting = (string)_roleEnvironmentType.GetMethod("GetConfigurationSettingValue").Invoke(null, new object[] { name });
                return !string.IsNullOrEmpty(setting);
            }
            catch
            {
                return false;
            }
        }

        public static void RequestRecycle()
        {
            if (!IsAvailable)
            {
                throw new Exception("Role environment is not available, check IsAvailable before calling this method");
            }

            _roleEnvironmentType.GetMethod("RequestRecycle").Invoke(null, null);
        }

        public static string GetRootPath(string name)
        {
            if (!IsAvailable)
            {
                throw new Exception("Role environment is not available, check IsAvailable before calling this method");
            }

            var o = _roleEnvironmentType.GetMethod("GetLocalResource").Invoke(null, new object[] { name });
            return (string)_localResourceType.GetProperty("RootPath").GetValue(o, null);
        }

        public static bool TryGetRootPath(string name, out string path)
        {
            if (!IsAvailable)
            {
                throw new Exception("Role environment is not available, check IsAvailable before calling this method");
            }

            path = string.Empty;

            try
            {
                path = GetRootPath(name);
                return path != null;
            }
            catch
            {
                return false;
            }
        }

        static void TryLoadRoleEnvironment()
        {
            var serviceRuntimeAssembly = TryLoadServiceRuntimeAssembly();
            if (!_isAvailable)
            {
                return;
            }

            TryGetRoleEnvironmentTypes(serviceRuntimeAssembly);
            if (!_isAvailable)
            {
                return;
            }

            _isAvailable = IsAvailableInternal();
        }

        static void TryGetRoleEnvironmentTypes(Assembly serviceRuntimeAssembly)
        {
            try
            {
                _roleEnvironmentType = serviceRuntimeAssembly.GetType("Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment");
                _roleInstanceType = serviceRuntimeAssembly.GetType("Microsoft.WindowsAzure.ServiceRuntime.RoleInstance");
                _roleType = serviceRuntimeAssembly.GetType("Microsoft.WindowsAzure.ServiceRuntime.Role");
                _localResourceType = serviceRuntimeAssembly.GetType("Microsoft.WindowsAzure.ServiceRuntime.LocalResource");
            }
            catch (ReflectionTypeLoadException)
            {
                _isAvailable = false;
            }
        }

        static bool IsAvailableInternal()
        {
            try
            {
                return (bool)_roleEnvironmentType.GetProperty("IsAvailable").GetValue(null, null);
            }
            catch
            {
                return false;
            }
        }

        static Assembly TryLoadServiceRuntimeAssembly()
        {
            try
            {
#pragma warning disable 618
                var ass = Assembly.LoadWithPartialName("Microsoft.WindowsAzure.ServiceRuntime");
#pragma warning restore 618
                _isAvailable = ass != null;
                return ass;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
    }
}