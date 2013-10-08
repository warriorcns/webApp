using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Management;
using System.IO;

namespace NETwork
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {

        public double? getGetCPUFrequency()
        {
         
            double? GHz = null;
            
            using (ManagementClass mc = new ManagementClass("Win32_Processor"))
            {
                foreach (ManagementObject mo in mc.GetInstances())
                {
                    GHz = 0.001 * (UInt32) mo.Properties["CurrentClockSpeed"].Value;
                    break;
                }
            }
            return GHz;
        }

        //public List<Temperature> Temperatures
        //{
        //    get
        //    {
        //        List<Temperature> result = new List<Temperature>();
        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
        //        foreach (ManagementObject obj in searcher.Get())
        //        {
        //            Double temp = Convert.ToDouble(obj["CurrentTemperature"].ToString());
        //            temp = (temp - 2732) / 10.0;
        //            result.Add(new Temperature { CurrentValue = temp, InstanceName = obj["InstanceName"].ToString() });
        //        }
        //        return result;

        //    }
        //}

        public Dictionary<string, int> getCPUInfo()
        {
            Dictionary<string, int> cpuInfo = new Dictionary<string, int>();

            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
                cpuInfo.Add("Number Of Physical Processors:", Convert.ToInt32(item["NumberOfProcessors"]));
            }

            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                cpuInfo.Add("Number of Cores",Convert.ToInt32(item["NumberOfCores"]));
            }
            cpuInfo.Add("Number of logical processors", Environment.ProcessorCount);
            
            return cpuInfo;
        }

        public List<string> getTemp()
        { 
            List<string> tmp = new List<string>();
            StreamWriter debugLog = new StreamWriter("_debug.txt");

            try
            {
                List<Temperature> result = new List<Temperature>();
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature"); //access denied - run visual studio as Administrator
                foreach (ManagementObject obj in searcher.Get())
                {
                    Double temp = Convert.ToDouble(obj["CurrentTemperature"].ToString());
                    temp = (temp - 2732) / 10.0;
                    result.Add(new Temperature { CurrentValue = temp, InstanceName = obj["InstanceName"].ToString() });
                }
            }
            catch (Exception ex)
            { 
            //ddddd
                debugLog.WriteLine("FATAL ERROR");
                debugLog.WriteLine("If you retrieve 'access denied' statement run Visual Studio as Administrator.");
                debugLog.WriteLine(ex);
            }
            debugLog.Close();
            debugLog.Dispose();
            return tmp;
        }

        /// <summary>
        /// Returns RAM Info
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, double> getGetRAMcount()
        {
            Dictionary<string, double> ramValues = new Dictionary<string, double>();

            try
            {
                ManagementObjectSearcher Search = new ManagementObjectSearcher("SELECT * From Win32_PhysicalMemory");

                foreach (ManagementObject mo in Search.Get())
                {
                    double Ram_Bytes = (Convert.ToDouble(mo["Capacity"]));
                    ramValues.Add("Capacity [MB]",Ram_Bytes / 1048576);
                    //ramValues.Add(Ram_Bytes);

                    //double PageFileSpace = Convert.ToDouble(mo["TotalPageFileSpace"]);
                    ramValues.Add("Speed [MHz]", Convert.ToDouble(mo["Speed"]));
                    //double availableMem = new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory;
                    //ramValues.Add(availableMem);
                    //Console.WriteLine("Total Physical Memory = " + item["TotalPhysicalMemory"]);
                    //Console.WriteLine("Total Virtual Memory = " + item["TotalVirtualMemory"]);
                    //Console.WriteLine("Available Virtual Memory = " + item["AvailableVirtualMemory"]);
                }
            }
            catch (ManagementException ex) 
            { 
                //todo - log exceptions
            }
            return ramValues;
        }

        public Dictionary<string, string> GetDriveInfo()
        {
            //dictionary object to hold the values
            Dictionary<string, string> driveInfo = new Dictionary<string, string>();
            //create our WMI searcher
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"select * from Win32_LogicalDisk WHERE DriveType = 3");
            //now loop through all the item found with the query
            foreach (ManagementObject obj in searcher.Get())
            {
                //create the used space by subtracting the size from the free space
                double used = Convert.ToDouble(obj["Size"]) - Convert.ToDouble(obj["FreeSpace"]);
                used /= 1073741824;
                //add all the items to the collection
                driveInfo.Add("FreeSpace", (Convert.ToDouble(obj["FreeSpace"]) / 1073741824).ToString());
                driveInfo.Add("Size", (Convert.ToDouble(obj["Size"]) / 1073741824).ToString());
                driveInfo.Add("UsedSpace", used.ToString());
                //driveInfo.Add("Temperature",
            }
            //return the info
            return driveInfo;
        }
        
        public class Temperature
        {

            public double CurrentValue { get; set; }

            public string InstanceName { get; set; }
        }

        #region default
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
        #endregion

    }
}
