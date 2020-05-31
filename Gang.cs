using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
namespace KukuEvent
{
    public class Gang : Script
    {
        private List<Ped> ballas; //List of thugs in ballas team
        private List<Ped> groves; //List of thugs in groves team

        Dictionary<Ped, int> enterCarCounterB;
        Dictionary<Ped, int> enterCarCounterG;

        Dictionary<Ped, int> driveAwayCounterB;
        Dictionary<Ped, int> driveAwayCounterG;

        public bool eventActive = false; //used to make sure that we dont retrigger the event twice


        //Responsible for initializing the thugs
        public void instantiateGangs(int totalThugPerTeam, Vector3 spawnB, Vector3 spawnG)
        {
            eventActive = true;
            deleteThugs(); //to be safe we delete our old ones
            ballas = null;
            groves = null;

            //Counters are used to count the value, once it reaches a thrshold we execute another action
            driveAwayCounterB = new Dictionary<Ped, int>();
            driveAwayCounterG = new Dictionary<Ped, int>();
            enterCarCounterB = new Dictionary<Ped, int>();
            enterCarCounterG = new Dictionary<Ped, int>();

            //Sometime game skips initiation for some reason prolly frame skips and etc so we keep a loop to exit when everything has been initialized
            while (true)
            {
                if (ballas == null)
                {
                    ballas = new List<Ped>();
                }
                if (groves == null)
                {
                    groves = new List<Ped>();
                }
                if (enterCarCounterB == null)
                {
                    enterCarCounterB = new Dictionary<Ped, int>();
                }
                if (enterCarCounterG == null)
                {
                    enterCarCounterG = new Dictionary<Ped, int>();
                }
                if (driveAwayCounterB == null)
                {
                    driveAwayCounterB = new Dictionary<Ped, int>();
                }
                if (driveAwayCounterG == null)
                {
                    driveAwayCounterG = new Dictionary<Ped, int>();
                }
                if (ballas != null && groves != null && enterCarCounterG != null && enterCarCounterB != null && driveAwayCounterB != null && driveAwayCounterG != null)
                {
                    break;
                }
            }

            //random used to create different random models based on the output 
            Random random = new Random();
            //Spawning
            Model thug = null;
            for (int i = 0; i < totalThugPerTeam; i++)
            {
                int modelToUse = random.Next(1, 5);
                switch (modelToUse)
                {
                    case 1:
                        thug = new Model(PedHash.BallaEast01GMY);
                        break;
                    case 2:
                        thug = new Model(PedHash.BallaOrig01GMY);
                        break;
                    case 3:
                        thug = new Model(PedHash.Ballas01GFY);
                        break;
                    case 4:
                        thug = new Model(PedHash.Ballasog);
                        break;
                    case 5:
                        thug = new Model(PedHash.BallaSout01GMY);
                        break;
                }
                //Checking if the model is valid
                if (thug.IsValid)
                {
                    while (true) //If model is loaded then exit, else request and loop 
                    {
                        if (thug.IsLoaded)
                        {
                            break;
                        }
                        else
                        {
                            thug.Request(100);
                            Wait(0);
                        }
                    }

                    ballas.Add(World.CreatePed(thug, spawnB));
                    while (!ballas[i].Exists()) //THIS IS WHERE THE GAME CRASHES AFTER FEW RUNS PREVIOUSLY
                    {
                        Wait(0);
                    }
                }
            }

            //Doing the same for Groves
            for (int i = 0; i < totalThugPerTeam; i++)
            {
                int modelToUse = random.Next(1, 5);
                switch (modelToUse)
                {
                    case 1:
                        thug = new Model(PedHash.Famca01GMY);
                        break;
                    case 2:
                        thug = new Model(PedHash.Famdd01);
                        break;
                    case 3:
                        thug = new Model(PedHash.Famdnf01GMY);
                        break;
                    case 4:
                        thug = new Model(PedHash.Famfor01GMY);
                        break;
                    case 5:
                        thug = new Model(PedHash.Families01GFY);
                        break;
                }
                if (thug.IsValid)
                {
                    while (true)
                    {
                        if (thug.IsLoaded)
                        {
                            break;
                        }
                        else
                        {
                            thug.Request(100);
                            Wait(0);
                        }
                    }
                    groves.Add(World.CreatePed(thug, spawnG));

                    while (!groves[i].Exists())//THIS IS WHERE THE GAME CRASHES AFTER FEW RUNS
                    {
                        Wait(0);
                    }
                }

            }
            thug.MarkAsNoLongerNeeded();

        }

        /**
         * Call after we instantiate the ballas list. Tihs function will set the stats, etc to make them ready for fighrting
         */
        public void postInstantiation()
        {
            //Relationship indexes
            int groupBallas = Function.Call<int>(Hash.GET_HASH_KEY, "AMBIENT_GANG_BALLAS");
            int groupGrove = Function.Call<int>(Hash.GET_HASH_KEY, "AMBIENT_GANG_FAMILY");
            int cop = Function.Call<int>(Hash.GET_HASH_KEY, "COP");

            Random rand = new Random();

            //Giving random weapoons to each gang member ped
            foreach (Ped i in ballas)
            {
                int weapon = rand.Next(1, 5);
                switch (weapon)
                {
                    case 1:
                        i.Weapons.Give(WeaponHash.VintagePistol, 100, true, true);

                        i.Weapons.Give(WeaponHash.MicroSMG, 100, true, true);
                        break;
                    case 2:
                        i.Weapons.Give(WeaponHash.APPistol, 100, true, true);

                        i.Weapons.Give(WeaponHash.AssaultShotgun, 900, true, true);
                        break;
                    case 3:
                        i.Weapons.Give(WeaponHash.CombatPistol, 100, true, true);

                        i.Weapons.Give(WeaponHash.SniperRifle, 900, true, true);
                        break;
                    case 4:
                        i.Weapons.Give(WeaponHash.MicroSMG, 100, true, true);

                        i.Weapons.Give(WeaponHash.AssaultrifleMk2, 800, true, true);
                        break;
                    case 5:
                        i.Weapons.Give(WeaponHash.Pistol50, 100, true, true);

                        i.Weapons.Give(WeaponHash.HomingLauncher, 5, true, true);
                        break;
                }
                //Setting up relatinships, stats, blips etc
                i.Accuracy = rand.Next(40, 100);
                i.RelationshipGroup = groupBallas;

                Function.Call(Hash.SET_PED_COMBAT_ABILITY, i, rand.Next(30, 100)); // random attack power
                Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, i, 0, 0); //ped wont flee

                i.MaxHealth = 200000;
                i.Armor = 200000;
                i.Health = 200000;

                i.AddBlip();
                i.CurrentBlip.Color = BlipColor.Blue;
                i.CurrentBlip.Name = "Ballas";
                i.Task.FightAgainstHatedTargets(999999); //ik the parameters are stupid asf lol
            }

            //Doing the same for grove
            foreach (Ped i in groves)
            {
                int weapon = rand.Next(1, 5);
                switch (weapon)
                {
                    case 1:
                        i.Weapons.Give(WeaponHash.Pistol50, 100, true, true);

                        i.Weapons.Give(WeaponHash.APPistol, 100, true, true);
                        break;
                    case 2:
                        i.Weapons.Give(WeaponHash.APPistol, 100, true, true);

                        i.Weapons.Give(WeaponHash.AssaultShotgun, 900, true, true);
                        break;
                    case 3:
                        i.Weapons.Give(WeaponHash.CombatPistol, 100, true, true);

                        i.Weapons.Give(WeaponHash.SniperRifle, 900, true, true);
                        break;
                    case 4:
                        i.Weapons.Give(WeaponHash.HeavyPistol, 100, true, true);

                        i.Weapons.Give(WeaponHash.AssaultrifleMk2, 800, true, true);
                        break;
                    case 5:
                        i.Weapons.Give(WeaponHash.DoubleActionRevolver, 100, true, true);

                        i.Weapons.Give(WeaponHash.HomingLauncher, 5, true, true);
                        break;
                }
                i.Accuracy = rand.Next(40, 100);
                i.RelationshipGroup = groupGrove;


                Function.Call(Hash.SET_PED_COMBAT_ABILITY, i, rand.Next(30, 100)); // random attack power
                Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, i, 0, 0); //ped wont flee
                i.MaxHealth = 200000;
                i.Armor = 200000;
                i.Health = 200000;
                i.AddBlip();
                i.CurrentBlip.Color = BlipColor.Green;
                i.CurrentBlip.Name = "Grove";
                i.Task.FightAgainstHatedTargets(99999);
            }
            //Modifying the relationship
            World.SetRelationshipBetweenGroups(Relationship.Hate, groupGrove, groupBallas);
            World.SetRelationshipBetweenGroups(Relationship.Hate, groupBallas, groupGrove);
            World.SetRelationshipBetweenGroups(Relationship.Companion, groupGrove, 0x6F0783F5);

            World.SetRelationshipBetweenGroups(Relationship.Hate, cop, groupBallas);
            World.SetRelationshipBetweenGroups(Relationship.Hate, cop, groupGrove);

            World.SetRelationshipBetweenGroups(Relationship.Hate, groupGrove, cop);
            World.SetRelationshipBetweenGroups(Relationship.Hate, groupBallas, cop);

            //COUNTERS 
            foreach (Ped p in ballas)
            {
                enterCarCounterB.Add(p, 0);
                driveAwayCounterB.Add(p, 0);
            }
            foreach (Ped p in groves)
            {
                enterCarCounterG.Add(p, 0);
                driveAwayCounterG.Add(p, 0);
            }

            //COMPLETE NOW THEY SHALL FIGHT. DO NOT USE BLOCK TEMPORARY TASK AS THE FIGHT IS SUPPOSED TO BE NATURAL AND COME AS A TEMP TASK

        }

        /**
         * This method checks for dead thugs or out of range from player thugs and detaches them and setting the ped value to null
         */
        public void onTickClean(Ped player, int cleanupDistance)
        {
            //safety check
            if (ballas == null || groves == null || player == null)
            {
                return;
            }

            //Responsible to check if the dead thugs are deleted once they die
            for (int i = 0; i < ballas.Count; i++)
            {
                if (ballas[i] != null)
                {
                    if (ballas[i].IsDead || ballas[i].Position.DistanceTo2D(player.Position) > cleanupDistance)
                    {
                        ballas[i].CurrentBlip.Remove();
                        ballas[i].MarkAsNoLongerNeeded();
                        ballas[i].Detach();
                        ballas[i] = null;
                    }
                }
            }
            for (int i = 0; i < groves.Count; i++)
            {
                if (groves[i] != null)
                {
                    if (groves[i].IsDead || groves[i].Position.DistanceTo2D(player.Position) > cleanupDistance)
                    {
                        groves[i].CurrentBlip.Remove();
                        groves[i].MarkAsNoLongerNeeded();
                        groves[i].Detach();
                        groves[i] = null;
                    }
                }
            }

            //If all thugs are null it means gang war is over 
            int counter = 0; //to counter the number of null peds. if the number of null peds are as the "count" of the ballas&groves list then everyone from the team is dead
            for (int i = 0; i < groves.Count; i++)
            {
                if (ballas[i] == null && groves[i] == null)
                {
                    counter++;
                }
            }
            if (counter == groves.Count)
            {
                ballas = null;
                groves = null;
                eventActive = false;
                enterCarCounterG = new Dictionary<Ped, int>();
                enterCarCounterB = new Dictionary<Ped, int>();
                driveAwayCounterB = new Dictionary<Ped, int>();
                driveAwayCounterG = new Dictionary<Ped, int>();
                UI.ShowHelpMessage("GangWar over,either dead or ran away");
            }
        }

        /**
         * Used to delete thugs forcefully
         */
        public void deleteThugs()
        {
            if (ballas == null && groves == null)
            {
                return;
            }
            for (int i = 0; i < ballas.Count; i++)
            {
                if (ballas[i] != null)
                {
                    ballas[i].MarkAsNoLongerNeeded();
                    ballas[i].CurrentBlip.Remove();
                    ballas[i].Delete();
                    ballas[i] = null;
                }
            }
            for (int i = 0; i < groves.Count; i++)
            {
                if (groves[i] != null)
                {
                    groves[i].MarkAsNoLongerNeeded();
                    groves[i].CurrentBlip.Remove();
                    groves[i].Delete();
                    groves[i] = null;
                }
            }
            ballas = null;
            groves = null;
            eventActive = false;
        }

        /**
         * Returns a list of thugs who are still alive in the fight
         */
        private List<Ped> thugsalive(List<Ped> group)
        {
            if (group != null)
            {
                List<Ped> temp = new List<Ped>();
                foreach (Ped p in group)
                {
                    if (p != null && p.IsAlive)
                    {
                        temp.Add(p);
                    }
                }
                return temp;
            }
            else
                return null;
        }

        private bool safetyCheck()
        {
            if (ballas == null || groves == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void ifEnterCar(Ped p, Dictionary<Ped, int> enterCounter, int threshold)
        {
            if (p.IsInCombat == false && p.IsGettingIntoAVehicle == false && p.IsTryingToEnterALockedVehicle == false && p.IsInVehicle() == false)
            {
                
                Vehicle v = World.GetClosestVehicle(p.Position, 50000);
                if (v == null)
                {
                    //No car nearby. FUTURE PLANS
                }
                else
                {
                    p.Task.EnterVehicle(v, VehicleSeat.Any);
                    enterCounter[p]++;
                    if (enterCounter[p] > threshold)
                    {
                         v = World.GetClosestVehicle(p.Position, 5000);
                        if (v == null)
                        {

                        }
                        else
                        {
                            p.Task.WarpIntoVehicle(v, VehicleSeat.Any);
                            enterCounter[p] = 0;
                        }
                    }
                }

            }
            else
            {
                enterCounter[p] = 0;
            }
            


        }
        public void check(int threshold)
        {
            if (!safetyCheck())
            {
                return;
            }

            List<Ped> aliveB = thugsalive(ballas);
            List<Ped> aliveG = thugsalive(groves);

            foreach (Ped p in aliveB)
            {
                ifEnterCar(p, enterCarCounterB, threshold);
            }
            foreach (Ped p in aliveG)
            {
                ifEnterCar(p, enterCarCounterG, threshold);
            }
        }

        private bool allThugIn(List<Ped> alive)
        {
            int counter = 0;
            if (alive != null)
            {
                foreach (Ped p in alive)
                {
                    if (p != null)
                    {
                        if (p.IsInVehicle() || p.IsSittingInVehicle())
                        {
                            counter++;
                        }
                    }
                }
                if (counter == alive.Count)
                {
                    return true;
                }
                else
                    return false;
            }
            return false;
        }
        private void ifDrive(Ped p, Dictionary<Ped, int> driveEncounter, int threshold, List<Ped> alive)
        {
            if (p.IsInVehicle() || p.IsSittingInVehicle())
            {
                /*1. -If car full drive away
                 *1.2-If driver dead warp seat n drive else let driver drive
                 *2. -If seat not full see if driver exists
                 *2.1-If driver exists check if dead, if dead warp else task drive
                 *3. -If no drive warp and drive
                 */



                Vehicle v = p.CurrentVehicle;
                driveEncounter[p]++;
                if (v == null)
                {
                    //.......
                }
                else
                {
                    //If threshold reached
                    if (driveEncounter[p] > threshold)
                    {
                        v = p.CurrentVehicle;
                        if (v != null)
                        {
                            if (v.IsSeatFree(VehicleSeat.Driver))
                            {
                                p.Task.WarpIntoVehicle(v, VehicleSeat.Driver);
                                v.GetPedOnSeat(VehicleSeat.Driver).Task.CruiseWithVehicle(v, 200);
                                driveEncounter[p] = 0;

                            }
                            else
                            {
                                if (v.GetPedOnSeat(VehicleSeat.Driver).IsAlive == false || v.GetPedOnSeat(VehicleSeat.Driver).Exists() == false)
                                {
                                    p.Task.WarpIntoVehicle(v, VehicleSeat.Driver);
                                    v.GetPedOnSeat(VehicleSeat.Driver).Task.CruiseWithVehicle(v, 200);
                                    driveEncounter[p] = 0;
                                }
                                else
                                {
                                    v.GetPedOnSeat(VehicleSeat.Driver).Task.CruiseWithVehicle(v, 200);
                                    driveEncounter[p] = 0;
                                }
                            }
                        }
                    }
                    //All thugs are in
                    else if (allThugIn(alive)) //all thugs are in car
                    {
                       

                        v = p.CurrentVehicle;
                        if (v != null)
                        {
                            if (v.IsSeatFree(VehicleSeat.Driver))
                            {
                                p.Task.WarpIntoVehicle(v, VehicleSeat.Driver);
                                v.GetPedOnSeat(VehicleSeat.Driver).Task.CruiseWithVehicle(v, 200);
                                driveEncounter[p] = 0;
                            }
                            else
                            {
                                if (v.GetPedOnSeat(VehicleSeat.Driver).IsAlive == false || v.GetPedOnSeat(VehicleSeat.Driver).Exists() == false)
                                {
                                    p.Task.WarpIntoVehicle(v, VehicleSeat.Driver);
                                    v.GetPedOnSeat(VehicleSeat.Driver).Task.CruiseWithVehicle(v, 200);
                                    driveEncounter[p] = 0;
                                }
                                else
                                {
                                    v.GetPedOnSeat(VehicleSeat.Driver).Task.CruiseWithVehicle(v, 200);
                                    driveEncounter[p] = 0;
                                }
                            }
                        }
                    }
                    //seats are full
                    else if (!v.IsSeatFree(VehicleSeat.Any))
                    {
                        if(v.GetPedOnSeat(VehicleSeat.Driver).IsAlive== false || v.GetPedOnSeat(VehicleSeat.Driver).Exists() == false)
                        {
                            p.Task.WarpIntoVehicle(v, VehicleSeat.Driver);
                            p.Task.CruiseWithVehicle(v, 200);
                            driveEncounter[p] = 0;
                        }
                        else
                        {
                            v.GetPedOnSeat(VehicleSeat.Driver).Task.CruiseWithVehicle(v, 200);
                            driveEncounter[p] = 0;
                        }
                    }
                    //If nothing has been triggered we increment counter
                    else 
                    {
                        driveEncounter[p]++;
                    }
                    
                }
            }
            else
            {
                driveEncounter[p] = 0;
            }
        }

        public void drive(int threshold)
        {
            if (!safetyCheck())
            {
                return;
            }
            List<Ped> aliveB = thugsalive(ballas);
            List<Ped> aliveG = thugsalive(groves);

            foreach (Ped p in aliveB)
            {
                ifDrive(p, driveAwayCounterB, threshold, aliveB);
            }
            foreach (Ped p in aliveG)
            {
                ifDrive(p, driveAwayCounterG, threshold, aliveG);
            }
        }

        
    }
}


