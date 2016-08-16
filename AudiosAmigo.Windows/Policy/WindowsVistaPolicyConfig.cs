using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;

namespace AudiosAmigo.Windows.Policy
{
    internal class WindowsVistaPolicyConfig : IPolicyConfig
    {
        [Guid("568B9108-44BF-40B4-9006-86AFE5B5A620"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IWindowsVistaPolicyConfig
        {
            int GetMixFormat(string deviceName, IntPtr format);

            int GetDeviceFormat(string deviceName, bool isDefault, IntPtr format);

            int ResetDeviceFormat(string deviceName);

            int SetDeviceFormat(string deviceName, IntPtr endpointFormat, IntPtr mixFormat);

            int GetProcessingPeriod(string deviceName, bool isDefault, IntPtr defaultPeriod, IntPtr minimumPeriod);

            int SetProcessingPeriod(string deviceName, IntPtr period);

            int GetShareMode(string deviceName, IntPtr mode);

            int SetShareMode(string deviceName, IntPtr mode);

            int GetPropertyValue(string deviceName, bool isFxStore, IntPtr key, IntPtr variant);

            int SetPropertyValue(string deviceName, bool isFxStore, IntPtr key, IntPtr variant);

            int SetDefaultEndpoint(string deviceName, Role role);

            int SetEndpointVisibility(string deviceName, bool bVisible);
        }

        private readonly IWindowsVistaPolicyConfig _config;

        internal static bool IsIWindowsVistaPolicyConfig(object config) => config is IWindowsVistaPolicyConfig;

        internal WindowsVistaPolicyConfig(object config)
        {
            _config = (IWindowsVistaPolicyConfig)config;
        }

        public int GetMixFormat(string deviceName, IntPtr format) =>
            _config.GetMixFormat(deviceName, format);

        public int GetDeviceFormat(string deviceName, bool isDefault, IntPtr format) =>
            _config.GetDeviceFormat(deviceName, isDefault, format);

        public int ResetDeviceFormat(string deviceName) =>
            _config.ResetDeviceFormat(deviceName);

        public int SetDeviceFormat(string deviceName, IntPtr endpointFormat, IntPtr mixFormat) =>
            _config.SetDeviceFormat(deviceName, endpointFormat, mixFormat);

        public int GetProcessingPeriod(string deviceName, bool isDefault, IntPtr defaultPeriod, IntPtr minimumPeriod) =>
            _config.GetProcessingPeriod(deviceName, isDefault, defaultPeriod, minimumPeriod);

        public int SetProcessingPeriod(string deviceName, IntPtr period) =>
            _config.SetProcessingPeriod(deviceName, period);

        public int GetShareMode(string deviceName, IntPtr mode) =>
            _config.GetShareMode(deviceName, mode);

        public int SetShareMode(string deviceName, IntPtr mode) =>
            _config.SetShareMode(deviceName, mode);

        public int GetPropertyValue(string deviceName, bool isFxStore, IntPtr key, IntPtr variant) =>
            _config.GetPropertyValue(deviceName, isFxStore, key, variant);

        public int SetPropertyValue(string deviceName, bool isFxStore, IntPtr key, IntPtr variant) =>
            _config.SetPropertyValue(deviceName, isFxStore, key, variant);

        public int SetDefaultEndpoint(string deviceName, Role role) =>
            _config.SetDefaultEndpoint(deviceName, role);

        public int SetEndpointVisibility(string deviceName, bool isVisible) =>
            _config.SetEndpointVisibility(deviceName, isVisible);
    }
}
