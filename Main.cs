using System;
using System.IO;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;

namespace KukuEvent
{
    public class Main : Script
    {
        Gang gang; //object to Gang class
        Vector3 ballasSpawn; //position to spawn ballas
        Vector3 grovesSpawn;//position to spawn grove
        Random rand; //used as a probability check

        //ini setting stuffs
        ScriptSettings config;
        int eventTriggerChance;//
        int warpToCarTimer;//
        int startDrivingTimer;//
        int thugsPerTeam;//
        int radiusToDeleteThugs;//
        int spawnDistanceBallas;//
        int spawnDistanceGroves;//
        Keys trigger;//
        Keys delete;//
        public Main()
        {
            UI.Notify("Kuku's RE-GangShootNRun");
            Tick += onTick;
            KeyDown += onKeyDown;
            gang = new Gang();
            rand = new Random();
            loadINI();
        }

        private void loadINI()
        {
            string path = "scripts\\gangShoot_kuku.ini";
            if (File.Exists(path) == false)
            {
                //No ini previously
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("[BASIC]");
                    sw.WriteLine("Enable_Event = L");
                    sw.WriteLine("Delete_Event = Delete");
                    sw.WriteLine("Number_of_Members_per_Team = 7");
                    sw.WriteLine("Event_Trigger_Chance = 200 //higher the number lower the chances");

                    sw.WriteLine("[ADVANCED]");
                    sw.WriteLine("//DONT SET SPAWN DISTANCE TOO FAR OR ESLE THEY WONT FIGHT");
                    sw.WriteLine("Spawn_Distance_Ballas = 115");
                    sw.WriteLine("Spawn_Distance_Groves = 110");
                    sw.WriteLine("Radius_To_Delete = 500 //Any ped farther from this range will be deleted");
                    sw.WriteLine("Start_Driving_Timer = 18 //ped will start driving after this much sec has been passed");
                    sw.WriteLine("Warp_Timer = 10 // Ped will be warped to the nearest car after this much seconds");

                }
            }
            else
            {
              config =  ScriptSettings.Load(path);
            }


            //setting the values
            eventTriggerChance = config.GetValue<int>("BASIC", "Event_Trigger_Chance", 200);
            warpToCarTimer = config.GetValue<int>("ADVANCED", "Warp_Timer", 10);
            startDrivingTimer = config.GetValue<int>("ADVANCED", "Start_Driving_Timer", 18);
            thugsPerTeam = config.GetValue<int>("BASIC", "Number_of_Members_per_Team", 5);
            radiusToDeleteThugs = config.GetValue<int>("ADVANCED", "Radius_To_Delete", 500);
            spawnDistanceBallas = config.GetValue<int>("ADVANCED", "Spawn_Distance_Ballas", 118);
            spawnDistanceGroves = config.GetValue<int>("ADVANCED", "Spawn_Distance_Groves", 110);
            delete = config.GetValue<Keys>("BASIC", "Delete_Event", Keys.Delete);
            trigger = config.GetValue<Keys>("BASIC", "Enable_Event", Keys.OemQuestion); 
        }


        private void onTick(object sender, EventArgs e)
        {

            int chance = rand.Next(1, eventTriggerChance); //probability of incident to occour
            int numb = rand.Next(1, eventTriggerChance);
            Interval = 1000;
            gang.onTickClean(Game.Player.Character, radiusToDeleteThugs);
           // gang.changeBlip();
            if (numb == chance) {
                
                if (gang.eventActive == false)
                {
                    UI.Notify("Gang event-Loading models");
                    ballasSpawn = Game.Player.Character.Position + Game.Player.Character.ForwardVector * spawnDistanceBallas;
                    grovesSpawn = Game.Player.Character.Position + Game.Player.Character.ForwardVector * spawnDistanceGroves;
                    gang.instantiateGangs(thugsPerTeam, World.GetNextPositionOnStreet(ballasSpawn), World.GetNextPositionOnStreet(grovesSpawn));
                    gang.postInstantiation();

                    UI.Notify("GangWar! Check the map for the blips");
                }
            }
            gang.check(warpToCarTimer);
            gang.drive(startDrivingTimer);
            gang.blipUpdate();
            
        }



        //TestFile to manually delete 
        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == trigger )
            {

                if (gang.eventActive == false)
                {
                    UI.Notify("Gang event-Loading models");
                    ballasSpawn = Game.Player.Character.Position + Game.Player.Character.ForwardVector * spawnDistanceBallas;
                    grovesSpawn = Game.Player.Character.Position + Game.Player.Character.ForwardVector * spawnDistanceGroves;
                    gang.instantiateGangs(thugsPerTeam, World.GetNextPositionOnStreet(ballasSpawn), World.GetNextPositionOnStreet(grovesSpawn));
                    gang.postInstantiation();

                    UI.Notify("GangWar! Check the map for the blips");
                }
                else
                {
                    UI.Notify("Previous event isn't over or models are still loading");
                }
            }
            else if(e.KeyCode == delete)
            {
                gang.deleteThugs();
            }


        }

    }
}
