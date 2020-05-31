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

        List<int> wrapToVehicleCounterB; //used as an alternate to Wait() so that the script continues without holding until wait command is up
        List<int> driveAwayCounterB; //''
        List<int> wrapToVehicleCounterG; //''
        List<int> driveAwayCounterG; //''

        int gangCounterB = 0;
        int gangCounterG = 0;


        public bool eventActive = false; //used to make sure that we dont retrigger the event twice


        //Responsible for initializing the thugs
        public void instantiateGangs(int totalThugPerTeam, Vector3 spawnB, Vector3 spawnG)
        {
            eventActive = true;
            deleteThugs(); //to be safe we delete our old ones
            ballas = null;
            groves = null;

            //Counters are used to count the value, once it reaches a thrshold we execute another action
            driveAwayCounterB = new List<int>();
            driveAwayCounterG = new List<int>();
            wrapToVehicleCounterB = new List<int>();
            wrapToVehicleCounterG = new List<int>();

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
                if (driveAwayCounterB == null)
                {
                    driveAwayCounterB = new List<int>();
                }
                if (driveAwayCounterG == null)
                {
                    driveAwayCounterG = new List<int>();
                }
                if (wrapToVehicleCounterB == null)
                {
                    wrapToVehicleCounterB = new List<int>();
                }
                if (wrapToVehicleCounterG == null)
                {
                    wrapToVehicleCounterG = new List<int>();
                }
                if (ballas != null && groves != null && wrapToVehicleCounterB != null && wrapToVehicleCounterG != null && driveAwayCounterB != null && driveAwayCounterG != null)
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
                i.RelationshipGroup = groupBallas;

                Function.Call(Hash.SET_PED_COMBAT_ABILITY, i, rand.Next(30, 100)); // random attack power
                Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, i, 0, 0); //ped wont flee

                i.MaxHealth = 200000;
                i.Armor = 10000;
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

                i.RelationshipGroup = groupGrove;


                Function.Call(Hash.SET_PED_COMBAT_ABILITY, i, rand.Next(30, 100)); // random attack power
                Function.Call(Hash.SET_PED_FLEE_ATTRIBUTES, i, 0, 0); //ped wont flee
                i.MaxHealth = 200000;
                i.Armor = 10000;
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
        public List<Ped> thugsalive(List<Ped> group)
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

        /**Renponsible to make sure that the thugs enters a car under the right circumstances, and if stuck warped to the vehicle after a threshold
         */
        public void check()
        {

            if (ballas == null || groves == null)
            {
                return;
            }

            int threshold = 6; //Used as a counter. Once wrapToVehicle reaches this threshold we wrap the thug into nearest vehicle assuming he is stuck

            List<Ped> aliveB = thugsalive(ballas); //getting list of thugs who are alive
            if (aliveB != null)
            {
                foreach (Ped p in aliveB)
                {
                    wrapToVehicleCounterB.Add(0); //creating wrapToVehicleCounter integer for every alive Ped
                }
            }
            else
                return;

            for (int i = 0; i < aliveB.Count; i++)
            {
                if (aliveB[i].IsInCombat == false && aliveB[i].IsInVehicle() == false && aliveB[i].IsGettingIntoAVehicle == false && aliveB[i].IsTryingToEnterALockedVehicle == false)
                { //Conditions needed to be satisfied to make sure he enters the car on the righ situation

                    Vehicle v = World.GetClosestVehicle(aliveB[i].Position, 500);
                    aliveB[i].Task.EnterVehicle(v, VehicleSeat.Any); //Tasking the thug to enter the vehile
                    wrapToVehicleCounterB[i]++; //incrementing the value of WrapToVehicleB by +1
                    if (wrapToVehicleCounterB[i] > threshold) //If the value of WrapTovehicle has crossed threshold which is 6 in our case(as of Right now) we warp him to the closest car
                    {
                        v = World.GetClosestVehicle(aliveB[i].Position, 500);
                        try
                        {
                            aliveB[i].Task.WarpIntoVehicle(v, VehicleSeat.Any);
                        }
                        catch
                        {
                            //Just in case there is no car nearby. PLanning to make a thug member bring car but too much work for now
                        }
                    }
                }
                else
                {
                    wrapToVehicleCounterB[i] = 0; //resetting the warpTovehicleCounter
                }
            }
            //Same as ballas
            List<Ped> aliveG = thugsalive(groves);
            if (aliveG != null)
            {
                foreach (Ped p in aliveG)
                {

                    wrapToVehicleCounterG.Add(0);
                }
            }
            else
                return;
            for (int i = 0; i < aliveG.Count; i++)
            {
                Wait(500);
                if (aliveG[i].IsInCombat == false && aliveG[i].IsInVehicle() == false && aliveG[i].IsGettingIntoAVehicle == false && aliveG[i].IsTryingToEnterALockedVehicle == false)
                {
                    Vehicle v = World.GetClosestVehicle(aliveG[i].Position, 500);
                    aliveG[i].Task.EnterVehicle(v, VehicleSeat.Any);
                    wrapToVehicleCounterG[i]++;
                    if (wrapToVehicleCounterG[i] > threshold)
                    {
                        v = World.GetClosestVehicle(aliveG[i].Position, 500);
                        try
                        {
                            aliveG[i].Task.WarpIntoVehicle(v, VehicleSeat.Any);
                        }
                        catch
                        {
                            //Just in case there is no car nearby. Didnt plan what to do yet. i mean i want to spawn a driver nearby and drive to this fella but too much to ask
                        }
                        wrapToVehicleCounterG[i] = 0;
                    }
                }
                else
                {
                    wrapToVehicleCounterG[i] = 0;
                }
            }
        }

        /**Responsible to make sure that thugs drive when seat is full,homies take too long to get in,all thugs are in a car so just drive dont wait
         */
        public void drive()
        {
            if (ballas == null || groves == null)
            {
                return;
            }
            int threshold = 10; //USed as a timer to wait for homies to enter the car, if failed to do so within this mentioned threshold drive away
            List<Ped> aliveB = thugsalive(ballas);
            foreach (Ped p in aliveB)
            {
                driveAwayCounterB.Add(0); //Creating the number of driveAwayCounter needed
            }


            for (int i = 0; i < aliveB.Count; i++)
            {
                if (aliveB[i].IsInVehicle() || aliveB[i].IsGettingIntoAVehicle || aliveB[i].IsTryingToEnterALockedVehicle || aliveB[i].IsSittingInVehicle())
                {
                    //If ped is in car or gettng in car, increase the gang counter, indicating one of the alive thug is in car. And increase the value ofdriveAwayCounterB by +1
                    driveAwayCounterB[i]++;
                    gangCounterB++;
                    if (gangCounterB >= aliveB.Count) //if gangCounter == the number of thugs alive means that all the thugs are inside a car so we dont have to wait, just drive
                    {

                        Ped driver = aliveB[i].CurrentVehicle.GetPedOnSeat(VehicleSeat.Driver);

                        if (aliveB[i].CurrentVehicle.IsSeatFree(VehicleSeat.Driver)) //if seat is free just warp into driver seat and drive away
                        {
                            aliveB[i].Task.WarpIntoVehicle(aliveB[i].CurrentVehicle, VehicleSeat.Driver);
                            aliveB[i].Task.CruiseWithVehicle(aliveB[i].CurrentVehicle, 400);
                        }
                        else  //seat isnt free
                        {
                            if (driver.IsAlive == false)//if driveris dead then warp 
                            {
                                aliveB[i].Task.WarpIntoVehicle(aliveB[i].CurrentVehicle, VehicleSeat.Driver);
                                aliveB[i].Task.CruiseWithVehicle(aliveB[i].CurrentVehicle, 400);
                            }
                            else // driver is alive so we tell the driver to drive
                            {
                                driver.Task.CruiseWithVehicle(aliveB[i].CurrentVehicle, 400);
                            }
                        }

                        driveAwayCounterB[i] = 0;
                        gangCounterB = 0;
                    }
                    else if (driveAwayCounterB[i] > threshold || aliveB[i].CurrentVehicle.IsSeatFree(VehicleSeat.Any) == false) //else if all gangs arent in but counter threshold reaches we drive away
                    {
                        if (aliveB[i].CurrentVehicle.IsSeatFree(VehicleSeat.Any) == false) //if no free seats then we can directly go
                        {
                            Ped driver = aliveB[i].CurrentVehicle.GetPedOnSeat(VehicleSeat.Driver);
                            if (driver.IsAlive == false) //making sure that the driver is alive
                            {
                                aliveB[i].Task.WarpIntoVehicle(aliveB[i].CurrentVehicle, VehicleSeat.Driver);
                                aliveB[i].Task.CruiseWithVehicle(aliveB[i].CurrentVehicle, 400);
                            }
                            else
                            {
                                driver.Task.CruiseWithVehicle(aliveB[i].CurrentVehicle, 400);
                            }
                        }
                        else //Means there aer free seats but counter threshold reached
                        {
                            if (aliveB[i].CurrentVehicle.IsSeatFree(VehicleSeat.Driver))
                            {
                                //If driver seat is free
                                aliveB[i].Task.WarpIntoVehicle(aliveB[i].CurrentVehicle, VehicleSeat.Driver);
                                aliveB[i].Task.CruiseWithVehicle(aliveB[i].CurrentVehicle, 400);
                            }
                            else
                            {
                                //if driver seat isnt free
                                if (aliveB[i].CurrentVehicle.Driver.IsAlive)
                                {
                                    aliveB[i].CurrentVehicle.Driver.Task.CruiseWithVehicle(aliveB[i].CurrentVehicle, 400);
                                }
                                else
                                {
                                    aliveB[i].Task.WarpIntoVehicle(aliveB[i].CurrentVehicle, VehicleSeat.Driver);
                                    aliveB[i].Task.CruiseWithVehicle(aliveB[i].CurrentVehicle, 400);
                                }
                            }
                        }
                        driveAwayCounterB[i] = 0;//resetting the driveAwayCounter and gangcounter so that it is 0 when thugs exit and re enter the car
                        gangCounterB = 0;
                    }
                }
                else
                {
                    driveAwayCounterB[i] = 0;
                    gangCounterB = 0;
                }
            }


            //Same for groves

            List<Ped> aliveG = thugsalive(groves);
            foreach (Ped p in aliveG)
            {
                driveAwayCounterG.Add(0);
            }


            for (int i = 0; i < aliveG.Count; i++)
            {
                if (aliveG[i].IsInVehicle() || aliveG[i].IsGettingIntoAVehicle || aliveG[i].IsTryingToEnterALockedVehicle || aliveG[i].IsSittingInVehicle())
                {
                    //If ped is in car or gettng in car, increase the gang counter, indicating one of the alive thug is in car. And increase the value ofdriveAwayCounterB by +1
                    driveAwayCounterG[i]++;
                    gangCounterG++;
                    if (gangCounterG >= aliveG.Count) //if gangCounter == the number of thugs alive means that all the thugs are inside a car so we dont have to wait, just drive
                    {

                        Ped driver = aliveG[i].CurrentVehicle.GetPedOnSeat(VehicleSeat.Driver);

                        if (aliveG[i].CurrentVehicle.IsSeatFree(VehicleSeat.Driver)) //if seat is free just warp into driver seat and drive away
                        {
                            aliveG[i].Task.WarpIntoVehicle(aliveG[i].CurrentVehicle, VehicleSeat.Driver);
                            aliveG[i].Task.CruiseWithVehicle(aliveG[i].CurrentVehicle, 400);
                        }
                        else  //seat isnt free
                        {
                            if (driver.IsAlive == false)//if driveris dead then warp 
                            {
                                aliveG[i].Task.WarpIntoVehicle(aliveG[i].CurrentVehicle, VehicleSeat.Driver);
                                aliveG[i].Task.CruiseWithVehicle(aliveG[i].CurrentVehicle, 400);
                            }
                            else // driver is alive so we tell the driver to drive
                            {
                                driver.Task.CruiseWithVehicle(aliveG[i].CurrentVehicle, 400);
                            }
                        }

                        driveAwayCounterG[i] = 0;
                        gangCounterG = 0;
                    }
                    else if (driveAwayCounterG[i] > threshold || aliveG[i].CurrentVehicle.IsSeatFree(VehicleSeat.Any) == false) //else if counter thresohold reahes or no seat free we drive  away
                    {
                        if (aliveG[i].CurrentVehicle.IsSeatFree(VehicleSeat.Any) == false) //if no free seats then we can directly go
                        {
                            Ped driver = aliveG[i].CurrentVehicle.GetPedOnSeat(VehicleSeat.Driver);
                            if (driver.IsAlive == false) //making sure that the driver is alive
                            {
                                aliveG[i].Task.WarpIntoVehicle(aliveG[i].CurrentVehicle, VehicleSeat.Driver);
                                aliveG[i].Task.CruiseWithVehicle(aliveG[i].CurrentVehicle, 400);
                            }
                            else
                            {
                                driver.Task.CruiseWithVehicle(aliveG[i].CurrentVehicle, 400);
                            }
                        }
                        else //Means there aer free seats but counter threshold reached
                        {
                            if (aliveG[i].CurrentVehicle.IsSeatFree(VehicleSeat.Driver))
                            {
                                //If driver seat is free
                                aliveG[i].Task.WarpIntoVehicle(aliveG[i].CurrentVehicle, VehicleSeat.Driver);
                                aliveG[i].Task.CruiseWithVehicle(aliveG[i].CurrentVehicle, 400);
                            }
                            else
                            {
                                //if driver seat isnt free
                                if (aliveG[i].CurrentVehicle.Driver.IsAlive)
                                {
                                    aliveG[i].CurrentVehicle.Driver.Task.CruiseWithVehicle(aliveG[i].CurrentVehicle, 400);
                                }
                                else
                                {
                                    aliveG[i].Task.WarpIntoVehicle(aliveG[i].CurrentVehicle, VehicleSeat.Driver);
                                    aliveG[i].Task.CruiseWithVehicle(aliveG[i].CurrentVehicle, 400);
                                }
                            }
                        }
                        driveAwayCounterG[i] = 0;//resetting the driveAwayCounter and gangcounter so that it is 0 when thugs exit and re enter the car
                        gangCounterG = 0;
                    }
                }
                else
                {
                    driveAwayCounterG[i] = 0;
                    gangCounterG = 0;
                }
            }


        }
    }
}


