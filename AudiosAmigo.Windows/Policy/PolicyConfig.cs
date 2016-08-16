using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;

namespace AudiosAmigo.Windows.Policy
{
    public class PolicyConfig : IPolicyConfig
    {
        private readonly IPolicyConfig _config;

        [ComImport, Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")]
        internal class PolicyConfigClient
        {
        }

        public PolicyConfig()
        {
            var config = new PolicyConfigClient();
            if (WindowsVistaPolicyConfig.IsIWindowsVistaPolicyConfig(config))
            {
                _config = new WindowsVistaPolicyConfig(config);
            }
            else if (Windows7PolicyConfig.IsIWindows7PolicyConfig(config))
            {
                _config = new Windows7PolicyConfig(config);
            }
            else if (Windows10PolicyConfig.IsIWindows10PolicyConfig(config))
            {
                _config = new Windows10PolicyConfig(config);
            }
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
