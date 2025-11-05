using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace RedSkullShoot
{
    class AuthCheck
    {
		public static string GetMachineUUID()
		{
			try
			{
				ManagementClass mc = new ManagementClass("Win32_ComputerSystemProduct");
				ManagementObjectCollection moc = mc.GetInstances();

				foreach (ManagementObject mo in moc)
				{
					return mo.Properties["UUID"].Value.ToString();
				}
				return "UNKNOWN";
			}
			catch
			{
				return "UNKNOWN";
			}
		}
	}
}
