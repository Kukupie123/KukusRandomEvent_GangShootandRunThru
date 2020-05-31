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
        bool startup; //to show description at start of game
        public Main()
        {
            startup = true;
            Tick += onTick;
            KeyDown += onKeyDown;
            gang = new Gang();
            rand = new Random();
        }


        private void onTick(object sender, EventArgs e)
        {
           
            int chance = rand.Next(1, 60); //probability of incident to occour
            int numb = rand.Next(1, 60);
            Interval = 1000;
            gang.onTickClean(Game.Player.Character, 500);
            if (numb == chance) {
                if (gang.eventActive == false)
                {
                    UI.Notify("Gang event on");
                    ballasSpawn = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 150;
                    grovesSpawn = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 100;
                    gang.instantiateGangs(5, World.GetNextPositionOnSidewalk(ballasSpawn), World.GetNextPositionOnSidewalk(grovesSpawn));
                    gang.postInstantiation();
                    UI.ShowHelpMessage("GangWar! Check the map for the blips");
                }
            }
            
        }



        //TestFile to manually delete and trigger events
        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageDown)
            {
                gang.instantiateGangs(5, World.GetNextPositionOnSidewalk(ballasSpawn), World.GetNextPositionOnSidewalk(grovesSpawn));
                gang.postInstantiation();
            }
            if (e.KeyCode == Keys.Delete)
            {
                gang.deleteThugs();
            }


        }

    }
}
