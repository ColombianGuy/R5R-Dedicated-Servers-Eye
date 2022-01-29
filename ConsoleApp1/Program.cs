using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace ColombiasDediWatcher
{
	//todo config file
	//todo shuffle map array
	//todo map unique selection for servers //DONE
	class Program
	{
		public static string[,] PIDS;
		public static string[] chosenMapsArray;
		public static int chosenMapsQt;
		public static int ServersQt;
		public static int num_crashes = 0;
		public static void Title()
		{
			Console.Write("\n");
			Console.Write("-----------------------R5R DEDICATED SERVERS - MANAGEMENT TOOL v0.9------------------\n");
			Console.Write("--------------------------------------By ColombiaFPS---------------------------------\n");
		}
		public static void Main()
		{
			Title();
			Console.Write("\n[!] Enter the amount of servers: ");
			string svQt = Console.ReadLine();
			ServersQt = Int16.Parse(svQt);
			PIDS = new string[ServersQt,5];

			//hostnames = new string[ServersQt];
			for(int i = 0; i < ServersQt; i++)  
			{
				Console.Write("\n[!] Enter the HOSTNAME for server " + (i+1) + ": ");
				PIDS[i, 0] = Console.ReadLine();
				Console.Write("\n[!] Enter the -GAMEMODE- for server " + (i + 1) + ": ");
				PIDS[i, 4] = Console.ReadLine();

			}            
			Console.Write("\n[!] Do you want to change maps periodically? [y/n] ");
			string key2 = Console.ReadLine();
			if (key2 == "Y" || key2 == "y")
			{
				for (int i = 0; i < ServersQt; i++)
				{
					Console.Write("\n[!] Do you want to change map periodically in server with playlist_" + (i + 1) + ".txt? [y/n] ");
					string key3 = Console.ReadLine();
					if (key3 == "Y" || key3 == "y")
					{
						PIDS[i, 2] = "TRUE";
						Console.Write("\n[!] Enter the time limit for map changing in server with playlist_" + (i + 1) + ".txt in minutes: ");
						PIDS[i, 3] = Console.ReadLine();
					} else if (key3 == "N" || key3 == "n")
					{
						PIDS[i, 2] = "FALSE";
					}
				}

				chosenMapsQt = 0;


				string[] allmaps = { "mp_rr_canyonlands_64k_x_64k", "mp_rr_canyonlands_mu1", "mp_rr_canyonlands_mu1_night", "mp_rr_desertlands_64k_x_64k", "mp_rr_desertlands_64k_x_64k_nx", "mp_rr_canyonlands_staging" };
				List<string> chosenMaps = new List<string>();
				foreach (string map in allmaps)
				{

					Console.Write("\n[!] Do you want to add " + ReturnMapString(map) + " in the rotation? [y/n]");
					string key = Console.ReadLine();
					if (key == "Y" || key == "y")
					{
						chosenMaps.Add(map);
						chosenMapsQt++;
					}
					else if (key == "N" || key == "n")
					{
						continue;
					}
				}

				chosenMapsArray = chosenMaps.ToArray();
				Console.Write("\n----------------------------------------------------------------");
				Console.Write("\n[!] These are the maps you chose for the rotation: \n");
				Console.Write("----------------------------------------------------------------\n\n");
				foreach (var item in chosenMapsArray)
				{
					Console.WriteLine("  " + item.ToString() + ": " + ReturnMapString(item.ToString()) + "\n");
				}
				Console.Write("----------------------------------------------------------------\n\n");
				Console.Write("[!] Are you sure about this map rotation? [y/n] ");
				string key4 = Console.ReadLine();
				if (key4 == "N" || key4 == "n")
				{
					chosenMapsQt = 0;
					Console.Write("\n[!] Ok, let's do it again.");
					foreach (string map in allmaps)
					{

						Console.Write("\n[!] Do you want to add " + ReturnMapString(map) + " to the rotation? [y/n]");
						string key = Console.ReadLine();
						if (key == "Y" || key == "y")
						{
							chosenMaps.Add(map);
							chosenMapsQt++;
						}
						else if (key == "N" || key == "n")
						{
							continue;
						}
					}
					Console.Write("----------------------------------------------------------------\n\n");
					Console.Write("\n[!] These are the maps you chose for the rotation: \n");
					Console.Write("----------------------------------------------------------------\n\n");
					foreach (var item in chosenMapsArray)
					{
						Console.WriteLine("  " + item.ToString() + ": " + ReturnMapString(item.ToString()) + "\n");
					}
				}

			}
			else if (key2 == "N" || key2 == "n")
			{
				for (int i = 0; i < ServersQt; i++)
				{
					PIDS[i, 2] = "FALSE";
				}
			}
			Console.Write("\n[!] SETUP IS DONE. Press a key to launch servers. \n");
			Console.ReadLine();
			LaunchServers();
		}
		static string ReturnMapString(string mapname)
		{
			switch (mapname)
			{
				case "mp_rr_canyonlands_64k_x_64k":
					return "Kings Canyon S0";
				case "mp_rr_canyonlands_mu1":
					return "Kings Canyon MU1";
				case "mp_rr_canyonlands_mu1_night":
					return "Kings Canyon Night";
				case "mp_rr_desertlands_64k_x_64k":
					return "World's Edge S3";
				case "mp_rr_desertlands_64k_x_64k_nx":
					return "World's Edge S3 Night";
				case "mp_rr_canyonlands_staging":
					return "Firing Range";
			}
			return "";    
		}
		static void LaunchServers()
		{
				Console.Write("[!] LAUNCHING " + ServersQt + " DEDICATED SERVERS...\n\n");
				//editing the startup_dedi.cfg file so we can have different playlists.
				//TODO: Update to startup_dedi_retail.cfg when necessary
				File.Copy(Path.Combine(Environment.CurrentDirectory, "platform\\cfg\\startup_dedi_retail.cfg"), Path.Combine(Environment.CurrentDirectory, "platform\\cfg\\startup_dedi_retail_BACKUP.cfg"), true);

				for (int i = 0; i < ServersQt; i++)
				{
					string DediCfg = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "platform\\cfg\\startup_dedi_retail_BACKUP.cfg"));
					DediCfg = DediCfg.Replace("-playlistfile \"playlists_r5_patch.txt\"", "-playlistfile \"playlist_" + (i + 1) + ".txt\"");
					File.WriteAllText("platform\\cfg\\startup_dedi_retail.cfg", DediCfg);
				
					File.AppendAllText("platform\\cfg\\startup_dedi_retail.cfg",
					   Environment.NewLine + "+launchplaylist " + PIDS[i,4] + Environment.NewLine);

					File.AppendAllText("platform\\cfg\\startup_dedi_retail.cfg",
					   "+hostname \"" + PIDS[i,0] + "\"" + Environment.NewLine);

					File.AppendAllText("platform\\cfg\\startup_dedi_retail.cfg",
					   "+sv_pylonvisibility 1" + Environment.NewLine);
				
					File.Copy(Path.Combine(Environment.CurrentDirectory, "build.txt"), Path.Combine(Environment.CurrentDirectory, "build_BACKUP.txt"), true);
					string buildfile = "SV " + (i + 1) + " - " + PIDS[i, 0];
					File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "build.txt"), buildfile);
					Thread.Sleep(3000);
					string r5rArguments = "-dedi";
					Process.Start("r5reloaded.exe", r5rArguments);
					Thread.Sleep(3000);
				}
				//File.Delete(Path.Combine(Environment.CurrentDirectory, "platform\\cfg\\startup_dedi_retail_BACKUP.cfg"));
				//end of dedi startup file modification
				Thread.Sleep(3000);
				Console.Write("----------------------------------------------------------------\n\n");
				Console.Write("[!] ALL " + ServersQt + " DEDICATED SERVERS LAUNCHED.\n\n");
				Console.Write("----------------------------------------------------------------\n");
				File.Copy(Path.Combine(Environment.CurrentDirectory, "platform\\cfg\\startup_dedi_retail_BACKUP.cfg"), Path.Combine(Environment.CurrentDirectory, "platform\\cfg\\startup_dedi_retail.cfg"), true);
				File.Copy(Path.Combine(Environment.CurrentDirectory, "build_BACKUP.txt"), Path.Combine(Environment.CurrentDirectory, "build.txt"), true);
				Process[] r5rdedis = Process.GetProcessesByName("r5apex_ds");
			foreach (Process process in r5rdedis)
			{ 
				process.EnableRaisingEvents = true;
			}
				for (int i = 0; i < ServersQt; i++)
				{
					int dediSlot = i + 1;
					PIDS[i, 1] = ((r5rdedis[i].Id).ToString()).Trim();
					
					Console.Write("[+] Creating autorestart thread for dedicated " + dediSlot + " - Saved PID: " + PIDS[i, 1] + "\n");
					Thread thread = new Thread(() => ActualWatcher(PIDS[i, 1]));
					thread.Start();
					Thread.Sleep(100);

							if (PIDS[i, 2] == "TRUE") { 
								Thread thread2 = new Thread(() => ChangeLevelThread(i));
								thread2.Start();
							}
						Thread.Sleep(100);
				}
				Console.Write("----------------------------------------------------------------\n");
		}
		public static void ChangeLevelThread(int dediSlot)
			{
					Console.Write("----------------------------------------------------------------\n");
					Console.Write("[!] THREAD " + (dediSlot+1) + ": changelevel thread is activated!\n");
					Console.Write("----------------------------------------------------------------\n");
					int ThisThreadTime = Convert.ToInt32(PIDS[dediSlot, 3])*60; //grab the time for this playlist
					//int ThisThreadTime = Convert.ToInt32(PIDS[dediSlot, 3]); //grab the time for this playlist
					DateTime endTime = DateTime.Now.AddSeconds(ThisThreadTime);
					int PID = 0;
					Random getrandom = new Random();
				
					while (DateTime.Now < endTime)
					{
						PID = Convert.ToInt32(PIDS[dediSlot, 1]);
						Thread.Sleep(1000);
					}

					Process dediToWatch = Process.GetProcessById(PID);
					//this is not very optimized. TODO: shuffle array
					string nextmap = chosenMapsArray[getrandom.Next(0, chosenMapsQt)];
					//this is very optimized. Colombia.
					string ConCommand = "changelevel " + nextmap;
					Console.Write("----------------------------------------------------------------\n");
					Console.Write("[!] CHANGING MAP! - " + nextmap);
					Console.Write("\n----------------------------------------------------------------\n");
					Keyboard.Messaging.SendStringToProcess(dediToWatch.MainWindowHandle, ConCommand);
					Thread.Sleep(1000);
					ChangeLevelThread(dediSlot);
			}
		public static void ActualWatcher(string PID)
		{
			int PIDasInt = Convert.ToInt32(PID);
			int pos = 0;
			for (int i = 0; i < ServersQt; ++i)
			{
				if (PIDS[i, 1] == PID)
				{
					pos = i;
					break;
				}
			}

			int dediSlot = pos + 1;
			string gethostname = PIDS[pos, 0];
			Process dediToWatch = Process.GetProcessById(PIDasInt);

			dediToWatch.EnableRaisingEvents = true;

			Thread.Sleep(1000);
			while (!dediToWatch.HasExited)
			{
				Console.Write("[+] THREAD " + dediSlot + ": Dedicated with PID " + PID + " is up. \n");
				Thread.Sleep(500);
			}
			Thread.Sleep(1000);

			if (dediToWatch.HasExited)
			{
				DateTime crashedtime = DateTime.Now;
				++num_crashes;
				Thread.Sleep(3000);
				Console.Write("----------------------------------------------------------------\n\n"); 
				Console.Write("[!] THREAD " + dediSlot + ": DEDICATED WITH PID " + PID + " HAS CRASHED!\n[!] " + crashedtime + "  - Total observed crashes: " + num_crashes.ToString() + "\n[!] THREAD " + dediSlot + ": Server name: " + gethostname + "\n[!] THREAD " + dediSlot + ": CREATING NEW DEDICATED INSTANCE. \n\n");
				Console.Write("----------------------------------------------------------------\n");
				//File.Copy(Path.Combine(Environment.CurrentDirectory, "platform\\logs\\sqvm_print.log"), Path.Combine(Environment.CurrentDirectory, "platform\\logs\\sv" + dediSlot + "_" + "CRASH" + num_crashes + ".log"), true);
				//Thread.Sleep(1000);
				//File.Copy(Path.Combine(Environment.CurrentDirectory, "platform\\logs\\sqvm_print_EMPTY.log"), Path.Combine(Environment.CurrentDirectory, "platform\\logs\\sqvm_print.log"), true);
				//Thread.Sleep(1000);
				File.Copy(Path.Combine(Environment.CurrentDirectory, "platform\\cfg\\startup_dedi_retail.cfg"), Path.Combine(Environment.CurrentDirectory, "platform\\cfg\\startup_dedi_retail_BACKUP.cfg"), true);
				Thread.Sleep(1000);
				string DediCfg = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "platform\\cfg\\startup_dedi_retail_BACKUP.cfg"));
				DediCfg = DediCfg.Replace("-playlistfile \"playlists_r5_patch.txt\"", "-playlistfile \"playlist_" + dediSlot + ".txt\"");
				File.WriteAllText("platform\\cfg\\startup_dedi_retail.cfg", DediCfg);

				File.AppendAllText("platform\\cfg\\startup_dedi_retail.cfg",
				   Environment.NewLine + "+launchplaylist " + PIDS[pos, 4] + Environment.NewLine);

				File.AppendAllText("platform\\cfg\\startup_dedi_retail.cfg",
				   "+hostname \"" + PIDS[pos, 0] + "\"" + Environment.NewLine);

				File.AppendAllText("platform\\cfg\\startup_dedi_retail.cfg",
				   "+sv_pylonvisibility 1" + Environment.NewLine);
				File.Copy(Path.Combine(Environment.CurrentDirectory, "build.txt"), Path.Combine(Environment.CurrentDirectory, "build_BACKUP.txt"), true);
				string buildfile = "SV " + (dediSlot + 1) + " - " + PIDS[pos, 0];
				File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "build.txt"), buildfile);

				Thread.Sleep(1000);
				string r5rArguments = "-dedi";
				Process.Start("r5reloaded.exe", r5rArguments);
				Thread.Sleep(10000);

				Process[] r5rdedisnew = Process.GetProcessesByName("r5apex_ds");
				for (int i = 0; i < ServersQt; i++)
				{
					if (r5rdedisnew[i].Id != Convert.ToInt16(PIDS[i, 1].Trim()));
					{
						PIDS[pos, 1] = ((r5rdedisnew[i].Id).ToString()).Trim();
						Console.Write("------------------------------------------------------------------------------------\n\n");
						Console.Write("[!] THREAD " + dediSlot + ": NEW DEDICATED INSTANCE DETECTED.\n\n[!] THREAD " + dediSlot + ": Server name: " + gethostname + "\n\n[!] THREAD " + dediSlot + ": SAVED PID " + PIDS[i, 1] + "\n\n");
						Console.Write("------------------------------------------------------------------------------------\n");
						File.Copy(Path.Combine(Environment.CurrentDirectory, "platform\\cfg\\startup_dedi_retail_BACKUP.cfg"), Path.Combine(Environment.CurrentDirectory, "platform\\cfg\\startup_dedi_retail.cfg"), true);
						File.Copy(Path.Combine(Environment.CurrentDirectory, "build_BACKUP.txt"), Path.Combine(Environment.CurrentDirectory, "build.txt"), true);
						Thread.Sleep(1000);
						Thread threadx = new Thread(() => ActualWatcher(PIDS[pos, 1]));
						threadx.Start();
						break;
					}

				}

			}


			}
	}
}