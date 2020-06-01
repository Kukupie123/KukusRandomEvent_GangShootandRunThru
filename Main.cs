using System;
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
        public Main()
        {
            UI.Notify("Kuku's RE-GangShootNRun");
            Tick += onTick;
            KeyDown += onKeyDown;
            gang = new Gang();
            rand = new Random();
        }


        private void onTick(object sender, EventArgs e)
        {

            int chance = rand.Next(1, 200); //probability of incident to occour
            int numb = rand.Next(1, 200);
            Interval = 1000;
            gang.onTickClean(Game.Player.Character, 500);
           // gang.changeBlip();
            if (numb == chance) {
                
                if (gang.eventActive == false)
                {
                    UI.Notify("Gang event-Loading models");
                    ballasSpawn = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 130;
                    grovesSpawn = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 110;
                    gang.instantiateGangs(15, World.GetNextPositionOnSidewalk(ballasSpawn), World.GetNextPositionOnSidewalk(grovesSpawn));
                    gang.postInstantiation();
                    UI.Notify("GangWar! Check the map for the blips");
                }
            }
            gang.check(10);
            gang.drive(18);
            gang.blipUpdate();
            
        }



        //TestFile to manually delete 
        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.OemQuestion)
            {
               
                UI.Notify("Gang event-Loading models");
                ballasSpawn = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 160;
                grovesSpawn = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 100;
                gang.instantiateGangs(15, World.GetNextPositionOnSidewalk(ballasSpawn), World.GetNextPositionOnSidewalk(grovesSpawn));
                gang.postInstantiation();
                UI.Notify("GangWar! Check the map for the blips");
            }
            else if(e.KeyCode == Keys.Delete)
            {
                gang.deleteThugs();
            }


        }

    }
}
