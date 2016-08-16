using System;
using NAudio.CoreAudioApi;

namespace AudiosAmigo.Windows.Policy
{
    public interface IPolicyConfig
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

        int SetEndpointVisibility(string deviceName, bool isVisible);
    }
}