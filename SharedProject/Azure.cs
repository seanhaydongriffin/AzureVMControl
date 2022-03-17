using System;
using System.Linq;
using System.Management.Automation;

namespace SharedProject
{
    class Azure
    {

        public static void Authenticate(bool force_auth = false)
        {
            System.Collections.ObjectModel.Collection<PSObject> output = null;
            PowerShell ps = PowerShell.Create();

            if (force_auth == false)
            {
                ps.AddScript("Get-AzureRmSubscription");
                output = ps.Invoke();
            }

            if (force_auth == true || output.Count == 0)
            {
                ps = PowerShell.Create();
                ps.AddScript("Login-AzureRmAccount");
                ps.Invoke();
            }
        }



        public static System.Collections.ObjectModel.Collection<PSObject> QueryVMs(String Subscription, String ResourceGroupName, String ScaleSetName, String InstanceArray)
        {
            //Authenticate();
            PowerShell ps = PowerShell.Create();
            ps.AddScript("Set-ExecutionPolicy -ExecutionPolicy Unrestricted");
            ps.AddScript("Select-AzureRmSubscription -Subscription '" + Subscription + "'");
            ps.AddScript("$d = " + InstanceArray + " ; Foreach ($i in $d) { (Get-AzureRmVmssVM -ResourceGroupName '" + ResourceGroupName + "' -VMScaleSetName '" + ScaleSetName + "' -InstanceView -InstanceId $i).Statuses.DisplayStatus[1] }");
            var output = ps.Invoke();
            return output;
        }

        public static System.Collections.ObjectModel.Collection<PSObject> StartVMs(String Subscription, String ResourceGroupName, String ScaleSetName, String InstanceArray)
        {
            //Authenticate();
            PowerShell ps = PowerShell.Create();
            ps.AddScript("Set-ExecutionPolicy -ExecutionPolicy Unrestricted");
            Console.WriteLine("Set-ExecutionPolicy -ExecutionPolicy Unrestricted");
            ps.AddScript("Select-AzureRmSubscription -Subscription '" + Subscription + "'");
            Console.WriteLine("Select-AzureRmSubscription -Subscription '" + Subscription + "'");
            ps.AddScript("Start-AzureRmVmss -ResourceGroupName '" + ResourceGroupName + "' -VMScaleSetName '" + ScaleSetName + "' -InstanceId " + InstanceArray + " -AsJob");
            Console.WriteLine("Start-AzureRmVmss -ResourceGroupName '" + ResourceGroupName + "' -VMScaleSetName '" + ScaleSetName + "' -InstanceId " + InstanceArray + " -AsJob");
            var output = ps.Invoke();
            return output;
        }

        public static System.Collections.ObjectModel.Collection<PSObject> StopVMs(String Subscription, String ResourceGroupName, String ScaleSetName, String InstanceArray)
        {
            //Authenticate();
            PowerShell ps = PowerShell.Create();
            ps.AddScript("Set-ExecutionPolicy -ExecutionPolicy Unrestricted");
            Console.WriteLine("Set-ExecutionPolicy -ExecutionPolicy Unrestricted");
            ps.AddScript("Select-AzureRmSubscription -Subscription '" + Subscription + "'");
            Console.WriteLine("Select-AzureRmSubscription -Subscription '" + Subscription + "'");
            ps.AddScript("Stop-AzureRmVmss -ResourceGroupName '" + ResourceGroupName + "' -VMScaleSetName '" + ScaleSetName + "' -InstanceId " + InstanceArray + " -Force -AsJob");
            Console.WriteLine("Stop-AzureRmVmss -ResourceGroupName '" + ResourceGroupName + "' -VMScaleSetName '" + ScaleSetName + "' -InstanceId " + InstanceArray + " -Force -AsJob");
            var output = ps.Invoke();
            return output;
        }



    }
}
