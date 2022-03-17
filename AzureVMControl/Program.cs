using SharedProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AzureVMControl
{
    class Program
    {

        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 6;
        // more here: http://www.pinvoke.net/default.aspx/user32.showwindow

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main(string[] args)
        {
            //Network.PingUntilUnavailable("azureauto31");
//            Network.PingUntilAvailable("azureauto31");
  //          Environment.Exit(0);


            var scale_set_names = new List<string>();
            var hostnames = new List<string>();
            var instance_ids = new List<int>();
            var project_ids = new List<int>();
            var execution_group_ids = new List<int>();
            int deploy_int = 0;

            //Console.Clear();

            if (args.Contains("/?", StringComparer.OrdinalIgnoreCase) || args.Contains("/help", StringComparer.OrdinalIgnoreCase) || args.Contains("/h", StringComparer.OrdinalIgnoreCase))
            {
                Log.WriteLine("-------------------------------------------------------------------------------");
                Log.WriteLine("   AzureVMControl");
                Log.WriteLine("-------------------------------------------------------------------------------");
                Log.WriteLine("");
                Log.WriteLine("   Control Azure VMs.");
                Log.WriteLine("");
                Log.WriteLine("              Usage      :: AzureVMControl [options]");
                Log.WriteLine("");
                Log.WriteLine("::");
                Log.WriteLine("");
                Log.WriteLine("/ACT:action              :: the action to perform.");
                Log.WriteLine("                            \"query\" to get the state of the VMs.");
                Log.WriteLine("                            \"start\" to start the VMs.");
                Log.WriteLine("                            \"stop\" to stop the VMs.");
                Log.WriteLine("                            \"authenticate\" to re-authenticate to Azure.");
                Log.WriteLine("                            \"runtests\" to start the VMs, run tests, and stop the same VMs.");
                Log.WriteLine("");
                Log.WriteLine("/SUB:subscription        :: the name of the Azure subscription.");
                Log.WriteLine("                            eg. \"Load Testing - Dev/Test\"");
                Log.WriteLine("");
                Log.WriteLine("/RES:resource group      :: the name of the Azure resource group.");
                Log.WriteLine("                            eg. \"LNB-Test\"");
                Log.WriteLine("");
                Log.WriteLine("/SSET:scale set[s]       :: comma separated list of Azure scale set names.");
                Log.WriteLine("                            eg. \"lnbtest\"");
                Log.WriteLine("");
                Log.WriteLine("/INST:instance[s]        :: comma separated list of Azure VM instances.");
                Log.WriteLine("");
                Log.WriteLine("/HOST:host[s]            :: comma separated list of Azure VM hostnames.");
                Log.WriteLine("");
                Log.WriteLine("/PROJ:project[s]         :: comma separated list of CoPilot project IDs.");
                Log.WriteLine("");
                Log.WriteLine("/EG:execution group[s]   :: comma separated list of CoPilot execution group IDs.");
                Log.WriteLine("");
                Log.WriteLine("/DEPLOY:flag             :: A flag to indicate if we should deploy prior to runs.");
                Log.WriteLine("                            N = no deploy");
                Log.WriteLine("                            Y = deploy");
                Log.WriteLine("");
                Log.WriteLine("/TFSUSER:username        :: TFS username for deployments.");
                Log.WriteLine("");
                Log.WriteLine("/TFSPASS:password        :: TFS password for deployments.");
                Log.WriteLine("");

                //Log.WriteLine("\nPress any key to continue...");
                //Console.ReadKey();

                Environment.Exit(0);
            }

            //Log.Initialise(Array.Find(args, element => element.StartsWith("/LOG:", StringComparison.InvariantCultureIgnoreCase)));

            // if /ACT switch provided, then use this to determine the action to perform

            var action = Array.Find(args, element => element.StartsWith("/ACT:", StringComparison.InvariantCultureIgnoreCase));

            if (action != null)

                action = action.ReplaceIgnoreCase("/ACT:", "");

            if (action.Equals("authenticate"))
            {
                Azure.Authenticate(true);
                Environment.Exit(0);
            }

            // if /SUB switch provided, then use this to determine the Azure subscription

            var subscription = Array.Find(args, element => element.StartsWith("/SUB:", StringComparison.InvariantCultureIgnoreCase));

            if (subscription != null)

                subscription = subscription.ReplaceIgnoreCase("/SUB:", "");

            // if /RES switch provided, then use this to determine the Azure resource group

            var resource_group = Array.Find(args, element => element.StartsWith("/RES:", StringComparison.InvariantCultureIgnoreCase));

            if (resource_group != null)

                resource_group = resource_group.ReplaceIgnoreCase("/RES:", "");

            // if /SSET switch provided, then get the list of scale sets

            var scale_set_names_str = Array.Find(args, element => element.StartsWith("/SSET:", StringComparison.InvariantCultureIgnoreCase));

            if (scale_set_names_str != null)
            {
                scale_set_names_str = scale_set_names_str.ReplaceIgnoreCase("/SSET:", "");
                scale_set_names = scale_set_names_str.Split(',').ToList();
            }

            // if /INST switch provided, then get the list of instances

            var instance_ids_str = Array.Find(args, element => element.StartsWith("/INST:", StringComparison.InvariantCultureIgnoreCase));

            if (instance_ids_str != null)
            {
                instance_ids_str = instance_ids_str.ReplaceIgnoreCase("/INST:", "");
                instance_ids = instance_ids_str.Split(',').Select(int.Parse).ToList();
            }

            // if /HOST switch provided, then get the list of hostnames

            var hostnames_str = Array.Find(args, element => element.StartsWith("/HOST:", StringComparison.InvariantCultureIgnoreCase));

            if (hostnames_str != null)
            {
                hostnames_str = hostnames_str.ReplaceIgnoreCase("/HOST:", "");
                hostnames = hostnames_str.Split(',').ToList();
            }

            // if /PROJ switch provided, then get the list of project IDs

            var project_ids_str = Array.Find(args, element => element.StartsWith("/PROJ:", StringComparison.InvariantCultureIgnoreCase));

            if (project_ids_str != null)
            {
                project_ids_str = project_ids_str.ReplaceIgnoreCase("/PROJ:", "");
                project_ids = project_ids_str.Split(',').Select(int.Parse).ToList();
            }

            // if /PROJ switch provided, then get the list of project IDs

            var execution_group_ids_str = Array.Find(args, element => element.StartsWith("/EG:", StringComparison.InvariantCultureIgnoreCase));

            if (execution_group_ids_str != null)
            {
                execution_group_ids_str = execution_group_ids_str.ReplaceIgnoreCase("/EG:", "");
                execution_group_ids = execution_group_ids_str.Split(',').Select(int.Parse).ToList();
            }

            // if /DEPLOY switch provided, then use this to determine the Azure resource group

            var deploy = Array.Find(args, element => element.StartsWith("/DEPLOY:", StringComparison.InvariantCultureIgnoreCase));

            if (deploy != null)
            {
                deploy = deploy.ReplaceIgnoreCase("/DEPLOY:", "");

                if (deploy.Equals("Y"))

                    deploy_int = 1;
            }

            // if /TFSUSER switch provided, then use this to determine the Azure resource group

            var tfs_user = Array.Find(args, element => element.StartsWith("/TFSUSER:", StringComparison.InvariantCultureIgnoreCase));

            if (tfs_user != null)
            
                tfs_user = tfs_user.ReplaceIgnoreCase("/TFSUSER:", "");

            // if /TFSPASS switch provided, then use this to determine the Azure resource group

            var tfs_pass = Array.Find(args, element => element.StartsWith("/TFSPASS:", StringComparison.InvariantCultureIgnoreCase));

            if (tfs_pass != null)

                tfs_pass = tfs_pass.ReplaceIgnoreCase("/TFSPASS:", "");


            // Check Azure authentication and re-authenticate if required
            Azure.Authenticate();

            if (action.ToLower().Equals("query"))
            {
                //                var output = Azure.QueryVMs(subscription, resource_group, scale_set, instance_ids_str);

                for (int i = 0; i < instance_ids.Count; i++)
                {
                    var output = Azure.QueryVMs(subscription, resource_group, scale_set_names[i], instance_ids[i].ToString());
                    Console.WriteLine("Instance " + instance_ids[i] + " = " + output[0].BaseObject.ToString());

                    //for (int azure_instance_num = 0; azure_instance_num < output.Count; azure_instance_num++)

                    //    Console.WriteLine("Instance " + instance_ids[azure_instance_num] + " = " + output[azure_instance_num].BaseObject.ToString());
                }
            }

            if (action.ToLower().Equals("start"))
            {
                //   Azure.StartVMs(subscription, resource_group, scale_set, instance_ids_str);


                for (int i = 0; i < instance_ids.Count; i++)

                    Azure.StartVMs(subscription, resource_group, scale_set_names[i], instance_ids[i].ToString());

                foreach (var hostname in hostnames)
                
                    Network.PingUntilAvailable(hostname);
                


            }

            if (action.ToLower().Equals("stop"))
            {
                //    Azure.StopVMs(subscription, resource_group, scale_set, instance_ids_str);


                for (int i = 0; i < instance_ids.Count; i++)

                    Azure.StopVMs(subscription, resource_group, scale_set_names[i], instance_ids[i].ToString());

                foreach (var hostname in hostnames)
                
                    Network.PingUntilUnavailable(hostname);
                
            }

            if (action.ToLower().Equals("runtests"))
            {
                // add the credentials needed for the RDP connections to the VMs in the run
                foreach (var hostname in hostnames)
                {
                    Console.WriteLine("Adding RDP credentials for hostname " + hostname);
                    Command.Execute(Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe"), "/generic:TERMSRV/" + hostname + " /user:auto /pass:janison");
                }

                // Stop the VMs for the runs
                Console.WriteLine("Stopping VM instances " + instance_ids_str);

                for (int i = 0; i < instance_ids.Count; i++)

                    Azure.StopVMs(subscription, resource_group, scale_set_names[i], instance_ids[i].ToString());

                // Ping the VMs until they become unavailable
                foreach (var hostname in hostnames)

                    Network.PingUntilUnavailable(hostname);

                // Start the VMs for the runs
                Console.WriteLine("Starting VM instances " + instance_ids_str);

                for (int i = 0; i < instance_ids.Count; i++)

                    Azure.StartVMs(subscription, resource_group, scale_set_names[i], instance_ids[i].ToString());

                // Ping the VMs until they become available
                foreach (var hostname in hostnames)

                    Network.PingUntilAvailable(hostname);

                // Delay for a safe period until VMs should be ready for use
//                Console.WriteLine("Allowing 3 minutes for VMs to start");
  //              System.Threading.Thread.Sleep(3 * 60 * 1000);

                // RDP to each VM to create a desktop session for the run
                foreach (var hostname in hostnames)
                {
                    Console.WriteLine("Creating a RDP connection to hostname " + hostname);
                    Command.Execute(Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe"), "/v:" + hostname);
                }

                // Stop the desktop of each VM locking and breaking GUI based automated runs by converting the terminal session into a persistent console session
                //System.Threading.Thread.Sleep(10 * 1000);

                //foreach (var instance_id in instance_ids)
                //{
                //    var sessions = Command.Execute(Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\quser.exe"), "/server:azureauto" + instance_id, "", Encoding.UTF8);
                //    //Console.WriteLine(sessions);

                //    string[] lines = sessions.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                //    for (int i = 1; i < lines.Length; i++)
                //    {
                //        if (lines[i].Length >= 43)
                //        {
                //            var fred = lines[i].Substring(23, 20).Trim();
                //      //      Console.WriteLine(fred);
                //            Command.Execute("psexec.exe", "\\\\azureauto" + instance_id + " -u auto -p janison -s tscon " + fred + " /dest:console", System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                //        }
                //    }
                //}

                //foreach (var instance_id in instance_ids)
                //{
                //    IntPtr hwnd = FindWindowByCaption(IntPtr.Zero, "azureauto" + instance_id + " - Remote Desktop Connection");
                //    ShowWindow(hwnd, SW_MINIMIZE);
                //}


                // The runs
                var RunExecutionGroup_process = new List<System.Diagnostics.Process>();

                for (int i = 0; i < execution_group_ids.Count; i++)
                {
                    Console.WriteLine("Running project id " + project_ids[i] + " execution group id " + execution_group_ids[i]);
                    var process = Command.Execute("C:\\CoPilot.Net\\RunExecutionGroup.exe", "azureauto3 auto janison " + project_ids[i] + " \"" + execution_group_ids[i] + "\" \"Sprint 1\" \"SIT\" \"\" " + deploy_int + " \"" + tfs_user + "\" \"" + tfs_pass + "\"", "C:\\CoPilot");
                    RunExecutionGroup_process.Add(process);
                }

                // Wait for all execution groups to end
                Console.WriteLine("Waiting for " + execution_group_ids.Count + " runs to end");
                foreach (var process in RunExecutionGroup_process)
                {
                    process.WaitForExit();
                }


            }

        }
    }
}
